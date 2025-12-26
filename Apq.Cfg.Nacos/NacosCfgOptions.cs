namespace Apq.Cfg.Nacos;

/// <summary>
/// Nacos 配置选项
/// </summary>
public sealed class NacosCfgOptions
{
    /// <summary>
    /// Nacos 服务地址列表，多个地址用逗号分隔，默认 "localhost:8848"
    /// </summary>
    public string ServerAddresses { get; set; } = "localhost:8848";

    /// <summary>
    /// 命名空间 ID，默认 "public"
    /// </summary>
    public string Namespace { get; set; } = "public";

    /// <summary>
    /// 配置的 DataId
    /// </summary>
    public string DataId { get; set; } = "";

    /// <summary>
    /// 配置分组，默认 "DEFAULT_GROUP"
    /// </summary>
    public string Group { get; set; } = "DEFAULT_GROUP";

    /// <summary>
    /// 用户名（可选）
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码（可选）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Access Key（可选，用于阿里云 MSE）
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// Secret Key（可选，用于阿里云 MSE）
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// 连接超时时间（毫秒），默认 10000
    /// </summary>
    public int ConnectTimeoutMs { get; set; } = 10000;

    /// <summary>
    /// 配置数据格式，默认 Json
    /// </summary>
    public NacosDataFormat DataFormat { get; set; } = NacosDataFormat.Json;
}

/// <summary>
/// Nacos 数据格式
/// </summary>
public enum NacosDataFormat
{
    /// <summary>
    /// JSON 格式
    /// </summary>
    Json,

    /// <summary>
    /// YAML 格式
    /// </summary>
    Yaml,

    /// <summary>
    /// Properties 格式（key=value）
    /// </summary>
    Properties
}
