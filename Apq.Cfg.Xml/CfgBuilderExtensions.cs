namespace Apq.Cfg.Xml;

/// <summary>
/// CfgBuilder 的 XML 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 XML 文件配置源
    /// </summary>
    public static CfgBuilder AddXml(this CfgBuilder builder, string path, int level, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        return builder.AddSource(new XmlFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
    }
}
