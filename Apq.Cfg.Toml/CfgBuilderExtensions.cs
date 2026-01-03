namespace Apq.Cfg.Toml;

/// <summary>
/// CfgBuilder 的 TOML 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 TOML 文件配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="path">TOML 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Toml"/> (0)</param>
    /// <param name="writeable">是否可写，默认为false</param>
    /// <param name="optional">是否为可选文件，默认为true</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载，默认为true</param>
    /// <param name="isPrimaryWriter">是否为主要写入器，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddToml(this CfgBuilder builder, string path, int level = CfgSourceLevels.Toml, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        builder.AddSource(new TomlFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
        return builder;
    }
}
