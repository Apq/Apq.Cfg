namespace Apq.Cfg.Etcd;

/// <summary>
/// CfgBuilder 的 Etcd 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Etcd 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="isPrimaryWriter">是否为主写入源</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddEtcd(
        this CfgBuilder builder,
        Action<EtcdCfgOptions> configure,
        int level,
        bool isPrimaryWriter = false)
    {
        var options = new EtcdCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new EtcdCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Etcd 配置源（使用默认选项）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="endpoints">Etcd 服务端点列表</param>
    /// <param name="keyPrefix">KV 键前缀</param>
    /// <param name="level">配置层级</param>
    /// <param name="enableHotReload">是否启用热重载</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddEtcd(
        this CfgBuilder builder,
        string[] endpoints,
        string keyPrefix = "/config/",
        int level = 0,
        bool enableHotReload = true)
    {
        return builder.AddEtcd(options =>
        {
            options.Endpoints = endpoints;
            options.KeyPrefix = keyPrefix;
            options.EnableHotReload = enableHotReload;
        }, level);
    }

    /// <summary>
    /// 添加 Etcd 配置源（单端点）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="endpoint">Etcd 服务端点</param>
    /// <param name="keyPrefix">KV 键前缀</param>
    /// <param name="level">配置层级</param>
    /// <param name="enableHotReload">是否启用热重载</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddEtcd(
        this CfgBuilder builder,
        string endpoint,
        string keyPrefix = "/config/",
        int level = 0,
        bool enableHotReload = true)
    {
        return builder.AddEtcd(new[] { endpoint }, keyPrefix, level, enableHotReload);
    }
}
