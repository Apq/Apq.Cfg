using Apq.Cfg.Template;

namespace Apq.Cfg;

/// <summary>
/// CfgBuilder 模板扩展方法
/// </summary>
public static class CfgBuilderTemplateExtensions
{
    /// <summary>
    /// 启用变量解析功能
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <remarks>
    /// 启用后，可以使用 GetResolved() 方法获取解析变量后的配置值。
    /// 此方法主要用于配置自定义解析选项，如果使用默认选项，可以直接调用 GetResolved()。
    /// </remarks>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .ConfigureVariableResolution(options =>
    ///     {
    ///         options.MaxRecursionDepth = 5;
    ///         options.UnresolvedBehavior = UnresolvedVariableBehavior.Throw;
    ///     })
    ///     .Build();
    ///
    /// // 使用解析后的值
    /// var logPath = cfg.GetResolved("App:LogPath");
    /// </code>
    /// </example>
    public static CfgBuilder ConfigureVariableResolution(this CfgBuilder builder, Action<VariableResolutionOptions> configure)
    {
        var options = VariableResolutionOptions.Default;
        configure(options);
        TemplateEngineRegistry.SetOptions(options);
        return builder;
    }

    /// <summary>
    /// 添加自定义变量解析器
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="resolver">变量解析器</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddVariableResolver(new CustomResolver())
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddVariableResolver(this CfgBuilder builder, IVariableResolver resolver)
    {
        TemplateEngineRegistry.AddResolver(resolver);
        return builder;
    }
}

/// <summary>
/// 模板引擎注册表（内部使用）
/// </summary>
internal static class TemplateEngineRegistry
{
    private static VariableResolutionOptions? _options;
    private static readonly List<IVariableResolver> _additionalResolvers = new();
    private static TemplateEngine? _engine;
    private static readonly object _lock = new();

    public static void SetOptions(VariableResolutionOptions options)
    {
        lock (_lock)
        {
            _options = options;
            _engine = null; // 重置引擎
        }
    }

    public static void AddResolver(IVariableResolver resolver)
    {
        lock (_lock)
        {
            _additionalResolvers.Add(resolver);
            _engine = null; // 重置引擎
        }
    }

    public static TemplateEngine GetEngine()
    {
        if (_engine != null)
            return _engine;

        lock (_lock)
        {
            if (_engine != null)
                return _engine;

            var options = _options ?? VariableResolutionOptions.Default;

            foreach (var resolver in _additionalResolvers)
            {
                if (!options.Resolvers.Contains(resolver))
                    options.Resolvers.Add(resolver);
            }

            _engine = new TemplateEngine(options);
            return _engine;
        }
    }

    public static void Reset()
    {
        lock (_lock)
        {
            _options = null;
            _additionalResolvers.Clear();
            _engine = null;
        }
    }
}
