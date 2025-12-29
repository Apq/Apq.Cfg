using System.Text.RegularExpressions;
using Apq.Cfg.Security;

namespace Apq.Cfg.Crypto;

/// <summary>
/// 加密值转换器
/// </summary>
/// <remarks>
/// 性能优化：
/// 1. 使用 Lazy 延迟编译正则表达式
/// 2. 使用 RegexOptions.Compiled 提升匹配性能
/// </remarks>
public class EncryptionTransformer : IValueTransformer
{
    private readonly ICryptoProvider _provider;
    private readonly EncryptionOptions _options;

    // 缓存编译后的正则表达式，延迟初始化
    private readonly Lazy<Regex[]> _compiledPatterns;

    /// <summary>
    /// 转换器名称
    /// </summary>
    public string Name => "Encryption";

    /// <summary>
    /// 优先级，数值越大优先级越高
    /// </summary>
    public int Priority => 100;

    /// <summary>
    /// 初始化加密值转换器
    /// </summary>
    /// <param name="provider">加密提供者</param>
    /// <param name="options">加密选项，为 null 时使用默认选项</param>
    public EncryptionTransformer(ICryptoProvider provider, EncryptionOptions? options = null)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _options = options ?? new EncryptionOptions();

        // 延迟编译正则表达式，首次使用时才编译
        _compiledPatterns = new Lazy<Regex[]>(() =>
            _options.SensitiveKeyPatterns
                .Select(pattern => new Regex(
                    "^" + Regex.Escape(pattern)
                        .Replace("\\*", ".*")
                        .Replace("\\?", ".") + "$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled))
                .ToArray());
    }

    /// <summary>
    /// 判断是否应该处理该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>如果应该处理返回 true，否则返回 false</returns>
    public bool ShouldTransform(string key, string? value)
    {
        // 读取时：检查是否有加密前缀
        if (value?.StartsWith(_options.EncryptedPrefix) == true)
            return true;

        // 写入时：使用缓存的正则表达式匹配敏感键模式
        if (_options.AutoEncryptOnWrite)
        {
            return MatchSensitiveKey(key);
        }

        return false;
    }

    /// <summary>
    /// 读取时转换（解密）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>解密后的值</returns>
    public string? TransformOnRead(string key, string? value)
    {
        if (value == null)
            return null;

        if (!value.StartsWith(_options.EncryptedPrefix))
            return value;

        var cipherText = value.Substring(_options.EncryptedPrefix.Length);
        return _provider.Decrypt(cipherText);
    }

    /// <summary>
    /// 写入时转换（加密）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>加密后的值</returns>
    public string? TransformOnWrite(string key, string? value)
    {
        if (value == null)
            return null;

        // 已加密的不重复加密
        if (value.StartsWith(_options.EncryptedPrefix))
            return value;

        // 使用缓存的正则表达式检查是否需要加密
        if (!_options.AutoEncryptOnWrite || !MatchSensitiveKey(key))
            return value;

        var cipherText = _provider.Encrypt(value);
        return _options.EncryptedPrefix + cipherText;
    }

    /// <summary>
    /// 使用缓存的正则表达式匹配敏感键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>是否匹配任一敏感键模式</returns>
    private bool MatchSensitiveKey(string key)
    {
        return _compiledPatterns.Value.Any(regex => regex.IsMatch(key));
    }
}
