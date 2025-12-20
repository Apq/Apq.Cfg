namespace Apq.Cfg.Ini;

/// <summary>
/// CfgBuilder 的 INI 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 INI 文件配置源
    /// </summary>
    public static CfgBuilder AddIni(this CfgBuilder builder, string path, int level, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        return builder.AddSource(new IniFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
    }
}
