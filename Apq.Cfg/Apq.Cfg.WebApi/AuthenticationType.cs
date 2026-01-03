namespace Apq.Cfg.WebApi;

/// <summary>
/// 认证类型
/// </summary>
public enum AuthenticationType
{
    /// <summary>
    /// 无认证
    /// </summary>
    None,

    /// <summary>
    /// API Key 认证
    /// </summary>
    ApiKey,

    /// <summary>
    /// JWT Bearer 认证
    /// </summary>
    JwtBearer
}
