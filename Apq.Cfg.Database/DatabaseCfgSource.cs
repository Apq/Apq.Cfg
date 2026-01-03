using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using SqlSugar;

namespace Apq.Cfg.Database;

/// <summary>
/// 数据库配置源
/// </summary>
internal sealed class DatabaseCfgSource : IWritableCfgSource
{
    private readonly SqlSugarDatabaseProvider _databaseProvider;
    private readonly DatabaseOptions _options;

    /// <summary>
    /// 初始化 DatabaseCfgSource 实例
    /// </summary>
    /// <param name="options">数据库连接选项</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    /// <param name="name">配置源名称（可选）</param>
    public DatabaseCfgSource(DatabaseOptions options, int level, bool isPrimaryWriter, string? name = null)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        Name = name ?? $"Database:{options.Table}";
        _databaseProvider = CreateProvider(options.Provider!);
    }

    /// <summary>
    /// 根据提供程序名称创建 SqlSugar 数据库提供程序
    /// </summary>
    /// <param name="providerName">数据库提供程序名称</param>
    /// <returns>SqlSugar 数据库提供程序实例</returns>
    /// <exception cref="ArgumentException">当提供程序名称不受支持时抛出</exception>
    private static SqlSugarDatabaseProvider CreateProvider(string providerName)
    {
        var dbType = providerName.ToLowerInvariant() switch
        {
            "sqlserver" => DbType.SqlServer,
            "mysql" => DbType.MySql,
            "postgresql" or "postgres" => DbType.PostgreSQL,
            "oracle" => DbType.Oracle,
            "sqlite" => DbType.Sqlite,
            _ => throw new ArgumentException($"不支持的数据库提供程序: '{providerName}'", nameof(providerName))
        };
        return new SqlSugarDatabaseProvider(dbType);
    }

    /// <summary>
    /// 获取配置层级，数值越大优先级越高
    /// </summary>
    public int Level { get; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <summary>
    /// 获取是否可写，数据库支持通过 API 写入配置，因此始终为 true
    /// </summary>
    public bool IsWriteable => true;

    /// <summary>
    /// 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标
    /// </summary>
    public bool IsPrimaryWriter { get; }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        try
        {
            using var cts = new CancellationTokenSource(_options.CommandTimeoutMs);
            var configData = _databaseProvider.LoadConfigurationAsync(
                _options.ConnectionString!, _options.Table!, _options.KeyColumn!, _options.ValueColumn!,
                cts.Token).GetAwaiter().GetResult();
            return configData.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)).ToList();
        }
        catch
        {
            return Enumerable.Empty<KeyValuePair<string, string?>>();
        }
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的内存配置源，从数据库加载数据
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource 实例</returns>
    public IConfigurationSource BuildSource()
    {
        var data = new List<KeyValuePair<string, string?>>();
        try
        {
            using var cts = new CancellationTokenSource(_options.CommandTimeoutMs);
            var configData = _databaseProvider.LoadConfigurationAsync(
                _options.ConnectionString!, _options.Table!, _options.KeyColumn!, _options.ValueColumn!,
                cts.Token).GetAwaiter().GetResult();

            foreach (var (key, value) in configData)
                data.Add(new KeyValuePair<string, string?>(key, value));
        }
        catch { }

        return new MemoryConfigurationSource { InitialData = data };
    }

    /// <summary>
    /// 应用配置更改到数据库
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(_options.CommandTimeoutMs);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        await _databaseProvider.ApplyChangesAsync(
            _options.ConnectionString!, _options.Table!, _options.KeyColumn!, _options.ValueColumn!,
            changes, linked.Token).ConfigureAwait(false);
    }
}
