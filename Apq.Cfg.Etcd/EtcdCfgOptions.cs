namespace Apq.Cfg.Etcd;

/// <summary>
/// Etcd 配置选项
/// </summary>
public sealed class EtcdCfgOptions
{
    /// <summary>
    /// Etcd 服务端点列表，默认 ["localhost:2379"]
    /// </summary>
    public string[] Endpoints { get; set; } = ["localhost:2379"];

    /// <summary>
    /// 用户名（可选）
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码（可选）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// CA 证书路径（可选，用于 TLS）
    /// </summary>
    public string? CaCertPath { get; set; }

    /// <summary>
    /// 客户端证书路径（可选，用于 mTLS）
    /// </summary>
    public string? ClientCertPath { get; set; }

    /// <summary>
    /// 客户端私钥路径（可选，用于 mTLS）
    /// </summary>
    public string? ClientKeyPath { get; set; }

    /// <summary>
    /// KV 键前缀，默认 "/config/"
    /// </summary>
    public string KeyPrefix { get; set; } = "/config/";

    /// <summary>
    /// 是否启用热重载，默认 true
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

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
    public EtcdDataFormat DataFormat { get; set; } = EtcdDataFormat.KeyValue;

    /// <summary>
    /// 当 DataFormat 为 Json 时，指定要读取的单个 key
    /// </summary>
    public string? SingleKey { get; set; }
}

/// <summary>
/// Etcd 数据格式
/// </summary>
public enum EtcdDataFormat
{
    /// <summary>
    /// 每个 KV 键对应一个配置项
    /// </summary>
    KeyValue,

    /// <summary>
    /// 单个 key 存储 JSON 格式的配置
    /// </summary>
    Json
}
