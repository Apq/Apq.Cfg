namespace Apq.Cfg.Apollo;

/// <summary>
/// Apollo 配置选项
/// </summary>
public sealed class ApolloCfgOptions
{
    /// <summary>
    /// Apollo 应用 ID
    /// </summary>
    public string AppId { get; set; } = "";

    /// <summary>
    /// Apollo Meta Server 地址，默认 "http://localhost:8080"
    /// </summary>
    public string MetaServer { get; set; } = "http://localhost:8080";

    /// <summary>
    /// 集群名称，默认 "default"
    /// </summary>
    public string Cluster { get; set; } = "default";

    /// <summary>
    /// 命名空间列表，默认 ["application"]
    /// </summary>
    public string[] Namespaces { get; set; } = ["application"];

    /// <summary>
    /// 访问密钥（可选，用于访问控制）
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// 是否启用热重载，默认 true
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

    /// <summary>
    /// 连接超时时间，默认 10 秒
    /// </summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 长轮询超时时间，默认 90 秒
    /// </summary>
    public TimeSpan LongPollingTimeout { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// 配置数据格式，默认 Properties
    /// </summary>
    public ApolloDataFormat DataFormat { get; set; } = ApolloDataFormat.Properties;
}

/// <summary>
/// Apollo 数据格式
/// </summary>
public enum ApolloDataFormat
{
    /// <summary>
    /// Properties 格式（key=value）
    /// </summary>
    Properties,

    /// <summary>
    /// JSON 格式
    /// </summary>
    Json,

    /// <summary>
    /// YAML 格式
    /// </summary>
    Yaml
}
