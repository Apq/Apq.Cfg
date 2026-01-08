using Microsoft.Extensions.DependencyInjection;

namespace Apq.Cfg.WebApi;

/// <summary>
/// ICfgRoot 扩展方法
/// </summary>
public static class CfgRootExtensions
{
    /// <summary>
    /// 为配置根添加 Web API 支持（链式调用）
    /// </summary>
    /// <param name="cfgRoot">配置根对象</param>
    /// <param name="services">服务集合</param>
    /// <param name="configure">可选的配置回调</param>
    /// <returns>配置根对象（支持链式调用）</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json")
    ///     .Build()
    ///     .AddWebApi(builder.Services, options =>
    ///     {
    ///         options.Authentication = AuthenticationType.ApiKey;
    ///         options.ApiKey = "your-secret-key";
    ///     });
    /// </code>
    /// </example>
    public static ICfgRoot AddWebApi(
        this ICfgRoot cfgRoot,
        IServiceCollection services,
        Action<WebApiOptions>? configure = null)
    {
        services.AddApqCfgWebApi(cfgRoot, configure);
        return cfgRoot;
    }
}
