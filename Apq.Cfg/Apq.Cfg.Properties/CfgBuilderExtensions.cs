namespace Apq.Cfg.Properties;

public static class CfgBuilderExtensions
{
    public static CfgBuilder AddPropertiesFile(this CfgBuilder builder, string path, int level = CfgSourceLevels.Properties, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        builder.AddSource(new PropertiesFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
        return builder;
    }
}
