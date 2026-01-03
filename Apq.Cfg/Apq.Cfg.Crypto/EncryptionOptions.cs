namespace Apq.Cfg.Crypto;

/// <summary>
/// 加密选项
/// </summary>
public class EncryptionOptions
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
    /// 加密值前缀标记（使用花括号避免与配置节分隔符 : 混淆）
    /// </summary>
    public string EncryptedPrefix { get; set; } = "{ENC}";

    /// <summary>
    /// 是否在写入时自动加密匹配的敏感键
    /// </summary>
    public bool AutoEncryptOnWrite { get; set; } = true;
}
