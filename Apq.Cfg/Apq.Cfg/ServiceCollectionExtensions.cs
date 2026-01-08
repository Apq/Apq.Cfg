using Apq.Cfg.DependencyInjection;
using Apq.Cfg.Validation;
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
    /// <param name="configure">配置构建器委托，用于配置各种配置源</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// services.AddApqCfg(cfg =&gt; cfg
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddJsonFile($"config.{environment}.json", level: 1)
    ///     .AddEnvironmentVariables(prefix: "APP_", level: 2));
    /// </code>
    /// </example>
    /// <remarks>
    /// 此方法会同时注册 ICfgRoot 和 IConfigurationRoot 服务，
    /// 使您可以在应用程序中同时使用 Apq.Cfg 和 Microsoft.Extensions.Configuration 的 API。
    /// </remarks>
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
    /// 添加 Apq.Cfg 配置服务（支持访问 IServiceProvider）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置构建器委托，接收 CfgBuilder 和 IServiceProvider</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// // 使用 Data Protection 加密
    /// services.AddDataProtection();
    /// services.AddApqCfg((builder, sp) =&gt; builder
    ///     .AddJsonFile("appsettings.json", level: 0, writeable: false)
    ///     .AddDataProtectionEncryption(sp.GetRequiredService&lt;IDataProtectionProvider&gt;())
    ///     .AddSensitiveMasking());
    /// </code>
    /// </example>
    /// <remarks>
    /// 此重载允许在配置构建过程中访问已注册的服务，
    /// 适用于需要依赖其他服务（如 IDataProtectionProvider）的场景。
    /// </remarks>
    public static IServiceCollection AddApqCfg(
        this IServiceCollection services,
        Action<CfgBuilder, IServiceProvider> configure)
    {
        services.TryAddSingleton<ICfgRoot>(sp =>
        {
            var builder = new CfgBuilder();
            configure(builder, sp);
            return builder.Build();
        });

        services.TryAddSingleton(sp => sp.GetRequiredService<ICfgRoot>().ToMicrosoftConfiguration());

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg 配置服务（使用工厂方法）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="factory">配置根工厂方法，接收服务提供者并返回配置根实例</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// services.AddApqCfg(sp =&gt; {
    ///     var env = sp.GetRequiredService&lt;IWebHostEnvironment&gt;();
    ///     return new CfgBuilder()
    ///         .AddJsonFile("config.json", level: 0)
    ///         .AddJsonFile($"config.{env.EnvironmentName}.json", level: 1)
    ///         .AddEnvironmentVariables(prefix: "APP_", level: 2)
    ///         .Build();
    /// });
    /// </code>
    /// </example>
    /// <remarks>
    /// 使用工厂方法可以访问其他已注册的服务，实现更复杂的配置逻辑。
    /// 工厂方法只会在首次请求配置时调用一次。
    /// </remarks>
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
    /// <param name="configure">配置构建器委托，用于配置各种配置源</param>
    /// <param name="sectionKey">配置节键名，用于定位 TOptions 的配置数据</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// // 定义配置选项类
    /// public class DatabaseOptions
    /// {
    ///     public string? ConnectionString { get; set; }
    ///     public int Timeout { get; set; } = 30;
    /// }
    /// 
    /// // 注册配置服务并绑定强类型配置
    /// services.AddApqCfg&lt;DatabaseOptions&gt;(cfg =&gt; cfg
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddEnvironmentVariables(prefix: "APP_", level: 2),
    ///     "Database");
    /// 
    /// // 使用配置
    /// var dbOptions = serviceProvider.GetRequiredService&lt;IOptions&lt;DatabaseOptions&gt;&gt;().Value;
    /// </code>
    /// </example>
    /// <remarks>
    /// 此方法会注册 ICfgRoot、IConfigurationRoot 和 IOptions&lt;TOptions&gt; 服务。
    /// 配置选项类必须有无参构造函数。
    /// </remarks>
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
    /// <param name="sectionKey">配置节键名，用于定位 TOptions 的配置数据</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// // 定义配置选项类
    /// public class DatabaseOptions
    /// {
    ///     public string? ConnectionString { get; set; }
    ///     public int Timeout { get; set; } = 30;
    ///     public RetryOptions Retry { get; set; } = new();
    /// }
    /// 
    /// public class RetryOptions
    /// {
    ///     public int Count { get; set; } = 3;
    ///     public int Delay { get; set; } = 1000;
    /// }
    /// 
    /// // 注册配置服务
    /// services.AddApqCfg(cfg =&gt; cfg
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddEnvironmentVariables(prefix: "APP_", level: 2));
    /// 
    /// // 绑定强类型配置
    /// services.ConfigureApqCfg&lt;DatabaseOptions&gt;("Database");
    /// 
    /// // 使用配置
    /// var dbOptions = serviceProvider.GetRequiredService&lt;IOptions&lt;DatabaseOptions&gt;&gt;().Value;
    /// var retryCount = dbOptions.Retry.Count;
    /// </code>
    /// </example>
    /// <remarks>
    /// 此方法会注册 IOptions&lt;TOptions&gt;、IOptionsMonitor&lt;TOptions&gt; 和 IOptionsSnapshot&lt;TOptions&gt; 服务。
    /// 配置选项类必须有无参构造函数。
    /// 支持嵌套对象和集合类型的自动绑定。
    /// </remarks>
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
    /// <param name="sectionKey">配置节键名，用于定位 TOptions 的配置数据</param>
    /// <param name="onChange">配置变更回调函数，当配置发生变化时调用</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// // 定义配置选项类
    /// public class DatabaseOptions
    /// {
    ///     public string? ConnectionString { get; set; }
    ///     public int Timeout { get; set; } = 30;
    /// }
    /// 
    /// // 注册配置服务并监听变更
    /// services.ConfigureApqCfg&lt;DatabaseOptions&gt;("Database", options => {
    ///     Console.WriteLine($"数据库连接字符串已更新: {options.ConnectionString}");
    ///     
    ///     // 执行必要的重新连接逻辑
    ///     ReconnectDatabase(options.ConnectionString);
    /// });
    /// 
    /// void ReconnectDatabase(string connectionString)
    /// {
    ///     // 实现数据库重新连接逻辑
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// 此方法会注册 IOptions&lt;TOptions&gt;、IOptionsMonitor&lt;TOptions&gt; 和 IOptionsSnapshot&lt;TOptions&gt; 服务，
    /// 同时添加一个配置变更监听器。
    /// 变更回调会在配置源发生变化且导致 TOptions 实例更新时触发。
    /// 返回的 IDisposable 对象可用于取消监听，但已自动注册到服务容器，会在应用关闭时释放。
    /// </remarks>
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

    #region 配置验证

    /// <summary>
    /// 添加 Apq.Cfg 配置服务并启用验证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置构建器委托</param>
    /// <param name="validateOnStartup">是否在启动时验证配置，默认为 true</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <exception cref="ConfigValidationException">当 validateOnStartup 为 true 且验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// services.AddApqCfgWithValidation(cfg => cfg
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddValidation(v => v
    ///         .Required("Database:ConnectionString")
    ///         .Range("Database:Port", 1, 65535)));
    /// </code>
    /// </example>
    public static IServiceCollection AddApqCfgWithValidation(
        this IServiceCollection services,
        Action<CfgBuilder> configure,
        bool validateOnStartup = true)
    {
        var builder = new CfgBuilder();
        configure(builder);

        if (validateOnStartup)
        {
            var (cfgRoot, _) = builder.BuildAndValidate(throwOnError: true);
            services.TryAddSingleton<ICfgRoot>(cfgRoot);
            services.TryAddSingleton(cfgRoot.ToMicrosoftConfiguration());

            // 注册验证器
            var validator = builder.GetValidator();
            if (validator != null)
                services.TryAddSingleton<IConfigValidator>(validator);
        }
        else
        {
            var cfgRoot = builder.Build();
            services.TryAddSingleton<ICfgRoot>(cfgRoot);
            services.TryAddSingleton(cfgRoot.ToMicrosoftConfiguration());

            // 注册验证器
            var validator = builder.GetValidator();
            if (validator != null)
                services.TryAddSingleton<IConfigValidator>(validator);
        }

        return services;
    }

    /// <summary>
    /// 添加配置验证器到服务集合
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">验证规则配置委托</param>
    /// <returns>服务集合，支持链式调用</returns>
    /// <example>
    /// <code>
    /// services.AddApqCfg(cfg => cfg.AddJsonFile("config.json", level: 0));
    /// services.AddConfigValidator(v => v
    ///     .Required("Database:ConnectionString")
    ///     .Range("Database:Port", 1, 65535));
    /// </code>
    /// </example>
    public static IServiceCollection AddConfigValidator(
        this IServiceCollection services,
        Action<ConfigValidationBuilder> configure)
    {
        var builder = new ConfigValidationBuilder();
        configure(builder);
        var validator = builder.Build();
        services.TryAddSingleton<IConfigValidator>(validator);
        return services;
    }

    /// <summary>
    /// 添加配置验证器到服务集合
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="validator">验证器实例</param>
    /// <returns>服务集合，支持链式调用</returns>
    public static IServiceCollection AddConfigValidator(
        this IServiceCollection services,
        IConfigValidator validator)
    {
        services.TryAddSingleton(validator);
        return services;
    }

    #endregion
}
