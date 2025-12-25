using Apq.Cfg.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Apq.Cfg;

/// <summary>
/// IServiceCollection 扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Apq.Cfg 配置服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置构建器委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApqCfg(
        this IServiceCollection services,
        Action<CfgBuilder> configure)
    {
        var builder = new CfgBuilder();
        configure(builder);
        var cfgRoot = builder.Build();

        // 注册为单例
        services.TryAddSingleton<ICfgRoot>(cfgRoot);

        // 同时注册 Microsoft Configuration
        services.TryAddSingleton(cfgRoot.ToMicrosoftConfiguration());

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg 配置服务（使用工厂方法）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="factory">配置根工厂方法</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApqCfg(
        this IServiceCollection services,
        Func<IServiceProvider, ICfgRoot> factory)
    {
        services.TryAddSingleton(factory);
        services.TryAddSingleton(sp => sp.GetRequiredService<ICfgRoot>().ToMicrosoftConfiguration());

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg 配置服务并绑定强类型配置
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置构建器委托</param>
    /// <param name="sectionKey">配置节键名</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApqCfg<TOptions>(
        this IServiceCollection services,
        Action<CfgBuilder> configure,
        string sectionKey)
        where TOptions : class, new()
    {
        services.AddApqCfg(configure);
        services.ConfigureApqCfg<TOptions>(sectionKey);

        return services;
    }

    /// <summary>
    /// 配置强类型选项（从 ICfgRoot 绑定）
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="sectionKey">配置节键名</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ConfigureApqCfg<TOptions>(
        this IServiceCollection services,
        string sectionKey)
        where TOptions : class, new()
    {
        services.AddOptions<TOptions>()
            .Configure<ICfgRoot>((options, cfg) =>
            {
                var section = cfg.GetSection(sectionKey);
                BindSection(section, options);
            });

        return services;
    }

    /// <summary>
    /// 将配置节绑定到对象
    /// </summary>
    private static void BindSection<T>(ICfgSection section, T target) where T : class
    {
        if (target == null) return;

        var type = typeof(T);
        var properties = type.GetProperties()
            .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

        foreach (var prop in properties)
        {
            var value = section.Get(prop.Name);
            if (value != null)
            {
                try
                {
                    var convertedValue = ValueConverter.ConvertToType(value, prop.PropertyType);
                    if (convertedValue != null)
                    {
                        prop.SetValue(target, convertedValue);
                    }
                }
                catch
                {
                    // 忽略转换失败
                }
            }
        }
    }
}
