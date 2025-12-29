using System.Collections.Concurrent;
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
/// 3. 使用 StringComparison.Ordinal 进行前缀检查
/// 4. 简单模式使用 string.Contains 快速路径
/// 5. 缓存敏感键匹配结果
/// </remarks>
public class EncryptionTransformer : IValueTransformer
{
    private readonly ICryptoProvider _provider;
    private readonly EncryptionOptions _options;

    // 缓存编译后的正则表达式，延迟初始化
    private readonly Lazy<Regex[]> _compiledPatterns;

    // 简单模式的快速路径（*Keyword* 形式）
    private readonly Lazy<string[]> _simpleContainsPatterns;

    // 缓存敏感键匹配结果
    private readonly ConcurrentDictionary<string, bool> _sensitiveKeyCache = new();

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
    /// 判断是否应该处理该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>如果应该处理返回 true，否则返回 false</returns>
    public bool ShouldTransform(string key, string? value)
    {
        // 读取时：检查是否有加密前缀（使用 Ordinal 比较）
        if (value != null && value.StartsWith(_options.EncryptedPrefix, StringComparison.Ordinal))
            return true;

        // 写入时：使用缓存的匹配结果
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

        if (!value.StartsWith(_options.EncryptedPrefix, StringComparison.Ordinal))
            return value;

        var cipherText = value.AsSpan(_options.EncryptedPrefix.Length).ToString();
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

        // 已加密的不重复加密（使用 Ordinal 比较）
        if (value.StartsWith(_options.EncryptedPrefix, StringComparison.Ordinal))
            return value;

        // 使用缓存检查是否需要加密
        if (!_options.AutoEncryptOnWrite || !MatchSensitiveKey(key))
            return value;

        var cipherText = _provider.Encrypt(value);
        return string.Concat(_options.EncryptedPrefix, cipherText);
    }

    /// <summary>
    /// 使用缓存匹配敏感键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>是否匹配任一敏感键模式</returns>
    private bool MatchSensitiveKey(string key)
    {
        return _sensitiveKeyCache.GetOrAdd(key, k =>
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
    /// 清除敏感键匹配缓存
    /// </summary>
    public void ClearCache()
    {
        _sensitiveKeyCache.Clear();
    }
}
