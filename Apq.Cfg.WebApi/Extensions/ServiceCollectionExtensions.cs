using Apq.Cfg.WebApi.Authentication;
using Apq.Cfg.WebApi.Controllers;
using Apq.Cfg.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apq.Cfg.WebApi;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Apq.Cfg Web API 服务
    /// </summary>
    public static IServiceCollection AddApqCfgWebApi(
        this IServiceCollection services,
        Action<WebApiOptions>? configure = null)
    {
        // 配置选项
        services.AddOptions<WebApiOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                config.GetSection(WebApiOptions.SectionName).Bind(options);
            });

        // 代码配置覆盖配置文件
        if (configure != null)
        {
            services.PostConfigure(configure);
        }

        // 注册服务
        services.AddSingleton<IConfigApiService, ConfigApiService>();

        // 添加控制器
        services.AddControllers()
            .AddApplicationPart(typeof(ConfigController).Assembly);

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg Web API 服务（从配置节绑定）
    /// </summary>
    public static IServiceCollection AddApqCfgWebApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WebApiOptions>? configure = null)
    {
        services.Configure<WebApiOptions>(configuration.GetSection(WebApiOptions.SectionName));

        if (configure != null)
        {
            services.PostConfigure(configure);
        }

        services.AddSingleton<IConfigApiService, ConfigApiService>();
        services.AddControllers()
            .AddApplicationPart(typeof(ConfigController).Assembly);

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg Web API 认证
    /// </summary>
    public static IServiceCollection AddApqCfgWebApiAuthentication(
        this IServiceCollection services,
        WebApiOptions options)
    {
        switch (options.Authentication)
        {
            case AuthenticationType.ApiKey:
                services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                        ApiKeyDefaults.AuthenticationScheme,
                        opt =>
                        {
                            opt.ApiKey = options.ApiKey;
                            opt.HeaderName = options.ApiKeyHeaderName;
                        });
                break;

            case AuthenticationType.JwtBearer:
                if (options.Jwt != null)
                {
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(opt =>
                        {
                            opt.Authority = options.Jwt.Authority;
                            opt.Audience = options.Jwt.Audience;
                            opt.RequireHttpsMetadata = options.Jwt.RequireHttpsMetadata;
                            opt.TokenValidationParameters.ValidateIssuer = options.Jwt.ValidateIssuer;
                            opt.TokenValidationParameters.ValidateAudience = options.Jwt.ValidateAudience;
                        });
                }
                break;
        }

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg Web API CORS
    /// </summary>
    public static IServiceCollection AddApqCfgWebApiCors(
        this IServiceCollection services,
        WebApiOptions options)
    {
        if (options.EnableCors)
        {
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("ApqCfgWebApiCors", builder =>
                {
                    if (options.CorsOrigins.Contains("*"))
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                    else
                    {
                        builder.WithOrigins(options.CorsOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                });
            });
        }

        return services;
    }
}
