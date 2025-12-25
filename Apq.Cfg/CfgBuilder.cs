using System.Text;
using Apq.Cfg.EncodingSupport;
using Apq.Cfg.Sources;
using Apq.Cfg.Sources.Environment;
using Apq.Cfg.Sources.File;

namespace Apq.Cfg;

/// <summary>
/// 配置构建器，用于创建和管理配置源
/// </summary>
public sealed class CfgBuilder
{
    private readonly List<ICfgSource> _sources = new();

    /// <summary>
    /// 添加JSON文件配置源
    /// </summary>
    public CfgBuilder AddJson(string path, int level, bool writeable, bool optional = true, bool reloadOnChange = true,
        bool isPrimaryWriter = false, EncodingOptions? encoding = null)
    {
        _sources.Add(new JsonFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, encoding));
        return this;
    }

    /// <summary>
    /// 添加环境变量配置源
    /// </summary>
    public CfgBuilder AddEnvironmentVariables(int level, string? prefix = null)
    {
        _sources.Add(new EnvVarsCfgSource(prefix, level));
        return this;
    }

    /// <summary>
    /// 添加自定义配置源（供扩展包使用）
    /// </summary>
    public CfgBuilder AddSource(ICfgSource source)
    {
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

    /// <summary>
    /// 构建配置根实例
    /// </summary>
    public ICfgRoot Build()
    {
        return new MergedCfgRoot(_sources);
    }
}
