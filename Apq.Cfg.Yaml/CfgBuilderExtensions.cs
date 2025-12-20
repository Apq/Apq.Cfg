namespace Apq.Cfg.Yaml;

/// <summary>
/// CfgBuilder 的 YAML 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 YAML 文件配置源
    /// </summary>
    public static CfgBuilder AddYaml(this CfgBuilder builder, string path, int level, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        builder.AddSource(new YamlFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
        return builder;
    }
}
