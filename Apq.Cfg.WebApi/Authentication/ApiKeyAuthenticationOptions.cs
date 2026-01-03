using Microsoft.AspNetCore.Authentication;

namespace Apq.Cfg.WebApi.Authentication;

/// <summary>
/// API Key 认证选项
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// API Key
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// API Key 请求头名称
    /// </summary>
    public string HeaderName { get; set; } = "X-Api-Key";
}
