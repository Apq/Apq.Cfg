using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using StackExchange.Redis;

namespace Apq.Cfg.Redis;

/// <summary>
/// Redis 配置源
/// </summary>
internal sealed class RedisCfgSource : IWritableCfgSource, IDisposable
{
    private const int MinScanPageSize = 10;
    private const int MaxScanPageSize = 1000;

    private readonly ConnectionMultiplexer _multiplexer;
    private readonly RedisOptions _options;
    private volatile bool _disposed;

    /// <summary>
    /// 初始化 RedisCfgSource 实例
    /// </summary>
    /// <param name="options">Redis 连接选项</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    /// <param name="name">配置源名称（可选）</param>
    public RedisCfgSource(RedisOptions options, int level, bool isPrimaryWriter, string? name = null)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        Name = name ?? $"Redis:{options.KeyPrefix ?? "config"}";

        var conn = EnsureAllowAdmin(options.ConnectionString!);
        if (_options.ConnectTimeoutMs > 0) conn += $",connectTimeout={_options.ConnectTimeoutMs}";
        if (_options.OperationTimeoutMs > 0) conn += $",syncTimeout={_options.OperationTimeoutMs}";
        conn += ",abortConnect=false";
        _multiplexer = ConnectionMultiplexer.Connect(conn);
    }

    /// <summary>
    /// 获取配置层级，数值越大优先级越高
    /// </summary>
    public int Level { get; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <summary>
    /// 获取是否可写，Redis 支持通过 API 写入配置，因此始终为 true
    /// </summary>
    public bool IsWriteable => true;

    /// <summary>
    /// 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标
    /// </summary>
    public bool IsPrimaryWriter { get; }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        ThrowIfDisposed();
        var data = new List<KeyValuePair<string, string?>>();

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            return data;

        try
        {
            var db = _multiplexer.GetDatabase(_options.Database ?? -1);
            var endpoints = _multiplexer.GetEndPoints();
            var server = endpoints.Length > 0 ? _multiplexer.GetServer(endpoints[0]) : null;
            if (server != null)
            {
                var pattern = string.IsNullOrEmpty(_options.KeyPrefix) ? "*" : _options.KeyPrefix + "*";
                var pageSize = Math.Clamp(_options.ScanPageSize, MinScanPageSize, MaxScanPageSize);
                var prefixLen = _options.KeyPrefix?.Length ?? 0;
                foreach (var key in server.Keys(db.Database, pattern, pageSize))
                {
                    var val = db.StringGet(key);
                    var keyStr = key.ToString();
                    if (!string.IsNullOrEmpty(keyStr))
                    {
                        var configKey = prefixLen > 0 ? keyStr.Substring(prefixLen) : keyStr;
                        data.Add(new KeyValuePair<string, string?>(configKey, val.HasValue ? val.ToString() : null));
                    }
                }
            }
        }
        catch { }

        return data;
    }

    /// <summary>
    /// 释放资源，关闭 Redis 连接
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _multiplexer?.Dispose(); }
        catch { }
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的内存配置源，从 Redis 加载数据
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource 实例</returns>
    /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        var data = new List<KeyValuePair<string, string?>>();

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            return new MemoryConfigurationSource { InitialData = data };

        try
        {
            var db = _multiplexer.GetDatabase(_options.Database ?? -1);
            var endpoints = _multiplexer.GetEndPoints();
            var server = endpoints.Length > 0 ? _multiplexer.GetServer(endpoints[0]) : null;
            if (server != null)
            {
                var pattern = string.IsNullOrEmpty(_options.KeyPrefix) ? "*" : _options.KeyPrefix + "*";
                var pageSize = Math.Clamp(_options.ScanPageSize, MinScanPageSize, MaxScanPageSize);
                var prefixLen = _options.KeyPrefix?.Length ?? 0;
                foreach (var key in server.Keys(db.Database, pattern, pageSize))
                {
                    var val = db.StringGet(key);
                    var keyStr = key.ToString();
                    if (!string.IsNullOrEmpty(keyStr))
                    {
                        // 去掉前缀，还原为原始配置 key
                        var configKey = prefixLen > 0 ? keyStr.Substring(prefixLen) : keyStr;
                        data.Add(new KeyValuePair<string, string?>(configKey, val.HasValue ? val.ToString() : null));
                    }
                }
            }
        }
        catch { }

        return new MemoryConfigurationSource { InitialData = data };
    }

    /// <summary>
    /// 应用配置更改到 Redis
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(_options.ConnectionString)) return;

        var db = _multiplexer.GetDatabase(_options.Database ?? -1);
        var batch = db.CreateBatch();
        var tasks = new List<Task>();

        foreach (var (key, value) in changes)
        {
            var k = string.IsNullOrEmpty(_options.KeyPrefix) ? key : _options.KeyPrefix + key;
            tasks.Add(value is null ? batch.KeyDeleteAsync(k) : batch.StringSetAsync(k, value));
        }

        batch.Execute();
        await Task.WhenAll(tasks).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(_options.Channel))
        {
            try
            {
                var sub = _multiplexer.GetSubscriber();
                await sub.PublishAsync(RedisChannel.Literal(_options.Channel!), "update").ConfigureAwait(false);
            }
            catch { }
        }
    }

    /// <summary>
    /// 检查对象是否已释放，如果已释放则抛出异常
    /// </summary>
    /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RedisCfgSource));
    }

    /// <summary>
    /// 确保连接字符串包含 allowAdmin 选项
    /// </summary>
    /// <param name="connectionString">原始连接字符串</param>
    /// <returns>包含 allowAdmin 选项的连接字符串</returns>
    private static string EnsureAllowAdmin(string connectionString)
    {
        return connectionString.Contains("allowAdmin", StringComparison.OrdinalIgnoreCase)
            ? connectionString
            : connectionString + ",allowAdmin=true";
    }
}
