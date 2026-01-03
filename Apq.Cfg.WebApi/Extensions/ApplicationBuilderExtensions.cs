using Apq.Cfg.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        // Swagger（内置支持）
        if (options.SwaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.SwaggerVersion}/swagger.json", options.SwaggerTitle);
                c.RoutePrefix = options.SwaggerRoutePrefix;
                c.DocumentTitle = options.SwaggerTitle;
            });
        }

        // 启用检查中间件
        app.UseMiddleware<ConfigApiMiddleware>();

        // CORS
        if (options.EnableCors)
        {
            app.UseCors("ApqCfgWebApiCors");
        }

        // 认证
        if (options.Authentication != AuthenticationType.None)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        return app;
    }

    /// <summary>
    /// 映射 Apq.Cfg Web API 端点
    /// </summary>
    public static IEndpointRouteBuilder MapApqCfgWebApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllers();
        return endpoints;
    }
}
