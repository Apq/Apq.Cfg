# Apq.Cfg.WebApi

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

为 Apq.Cfg 提供 RESTful API 接口，支持远程配置管理，内置 OpenAPI 文档。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg.WebApi
```

## API 文档 UI

根据目标框架自动选择最适合的 API 文档 UI：

| 目标框架 | UI 库 | 访问路径 |
|---------|-------|---------|
| .NET 8 | Swagger UI | `/swagger` |
| .NET 10+ | Scalar | `/scalar/v1` |

## 快速开始

### 方式一：服务集合扩展

```csharp
using Apq.Cfg;
using Apq.Cfg.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddJson("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build();

// 添加服务
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});

var app = builder.Build();

app.UseApqCfgWebApi();
app.MapApqCfgWebApi();

app.Run();
```

### 方式二：链式调用

```csharp
using Apq.Cfg;
using Apq.Cfg.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 构建配置并添加 WebApi（链式调用）
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddJson("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build()
    .AddWebApi(builder.Services, options =>
    {
        options.Authentication = AuthenticationType.ApiKey;
        options.ApiKey = "your-secret-key";
    });

var app = builder.Build();

app.UseApqCfgWebApi();
app.MapApqCfgWebApi();

app.Run();
```

启动后访问 API 文档 UI 即可查看接口文档。

## 配置方式

WebApi 选项可以通过两种方式配置：

### 1. 从 ICfgRoot 读取（推荐）

在配置文件中添加 `ApqCfg:WebApi` 节：

```json
{
  "ApqCfg": {
    "WebApi": {
      "Authentication": "ApiKey",
      "ApiKey": "your-secret-key",
      "AllowWrite": true,
      "EnableCors": true,
      "CorsOrigins": ["*"]
    }
  }
}
```

然后直接调用：

```csharp
builder.Services.AddApqCfgWebApi(cfg);
```

### 2. 代码配置（覆盖配置文件）

```csharp
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});
```

**配置优先级**（从低到高）：
1. 默认值
2. 从 `ICfgRoot` 的 `ApqCfg:WebApi` 节读取的配置
3. `configure` 回调中的配置

## OpenAPI 文档配置

API 文档默认启用，可通过配置调整：

```csharp
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.OpenApiEnabled = true;              // 是否启用 API 文档（默认 true）
    options.OpenApiTitle = "My Config API";     // 文档标题
    options.OpenApiDescription = "配置管理 API"; // 文档描述
    options.OpenApiVersion = "v1";              // API 版本
    options.OpenApiRoutePrefix = "swagger";     // UI 路由前缀
    options.OpenApiShowAuthorizationButton = true; // 显示认证按钮
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

### 合并配置（Merged）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/merged` | 获取合并后的所有配置 |
| GET | `/api/apqcfg/merged/tree` | 获取合并后的配置树 |
| GET | `/api/apqcfg/merged/keys/{key}` | 获取合并配置的单个值 |
| GET | `/api/apqcfg/merged/sections/{section}` | 获取合并配置的配置节 |
| PUT | `/api/apqcfg/merged/keys/{key}` | 设置合并配置值 |
| PUT | `/api/apqcfg/merged/batch` | 批量更新合并配置 |
| DELETE | `/api/apqcfg/merged/keys/{key}` | 删除合并配置值 |
| GET | `/api/apqcfg/merged/export/{format}` | 导出合并配置（json/env/kv） |

### 配置源（Sources）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/sources` | 获取所有配置源信息 |
| GET | `/api/apqcfg/sources/{level}/{name}` | 获取指定配置源内容 |
| GET | `/api/apqcfg/sources/{level}/{name}/tree` | 获取指定配置源的配置树 |
| GET | `/api/apqcfg/sources/{level}/{name}/keys/{key}` | 获取指定配置源的单个配置值 |
| PUT | `/api/apqcfg/sources/{level}/{name}/keys/{key}` | 设置指定配置源的配置值 |
| DELETE | `/api/apqcfg/sources/{level}/{name}/keys/{key}` | 删除指定配置源的配置值 |
| GET | `/api/apqcfg/sources/{level}/{name}/export/{format}` | 导出指定配置源（json/env/kv） |

### 管理操作

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/apqcfg/save` | 保存所有可写配置源 |
| POST | `/api/apqcfg/reload` | 重新加载所有配置源 |

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Enabled` | bool | true | 是否启用 API |
| `RoutePrefix` | string | `/api/apqcfg` | API 路由前缀 |
| `Authentication` | enum | None | 认证类型 |
| `EnableCors` | bool | true | 是否启用 CORS（允许任意来源） |
| `CorsOrigins` | string[] | `["*"]` | CORS 允许的来源 |
| `OpenApiEnabled` | bool | true | 是否启用 API 文档 |
| `OpenApiTitle` | string | `Apq.Cfg Web API` | API 文档标题 |
| `OpenApiRoutePrefix` | string | `swagger` / `scalar/v1` | API 文档 UI 路由前缀 |

## 许可证

MIT License
