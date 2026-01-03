namespace Apq.Cfg.Redis;

/// <summary>
/// CfgBuilder 的 Redis 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Redis 配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">Redis 配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Redis"/> (10)</param>
    /// <param name="isPrimaryWriter">是否为主要写入器，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddRedis(this CfgBuilder builder, Action<RedisOptions> configure, int level = CfgSourceLevels.Redis, bool isPrimaryWriter = false)
    {
        var options = new RedisOptions();
        configure?.Invoke(options);
        builder.AddSource(new RedisCfgSource(options, level, isPrimaryWriter));
        return builder;
    }
}
