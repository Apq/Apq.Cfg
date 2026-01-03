namespace Apq.Cfg.Validation;

/// <summary>
/// ICfgRoot 验证扩展方法
/// </summary>
public static class CfgRootValidationExtensions
{
    /// <summary>
    /// 使用指定的验证器验证配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="validator">验证器</param>
    /// <returns>验证结果</returns>
    /// <example>
    /// <code>
    /// var validator = new ConfigValidationBuilder()
    ///     .Required("Database:ConnectionString")
    ///     .Range("Database:Port", 1, 65535)
    ///     .Build();
    ///
    /// var result = cfg.Validate(validator);
    /// if (!result.IsValid)
    /// {
    ///     foreach (var error in result.Errors)
    ///     {
    ///         Console.WriteLine($"配置错误: {error}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static ValidationResult Validate(this ICfgRoot cfg, IConfigValidator validator)
    {
        return validator.Validate(cfg);
    }

    /// <summary>
    /// 使用配置委托验证配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="configure">验证规则配置委托</param>
    /// <returns>验证结果</returns>
    /// <example>
    /// <code>
    /// var result = cfg.Validate(v => v
    ///     .Required("Database:ConnectionString")
    ///     .Range("Database:Port", 1, 65535)
    ///     .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"));
    ///
    /// if (!result.IsValid)
    /// {
    ///     foreach (var error in result.Errors)
    ///     {
    ///         Console.WriteLine($"配置错误: {error}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static ValidationResult Validate(this ICfgRoot cfg, Action<ConfigValidationBuilder> configure)
    {
        var builder = new ConfigValidationBuilder();
        configure(builder);
        var validator = builder.Build();
        return validator.Validate(cfg);
    }

    /// <summary>
    /// 验证配置并在失败时抛出异常
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="validator">验证器</param>
    /// <exception cref="ConfigValidationException">验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     cfg.ValidateAndThrow(validator);
    /// }
    /// catch (ConfigValidationException ex)
    /// {
    ///     Console.WriteLine($"配置验证失败: {ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static void ValidateAndThrow(this ICfgRoot cfg, IConfigValidator validator)
    {
        var result = validator.Validate(cfg);
        if (!result.IsValid)
            throw new ConfigValidationException(result);
    }

    /// <summary>
    /// 验证配置并在失败时抛出异常
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="configure">验证规则配置委托</param>
    /// <exception cref="ConfigValidationException">验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     cfg.ValidateAndThrow(v => v
    ///         .Required("Database:ConnectionString")
    ///         .Range("Database:Port", 1, 65535));
    /// }
    /// catch (ConfigValidationException ex)
    /// {
    ///     Console.WriteLine($"配置验证失败: {ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static void ValidateAndThrow(this ICfgRoot cfg, Action<ConfigValidationBuilder> configure)
    {
        var builder = new ConfigValidationBuilder();
        configure(builder);
        var validator = builder.Build();
        var result = validator.Validate(cfg);
        if (!result.IsValid)
            throw new ConfigValidationException(result);
    }

    /// <summary>
    /// 尝试验证配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="validator">验证器</param>
    /// <param name="result">验证结果</param>
    /// <returns>验证是否通过</returns>
    /// <example>
    /// <code>
    /// if (cfg.TryValidate(validator, out var result))
    /// {
    ///     Console.WriteLine("配置验证通过");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"配置验证失败，共 {result.ErrorCount} 个错误");
    /// }
    /// </code>
    /// </example>
    public static bool TryValidate(this ICfgRoot cfg, IConfigValidator validator, out ValidationResult result)
    {
        result = validator.Validate(cfg);
        return result.IsValid;
    }

    /// <summary>
    /// 尝试验证配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="configure">验证规则配置委托</param>
    /// <param name="result">验证结果</param>
    /// <returns>验证是否通过</returns>
    /// <example>
    /// <code>
    /// if (cfg.TryValidate(v => v.Required("Database:ConnectionString"), out var result))
    /// {
    ///     Console.WriteLine("配置验证通过");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"配置验证失败，共 {result.ErrorCount} 个错误");
    /// }
    /// </code>
    /// </example>
    public static bool TryValidate(this ICfgRoot cfg, Action<ConfigValidationBuilder> configure, out ValidationResult result)
    {
        var builder = new ConfigValidationBuilder();
        configure(builder);
        var validator = builder.Build();
        result = validator.Validate(cfg);
        return result.IsValid;
    }
}
