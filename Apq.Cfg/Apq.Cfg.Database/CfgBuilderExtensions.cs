namespace Apq.Cfg.Database;

/// <summary>
/// CfgBuilder 的 Database 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加数据库配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">数据库配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Database"/> (10)</param>
    /// <param name="isPrimaryWriter">是否为主要写入器，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddDatabase(this CfgBuilder builder, Action<DatabaseOptions> configure, int level = CfgSourceLevels.Database, bool isPrimaryWriter = false)
    {
        var options = new DatabaseOptions();
        configure?.Invoke(options);
        builder.AddSource(new DatabaseCfgSource(options, level, isPrimaryWriter));
        return builder;
    }
}
