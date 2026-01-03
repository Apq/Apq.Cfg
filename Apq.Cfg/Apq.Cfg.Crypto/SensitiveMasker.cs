using System.Collections.Concurrent;
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
/// 3. 简单模式使用 string.Contains 快速路径
/// 4. 缓存 ShouldMask 结果
/// 5. 使用 string.Create 减少字符串分配
/// </remarks>
public class SensitiveMasker : IValueMasker
{
    private readonly MaskingOptions _options;

    // 缓存编译后的正则表达式，延迟初始化
    private readonly Lazy<Regex[]> _compiledPatterns;

    // 简单模式的快速路径（*Keyword* 形式）
    private readonly Lazy<string[]> _simpleContainsPatterns;

    // 缓存 ShouldMask 结果
    private readonly ConcurrentDictionary<string, bool> _shouldMaskCache = new();

    /// <summary>
    /// 初始化敏感值脱敏器
    /// </summary>
    /// <param name="options">脱敏选项，为 null 时使用默认选项</param>
    public SensitiveMasker(MaskingOptions? options = null)
    {
        _options = options ?? new MaskingOptions();

        // 分离简单模式和复杂模式
        _simpleContainsPatterns = new Lazy<string[]>(() =>
            _options.SensitiveKeyPatterns
                .Where(IsSimpleContainsPattern)
                .Select(p => p.Substring(1, p.Length - 2)) // 去掉首尾的 *
                .ToArray());

        // 延迟编译复杂正则表达式
        _compiledPatterns = new Lazy<Regex[]>(() =>
            _options.SensitiveKeyPatterns
                .Where(p => !IsSimpleContainsPattern(p))
                .Select(pattern => new Regex(
                    "^" + Regex.Escape(pattern)
                        .Replace("\\*", ".*")
                        .Replace("\\?", ".") + "$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled))
                .ToArray());
    }

    /// <summary>
    /// 判断是否为简单的 *Keyword* 模式
    /// </summary>
    private static bool IsSimpleContainsPattern(string pattern)
    {
        if (pattern.Length < 3) return false;
        if (pattern[0] != '*' || pattern[^1] != '*') return false;
        // 检查中间是否有通配符
        var middle = pattern.AsSpan(1, pattern.Length - 2);
        return middle.IndexOf('*') < 0 && middle.IndexOf('?') < 0;
    }

    /// <summary>
    /// 判断是否应该脱敏该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>如果应该脱敏返回 true，否则返回 false</returns>
    public bool ShouldMask(string key)
    {
        return _shouldMaskCache.GetOrAdd(key, k =>
        {
            // 快速路径：简单的 Contains 检查
            foreach (var keyword in _simpleContainsPatterns.Value)
            {
                if (k.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 慢路径：正则表达式匹配
            foreach (var regex in _compiledPatterns.Value)
            {
                if (regex.IsMatch(k))
                    return true;
            }

            return false;
        });
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

        var visibleChars = _options.VisibleChars;
        if (value.Length <= visibleChars * 2)
            return _options.MaskString;

        var maskString = _options.MaskString;
        var totalLength = visibleChars + maskString.Length + visibleChars;

        return string.Create(totalLength, (value, visibleChars, maskString), static (span, state) =>
        {
            var (val, visible, mask) = state;
            val.AsSpan(0, visible).CopyTo(span);
            mask.AsSpan().CopyTo(span.Slice(visible));
            val.AsSpan(val.Length - visible).CopyTo(span.Slice(visible + mask.Length));
        });
    }

    /// <summary>
    /// 清除 ShouldMask 缓存
    /// </summary>
    public void ClearCache()
    {
        _shouldMaskCache.Clear();
    }
}
