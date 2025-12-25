using System.Collections.Concurrent;

namespace Apq.Cfg.Internal;

/// <summary>
/// 配置值缓存，用于缓存类型转换结果
/// </summary>
internal sealed class ValueCache
{
    // 使用复合键：(key, typeof(T).TypeHandle.Value) 作为缓存键
    // TypeHandle.Value 是 IntPtr，比 Type 对象更高效
    private readonly ConcurrentDictionary<(string Key, IntPtr TypeHandle), object?> _cache = new();

    // 缓存版本号，用于失效控制
    private long _version;

    /// <summary>
    /// 当前缓存版本
    /// </summary>
    public long Version => Volatile.Read(ref _version);

    /// <summary>
    /// 尝试从缓存获取值
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="value">缓存的值</param>
    /// <returns>是否命中缓存</returns>
    public bool TryGet<T>(string key, out T? value)
    {
        var cacheKey = (key, typeof(T).TypeHandle.Value);
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            value = (T?)cached;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="value">要缓存的值</param>
    public void Set<T>(string key, T? value)
    {
        var cacheKey = (key, typeof(T).TypeHandle.Value);
        _cache[cacheKey] = value;
    }

    /// <summary>
    /// 使指定键的所有类型缓存失效
    /// </summary>
    /// <param name="key">配置键</param>
    public void Invalidate(string key)
    {
        // 移除所有以该 key 开头的缓存项
        var keysToRemove = _cache.Keys.Where(k => k.Key == key).ToList();
        foreach (var k in keysToRemove)
        {
            _cache.TryRemove(k, out _);
        }
        Interlocked.Increment(ref _version);
    }

    /// <summary>
    /// 使多个键的缓存失效
    /// </summary>
    /// <param name="keys">配置键集合</param>
    public void Invalidate(IEnumerable<string> keys)
    {
        var keySet = keys as HashSet<string> ?? new HashSet<string>(keys);
        var keysToRemove = _cache.Keys.Where(k => keySet.Contains(k.Key)).ToList();
        foreach (var k in keysToRemove)
        {
            _cache.TryRemove(k, out _);
        }
        Interlocked.Increment(ref _version);
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
        Interlocked.Increment(ref _version);
    }

    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    public (int Count, long Version) GetStats() => (_cache.Count, Version);
}
