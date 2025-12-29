using System.Text.RegularExpressions;
using Apq.Cfg.Security;

namespace Apq.Cfg.Crypto;

/// <summary>
/// 敏感值脱敏器
/// </summary>
/// <remarks>
/// 性能优化：
/// 1. 使用 Lazy 延迟编译正则表达式
/// 2. 使用 RegexOptions.Compiled 提升匹配性能
/// </remarks>
public class SensitiveMasker : IValueMasker
{
    private readonly MaskingOptions _options;

    // 缓存编译后的正则表达式，延迟初始化
    private readonly Lazy<Regex[]> _compiledPatterns;

    /// <summary>
    /// 初始化敏感值脱敏器
    /// </summary>
    /// <param name="options">脱敏选项，为 null 时使用默认选项</param>
    public SensitiveMasker(MaskingOptions? options = null)
    {
        _options = options ?? new MaskingOptions();

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
    /// 判断是否应该脱敏该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>如果应该脱敏返回 true，否则返回 false</returns>
    public bool ShouldMask(string key)
    {
        return _compiledPatterns.Value.Any(regex => regex.IsMatch(key));
    }

    /// <summary>
    /// 脱敏处理
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>脱敏后的值</returns>
    public string Mask(string key, string? value)
    {
        if (value == null)
            return _options.NullPlaceholder;

        if (value.Length <= _options.VisibleChars * 2)
            return _options.MaskString;

        return value.Substring(0, _options.VisibleChars)
            + _options.MaskString
            + value.Substring(value.Length - _options.VisibleChars);
    }
}
