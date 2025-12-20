using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Apq.Cfg.Sources.Environment;

/// <summary>
/// 环境变量配置源
/// </summary>
internal sealed class EnvVarsCfgSource : ICfgSource
{
    private readonly string? _prefix;

    public EnvVarsCfgSource(string? prefix, int level)
    {
        _prefix = prefix;
        Level = level;
    }

    public int Level { get; }
    public bool IsWriteable => false;
    public bool IsPrimaryWriter => false;

    public IConfigurationSource BuildSource()
        => new EnvironmentVariablesConfigurationSource { Prefix = _prefix };
}
