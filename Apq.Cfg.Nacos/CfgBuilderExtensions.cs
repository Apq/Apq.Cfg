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
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Nacos"/> (15)</param>
    /// <param name="isPrimaryWriter">是否为主写入源，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddNacos(
        this CfgBuilder builder,
        Action<NacosCfgOptions> configure,
        int level = CfgSourceLevels.Nacos,
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
    /// <param name="group">配置分组，默认为 "DEFAULT_GROUP"</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Nacos"/> (15)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddNacos(
        this CfgBuilder builder,
        string serverAddresses,
        string dataId,
        string group = "DEFAULT_GROUP",
        int level = CfgSourceLevels.Nacos,
        bool enableHotReload = false)
    {
        return builder.AddNacos(options =>
        {
            options.ServerAddresses = serverAddresses;
            options.DataId = dataId;
            options.Group = group;
            options.EnableHotReload = enableHotReload;
        }, level);
    }

    /// <summary>
    /// 添加 Nacos JSON 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="serverAddresses">Nacos 服务地址</param>
    /// <param name="dataId">配置的 DataId</param>
    /// <param name="group">配置分组，默认为 "DEFAULT_GROUP"</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Nacos"/> (15)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddNacosJson(
        this CfgBuilder builder,
        string serverAddresses,
        string dataId,
        string group = "DEFAULT_GROUP",
        int level = CfgSourceLevels.Nacos,
        bool enableHotReload = false)
    {
        return builder.AddNacos(options =>
        {
            options.ServerAddresses = serverAddresses;
            options.DataId = dataId;
            options.Group = group;
            options.DataFormat = NacosDataFormat.Json;
            options.EnableHotReload = enableHotReload;
        }, level);
    }

    /// <summary>
    /// 添加 Nacos Properties 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="serverAddresses">Nacos 服务地址</param>
    /// <param name="dataId">配置的 DataId</param>
    /// <param name="group">配置分组，默认为 "DEFAULT_GROUP"</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Nacos"/> (15)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddNacosProperties(
        this CfgBuilder builder,
        string serverAddresses,
        string dataId,
        string group = "DEFAULT_GROUP",
        int level = CfgSourceLevels.Nacos,
        bool enableHotReload = false)
    {
        return builder.AddNacos(options =>
        {
            options.ServerAddresses = serverAddresses;
            options.DataId = dataId;
            options.Group = group;
            options.DataFormat = NacosDataFormat.Properties;
            options.EnableHotReload = enableHotReload;
        }, level);
    }
}
