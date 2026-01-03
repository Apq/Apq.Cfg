namespace Apq.Cfg.WebUI.Models;

/// <summary>
/// 认证类型
/// </summary>
public enum AuthType
{
    None,
    ApiKey,
    JwtBearer
}

/// <summary>
/// 应用端点配置
/// </summary>
public class AppEndpoint
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// WebApi 地址（如 http://app-a:5000/api/apqcfg）
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// 认证方式
    /// </summary>
    public AuthType AuthType { get; set; } = AuthType.None;

    /// <summary>
    /// API Key（当 AuthType 为 ApiKey 时）
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// JWT Token（当 AuthType 为 JwtBearer 时）
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
