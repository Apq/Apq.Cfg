using System.Collections;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace Apq.Cfg.Internal;

/// <summary>
/// 高性能只读字典包装器
/// 在 .NET 8+ 使用 FrozenDictionary，其他版本使用普通 Dictionary
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
internal sealed class FastReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
#if NET8_0_OR_GREATER
    private readonly FrozenDictionary<TKey, TValue> _frozen;
#else
    private readonly Dictionary<TKey, TValue> _dict;
#endif

    public FastReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
#if NET8_0_OR_GREATER
        _frozen = source.ToFrozenDictionary();
#else
        _dict = source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
#endif
    }

    public FastReadOnlyDictionary(Dictionary<TKey, TValue> source)
    {
#if NET8_0_OR_GREATER
        _frozen = source.ToFrozenDictionary();
#else
        _dict = source;
#endif
    }

    public TValue this[TKey key] =>
#if NET8_0_OR_GREATER
        _frozen[key];
#else
        _dict[key];
#endif

    public IEnumerable<TKey> Keys =>
#if NET8_0_OR_GREATER
        _frozen.Keys;
#else
        _dict.Keys;
#endif

    public IEnumerable<TValue> Values =>
#if NET8_0_OR_GREATER
        _frozen.Values;
#else
        _dict.Values;
#endif

    public int Count =>
#if NET8_0_OR_GREATER
        _frozen.Count;
#else
        _dict.Count;
#endif

    public bool ContainsKey(TKey key) =>
#if NET8_0_OR_GREATER
        _frozen.ContainsKey(key);
#else
        _dict.ContainsKey(key);
#endif

    public bool TryGetValue(TKey key, out TValue value) =>
#if NET8_0_OR_GREATER
        _frozen.TryGetValue(key, out value!);
#else
        _dict.TryGetValue(key, out value!);
#endif

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
#if NET8_0_OR_GREATER
        _frozen.GetEnumerator();
#else
        _dict.GetEnumerator();
#endif

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// 高性能只读集合包装器
/// 在 .NET 8+ 使用 FrozenSet，其他版本使用 HashSet
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
internal sealed class FastReadOnlySet<T> : IReadOnlyCollection<T>
    where T : notnull
{
#if NET8_0_OR_GREATER
    private readonly FrozenSet<T> _frozen;
#else
    private readonly HashSet<T> _set;
#endif

    public FastReadOnlySet(IEnumerable<T> source)
    {
#if NET8_0_OR_GREATER
        _frozen = source.ToFrozenSet();
#else
        _set = new HashSet<T>(source);
#endif
    }

    public int Count =>
#if NET8_0_OR_GREATER
        _frozen.Count;
#else
        _set.Count;
#endif

    public bool Contains(T item) =>
#if NET8_0_OR_GREATER
        _frozen.Contains(item);
#else
        _set.Contains(item);
#endif

    public IEnumerator<T> GetEnumerator() =>
#if NET8_0_OR_GREATER
        _frozen.GetEnumerator();
#else
        _set.GetEnumerator();
#endif

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// 快速集合工厂方法
/// </summary>
internal static class FastCollections
{
    /// <summary>
    /// 创建高性能只读字典
    /// </summary>
    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnly<TKey, TValue>(
        this Dictionary<TKey, TValue> source)
        where TKey : notnull
    {
        return new FastReadOnlyDictionary<TKey, TValue>(source);
    }

    /// <summary>
    /// 创建高性能只读字典
    /// </summary>
    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnly<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull
    {
        return new FastReadOnlyDictionary<TKey, TValue>(source);
    }

    /// <summary>
    /// 创建高性能只读集合
    /// </summary>
    public static FastReadOnlySet<T> ToFastReadOnlySet<T>(this IEnumerable<T> source)
        where T : notnull
    {
        return new FastReadOnlySet<T>(source);
    }
}
