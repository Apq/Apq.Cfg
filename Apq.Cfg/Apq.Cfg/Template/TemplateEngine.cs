using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace Apq.Cfg.Template;

/// <summary>
/// 模板解析引擎
/// </summary>
/// <remarks>
/// 负责解析配置值中的变量引用，支持嵌套解析和循环引用检测
/// </remarks>
public sealed class TemplateEngine
{
    private readonly VariableResolutionOptions _options;
    private readonly Dictionary<string, IVariableResolver> _resolversByPrefix;
    private readonly IVariableResolver? _defaultResolver;
    private readonly ConcurrentDictionary<string, string?> _cache = new();
    private readonly Regex _variablePattern;

    /// <summary>
    /// 创建模板解析引擎
    /// </summary>
    /// <param name="options">解析选项，为 null 时使用默认选项</param>
    public TemplateEngine(VariableResolutionOptions? options = null)
    {
        _options = options ?? VariableResolutionOptions.Default;

        // 构建解析器字典
        _resolversByPrefix = new Dictionary<string, IVariableResolver>(StringComparer.OrdinalIgnoreCase);
        foreach (var resolver in _options.Resolvers)
        {
            if (resolver.Prefix == null)
            {
                _defaultResolver = resolver;
            }
            else
            {
                _resolversByPrefix[resolver.Prefix] = resolver;
            }
        }

        // 如果没有默认解析器，添加配置引用解析器
        _defaultResolver ??= VariableResolvers.Config;

        // 构建正则表达式
        var prefix = Regex.Escape(_options.VariablePrefix);
        var suffix = Regex.Escape(_options.VariableSuffix);
        _variablePattern = new Regex($@"{prefix}([^{suffix}]+){suffix}", RegexOptions.Compiled);
    }

    /// <summary>
    /// 解析模板字符串中的所有变量
    /// </summary>
    /// <param name="template">模板字符串</param>
    /// <param name="cfg">配置根</param>
    /// <returns>解析后的字符串</returns>
    public string? Resolve(string? template, ICfgRoot cfg)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        // 检查是否包含变量
        if (!template.Contains(_options.VariablePrefix))
            return template;

        // 检查缓存
        if (_options.CacheResults && _cache.TryGetValue(template, out var cached))
            return cached;

        var result = ResolveInternal(template, cfg, new HashSet<string>(), 0);

        // 缓存结果
        if (_options.CacheResults)
            _cache[template] = result;

        return result;
    }

    /// <summary>
    /// 清除解析缓存
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    private string? ResolveInternal(string template, ICfgRoot cfg, HashSet<string> resolving, int depth)
    {
        if (depth > _options.MaxRecursionDepth)
        {
            throw new InvalidOperationException(
                $"变量解析超过最大递归深度 {_options.MaxRecursionDepth}，可能存在循环引用");
        }

        var result = new StringBuilder(template);
        var matches = _variablePattern.Matches(template);

        // 从后向前替换，避免索引偏移
        for (var i = matches.Count - 1; i >= 0; i--)
        {
            var match = matches[i];
            var expression = match.Groups[1].Value;
            var resolved = ResolveExpression(expression, cfg, resolving, depth);

            if (resolved != null)
            {
                result.Remove(match.Index, match.Length);
                result.Insert(match.Index, resolved);
            }
            else
            {
                // 处理未解析的变量
                switch (_options.UnresolvedBehavior)
                {
                    case UnresolvedVariableBehavior.Empty:
                        result.Remove(match.Index, match.Length);
                        break;
                    case UnresolvedVariableBehavior.Throw:
                        throw new InvalidOperationException($"无法解析变量: {expression}");
                    case UnresolvedVariableBehavior.Keep:
                    default:
                        // 保留原始表达式
                        break;
                }
            }
        }

        return result.ToString();
    }

    private string? ResolveExpression(string expression, ICfgRoot cfg, HashSet<string> resolving, int depth)
    {
        // 检查循环引用
        if (!resolving.Add(expression))
        {
            throw new InvalidOperationException($"检测到循环引用: {expression}");
        }

        try
        {
            // 解析前缀
            var separatorIndex = expression.IndexOf(_options.PrefixSeparator, StringComparison.Ordinal);
            string? prefix = null;
            string variableName;

            if (separatorIndex > 0)
            {
                var potentialPrefix = expression[..separatorIndex];
                if (_resolversByPrefix.ContainsKey(potentialPrefix))
                {
                    prefix = potentialPrefix;
                    variableName = expression[(separatorIndex + _options.PrefixSeparator.Length)..];
                }
                else
                {
                    // 不是已知前缀，整个表达式作为变量名
                    variableName = expression;
                }
            }
            else
            {
                variableName = expression;
            }

            // 选择解析器
            IVariableResolver resolver;
            if (prefix != null && _resolversByPrefix.TryGetValue(prefix, out var prefixResolver))
            {
                resolver = prefixResolver;
            }
            else
            {
                resolver = _defaultResolver!;
            }

            // 解析变量
            var value = resolver.Resolve(variableName, cfg);

            // 递归解析结果中的变量
            if (value != null && value.Contains(_options.VariablePrefix))
            {
                value = ResolveInternal(value, cfg, resolving, depth + 1);
            }

            return value;
        }
        finally
        {
            resolving.Remove(expression);
        }
    }
}
