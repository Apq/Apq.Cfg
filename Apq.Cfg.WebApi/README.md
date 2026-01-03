# Apq.Cfg.WebApi

为 Apq.Cfg 配置系统提供 RESTful API 接口，支持远程配置管理。

## 安装

```bash
dotnet add package Apq.Cfg.WebApi
```

## 快速开始

### 1. 注册服务

```csharp
using Apq.Cfg;
using Apq.Cfg.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build();

// 注册配置 API 服务
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.RoutePrefix = "api/apqcfg";
    options.EnableSwagger = true;
    options.AuthenticationType = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-api-key";
});

var app = builder.Build();

// 使用配置 API 中间件
app.UseApqCfgWebApi();

app.Run();
```

### 2. 使用 JWT 认证

```csharp
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.AuthenticationType = AuthenticationType.JwtBearer;
    options.JwtOptions = new JwtOptions
    {
        Secret = "your-jwt-secret-key-at-least-32-characters",
        Issuer = "your-app",
        Audience = "your-api"
    };
});
```

### 3. 无认证（仅开发环境）

```csharp
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.AuthenticationType = AuthenticationType.None;
});
```

## API 端点

### 配置值操作

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/values/{key}` | 获取配置值 |
| PUT | `/api/apqcfg/values/{key}` | 设置配置值 |
| DELETE | `/api/apqcfg/values/{key}` | 删除配置值 |
| GET | `/api/apqcfg/values` | 获取所有配置值 |
| PUT | `/api/apqcfg/values/batch` | 批量更新配置 |

### 配置树操作

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/tree` | 获取合并后的配置树 |
| GET | `/api/apqcfg/sources/{level}/{name}/tree` | 获取指定配置源的配置树 |

### 配置源操作

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/sources` | 获取所有配置源信息 |
| GET | `/api/apqcfg/sources/{level}/{name}/values` | 获取指定配置源的所有值 |
| PUT | `/api/apqcfg/sources/{level}/{name}/values/{key}` | 设置指定配置源的值 |
| DELETE | `/api/apqcfg/sources/{level}/{name}/values/{key}` | 删除指定配置源的值 |

### 管理操作

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/apqcfg/save` | 保存配置到持久化存储 |
| POST | `/api/apqcfg/reload` | 重新加载配置 |
| GET | `/api/apqcfg/export?format=json` | 导出配置（json/env/kv） |

## 配置选项

```csharp
public class WebApiOptions
{
    /// <summary>
    /// API 路由前缀，默认 "api/apqcfg"
    /// </summary>
    public string RoutePrefix { get; set; } = "api/apqcfg";

    /// <summary>
    /// 是否启用 Swagger 文档
    /// </summary>
    public bool EnableSwagger { get; set; } = true;

    /// <summary>
    /// 认证类型
    /// </summary>
    public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.None;

    /// <summary>
    /// API Key（当 AuthenticationType 为 ApiKey 时使用）
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// JWT 配置（当 AuthenticationType 为 JwtBearer 时使用）
    /// </summary>
    public JwtOptions? JwtOptions { get; set; }

    /// <summary>
    /// 敏感键模式列表，匹配的键值将被脱敏显示
    /// </summary>
    public List<string> SensitiveKeyPatterns { get; set; } = new()
    {
        "*Password*", "*Secret*", "*Key*", "*Token*", "*ConnectionString*"
    };
}
```

## 响应格式

所有 API 返回统一的响应格式：

```json
{
    "success": true,
    "data": { ... },
    "error": null
}
```

错误响应：

```json
{
    "success": false,
    "data": null,
    "error": "错误信息"
}
```

## 配置源信息

`GET /api/apqcfg/sources` 返回：

```json
{
    "success": true,
    "data": [
        {
            "level": 0,
            "name": "config.json",
            "isWriteable": false,
            "isPrimaryWriter": false,
            "keyCount": 10
        },
        {
            "level": 5,
            "name": "config.local.json",
            "isWriteable": true,
            "isPrimaryWriter": true,
            "keyCount": 3
        }
    ]
}
```

## 许可证

MIT License
