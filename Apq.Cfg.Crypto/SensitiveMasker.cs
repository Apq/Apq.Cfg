using System.Text.RegularExpressions;
using Apq.Cfg.Security;

namespace Apq.Cfg.Crypto;

/// <summary>
/// 敏感值脱敏器
/// </summary>
public class SensitiveMasker : IValueMasker
{
    private readonly MaskingOptions _options;

    /// <summary>
    /// 初始化敏感值脱敏器
    /// </summary>
    /// <param name="options">脱敏选项，为 null 时使用默认选项</param>
    public SensitiveMasker(MaskingOptions? options = null)
    {
        _options = options ?? new MaskingOptions();
    }

    /// <summary>
    /// 判断是否应该脱敏该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>如果应该脱敏返回 true，否则返回 false</returns>
    public bool ShouldMask(string key)
    {
        return _options.SensitiveKeyPatterns.Any(pattern =>
            MatchPattern(key, pattern));
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

    /// <summary>
    /// 简单通配符匹配实现
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="pattern">通配符模式</param>
    /// <returns>是否匹配</returns>
    private static bool MatchPattern(string key, string pattern)
    {
        var regex = "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        return Regex.IsMatch(key, regex, RegexOptions.IgnoreCase);
    }
}
