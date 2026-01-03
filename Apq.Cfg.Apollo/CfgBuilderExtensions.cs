namespace Apq.Cfg.Apollo;

/// <summary>
/// CfgBuilder 的 Apollo 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Apollo 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Apollo"/> (15)</param>
    /// <param name="isPrimaryWriter">是否为主写入源（Apollo 不支持写入），默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddApollo(
        this CfgBuilder builder,
        Action<ApolloCfgOptions> configure,
        int level = CfgSourceLevels.Apollo,
        bool isPrimaryWriter = false)
    {
        var options = new ApolloCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new ApolloCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Apollo 配置源（使用默认选项）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="appId">Apollo 应用 ID</param>
    /// <param name="metaServer">Meta Server 地址，默认为 "http://localhost:8080"</param>
    /// <param name="namespaces">命名空间列表，默认为 ["application"]</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Apollo"/> (15)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为true</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddApollo(
        this CfgBuilder builder,
        string appId,
        string metaServer = "http://localhost:8080",
        string[]? namespaces = null,
        int level = CfgSourceLevels.Apollo,
        bool enableHotReload = true)
    {
        return builder.AddApollo(options =>
        {
            options.AppId = appId;
            options.MetaServer = metaServer;
            options.Namespaces = namespaces ?? ["application"];
            options.EnableHotReload = enableHotReload;
        }, level);
    }
}
