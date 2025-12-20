namespace Apq.Cfg.Redis;

/// <summary>
/// Redis 配置选项
/// </summary>
public sealed class RedisOptions
{
    public string? ConnectionString { get; set; }
    public string? KeyPrefix { get; set; }
    public int? Database { get; set; }
    public string? Channel { get; set; }
    public int ConnectTimeoutMs { get; set; } = 5000;
    public int OperationTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// SCAN 命令每次返回的键数量，默认 250
    /// </summary>
    public int ScanPageSize { get; set; } = 250;
}
