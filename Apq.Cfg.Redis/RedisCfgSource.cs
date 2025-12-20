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

    public RedisCfgSource(RedisOptions options, int level, bool isPrimaryWriter)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;

        var conn = EnsureAllowAdmin(options.ConnectionString!);
        if (_options.ConnectTimeoutMs > 0) conn += $",connectTimeout={_options.ConnectTimeoutMs}";
        if (_options.OperationTimeoutMs > 0) conn += $",syncTimeout={_options.OperationTimeoutMs}";
        conn += ",abortConnect=false";
        _multiplexer = ConnectionMultiplexer.Connect(conn);
    }

    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _multiplexer?.Dispose(); }
        catch { }
    }

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
                foreach (var key in server.Keys(db.Database, pattern, pageSize))
                {
                    var val = db.StringGet(key);
                    var keyStr = key.ToString();
                    if (!string.IsNullOrEmpty(keyStr))
                        data.Add(new KeyValuePair<string, string?>(keyStr, val.HasValue ? val.ToString() : null));
                }
            }
        }
        catch { }

        return new MemoryConfigurationSource { InitialData = data };
    }

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

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RedisCfgSource));
    }

    private static string EnsureAllowAdmin(string connectionString)
    {
        return connectionString.Contains("allowAdmin", StringComparison.OrdinalIgnoreCase)
            ? connectionString
            : connectionString + ",allowAdmin=true";
    }
}
