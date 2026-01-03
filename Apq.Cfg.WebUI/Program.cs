using Apq.Cfg;
using Apq.Cfg.WebApi;
using Apq.Cfg.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// 构建 Apq.Cfg 配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true, optional: true)
    .AddEnvironmentVariables(level: 100, prefix: "APQCFG_")
    .Build();

builder.Services.AddSingleton<ICfgRoot>(cfg);

// 添加 Apq.Cfg WebApi 服务
builder.Services.AddApqCfgWebApi(options =>
{
    options.AllowRead = true;
    options.AllowWrite = true;
    options.AllowDelete = true;
});

// 添加服务
builder.Services.AddControllers();
builder.Services.AddSingleton<IAppService, AppService>();
builder.Services.AddHttpClient<ConfigProxyService>();

// 开发环境启用 SPA 代理
builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "wwwroot";
});

// 添加 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 可选：固定 PathBase（通常不需要，前端会自动检测）
// 仅在特殊场景下使用，如需要后端生成的 URL 包含虚拟目录前缀
var pathBase = cfg["PathBase"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

// 处理转发头（反向代理场景）
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
});

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
app.UseCors();

app.MapControllers();

app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/api"),
    appBranch =>
    {
        appBranch.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (app.Environment.IsDevelopment())
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:38690");
            }
        });
    });

app.Run();
