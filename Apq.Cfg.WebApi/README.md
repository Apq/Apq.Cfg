# Apq.Cfg.WebApi

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

为 Apq.Cfg 提供 RESTful API 接口，支持远程配置管理，内置 Swagger 文档。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg.WebApi
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddJson("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build();

// 添加服务（默认启用 Swagger）
builder.Services.AddSingleton<ICfgRoot>(cfg);
builder.Services.AddApqCfgWebApi(options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});

var app = builder.Build();

// 启用中间件（包含 Swagger）
app.UseApqCfgWebApi();
app.MapApqCfgWebApi();

app.Run();
```

启动后访问 `/swagger` 即可查看 API 文档。

## Swagger 配置

Swagger 默认启用，可通过 `WebApiOptions` 配置：

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.SwaggerEnabled = true;              // 是否启用 Swagger（默认 true）
    options.SwaggerTitle = "My Config API";     // 文档标题
    options.SwaggerDescription = "配置管理 API"; // 文档描述
    options.SwaggerVersion = "v1";              // API 版本
    options.SwaggerRoutePrefix = "swagger";     // Swagger UI 路由前缀
    options.SwaggerShowAuthorizationButton = true; // 显示认证按钮
});
```

## 认证方式

```csharp
// API Key 认证
options.Authentication = AuthenticationType.ApiKey;
options.ApiKey = "your-secret-key";
options.ApiKeyHeaderName = "X-Api-Key";  // 默认

// JWT Bearer 认证
options.Authentication = AuthenticationType.JwtBearer;
options.Jwt = new JwtOptions
{
    Authority = "https://your-auth-server",
    Audience = "your-api"
};

// 无认证（仅开发环境）
options.Authentication = AuthenticationType.None;
```

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/merged` | 获取合并后的所有配置 |
| GET | `/api/apqcfg/merged/tree` | 获取合并后的配置树 |
| GET | `/api/apqcfg/merged/keys/{key}` | 获取单个配置值 |
| GET | `/api/apqcfg/merged/sections/{section}` | 获取配置节 |
| GET | `/api/apqcfg/sources` | 获取所有配置源信息 |
| GET | `/api/apqcfg/sources/{level}/{name}` | 获取指定配置源内容 |
| GET | `/api/apqcfg/sources/{level}/{name}/tree` | 获取指定配置源的配置树 |
| PUT | `/api/apqcfg/keys/{key}` | 设置配置值 |
| PUT | `/api/apqcfg/batch` | 批量更新配置 |
| DELETE | `/api/apqcfg/keys/{key}` | 删除配置值 |
| POST | `/api/apqcfg/save` | 保存配置 |
| POST | `/api/apqcfg/reload` | 重新加载配置 |
| GET | `/api/apqcfg/export/{format}` | 导出配置（json/yaml） |

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Enabled` | bool | true | 是否启用 API |
| `RoutePrefix` | string | `/api/apqcfg` | API 路由前缀 |
| `Authentication` | enum | None | 认证类型 |
| `AllowRead` | bool | true | 是否允许读取 |
| `AllowWrite` | bool | false | 是否允许写入 |
| `AllowDelete` | bool | false | 是否允许删除 |
| `MaskSensitiveValues` | bool | true | 是否脱敏敏感值 |
| `SensitiveKeyPatterns` | string[] | `*Password*`, `*Secret*`... | 敏感键模式 |
| `EnableCors` | bool | false | 是否启用 CORS |
| `SwaggerEnabled` | bool | true | 是否启用 Swagger |
| `SwaggerTitle` | string | `Apq.Cfg Web API` | Swagger 文档标题 |
| `SwaggerRoutePrefix` | string | `swagger` | Swagger UI 路由前缀 |

## 许可证

MIT License
