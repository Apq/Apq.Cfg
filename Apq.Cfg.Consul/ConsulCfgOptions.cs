namespace Apq.Cfg.Consul;

/// <summary>
/// Consul 配置选项
/// </summary>
public sealed class ConsulCfgOptions
{
    /// <summary>
    /// Consul 服务地址，默认 http://localhost:8500
    /// </summary>
    public string Address { get; set; } = "http://localhost:8500";

    /// <summary>
    /// ACL Token（可选）
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 数据中心名称（可选）
    /// </summary>
    public string? Datacenter { get; set; }

    /// <summary>
    /// KV 键前缀，默认 "config/"
    /// </summary>
    public string KeyPrefix { get; set; } = "config/";

    /// <summary>
    /// 是否启用热重载，默认 true
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

    /// <summary>
    /// Blocking Query 等待时间，默认 5 分钟
    /// </summary>
    public TimeSpan WaitTime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 连接超时时间，默认 10 秒
    /// </summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 重连间隔，默认 5 秒
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 配置数据格式，默认 KeyValue（每个 key 一个值）
    /// </summary>
    public ConsulDataFormat DataFormat { get; set; } = ConsulDataFormat.KeyValue;

    /// <summary>
    /// 当 DataFormat 为 Json/Yaml 时，指定要读取的单个 key
    /// </summary>
    public string? SingleKey { get; set; }
}

/// <summary>
/// Consul 数据格式
/// </summary>
public enum ConsulDataFormat
{
    /// <summary>
    /// 每个 KV 键对应一个配置项
    /// </summary>
    KeyValue,

    /// <summary>
    /// 单个 key 存储 JSON 格式的配置
    /// </summary>
    Json,

    /// <summary>
    /// 单个 key 存储 YAML 格式的配置
    /// </summary>
    Yaml
}
