namespace Apq.Cfg.Zookeeper;

/// <summary>
/// CfgBuilder 的 Zookeeper 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Zookeeper 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="isPrimaryWriter">是否为主写入源</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddZookeeper(
        this CfgBuilder builder,
        Action<ZookeeperCfgOptions> configure,
        int level,
        bool isPrimaryWriter = false)
    {
        var options = new ZookeeperCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new ZookeeperCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Zookeeper 配置源（使用默认选项）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="connectionString">Zookeeper 连接字符串，如 localhost:2181</param>
    /// <param name="rootPath">根路径，默认 /config</param>
    /// <param name="level">配置层级</param>
    /// <param name="enableHotReload">是否启用热重载</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddZookeeper(
        this CfgBuilder builder,
        string connectionString,
        string rootPath = "/config",
        int level = 0,
        bool enableHotReload = true)
    {
        return builder.AddZookeeper(options =>
        {
            options.ConnectionString = connectionString;
            options.RootPath = rootPath;
            options.EnableHotReload = enableHotReload;
        }, level);
    }

    /// <summary>
    /// 添加 Zookeeper 配置源（JSON 格式）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="connectionString">Zookeeper 连接字符串</param>
    /// <param name="nodePath">存储 JSON 配置的节点路径</param>
    /// <param name="level">配置层级</param>
    /// <param name="enableHotReload">是否启用热重载</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddZookeeperJson(
        this CfgBuilder builder,
        string connectionString,
        string nodePath,
        int level = 0,
        bool enableHotReload = true)
    {
        return builder.AddZookeeper(options =>
        {
            options.ConnectionString = connectionString;
            options.RootPath = nodePath;
            options.DataFormat = ZookeeperDataFormat.Json;
            options.EnableHotReload = enableHotReload;
        }, level);
    }
}
