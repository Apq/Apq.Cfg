namespace Apq.Cfg.Security;

/// <summary>
/// 值转换器配置选项
/// </summary>
public class ValueTransformerOptions
{
    /// <summary>
    /// 是否启用值转换，默认 true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 敏感键模式列表（支持通配符 * 和 ?）
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
    /// 加密值前缀标记（使用花括号避免与配置节分隔符 : 混淆）
    /// </summary>
    public string EncryptedPrefix { get; set; } = "{ENC}";
}
