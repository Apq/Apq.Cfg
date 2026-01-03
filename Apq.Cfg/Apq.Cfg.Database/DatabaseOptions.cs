namespace Apq.Cfg.Database;

/// <summary>
/// 数据库配置选项
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// 数据库提供程序: SqlServer/MySql/PostgreSql/Oracle/SQLite
    /// </summary>
    public string? Provider { get; set; }
    public string? ConnectionString { get; set; }
    public string? Table { get; set; }
    public string? KeyColumn { get; set; }
    public string? ValueColumn { get; set; }
    public int CommandTimeoutMs { get; set; } = 5000;
}
