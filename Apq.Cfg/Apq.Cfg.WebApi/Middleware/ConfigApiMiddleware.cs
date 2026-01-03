using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.WebApi.Middleware;

/// <summary>
/// 配置 API 启用检查中间件
/// </summary>
public class ConfigApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebApiOptions _options;

    public ConfigApiMiddleware(RequestDelegate next, IOptions<WebApiOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // 检查是否是配置 API 路径
        if (path.StartsWith(_options.RoutePrefix, StringComparison.OrdinalIgnoreCase))
        {
            if (!_options.Enabled)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new { error = "Configuration API is disabled" });
                return;
            }
        }

        await _next(context);
    }
}
