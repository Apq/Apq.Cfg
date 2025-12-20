namespace Apq.Cfg.Toml;

/// <summary>
/// CfgBuilder 的 TOML 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 TOML 文件配置源
    /// </summary>
    public static CfgBuilder AddToml(this CfgBuilder builder, string path, int level, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        builder.AddSource(new TomlFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
        return builder;
    }
}
