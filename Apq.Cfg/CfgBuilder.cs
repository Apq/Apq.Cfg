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
        bool isPrimaryWriter = false)
    {
        _sources.Add(new JsonFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
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

    /// <summary>
    /// 构建配置根实例
    /// </summary>
    public ICfgRoot Build()
    {
        return new MergedCfgRoot(_sources);
    }
}
