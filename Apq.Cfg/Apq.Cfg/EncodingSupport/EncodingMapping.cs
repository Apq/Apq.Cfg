using System.Text;
using System.Text.RegularExpressions;

namespace Apq.Cfg.EncodingSupport;

/// <summary>
/// 编码映射匹配类型
/// </summary>
public enum EncodingMappingType
{
    /// <summary>
    /// 完整路径匹配（精确匹配）
    /// </summary>
    ExactPath,

    /// <summary>
    /// 通配符匹配（支持 * 和 ?）
    /// </summary>
    Wildcard,

    /// <summary>
    /// 正则表达式匹配
    /// </summary>
    Regex
}

/// <summary>
/// 编码映射规则
/// </summary>
public sealed class EncodingMappingRule
{
    /// <summary>
    /// 匹配模式
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// 匹配类型
    /// </summary>
    public EncodingMappingType Type { get; }

    /// <summary>
    /// 目标编码
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// 优先级（数值越大优先级越高，默认 0）
    /// </summary>
    public int Priority { get; }

    // 缓存编译后的正则表达式
    private readonly Regex? _compiledRegex;

    /// <summary>
    /// 创建编码映射规则
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="type">匹配类型</param>
    /// <param name="encoding">目标编码</param>
    /// <param name="priority">优先级（数值越大优先级越高）</param>
    public EncodingMappingRule(string pattern, EncodingMappingType type, Encoding encoding, int priority = 0)
    {
        Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        Type = type;
        Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        Priority = priority;

        // 预编译正则表达式
        _compiledRegex = type switch
        {
            EncodingMappingType.Regex => new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase),
            EncodingMappingType.Wildcard => new Regex(WildcardToRegex(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase),
            _ => null
        };
    }

    /// <summary>
    /// 检查文件路径是否匹配此规则
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否匹配</returns>
    public bool IsMatch(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        var normalizedPath = NormalizePath(filePath);

        return Type switch
        {
            EncodingMappingType.ExactPath => IsExactMatch(normalizedPath),
            EncodingMappingType.Wildcard or EncodingMappingType.Regex => _compiledRegex?.IsMatch(normalizedPath) ?? false,
            _ => false
        };
    }

    private bool IsExactMatch(string normalizedPath)
    {
        var normalizedPattern = NormalizePath(Pattern);
        return string.Equals(normalizedPath, normalizedPattern, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string path)
    {
        // 统一使用正斜杠，并转为小写进行比较
        return Path.GetFullPath(path).Replace('\\', '/').ToLowerInvariant();
    }

    private static string WildcardToRegex(string pattern)
    {
        // 将通配符模式转换为正则表达式
        // * -> 匹配任意字符（不包括路径分隔符）
        // ** -> 匹配任意字符（包括路径分隔符）
        // ? -> 匹配单个字符

        var normalizedPattern = pattern.Replace('\\', '/');
        var escaped = Regex.Escape(normalizedPattern);

        // 处理 ** （必须在 * 之前处理）
        escaped = escaped.Replace(@"\*\*", ".*");
        // 处理 *
        escaped = escaped.Replace(@"\*", @"[^/]*");
        // 处理 ?
        escaped = escaped.Replace(@"\?", ".");

        // 如果模式不包含路径分隔符，则匹配路径末尾（文件名部分）
        if (!normalizedPattern.Contains('/'))
        {
            return "(?:^|/)" + escaped + "$";
        }

        return "^" + escaped + "$";
    }

    public override string ToString()
    {
        return $"[{Type}] {Pattern} -> {Encoding.EncodingName} (Priority: {Priority})";
    }
}

/// <summary>
/// 编码映射配置
/// </summary>
public sealed class EncodingMappingConfig
{
    private readonly List<EncodingMappingRule> _readRules = new();
    private readonly List<EncodingMappingRule> _writeRules = new();
    private readonly object _lock = new();

    /// <summary>
    /// 读取编码映射规则（只读）
    /// </summary>
    public IReadOnlyList<EncodingMappingRule> ReadRules
    {
        get
        {
            lock (_lock)
            {
                return _readRules.ToList();
            }
        }
    }

    /// <summary>
    /// 写入编码映射规则（只读）
    /// </summary>
    public IReadOnlyList<EncodingMappingRule> WriteRules
    {
        get
        {
            lock (_lock)
            {
                return _writeRules.ToList();
            }
        }
    }

    /// <summary>
    /// 添加读取编码映射规则
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="type">匹配类型</param>
    /// <param name="encoding">目标编码</param>
    /// <param name="priority">优先级（数值越大优先级越高）</param>
    public EncodingMappingConfig AddReadMapping(string pattern, EncodingMappingType type, Encoding encoding, int priority = 0)
    {
        var rule = new EncodingMappingRule(pattern, type, encoding, priority);
        lock (_lock)
        {
            _readRules.Add(rule);
            // 按优先级降序排序
            _readRules.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
        return this;
    }

    /// <summary>
    /// 添加写入编码映射规则
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="type">匹配类型</param>
    /// <param name="encoding">目标编码</param>
    /// <param name="priority">优先级（数值越大优先级越高）</param>
    public EncodingMappingConfig AddWriteMapping(string pattern, EncodingMappingType type, Encoding encoding, int priority = 0)
    {
        var rule = new EncodingMappingRule(pattern, type, encoding, priority);
        lock (_lock)
        {
            _writeRules.Add(rule);
            // 按优先级降序排序
            _writeRules.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
        return this;
    }

    /// <summary>
    /// 添加读取编码映射（完整路径）
    /// </summary>
    public EncodingMappingConfig AddReadMapping(string filePath, Encoding encoding, int priority = 0)
    {
        return AddReadMapping(filePath, EncodingMappingType.ExactPath, encoding, priority);
    }

    /// <summary>
    /// 添加写入编码映射（完整路径）
    /// </summary>
    public EncodingMappingConfig AddWriteMapping(string filePath, Encoding encoding, int priority = 0)
    {
        return AddWriteMapping(filePath, EncodingMappingType.ExactPath, encoding, priority);
    }

    /// <summary>
    /// 移除读取编码映射规则
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="type">匹配类型（可选，不指定则移除所有匹配的模式）</param>
    public EncodingMappingConfig RemoveReadMapping(string pattern, EncodingMappingType? type = null)
    {
        lock (_lock)
        {
            _readRules.RemoveAll(r =>
                r.Pattern.Equals(pattern, StringComparison.OrdinalIgnoreCase) &&
                (type == null || r.Type == type));
        }
        return this;
    }

    /// <summary>
    /// 移除写入编码映射规则
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="type">匹配类型（可选，不指定则移除所有匹配的模式）</param>
    public EncodingMappingConfig RemoveWriteMapping(string pattern, EncodingMappingType? type = null)
    {
        lock (_lock)
        {
            _writeRules.RemoveAll(r =>
                r.Pattern.Equals(pattern, StringComparison.OrdinalIgnoreCase) &&
                (type == null || r.Type == type));
        }
        return this;
    }

    /// <summary>
    /// 清除所有读取映射规则
    /// </summary>
    public EncodingMappingConfig ClearReadMappings()
    {
        lock (_lock)
        {
            _readRules.Clear();
        }
        return this;
    }

    /// <summary>
    /// 清除所有写入映射规则
    /// </summary>
    public EncodingMappingConfig ClearWriteMappings()
    {
        lock (_lock)
        {
            _writeRules.Clear();
        }
        return this;
    }

    /// <summary>
    /// 清除所有映射规则
    /// </summary>
    public EncodingMappingConfig Clear()
    {
        lock (_lock)
        {
            _readRules.Clear();
            _writeRules.Clear();
        }
        return this;
    }

    /// <summary>
    /// 获取文件的读取编码
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>匹配的编码，如果没有匹配则返回 null</returns>
    public Encoding? GetReadEncoding(string filePath)
    {
        lock (_lock)
        {
            // 规则已按优先级排序，返回第一个匹配的
            foreach (var rule in _readRules)
            {
                if (rule.IsMatch(filePath))
                    return rule.Encoding;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取文件的写入编码
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>匹配的编码，如果没有匹配则返回 null</returns>
    public Encoding? GetWriteEncoding(string filePath)
    {
        lock (_lock)
        {
            // 规则已按优先级排序，返回第一个匹配的
            foreach (var rule in _writeRules)
            {
                if (rule.IsMatch(filePath))
                    return rule.Encoding;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取统计信息
    /// </summary>
    public (int ReadRuleCount, int WriteRuleCount) GetStats()
    {
        lock (_lock)
        {
            return (_readRules.Count, _writeRules.Count);
        }
    }
}
