namespace Apq.Cfg.Zookeeper;

/// <summary>
/// Zookeeper 配置选项
/// </summary>
public sealed class ZookeeperCfgOptions
{
    /// <summary>
    /// Zookeeper 连接字符串，默认 localhost:2181
    /// 支持多节点：host1:2181,host2:2181,host3:2181
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:2181";

    /// <summary>
    /// 会话超时时间，默认 30 秒
    /// </summary>
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 连接超时时间，默认 10 秒
    /// </summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 根路径，默认 "/config"
    /// </summary>
    public string RootPath { get; set; } = "/config";

    /// <summary>
    /// 是否启用热重载，默认 true
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

    /// <summary>
    /// 重连间隔，默认 5 秒
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 配置数据格式，默认 KeyValue（每个节点一个值）
    /// </summary>
    public ZookeeperDataFormat DataFormat { get; set; } = ZookeeperDataFormat.KeyValue;

    /// <summary>
    /// 当 DataFormat 为 Json 时，指定要读取的节点路径（相对于 RootPath）
    /// </summary>
    public string? SingleNode { get; set; }

    /// <summary>
    /// 认证方案（可选），如 "digest"
    /// </summary>
    public string? AuthScheme { get; set; }

    /// <summary>
    /// 认证信息（可选），如 "user:password"
    /// </summary>
    public string? AuthInfo { get; set; }
}

/// <summary>
/// Zookeeper 数据格式
/// </summary>
public enum ZookeeperDataFormat
{
    /// <summary>
    /// 每个 ZNode 对应一个配置项，节点路径即为配置键
    /// </summary>
    KeyValue,

    /// <summary>
    /// 单个节点存储 JSON 格式的配置
    /// </summary>
    Json
}
