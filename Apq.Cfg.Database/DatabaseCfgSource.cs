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

    public DatabaseCfgSource(DatabaseOptions options, int level, bool isPrimaryWriter)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        _databaseProvider = CreateProvider(options.Provider!);
    }

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

    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

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

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(_options.CommandTimeoutMs);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        await _databaseProvider.ApplyChangesAsync(
            _options.ConnectionString!, _options.Table!, _options.KeyColumn!, _options.ValueColumn!,
            changes, linked.Token).ConfigureAwait(false);
    }
}
