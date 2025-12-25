namespace Apq.Cfg.Internal;

/// <summary>
/// 键路径解析工具，使用 Span 优化避免字符串分配
/// </summary>
internal static class KeyPathParser
{
    /// <summary>
    /// 配置键分隔符
    /// </summary>
    public const char Separator = ':';

    /// <summary>
    /// 获取键的第一个段（不分配新字符串）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>第一个段</returns>
    public static ReadOnlySpan<char> GetFirstSegment(ReadOnlySpan<char> key)
    {
        var index = key.IndexOf(Separator);
        return index < 0 ? key : key[..index];
    }

    /// <summary>
    /// 获取键的剩余部分（第一个分隔符之后）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>剩余部分，如果没有分隔符则返回空</returns>
    public static ReadOnlySpan<char> GetRemainder(ReadOnlySpan<char> key)
    {
        var index = key.IndexOf(Separator);
        return index < 0 ? ReadOnlySpan<char>.Empty : key[(index + 1)..];
    }

    /// <summary>
    /// 获取键的最后一个段
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>最后一个段</returns>
    public static ReadOnlySpan<char> GetLastSegment(ReadOnlySpan<char> key)
    {
        var index = key.LastIndexOf(Separator);
        return index < 0 ? key : key[(index + 1)..];
    }

    /// <summary>
    /// 获取键的父路径（最后一个分隔符之前）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>父路径，如果没有分隔符则返回空</returns>
    public static ReadOnlySpan<char> GetParentPath(ReadOnlySpan<char> key)
    {
        var index = key.LastIndexOf(Separator);
        return index < 0 ? ReadOnlySpan<char>.Empty : key[..index];
    }

    /// <summary>
    /// 计算键的深度（分隔符数量 + 1）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>深度</returns>
    public static int GetDepth(ReadOnlySpan<char> key)
    {
        if (key.IsEmpty) return 0;

        var count = 1;
        foreach (var c in key)
        {
            if (c == Separator) count++;
        }
        return count;
    }

    /// <summary>
    /// 检查键是否以指定前缀开头（支持精确段匹配）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <param name="prefix">前缀</param>
    /// <returns>是否匹配</returns>
    public static bool StartsWithSegment(ReadOnlySpan<char> key, ReadOnlySpan<char> prefix)
    {
        if (!key.StartsWith(prefix, StringComparison.Ordinal))
            return false;

        // 确保是完整段匹配：前缀后面要么是结尾，要么是分隔符
        if (key.Length == prefix.Length)
            return true;

        return key[prefix.Length] == Separator;
    }

    /// <summary>
    /// 组合两个键路径（避免不必要的分配）
    /// </summary>
    /// <param name="parent">父路径</param>
    /// <param name="child">子键</param>
    /// <returns>组合后的完整键</returns>
    public static string Combine(ReadOnlySpan<char> parent, ReadOnlySpan<char> child)
    {
        if (parent.IsEmpty) return child.ToString();
        if (child.IsEmpty) return parent.ToString();

        return string.Concat(parent, stackalloc char[] { Separator }, child);
    }

    /// <summary>
    /// 组合两个键路径（字符串版本，用于常见场景）
    /// </summary>
    /// <param name="parent">父路径</param>
    /// <param name="child">子键</param>
    /// <returns>组合后的完整键</returns>
    public static string Combine(string? parent, string child)
    {
        if (string.IsNullOrEmpty(parent)) return child;
        if (string.IsNullOrEmpty(child)) return parent;

        return string.Concat(parent, Separator, child);
    }

    /// <summary>
    /// 枚举键的所有段（零分配迭代器）
    /// </summary>
    /// <param name="key">完整键</param>
    /// <returns>段枚举器</returns>
    public static SegmentEnumerator EnumerateSegments(ReadOnlySpan<char> key)
    {
        return new SegmentEnumerator(key);
    }

    /// <summary>
    /// 键段枚举器（ref struct，零堆分配）
    /// </summary>
    public ref struct SegmentEnumerator
    {
        private ReadOnlySpan<char> _remaining;
        private ReadOnlySpan<char> _current;

        internal SegmentEnumerator(ReadOnlySpan<char> key)
        {
            _remaining = key;
            _current = default;
        }

        public readonly ReadOnlySpan<char> Current => _current;

        public bool MoveNext()
        {
            if (_remaining.IsEmpty)
            {
                _current = default;
                return false;
            }

            var index = _remaining.IndexOf(Separator);
            if (index < 0)
            {
                _current = _remaining;
                _remaining = default;
            }
            else
            {
                _current = _remaining[..index];
                _remaining = _remaining[(index + 1)..];
            }
            return true;
        }

        public readonly SegmentEnumerator GetEnumerator() => this;
    }
}
