namespace Apq.Cfg.Template;

/// <summary>
/// 内置变量解析器
/// </summary>
public static class VariableResolvers
{
    /// <summary>
    /// 配置引用解析器（默认）
    /// </summary>
    /// <remarks>
    /// 解析对其他配置键的引用，如 ${App:Name}
    /// </remarks>
    public static IVariableResolver Config { get; } = new ConfigVariableResolver();

    /// <summary>
    /// 环境变量解析器
    /// </summary>
    /// <remarks>
    /// 解析环境变量引用，如 ${ENV:PATH}、${ENV:APPDATA}
    /// </remarks>
    public static IVariableResolver Environment { get; } = new EnvironmentVariableResolver();

    /// <summary>
    /// 系统属性解析器
    /// </summary>
    /// <remarks>
    /// 解析系统属性引用，如 ${SYS:MachineName}、${SYS:UserName}
    /// </remarks>
    public static IVariableResolver System { get; } = new SystemVariableResolver();

    /// <summary>
    /// 获取所有内置解析器
    /// </summary>
    public static IEnumerable<IVariableResolver> All
    {
        get
        {
            yield return Config;
            yield return Environment;
            yield return System;
        }
    }
}

/// <summary>
/// 配置引用解析器
/// </summary>
internal sealed class ConfigVariableResolver : IVariableResolver
{
    public string? Prefix => null;

    public string? Resolve(string variableName, ICfgRoot cfg)
    {
        return cfg.Get(variableName);
    }
}

/// <summary>
/// 环境变量解析器
/// </summary>
internal sealed class EnvironmentVariableResolver : IVariableResolver
{
    public string? Prefix => "ENV";

    public string? Resolve(string variableName, ICfgRoot cfg)
    {
        return System.Environment.GetEnvironmentVariable(variableName);
    }
}

/// <summary>
/// 系统属性解析器
/// </summary>
internal sealed class SystemVariableResolver : IVariableResolver
{
    public string? Prefix => "SYS";

    public string? Resolve(string variableName, ICfgRoot cfg)
    {
        return variableName.ToUpperInvariant() switch
        {
            "MACHINENAME" => System.Environment.MachineName,
            "USERNAME" => System.Environment.UserName,
            "USERDOMAINNAME" => System.Environment.UserDomainName,
            "OSVERSION" => System.Environment.OSVersion.ToString(),
            "PROCESSID" => System.Environment.ProcessId.ToString(),
            "CURRENTDIRECTORY" => System.Environment.CurrentDirectory,
            "SYSTEMDIRECTORY" => System.Environment.SystemDirectory,
            "NEWLINE" => System.Environment.NewLine,
            "PROCESSORCOUNT" => System.Environment.ProcessorCount.ToString(),
            "IS64BITPROCESS" => System.Environment.Is64BitProcess.ToString(),
            "IS64BITOPERATINGSYSTEM" => System.Environment.Is64BitOperatingSystem.ToString(),
            "CLRVERSION" => System.Environment.Version.ToString(),
            "TICKCOUNT" => System.Environment.TickCount64.ToString(),
            "NOW" => DateTime.Now.ToString("O"),
            "UTCNOW" => DateTime.UtcNow.ToString("O"),
            "TODAY" => DateTime.Today.ToString("yyyy-MM-dd"),
            _ => null
        };
    }
}
