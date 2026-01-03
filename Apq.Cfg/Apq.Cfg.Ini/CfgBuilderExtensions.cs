namespace Apq.Cfg.Ini;

/// <summary>
/// CfgBuilder 的 INI 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 INI 文件配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="path">INI 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Ini"/> (0)</param>
    /// <param name="writeable">是否可写，默认为false</param>
    /// <param name="optional">是否为可选文件，默认为true</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载，默认为true</param>
    /// <param name="isPrimaryWriter">是否为主要写入器，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddIni(this CfgBuilder builder, string path, int level = CfgSourceLevels.Ini, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        return builder.AddSource(new IniFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
    }
}
