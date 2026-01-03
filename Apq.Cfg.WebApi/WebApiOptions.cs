namespace Apq.Cfg.WebApi;

/// <summary>
/// Web API 配置选项
/// </summary>
public sealed class WebApiOptions
{
    /// <summary>
    /// 配置节名称，用于从 IConfiguration 绑定
    /// </summary>
    public const string SectionName = "ApqCfg:WebApi";

    /// <summary>
    /// 是否启用 API，默认 true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// API 路由前缀，默认 "/api/apqcfg"
    /// </summary>
    public string RoutePrefix { get; set; } = "/api/apqcfg";

    /// <summary>
    /// 认证方式，默认无认证
    /// </summary>
    public AuthenticationType Authentication { get; set; } = AuthenticationType.None;

    /// <summary>
    /// API Key（当 Authentication 为 ApiKey 时使用）
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// API Key 请求头名称，默认 "X-Api-Key"
    /// </summary>
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";

    /// <summary>
    /// JWT 配置（当 Authentication 为 JwtBearer 时使用）
    /// </summary>
    public JwtOptions? Jwt { get; set; }

    /// <summary>
    /// 是否允许读取配置，默认 true
    /// </summary>
    public bool AllowRead { get; set; } = true;

    /// <summary>
    /// 是否允许写入配置，默认 false
    /// </summary>
    public bool AllowWrite { get; set; } = false;

    /// <summary>
    /// 是否允许删除配置，默认 false
    /// </summary>
    public bool AllowDelete { get; set; } = false;

    /// <summary>
    /// 是否脱敏敏感值，默认 true
    /// </summary>
    public bool MaskSensitiveValues { get; set; } = true;

    /// <summary>
    /// 敏感键模式（支持通配符）
    /// </summary>
    public string[] SensitiveKeyPatterns { get; set; } =
        ["*Password*", "*Secret*", "*Key*", "*Token*", "*ConnectionString*"];

    /// <summary>
    /// 是否启用 CORS，默认 false
    /// </summary>
    public bool EnableCors { get; set; } = false;

    /// <summary>
    /// CORS 允许的源
    /// </summary>
    public string[] CorsOrigins { get; set; } = ["*"];
}
