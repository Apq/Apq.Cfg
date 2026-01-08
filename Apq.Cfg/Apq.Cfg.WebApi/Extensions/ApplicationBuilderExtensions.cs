using Apq.Cfg.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Scalar.AspNetCore;
#endif

namespace Apq.Cfg.WebApi;

/// <summary>
/// 应用程序构建器扩展方法
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用 Apq.Cfg Web API
    /// </summary>
    public static IApplicationBuilder UseApqCfgWebApi(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<WebApiOptions>>().Value;

        if (!options.Enabled)
            return app;

#if NET8_0
        // .NET 8: 使用 Swagger UI
        if (options.OpenApiEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.OpenApiVersion}/swagger.json", options.OpenApiTitle);
                c.RoutePrefix = options.OpenApiRoutePrefix;
                c.DocumentTitle = options.OpenApiTitle;
            });
        }
#endif

        // 启用检查中间件
        app.UseMiddleware<ConfigApiMiddleware>();

        // CORS（在 AddApqCfgWebApi 中已配置策略）
        if (options.EnableCors)
        {
            app.UseCors("ApqCfgWebApiCors");
        }

        // 认证和授权（在 AddApqCfgWebApi 中已配置）
        if (options.Authentication != AuthenticationType.None)
        {
            app.UseAuthentication();
        }
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// 映射 Apq.Cfg Web API 端点
    /// </summary>
    public static IEndpointRouteBuilder MapApqCfgWebApi(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider
            .GetRequiredService<IOptions<WebApiOptions>>().Value;

        endpoints.MapControllers();

#if NET10_0_OR_GREATER
        // .NET 10+: 使用 Scalar UI
        if (options.OpenApiEnabled)
        {
            // 映射 OpenAPI 文档端点，使用配置的版本号作为文档名
            endpoints.MapOpenApi("/openapi/{documentName}.json");
            endpoints.MapScalarApiReference(options.OpenApiRoutePrefix, scalarOptions =>
            {
                scalarOptions
                    .WithTitle(options.OpenApiTitle)
                    .AddDocument(options.OpenApiVersion);
            });
        }
#endif

        return endpoints;
    }
}
