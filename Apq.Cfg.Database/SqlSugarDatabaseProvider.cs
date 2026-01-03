using System.Text.RegularExpressions;
using SqlSugar;

namespace Apq.Cfg.Database;

/// <summary>
///     基于 SqlSugar 的统一数据库配置提供程序
/// </summary>
internal sealed partial class SqlSugarDatabaseProvider
{
    private readonly DbType _dbType;

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)]
    private static partial Regex SafeIdentifierRegex();
#else
    private static readonly Regex _safeIdentifierRegex = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
    private static Regex SafeIdentifierRegex() => _safeIdentifierRegex;
#endif

    public SqlSugarDatabaseProvider(DbType dbType)
    {
        _dbType = dbType;
    }

    private static void ValidateIdentifier(string identifier, string paramName)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException($"标识符 '{paramName}' 不能为空", paramName);

        if (identifier.Length > 128)
            throw new ArgumentException($"标识符 '{paramName}' 长度不能超过 128 个字符", paramName);

        if (!SafeIdentifierRegex().IsMatch(identifier))
            throw new ArgumentException(
                $"标识符 '{paramName}' 包含非法字符。只允许字母、数字和下划线，且不能以数字开头。值: '{identifier}'",
                paramName);
    }

    public async Task<Dictionary<string, string?>> LoadConfigurationAsync(
        string connectionString,
        string tableName,
        string keyColumn,
        string valueColumn,
        CancellationToken cancellationToken = default)
    {
        ValidateIdentifier(tableName, nameof(tableName));
        ValidateIdentifier(keyColumn, nameof(keyColumn));
        ValidateIdentifier(valueColumn, nameof(valueColumn));

        var result = new Dictionary<string, string?>();

        using var db = CreateDb(connectionString);
        var query = $"SELECT {Quote(keyColumn)}, {Quote(valueColumn)} FROM {Quote(tableName)}";
        var dataTable = await Task.Run(() => db.Ado.GetDataTable(query), cancellationToken);

        foreach (System.Data.DataRow row in dataTable.Rows)
        {
            var key = row[keyColumn]?.ToString();
            if (key != null)
            {
                var value = row[valueColumn] == DBNull.Value ? null : row[valueColumn]?.ToString();
                result[key] = value;
            }
        }

        return result;
    }

    public async Task ApplyChangesAsync(
        string connectionString,
        string tableName,
        string keyColumn,
        string valueColumn,
        IReadOnlyDictionary<string, string?> changes,
        CancellationToken cancellationToken = default)
    {
        ValidateIdentifier(tableName, nameof(tableName));
        ValidateIdentifier(keyColumn, nameof(keyColumn));
        ValidateIdentifier(valueColumn, nameof(valueColumn));

        using var db = CreateDb(connectionString);

        await Task.Run(() =>
        {
            // 确保表存在
            EnsureTableExists(db, tableName, keyColumn, valueColumn);

            db.Ado.BeginTran();
            try
            {
                foreach (var (key, value) in changes)
                {
                    var existsQuery = $"SELECT COUNT(1) FROM {Quote(tableName)} WHERE {Quote(keyColumn)} = @key";
                    var exists = db.Ado.GetInt(existsQuery, new { key }) > 0;

                    if (exists)
                    {
                        var updateQuery = $"UPDATE {Quote(tableName)} SET {Quote(valueColumn)} = @value WHERE {Quote(keyColumn)} = @key";
                        db.Ado.ExecuteCommand(updateQuery, new { key, value });
                    }
                    else
                    {
                        var insertQuery = $"INSERT INTO {Quote(tableName)} ({Quote(keyColumn)}, {Quote(valueColumn)}) VALUES (@key, @value)";
                        db.Ado.ExecuteCommand(insertQuery, new { key, value });
                    }
                }

                db.Ado.CommitTran();
            }
            catch
            {
                db.Ado.RollbackTran();
                throw;
            }
        }, cancellationToken);
    }

    private SqlSugarClient CreateDb(string connectionString)
    {
        return new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = _dbType,
            IsAutoCloseConnection = false
        });
    }

    private void EnsureTableExists(SqlSugarClient db, string tableName, string keyColumn, string valueColumn)
    {
        var createSql = _dbType switch
        {
            DbType.SqlServer => $@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{tableName}')
                CREATE TABLE [{tableName}] (
                    [{keyColumn}] NVARCHAR(512) NOT NULL PRIMARY KEY,
                    [{valueColumn}] NVARCHAR(MAX) NULL
                )",
            DbType.MySql => $@"
                CREATE TABLE IF NOT EXISTS `{tableName}` (
                    `{keyColumn}` VARCHAR(512) NOT NULL PRIMARY KEY,
                    `{valueColumn}` TEXT NULL
                )",
            DbType.PostgreSQL => $@"
                CREATE TABLE IF NOT EXISTS ""{tableName}"" (
                    ""{keyColumn}"" VARCHAR(512) NOT NULL PRIMARY KEY,
                    ""{valueColumn}"" TEXT NULL
                )",
            DbType.Sqlite => $@"
                CREATE TABLE IF NOT EXISTS ""{tableName}"" (
                    ""{keyColumn}"" TEXT NOT NULL PRIMARY KEY,
                    ""{valueColumn}"" TEXT NULL
                )",
            DbType.Oracle => $@"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE ""{tableName.ToUpperInvariant()}"" (
                        ""{keyColumn.ToUpperInvariant()}"" VARCHAR2(512) NOT NULL PRIMARY KEY,
                        ""{valueColumn.ToUpperInvariant()}"" CLOB NULL
                    )';
                EXCEPTION WHEN OTHERS THEN
                    IF SQLCODE != -955 THEN RAISE; END IF;
                END;",
            _ => throw new NotSupportedException($"不支持的数据库类型: {_dbType}")
        };

        db.Ado.ExecuteCommand(createSql);
    }

    private string Quote(string identifier)
    {
        return _dbType switch
        {
            DbType.SqlServer => $"[{identifier}]",
            DbType.MySql => $"`{identifier}`",
            DbType.PostgreSQL => $"\"{identifier}\"",
            DbType.Oracle => $"\"{identifier.ToUpperInvariant()}\"",
            DbType.Sqlite => $"\"{identifier}\"",
            _ => identifier
        };
    }
}
