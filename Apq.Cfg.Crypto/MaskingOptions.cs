namespace Apq.Cfg.Crypto;

/// <summary>
/// 脱敏选项
/// </summary>
public class MaskingOptions
{
    /// <summary>
    /// 敏感键模式（支持通配符 * 和 ?）
    /// </summary>
    public List<string> SensitiveKeyPatterns { get; set; } = new()
    {
        "*Password*",
        "*Secret*",
        "*ApiKey*",
        "*ConnectionString*",
        "*Credential*",
        "*Token*"
    };

    /// <summary>
    /// 脱敏字符串
    /// </summary>
    public string MaskString { get; set; } = "***";

    /// <summary>
    /// 保留可见字符数（首尾各保留的字符数）
    /// </summary>
    public int VisibleChars { get; set; } = 3;

    /// <summary>
    /// null 值占位符
    /// </summary>
    public string NullPlaceholder { get; set; } = "[null]";
}
