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
    /// 是否启用 CORS，默认 true（允许 WebUI 跨域访问）
    /// </summary>
    public bool EnableCors { get; set; } = true;

    /// <summary>
    /// CORS 允许的源
    /// </summary>
    public string[] CorsOrigins { get; set; } = ["*"];

    #region OpenAPI 文档配置

    /// <summary>
    /// 是否启用 OpenAPI 文档，默认 true
    /// （.NET 8 使用 Swagger UI，.NET 10+ 使用 Scalar）
    /// </summary>
    public bool OpenApiEnabled { get; set; } = true;

    /// <summary>
    /// API 文档标题
    /// </summary>
    public string OpenApiTitle { get; set; } = "Apq.Cfg Web API";

    /// <summary>
    /// API 文档描述
    /// </summary>
    public string OpenApiDescription { get; set; } = "Apq.Cfg 配置管理 RESTful API";

    /// <summary>
    /// API 版本
    /// </summary>
    public string OpenApiVersion { get; set; } = "v1";

    /// <summary>
    /// API 文档 UI 路由前缀
    /// （.NET 8 默认 "swagger"，.NET 10+ 默认 "scalar"）
    /// </summary>
    public string OpenApiRoutePrefix { get; set; } =
#if NET8_0
        "swagger";
#else
        "/scalar/v1";
#endif

    /// <summary>
    /// 是否在文档 UI 中显示认证按钮
    /// </summary>
    public bool OpenApiShowAuthorizationButton { get; set; } = true;

    #endregion
}
