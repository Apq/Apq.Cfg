namespace Apq.Cfg.Hcl;

public static class CfgBuilderExtensions
{
    public static CfgBuilder AddHclFile(this CfgBuilder builder, string path, int level = CfgSourceLevels.Hcl, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false)
    {
        builder.AddSource(new HclFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter));
        return builder;
    }
}
