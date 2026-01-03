# Apq.Cfg.WebApi

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

为 Apq.Cfg 提供 RESTful API 接口，支持远程配置管理。

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

var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddJson("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build();

builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.RoutePrefix = "api/apqcfg";
    options.AuthenticationType = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});

var app = builder.Build();
app.UseApqCfgWebApi();
app.Run();
```

## 认证方式

```csharp
// API Key 认证
options.AuthenticationType = AuthenticationType.ApiKey;
options.ApiKey = "your-secret-key";

// JWT Bearer 认证
options.AuthenticationType = AuthenticationType.JwtBearer;
options.JwtOptions = new JwtOptions
{
    Secret = "your-jwt-secret-key-at-least-32-characters",
    Issuer = "your-app",
    Audience = "your-api"
};

// 无认证（仅开发环境）
options.AuthenticationType = AuthenticationType.None;
```

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/merged` | 获取合并后的所有配置 |
| GET | `/api/apqcfg/merged/tree` | 获取合并后的配置树 |
| GET | `/api/apqcfg/merged/{key}` | 获取单个配置值 |
| PUT | `/api/apqcfg/merged/{key}` | 设置配置值 |
| DELETE | `/api/apqcfg/merged/{key}` | 删除配置值 |
| GET | `/api/apqcfg/sources` | 获取所有配置源信息 |
| GET | `/api/apqcfg/sources/{level}/{name}` | 获取指定配置源内容 |
| POST | `/api/apqcfg/save` | 保存配置 |
| POST | `/api/apqcfg/reload` | 重新加载配置 |
| GET | `/api/apqcfg/export?format=json` | 导出配置 |

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `RoutePrefix` | string | `api/apqcfg` | API 路由前缀 |
| `EnableSwagger` | bool | true | 是否启用 Swagger |
| `AuthenticationType` | enum | None | 认证类型 |
| `MaskSensitiveValues` | bool | true | 是否脱敏敏感值 |
| `SensitiveKeyPatterns` | List | `*Password*`, `*Secret*`... | 敏感键模式 |

## 许可证

MIT License
