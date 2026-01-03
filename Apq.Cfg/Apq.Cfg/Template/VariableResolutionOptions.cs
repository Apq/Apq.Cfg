namespace Apq.Cfg.Template;

/// <summary>
/// 变量解析选项
/// </summary>
public sealed class VariableResolutionOptions
{
    /// <summary>
    /// 变量前缀，默认 "${"
    /// </summary>
    public string VariablePrefix { get; set; } = "${";

    /// <summary>
    /// 变量后缀，默认 "}"
    /// </summary>
    public string VariableSuffix { get; set; } = "}";

    /// <summary>
    /// 前缀分隔符，默认 ":"
    /// </summary>
    /// <remarks>
    /// 用于分隔解析器前缀和变量名，如 ${ENV:PATH} 中的 ":"
    /// </remarks>
    public string PrefixSeparator { get; set; } = ":";

    /// <summary>
    /// 防止过深嵌套或潜在的无限递归
    /// </summary>
    public int MaxRecursionDepth { get; set; } = 10;

    /// <summary>
    /// 未解析变量的处理方式
    /// </summary>
    public UnresolvedVariableBehavior UnresolvedBehavior { get; set; } = UnresolvedVariableBehavior.Keep;

    /// <summary>
    /// 是否缓存解析结果
    /// </summary>
    public bool CacheResults { get; set; } = true;

    /// <summary>
    /// 配置变更时是否清除缓存
    /// </summary>
    public bool InvalidateCacheOnChange { get; set; } = true;

    /// <summary>
    /// 自定义解析器列表
    /// </summary>
    public IList<IVariableResolver> Resolvers { get; } = new List<IVariableResolver>();

    /// <summary>
    /// 创建默认选项（包含所有内置解析器）
    /// </summary>
    public static VariableResolutionOptions Default
    {
        get
        {
            var options = new VariableResolutionOptions();
            foreach (var resolver in VariableResolvers.All)
            {
                options.Resolvers.Add(resolver);
            }
            return options;
        }
    }
}

/// <summary>
/// 未解析变量的处理方式
/// </summary>
public enum UnresolvedVariableBehavior
{
    /// <summary>
    /// 保留原始变量表达式
    /// </summary>
    Keep,

    /// <summary>
    /// 替换为空字符串
    /// </summary>
    Empty,

    /// <summary>
    /// 抛出异常
    /// </summary>
    Throw
}
