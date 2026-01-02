namespace Apq.Cfg.Consul;

/// <summary>
/// CfgBuilder 的 Consul 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Consul 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Consul"/> (15)</param>
    /// <param name="isPrimaryWriter">是否为主写入源，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddConsul(
        this CfgBuilder builder,
        Action<ConsulCfgOptions> configure,
        int level = CfgSourceLevels.Consul,
        bool isPrimaryWriter = false)
    {
        var options = new ConsulCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new ConsulCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Consul 配置源（使用默认选项）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="address">Consul 服务地址</param>
    /// <param name="keyPrefix">KV 键前缀，默认为 "config/"</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Consul"/> (15)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为true</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddConsul(
        this CfgBuilder builder,
        string address,
        string keyPrefix = "config/",
        int level = CfgSourceLevels.Consul,
        bool enableHotReload = true)
    {
        return builder.AddConsul(options =>
        {
            options.Address = address;
            options.KeyPrefix = keyPrefix;
            options.EnableHotReload = enableHotReload;
        }, level);
    }
}
