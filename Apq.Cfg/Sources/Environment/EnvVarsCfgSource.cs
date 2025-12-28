using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Apq.Cfg.Sources.Environment;

/// <summary>
/// 环境变量配置源
/// </summary>
internal sealed class EnvVarsCfgSource : ICfgSource
{
    private readonly string? _prefix;

    /// <summary>
    /// 初始化 EnvVarsCfgSource 实例
    /// </summary>
    /// <param name="prefix">环境变量前缀，为 null 时加载所有环境变量</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    public EnvVarsCfgSource(string? prefix, int level)
    {
        _prefix = prefix;
        Level = level;
    }

    /// <summary>
    /// 获取配置层级，数值越大优先级越高
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// 获取是否可写，环境变量配置源不支持写入，因此始终为 false
    /// </summary>
    public bool IsWriteable => false;

    /// <summary>
    /// 获取是否为主要写入源，环境变量配置源不支持写入，因此始终为 false
    /// </summary>
    public bool IsPrimaryWriter => false;

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的环境变量配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.EnvironmentVariables.EnvironmentVariablesConfigurationSource 实例</returns>
    public IConfigurationSource BuildSource()
        => new EnvironmentVariablesConfigurationSource { Prefix = _prefix };
}
