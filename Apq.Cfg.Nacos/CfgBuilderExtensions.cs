namespace Apq.Cfg.Nacos;

/// <summary>
/// CfgBuilder 的 Nacos 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Nacos 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="isPrimaryWriter">是否为主写入源</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddNacos(
        this CfgBuilder builder,
        Action<NacosCfgOptions> configure,
        int level,
        bool isPrimaryWriter = false)
    {
        var options = new NacosCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new NacosCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Nacos 配置源（使用默认选项）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="serverAddresses">Nacos 服务地址</param>
    /// <param name="dataId">配置的 DataId</param>
    /// <param name="group">配置分组</param>
    /// <param name="level">配置层级</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddNacos(
        this CfgBuilder builder,
        string serverAddresses,
        string dataId,
        string group = "DEFAULT_GROUP",
        int level = 0)
    {
        return builder.AddNacos(options =>
        {
            options.ServerAddresses = serverAddresses;
            options.DataId = dataId;
            options.Group = group;
        }, level);
    }
}
