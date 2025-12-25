using Apq.Cfg.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
    /// 配置强类型选项（从 ICfgRoot 绑定），支持嵌套对象和集合
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
        // 注册 IOptions<TOptions>
        services.AddOptions<TOptions>()
            .Configure<ICfgRoot>((options, cfg) =>
            {
                var section = cfg.GetSection(sectionKey);
                ObjectBinder.BindSection(section, options);
            });

        // 注册 IOptionsMonitor<TOptions>（支持配置变更自动更新）
        services.TryAddSingleton<IOptionsMonitor<TOptions>>(sp =>
        {
            var cfgRoot = sp.GetRequiredService<ICfgRoot>();
            return new ApqCfgOptionsMonitor<TOptions>(cfgRoot, sectionKey);
        });

        // 注册 IOptionsSnapshot<TOptions>（每次请求重新绑定）
        services.TryAddScoped<IOptionsSnapshot<TOptions>>(sp =>
        {
            var cfgRoot = sp.GetRequiredService<ICfgRoot>();
            return new ApqCfgOptionsSnapshot<TOptions>(cfgRoot, sectionKey);
        });

        return services;
    }

    /// <summary>
    /// 配置强类型选项并启用配置变更监听
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="sectionKey">配置节键名</param>
    /// <param name="onChange">配置变更回调</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ConfigureApqCfg<TOptions>(
        this IServiceCollection services,
        string sectionKey,
        Action<TOptions> onChange)
        where TOptions : class, new()
    {
        services.ConfigureApqCfg<TOptions>(sectionKey);

        // 注册变更监听器
        services.AddSingleton<IDisposable>(sp =>
        {
            var monitor = sp.GetRequiredService<IOptionsMonitor<TOptions>>();
            return monitor.OnChange((options, _) => onChange(options))!;
        });

        return services;
    }
}
