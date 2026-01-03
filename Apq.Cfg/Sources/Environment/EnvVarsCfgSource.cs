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
    /// <param name="name">配置源名称，为 null 时自动生成</param>
    public EnvVarsCfgSource(string? prefix, int level, string? name = null)
    {
        _prefix = prefix;
        Level = level;
        // 默认名称：有前缀时使用 "EnvVars:{prefix}"，无前缀时使用 "EnvVars"
        Name = name ?? (string.IsNullOrEmpty(prefix) ? "EnvVars" : $"EnvVars:{prefix}");
    }

    /// <inheritdoc />
    public string Name { get; set; }

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

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        var envVars = System.Environment.GetEnvironmentVariables();
        var result = new List<KeyValuePair<string, string?>>();

        foreach (System.Collections.DictionaryEntry entry in envVars)
        {
            var key = entry.Key?.ToString();
            if (key == null) continue;

            // 如果有前缀，只返回匹配前缀的环境变量
            if (!string.IsNullOrEmpty(_prefix))
            {
                if (!key.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                    continue;
                // 移除前缀
                key = key.Substring(_prefix.Length);
            }

            // 将环境变量的 __ 转换为 : (Microsoft.Extensions.Configuration 的约定)
            key = key.Replace("__", ":");

            result.Add(new KeyValuePair<string, string?>(key, entry.Value?.ToString()));
        }

        return result;
    }
}
