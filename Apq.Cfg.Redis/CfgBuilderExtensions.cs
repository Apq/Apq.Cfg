namespace Apq.Cfg.Redis;

/// <summary>
/// CfgBuilder 的 Redis 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Redis 配置源
    /// </summary>
    public static CfgBuilder AddRedis(this CfgBuilder builder, Action<RedisOptions> configure, int level, bool isPrimaryWriter = false)
    {
        var options = new RedisOptions();
        configure?.Invoke(options);
        builder.AddSource(new RedisCfgSource(options, level, isPrimaryWriter));
        return builder;
    }
}
