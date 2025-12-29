using System.Collections.Concurrent;
using Apq.Cfg.Security;

namespace Apq.Cfg.Internal;

/// <summary>
/// 值转换器链，按优先级顺序执行转换器
/// </summary>
/// <remarks>
/// 性能优化：
/// 1. 解密结果缓存 - 首次解密后缓存明文，后续读取直接返回缓存
/// 2. 使用 StringComparison.Ordinal 进行前缀检查
/// 3. 支持缓存失效（配置变更时）
/// </remarks>
internal sealed class ValueTransformerChain
{
    private readonly IValueTransformer[] _transformers;
    private readonly ValueTransformerOptions _options;

    // 解密结果缓存：key -> 解密后的明文值
    // 使用 ConcurrentDictionary 保证线程安全
    private readonly ConcurrentDictionary<string, string?> _decryptedCache = new();

    // 标记哪些键不需要转换（快速跳过）
    private readonly ConcurrentDictionary<string, bool> _noTransformKeys = new();

    /// <summary>
    /// 初始化值转换器链
    /// </summary>
    /// <param name="transformers">转换器集合</param>
    /// <param name="options">转换器选项</param>
    public ValueTransformerChain(
        IEnumerable<IValueTransformer> transformers,
        ValueTransformerOptions options)
    {
        _transformers = transformers.OrderByDescending(t => t.Priority).ToArray();
        _options = options;
    }

    /// <summary>
    /// 读取时转换（如解密），带缓存优化
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值（原始值，可能是加密的）</param>
    /// <returns>转换后的值（明文）</returns>
    /// <remarks>
    /// 性能优化策略：
    /// 1. 快速路径：如果缓存中有解密结果，直接返回
    /// 2. 快速路径：如果键已知不需要转换，直接返回原值
    /// 3. 慢路径：执行转换并缓存结果
    /// </remarks>
    public string? TransformOnRead(string key, string? value)
    {
        if (!_options.Enabled || value == null)
            return value;

        // 快速路径1：检查解密缓存
        if (_decryptedCache.TryGetValue(key, out var cached))
            return cached;

        // 快速路径2：检查是否已知不需要转换
        if (_noTransformKeys.ContainsKey(key))
            return value;

        // 慢路径：执行转换
        var originalValue = value;
        var transformed = false;

        foreach (var transformer in _transformers)
        {
            if (transformer.ShouldTransform(key, value))
            {
                value = transformer.TransformOnRead(key, value);
                transformed = true;
            }
        }

        // 缓存结果
        if (transformed)
        {
            // 值被转换了，缓存解密结果
            _decryptedCache[key] = value;
        }
        else
        {
            // 值不需要转换，标记为快速跳过
            _noTransformKeys[key] = true;
        }

        return value;
    }

    /// <summary>
    /// 写入时转换（如加密）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值（明文）</param>
    /// <returns>转换后的值（加密后）</returns>
    public string? TransformOnWrite(string key, string? value)
    {
        if (!_options.Enabled || value == null)
            return value;

        foreach (var transformer in _transformers)
        {
            if (transformer.ShouldTransform(key, value))
            {
                value = transformer.TransformOnWrite(key, value);
            }
        }
        return value;
    }

    /// <summary>
    /// 使指定键的缓存失效（配置变更时调用）
    /// </summary>
    /// <param name="key">配置键</param>
    public void InvalidateCache(string key)
    {
        _decryptedCache.TryRemove(key, out _);
        _noTransformKeys.TryRemove(key, out _);
    }

    /// <summary>
    /// 清除所有缓存
    /// </summary>
    public void ClearCache()
    {
        _decryptedCache.Clear();
        _noTransformKeys.Clear();
    }

    /// <summary>
    /// 预热缓存：预先解密所有加密值
    /// </summary>
    /// <param name="keys">要预热的键集合</param>
    /// <param name="valueGetter">获取原始值的委托</param>
    /// <remarks>
    /// 在应用启动时调用此方法可以避免首次访问时的解密延迟
    /// </remarks>
    public void WarmupCache(IEnumerable<string> keys, Func<string, string?> valueGetter)
    {
        foreach (var key in keys)
        {
            var value = valueGetter(key);
            if (value != null)
            {
                // 触发解密并缓存
                TransformOnRead(key, value);
            }
        }
    }
}
