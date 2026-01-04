using Apq.Cfg.WebApi.Authentication;
using Apq.Cfg.WebApi.Controllers;
using Apq.Cfg.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    /// <param name="services">服务集合</param>
    /// <param name="cfgRoot">配置根对象</param>
    /// <param name="configure">可选的配置回调，用于覆盖从 cfgRoot 读取的配置</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// 配置优先级（从低到高）：
    /// 1. 默认值
    /// 2. 从 cfgRoot 的 "ApqCfg:WebApi" 节读取的配置
    /// 3. configure 回调中的配置
    /// </remarks>
    public static IServiceCollection AddApqCfgWebApi(
        this IServiceCollection services,
        ICfgRoot cfgRoot,
        Action<WebApiOptions>? configure = null)
    {
        // 注册 ICfgRoot
        services.AddSingleton(cfgRoot);

        // 从 ICfgRoot 读取配置
        var options = LoadOptionsFromCfgRoot(cfgRoot);

        // 代码配置覆盖
        configure?.Invoke(options);

        // 注册配置选项
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(options));

        // 注册服务
        services.AddSingleton<IConfigApiService, ConfigApiService>();

        // 添加控制器
        services.AddControllers()
            .AddApplicationPart(typeof(ConfigController).Assembly);

        // 配置 OpenAPI
        ConfigureOpenApi(services, options);

        // 配置 CORS
        ConfigureCors(services, options);

        // 配置认证
        ConfigureAuthentication(services, options);

        return services;
    }

    /// <summary>
    /// 从 ICfgRoot 加载 WebApiOptions
    /// </summary>
    private static WebApiOptions LoadOptionsFromCfgRoot(ICfgRoot cfgRoot)
    {
        var options = new WebApiOptions();
        var prefix = WebApiOptions.SectionName;

        // 读取各个配置项
        if (bool.TryParse(cfgRoot[$"{prefix}:Enabled"], out var enabled))
            options.Enabled = enabled;

        var routePrefix = cfgRoot[$"{prefix}:RoutePrefix"];
        if (!string.IsNullOrEmpty(routePrefix))
            options.RoutePrefix = routePrefix;

        if (Enum.TryParse<AuthenticationType>(cfgRoot[$"{prefix}:Authentication"], true, out var authType))
            options.Authentication = authType;

        var apiKey = cfgRoot[$"{prefix}:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
            options.ApiKey = apiKey;

        var apiKeyHeaderName = cfgRoot[$"{prefix}:ApiKeyHeaderName"];
        if (!string.IsNullOrEmpty(apiKeyHeaderName))
            options.ApiKeyHeaderName = apiKeyHeaderName;

        if (bool.TryParse(cfgRoot[$"{prefix}:AllowRead"], out var allowRead))
            options.AllowRead = allowRead;

        if (bool.TryParse(cfgRoot[$"{prefix}:AllowWrite"], out var allowWrite))
            options.AllowWrite = allowWrite;

        if (bool.TryParse(cfgRoot[$"{prefix}:AllowDelete"], out var allowDelete))
            options.AllowDelete = allowDelete;

        if (bool.TryParse(cfgRoot[$"{prefix}:MaskSensitiveValues"], out var maskSensitive))
            options.MaskSensitiveValues = maskSensitive;

        if (bool.TryParse(cfgRoot[$"{prefix}:EnableCors"], out var enableCors))
            options.EnableCors = enableCors;

        // 读取数组配置
        var corsOrigins = ReadArrayConfig(cfgRoot, $"{prefix}:CorsOrigins");
        if (corsOrigins.Length > 0)
            options.CorsOrigins = corsOrigins;

        var sensitivePatterns = ReadArrayConfig(cfgRoot, $"{prefix}:SensitiveKeyPatterns");
        if (sensitivePatterns.Length > 0)
            options.SensitiveKeyPatterns = sensitivePatterns;

        // OpenAPI 配置
        if (bool.TryParse(cfgRoot[$"{prefix}:OpenApiEnabled"], out var openApiEnabled))
            options.OpenApiEnabled = openApiEnabled;

        var openApiTitle = cfgRoot[$"{prefix}:OpenApiTitle"];
        if (!string.IsNullOrEmpty(openApiTitle))
            options.OpenApiTitle = openApiTitle;

        var openApiDescription = cfgRoot[$"{prefix}:OpenApiDescription"];
        if (!string.IsNullOrEmpty(openApiDescription))
            options.OpenApiDescription = openApiDescription;

        var openApiVersion = cfgRoot[$"{prefix}:OpenApiVersion"];
        if (!string.IsNullOrEmpty(openApiVersion))
            options.OpenApiVersion = openApiVersion;

        var openApiRoutePrefix = cfgRoot[$"{prefix}:OpenApiRoutePrefix"];
        if (!string.IsNullOrEmpty(openApiRoutePrefix))
            options.OpenApiRoutePrefix = openApiRoutePrefix;

        if (bool.TryParse(cfgRoot[$"{prefix}:OpenApiShowAuthorizationButton"], out var showAuthBtn))
            options.OpenApiShowAuthorizationButton = showAuthBtn;

        // JWT 配置
        var jwtAuthority = cfgRoot[$"{prefix}:Jwt:Authority"];
        if (!string.IsNullOrEmpty(jwtAuthority))
        {
            options.Jwt = new JwtOptions
            {
                Authority = jwtAuthority,
                Audience = cfgRoot[$"{prefix}:Jwt:Audience"] ?? string.Empty
            };

            if (bool.TryParse(cfgRoot[$"{prefix}:Jwt:RequireHttpsMetadata"], out var requireHttps))
                options.Jwt.RequireHttpsMetadata = requireHttps;

            if (bool.TryParse(cfgRoot[$"{prefix}:Jwt:ValidateIssuer"], out var validateIssuer))
                options.Jwt.ValidateIssuer = validateIssuer;

            if (bool.TryParse(cfgRoot[$"{prefix}:Jwt:ValidateAudience"], out var validateAudience))
                options.Jwt.ValidateAudience = validateAudience;
        }

        return options;
    }

    /// <summary>
    /// 读取数组配置（支持 Key:0, Key:1 格式）
    /// </summary>
    private static string[] ReadArrayConfig(ICfgRoot cfgRoot, string prefix)
    {
        var values = new List<string>();
        for (var i = 0; i < 100; i++) // 最多读取 100 个元素
        {
            var value = cfgRoot[$"{prefix}:{i}"];
            if (string.IsNullOrEmpty(value))
                break;
            values.Add(value);
        }
        return values.ToArray();
    }

    /// <summary>
    /// 配置 CORS
    /// </summary>
    private static void ConfigureCors(IServiceCollection services, WebApiOptions options)
    {
        if (!options.EnableCors)
            return;

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

    /// <summary>
    /// 配置认证
    /// </summary>
    private static void ConfigureAuthentication(IServiceCollection services, WebApiOptions options)
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
    }

    /// <summary>
    /// 配置 OpenAPI 文档服务
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
