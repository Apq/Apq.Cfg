using Apq.Cfg.WebApi.Authentication;
using Apq.Cfg.WebApi.Controllers;
using Apq.Cfg.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if NET8_0
using Microsoft.OpenApi;
#endif

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

        // 默认启用 API 文档（使用默认选项，可通过 configure 回调禁用）
        var defaultOptions = new WebApiOptions();
        configure?.Invoke(defaultOptions);
        ConfigureOpenApi(services, defaultOptions);

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

        // 默认启用 API 文档（从配置绑定，可通过 configure 回调覆盖）
        var options = new WebApiOptions();
        configuration.GetSection(WebApiOptions.SectionName).Bind(options);
        configure?.Invoke(options);
        ConfigureOpenApi(services, options);

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

    /// <summary>
    /// 内部方法：配置 OpenAPI 文档服务
    /// </summary>
    private static void ConfigureOpenApi(IServiceCollection services, WebApiOptions options)
    {
        if (!options.OpenApiEnabled)
            return;

#if NET8_0
        // .NET 8: 使用 Swashbuckle (Swagger UI)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(options.OpenApiVersion, new OpenApiInfo
            {
                Title = options.OpenApiTitle,
                Description = options.OpenApiDescription,
                Version = options.OpenApiVersion,
                Contact = new OpenApiContact
                {
                    Name = "Apq.Cfg",
                    Url = new Uri("https://github.com/AiPuZi/Apq.Cfg")
                }
            });

            // 添加认证支持
            if (options.OpenApiShowAuthorizationButton)
            {
                switch (options.Authentication)
                {
                    case AuthenticationType.ApiKey:
                        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Name = options.ApiKeyHeaderName,
                            Description = "API Key 认证，请在请求头中添加 API Key"
                        });
                        c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("ApiKey", doc),
                                new List<string>()
                            }
                        });
                        break;

                    case AuthenticationType.JwtBearer:
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = "JWT Bearer 认证，请输入 Token"
                        });
                        c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("Bearer", doc),
                                new List<string>()
                            }
                        });
                        break;
                }
            }

            // 包含 XML 注释
            var xmlFile = $"{typeof(ConfigController).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
#else
        // .NET 10+: 使用 Microsoft.AspNetCore.OpenApi + Scalar
        services.AddOpenApi(options.OpenApiVersion, openApiOptions =>
        {
            openApiOptions.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = options.OpenApiTitle;
                document.Info.Description = options.OpenApiDescription;
                document.Info.Version = options.OpenApiVersion;
                return Task.CompletedTask;
            });
        });
#endif
    }
}
