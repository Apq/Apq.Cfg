using Apq.Cfg;
using Apq.Cfg.Env;
using Apq.Cfg.Ini;
using Apq.Cfg.Toml;
using Apq.Cfg.WebApi;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;

var builder = WebApplication.CreateBuilder(args);

// 获取当前环境
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"当前环境: {environment}");

// ============================================================
// 构建多层级、多源配置
// 配置优先级（level 越高优先级越高）：
//   Level 0: 基础配置（JSON, YAML, TOML, XML, INI）
//   Level 5: 功能开关配置
//   Level 10: 环境变量配置
//   Level 15: 本地覆盖配置（可写）
// ============================================================
var cfg = new CfgBuilder()
    // === Level 0: 基础配置（多种格式） ===
    .AddJsonFile("config/base/app.json", level: 0)
    .AddYamlFile("config/base/database.yaml", level: 0)
    .AddTomlFile("config/base/cache.toml", level: 0)
    .AddXmlFile("config/base/services.xml", level: 0)
    .AddIniFile("config/base/security.ini", level: 0)

    // === Level 1: WebApi 配置 ===
    .AddJsonFile("config/apqcfg.json", level: 1)

    // === Level 5: 功能开关配置 ===
    .AddJsonFile("config/features/feature-flags.json", level: 5)

    // === Level 10: 环境特定配置 ===
    .AddEnvFile($"config/env/{environment.ToLower()}.env", level: 10, optional: true)

    // === Level 15: 本地覆盖配置（可写，作为主写入源） ===
    .AddJsonFile("config/local.json", level: 15, writeable: true, isPrimaryWriter: true, optional: true)

    .Build();

// 输出一些配置值
Console.WriteLine("\n已加载的配置:");
Console.WriteLine($"  App:Name = {cfg["App:Name"]}");
Console.WriteLine($"  Cache:Provider = {cfg["Cache:Provider"]}");

// ============================================================
// 添加 Apq.Cfg WebApi 服务
// ============================================================
builder.Services.AddApqCfgWebApi(cfg);

var app = builder.Build();

// 使用 Apq.Cfg WebApi 中间件
app.UseApqCfgWebApi();

// 映射 Apq.Cfg WebApi 端点
app.MapApqCfgWebApi();

// API 文档链接（根据框架版本不同）
#if NET8_0
var apiDocUrl = "/swagger";
var apiDocName = "Swagger";
#else
var apiDocUrl = "/scalar/v1";
var apiDocName = "Scalar";
#endif

// 添加一个简单的首页
app.MapGet("/", () => Results.Content($$"""
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Apq.Cfg WebApi Demo</title>
    <style>
        body { font-family: system-ui, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; }
        h1 { color: #333; }
        .links { margin: 20px 0; }
        .links a { display: inline-block; margin: 5px 10px 5px 0; padding: 10px 20px;
                   background: #0066cc; color: white; text-decoration: none; border-radius: 5px; }
        .links a:hover { background: #0052a3; }
        pre { background: #f5f5f5; padding: 15px; border-radius: 5px; overflow-x: auto; }
        .config-sources { margin: 20px 0; }
        .config-sources li { margin: 5px 0; }
    </style>
</head>
<body>
    <h1>🔧 Apq.Cfg WebApi Demo</h1>
    <p>这是一个演示 Apq.Cfg 配置系统的 Web API 项目，展示了多层级、多文件、多种源类型的配置管理。</p>

    <h2>配置源层级</h2>
    <ul class="config-sources">
        <li><strong>Level 0</strong>: 基础配置 (JSON, YAML, TOML, XML, INI)</li>
        <li><strong>Level 1</strong>: WebApi 配置</li>
        <li><strong>Level 5</strong>: 功能开关配置</li>
        <li><strong>Level 10</strong>: 环境变量配置</li>
        <li><strong>Level 15</strong>: 本地覆盖配置（可写）</li>
    </ul>

    <h2>快速链接</h2>
    <div class="links">
        <a href="{{apiDocUrl}}">📖 API 文档 ({{apiDocName}})</a>
        <a href="/api/apqcfg/merged">📋 查看合并配置</a>
        <a href="/api/apqcfg/merged/tree">🌳 配置树</a>
        <a href="/api/apqcfg/sources">📦 配置源列表</a>
    </div>

    <h2>API 端点示例</h2>
    <pre>
# 获取合并后的所有配置
GET /api/apqcfg/merged

# 获取配置树结构
GET /api/apqcfg/merged/tree

# 获取单个配置值
GET /api/apqcfg/merged/keys/App:Name

# 获取配置节
GET /api/apqcfg/merged/sections/Database

# 查看所有配置源
GET /api/apqcfg/sources

# 设置配置值（需要 API Key）
PUT /api/apqcfg/merged/keys/Local:Debug
Header: X-Api-Key: demo-api-key-12345
Body: "true"
    </pre>
</body>
</html>
""", "text/html"));

// 添加一个演示端点，展示如何读取配置
app.MapGet("/demo/config", () =>
{
    return new
    {
        AppName = cfg["App:Name"],
        AppVersion = cfg["App:Version"],
        DatabaseProvider = cfg["Database:Primary:Provider"],
        CacheProvider = cfg["Cache:Provider"],
        SecurityEnabled = cfg["Security:RequireAuthentication"],
        Features = new
        {
            NewDashboard = cfg["Features:NewDashboard:Enabled"],
            DarkMode = cfg["Features:DarkMode:Enabled"]
        }
    };
});

Console.WriteLine($"\n应用已启动！");
Console.WriteLine($"  首页: http://localhost:5000/");
Console.WriteLine($"  API 文档: http://localhost:5000{apiDocUrl}");
Console.WriteLine($"  配置 API: http://localhost:5000/api/apqcfg/merged");

app.Run();
