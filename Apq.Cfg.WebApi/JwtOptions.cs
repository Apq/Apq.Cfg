namespace Apq.Cfg.WebApi;

/// <summary>
/// JWT 认证配置选项
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// 认证服务器地址
    /// </summary>
    public string Authority { get; set; } = "";

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = "";

    /// <summary>
    /// 是否要求 HTTPS 元数据，默认 true
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// 是否验证颁发者，默认 true
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// 是否验证受众，默认 true
    /// </summary>
    public bool ValidateAudience { get; set; } = true;
}
