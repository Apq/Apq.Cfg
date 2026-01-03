namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 配置值响应
/// </summary>
public sealed class ConfigValueResponse
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public bool Exists { get; set; }
    public bool IsMasked { get; set; }
}
