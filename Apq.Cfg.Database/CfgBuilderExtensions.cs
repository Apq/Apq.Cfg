namespace Apq.Cfg.Database;

/// <summary>
/// CfgBuilder 的 Database 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加数据库配置源
    /// </summary>
    public static CfgBuilder AddDatabase(this CfgBuilder builder, Action<DatabaseOptions> configure, int level, bool isPrimaryWriter = false)
    {
        var options = new DatabaseOptions();
        configure?.Invoke(options);
        builder.AddSource(new DatabaseCfgSource(options, level, isPrimaryWriter));
        return builder;
    }
}
