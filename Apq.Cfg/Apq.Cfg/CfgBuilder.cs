using System.Text;
using Apq.Cfg.EncodingSupport;
using Apq.Cfg.Internal;
using Apq.Cfg.Security;
using Apq.Cfg.Sources;
using Apq.Cfg.Sources.Environment;
using Apq.Cfg.Sources.File;
using Apq.Cfg.Validation;

namespace Apq.Cfg;

/// <summary>
/// 配置构建器，用于创建和管理配置源
/// </summary>
public sealed class CfgBuilder
{
    private readonly List<ICfgSource> _sources = new();
    private readonly List<IValueTransformer> _transformers = new();
    private readonly List<IValueMasker> _maskers = new();
    private ValueTransformerOptions _transformerOptions = new();

    /// <summary>
    /// 添加JSON文件配置源
    /// </summary>
    /// <param name="path">JSON文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Json"/> (0)</param>
    /// <param name="writeable">是否可写，默认为false</param>
    /// <param name="optional">是否为可选文件，默认为true</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载，默认为true</param>
    /// <param name="isPrimaryWriter">是否为主要写入器，默认为false</param>
    /// <param name="encoding">编码选项，默认为null</param>
    /// <param name="name">配置源名称，为 null 时使用文件名</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json")  // 使用默认层级 0
    ///     .AddJsonFile("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddJsonFile(string path, int level = CfgSourceLevels.Json, bool writeable = false, bool optional = true, bool reloadOnChange = true,
        bool isPrimaryWriter = false, EncodingOptions? encoding = null, string? name = null)
    {
        _sources.Add(new JsonFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, encoding, name));
        return this;
    }

    /// <summary>
    /// 添加环境变量配置源
    /// </summary>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.EnvironmentVariables"/> (20)</param>
    /// <param name="prefix">环境变量前缀，为null时加载所有环境变量</param>
    /// <param name="name">配置源名称，为 null 时自动生成</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json")
    ///     .AddEnvironmentVariables(prefix: "APP_")  // 使用默认层级 20
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddEnvironmentVariables(int level = CfgSourceLevels.EnvironmentVariables, string? prefix = null, string? name = null)
    {
        _sources.Add(new EnvVarsCfgSource(prefix, level, name));
        return this;
    }

    /// <summary>
    /// 添加自定义配置源（供扩展包使用）
    /// </summary>
    /// <param name="source">配置源实例，实现 ICfgSource 接口</param>
    /// <param name="name">配置源名称，为 null 时使用配置源自身的 Name 属性</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// // 添加自定义配置源
    /// var customSource = new CustomCfgSource();
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddSource(customSource)
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddSource(ICfgSource source, string? name = null)
    {
        if (name != null)
            source.Name = name;
        _sources.Add(source);
        return this;
    }

    /// <summary>
    /// 设置编码检测置信度阈值（0.0-1.0）
    /// </summary>
    /// <param name="threshold">置信度阈值，范围 0.0-1.0，默认 0.6</param>
    public CfgBuilder WithEncodingConfidenceThreshold(float threshold)
    {
        FileCfgSourceBase.EncodingConfidenceThreshold = Math.Clamp(threshold, 0f, 1f);
        return this;
    }

    #region 编码映射配置

    /// <summary>
    /// 添加读取编码映射（完整路径）
    /// </summary>
    /// <param name="filePath">文件完整路径</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 100）</param>
    public CfgBuilder AddReadEncodingMapping(string filePath, Encoding encoding, int priority = 100)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddReadMapping(filePath, EncodingMappingType.ExactPath, encoding, priority);
        return this;
    }

    /// <summary>
    /// 添加读取编码映射（通配符）
    /// </summary>
    /// <param name="pattern">通配符模式（如 "*.json"、"config/*.xml"、"**/*.txt"）</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 0）</param>
    public CfgBuilder AddReadEncodingMappingWildcard(string pattern, Encoding encoding, int priority = 0)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddReadMapping(pattern, EncodingMappingType.Wildcard, encoding, priority);
        return this;
    }

    /// <summary>
    /// 添加读取编码映射（正则表达式）
    /// </summary>
    /// <param name="regexPattern">正则表达式模式</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 0）</param>
    public CfgBuilder AddReadEncodingMappingRegex(string regexPattern, Encoding encoding, int priority = 0)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddReadMapping(regexPattern, EncodingMappingType.Regex, encoding, priority);
        return this;
    }

    /// <summary>
    /// 添加写入编码映射（完整路径）
    /// </summary>
    /// <param name="filePath">文件完整路径</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 100）</param>
    public CfgBuilder AddWriteEncodingMapping(string filePath, Encoding encoding, int priority = 100)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddWriteMapping(filePath, EncodingMappingType.ExactPath, encoding, priority);
        return this;
    }

    /// <summary>
    /// 添加写入编码映射（通配符）
    /// </summary>
    /// <param name="pattern">通配符模式（如 "*.json"、"config/*.xml"、"**/*.txt"）</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 0）</param>
    public CfgBuilder AddWriteEncodingMappingWildcard(string pattern, Encoding encoding, int priority = 0)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddWriteMapping(pattern, EncodingMappingType.Wildcard, encoding, priority);
        return this;
    }

    /// <summary>
    /// 添加写入编码映射（正则表达式）
    /// </summary>
    /// <param name="regexPattern">正则表达式模式</param>
    /// <param name="encoding">编码</param>
    /// <param name="priority">优先级（数值越大优先级越高，默认 0）</param>
    public CfgBuilder AddWriteEncodingMappingRegex(string regexPattern, Encoding encoding, int priority = 0)
    {
        FileCfgSourceBase.EncodingDetector.MappingConfig.AddWriteMapping(regexPattern, EncodingMappingType.Regex, encoding, priority);
        return this;
    }

    /// <summary>
    /// 配置编码映射（使用 Action 委托进行更复杂的配置）
    /// </summary>
    /// <param name="configure">配置委托</param>
    public CfgBuilder ConfigureEncodingMapping(Action<EncodingMappingConfig> configure)
    {
        configure(FileCfgSourceBase.EncodingDetector.MappingConfig);
        return this;
    }

    #endregion

    /// <summary>
    /// 启用编码检测日志
    /// </summary>
    /// <param name="handler">日志处理器</param>
    public CfgBuilder WithEncodingDetectionLogging(Action<EncodingDetectionResult> handler)
    {
        FileCfgSourceBase.EncodingDetector.OnEncodingDetected += handler;
        return this;
    }

    #region 值转换器和脱敏器

    /// <summary>
    /// 添加值转换器（供扩展包使用）
    /// </summary>
    /// <param name="transformer">值转换器实例</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValueTransformer(new EncryptionTransformer(provider))
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddValueTransformer(IValueTransformer transformer)
    {
        _transformers.Add(transformer);
        return this;
    }

    /// <summary>
    /// 添加值脱敏器（供扩展包使用）
    /// </summary>
    /// <param name="masker">值脱敏器实例</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValueMasker(new SensitiveMasker())
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddValueMasker(IValueMasker masker)
    {
        _maskers.Add(masker);
        return this;
    }

    /// <summary>
    /// 配置值转换选项
    /// </summary>
    /// <param name="configure">配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .ConfigureValueTransformer(options =>
    ///     {
    ///         options.EncryptedPrefix = "[ENCRYPTED]";
    ///         options.SensitiveKeyPatterns.Add("*ApiSecret*");
    ///     })
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder ConfigureValueTransformer(Action<ValueTransformerOptions> configure)
    {
        configure(_transformerOptions);
        return this;
    }

    #endregion

    #region 配置验证

    private readonly List<IValidationRule> _validationRules = new();
    private IConfigValidator? _validator;

    /// <summary>
    /// 添加配置验证规则
    /// </summary>
    /// <param name="configure">验证规则配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValidation(v => v
    ///         .Required("Database:ConnectionString")
    ///         .Range("Database:Port", 1, 65535)
    ///         .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
    ///     .Build();
    /// </code>
    /// </example>
    public CfgBuilder AddValidation(Action<ConfigValidationBuilder> configure)
    {
        var builder = new ConfigValidationBuilder();
        configure(builder);
        _validationRules.AddRange(builder.Rules);
        return this;
    }

    /// <summary>
    /// 添加自定义验证器
    /// </summary>
    /// <param name="validator">验证器实例</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public CfgBuilder AddValidator(IConfigValidator validator)
    {
        _validator = validator;
        return this;
    }

    /// <summary>
    /// 获取配置验证器（内部使用）
    /// </summary>
    internal IConfigValidator? GetValidator()
    {
        if (_validator != null)
            return _validator;

        if (_validationRules.Count > 0)
            return new ConfigValidator(_validationRules);

        return null;
    }

    #endregion

    /// <summary>
    /// 构建配置根实例
    /// </summary>
    public ICfgRoot Build()
    {
        var transformerChain = _transformers.Count > 0
            ? new ValueTransformerChain(_transformers, _transformerOptions)
            : null;
        var maskerChain = _maskers.Count > 0
            ? new ValueMaskerChain(_maskers)
            : null;

        return new MergedCfgRoot(_sources, transformerChain, maskerChain);
    }

    /// <summary>
    /// 构建配置根实例并验证
    /// </summary>
    /// <param name="throwOnError">验证失败时是否抛出异常，默认为 true</param>
    /// <returns>配置根实例和验证结果的元组</returns>
    /// <exception cref="ConfigValidationException">当 throwOnError 为 true 且验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// // 构建并验证，失败时抛出异常
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValidation(v => v.Required("Database:ConnectionString"))
    ///     .BuildAndValidate();
    ///
    /// // 构建并验证，不抛出异常
    /// var (cfg2, result) = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValidation(v => v.Required("Database:ConnectionString"))
    ///     .BuildAndValidate(throwOnError: false);
    /// if (!result.IsValid)
    /// {
    ///     // 处理验证错误
    /// }
    /// </code>
    /// </example>
    public (ICfgRoot Config, ValidationResult Result) BuildAndValidate(bool throwOnError = true)
    {
        var cfg = Build();
        var validator = GetValidator();

        if (validator == null)
            return (cfg, ValidationResult.Success);

        var result = validator.Validate(cfg);

        if (throwOnError && !result.IsValid)
            throw new ConfigValidationException(result);

        return (cfg, result);
    }
}
