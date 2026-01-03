# Web API 集成

Apq.Cfg.WebApi 提供 RESTful API 接口，支持远程配置管理。

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

// 注册配置和 API 服务
builder.Services.AddSingleton<ICfgRoot>(cfg);
builder.Services.AddApqCfgWebApi(options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});

var app = builder.Build();
app.UseApqCfgWebApi();
app.MapApqCfgWebApi();
app.Run();
```

## API 文档

API 文档 UI 根据目标框架自动选择：
- **.NET 8**：Swagger UI (`/swagger`)
- **.NET 10+**：Scalar (`/scalar/v1`)

## 配置选项

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    // 启用/禁用 API
    options.Enabled = true;

    // 路由前缀
    options.RoutePrefix = "/api/apqcfg";

    // 认证方式
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
    options.ApiKeyHeaderName = "X-Api-Key";

    // 权限控制
    options.AllowRead = true;
    options.AllowWrite = false;
    options.AllowDelete = false;

    // 敏感值脱敏
    options.MaskSensitiveValues = true;
    options.SensitiveKeyPatterns = ["*Password*", "*Secret*", "*Key*", "*Token*", "*ConnectionString*"];

    // CORS 配置
    options.EnableCors = false;
    options.CorsOrigins = ["*"];

    // OpenAPI 配置
    options.OpenApiEnabled = true;
    options.OpenApiTitle = "Apq.Cfg Web API";
    options.OpenApiDescription = "Apq.Cfg 配置管理 RESTful API";
    options.OpenApiVersion = "v1";
});
```

## 认证方式

### 无认证

```csharp
options.Authentication = AuthenticationType.None;
```

### API Key 认证

```csharp
options.Authentication = AuthenticationType.ApiKey;
options.ApiKey = "your-secret-key";
options.ApiKeyHeaderName = "X-Api-Key"; // 默认
```

请求时在 Header 中添加：
```
X-Api-Key: your-secret-key
```

### JWT Bearer 认证

```csharp
options.Authentication = AuthenticationType.JwtBearer;
options.Jwt = new JwtOptions
{
    Authority = "https://auth.example.com",
    Audience = "my-api",
    RequireHttpsMetadata = true,
    ValidateIssuer = true,
    ValidateAudience = true
};
```

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apqcfg/config` | 获取所有配置 |
| GET | `/api/apqcfg/config/{key}` | 获取单个配置值 |
| GET | `/api/apqcfg/config/section/{path}` | 获取配置节 |
| GET | `/api/apqcfg/config/tree` | 获取配置树结构 |
| GET | `/api/apqcfg/sources` | 获取配置源列表 |
| PUT | `/api/apqcfg/config/{key}` | 设置配置值 |
| PUT | `/api/apqcfg/config/batch` | 批量更新配置 |
| DELETE | `/api/apqcfg/config/{key}` | 删除配置键 |
| POST | `/api/apqcfg/reload` | 重新加载配置 |
| GET | `/api/apqcfg/export/{format}` | 导出配置（json/env/kv） |

## 敏感值脱敏

默认情况下，包含以下关键字的配置键会被自动脱敏：
- `*Password*`
- `*Secret*`
- `*Key*`
- `*Token*`
- `*ConnectionString*`

脱敏后的值显示为 `***`。

可以自定义敏感键模式：

```csharp
options.SensitiveKeyPatterns = ["*Credential*", "*Private*"];
```

或禁用脱敏：

```csharp
options.MaskSensitiveValues = false;
```

## 导出格式

### JSON 格式

```
GET /api/apqcfg/export/json
```

返回嵌套的 JSON 结构。

### 环境变量格式

```
GET /api/apqcfg/export/env
```

返回：
```
APP__NAME=MyApp
DATABASE__HOST=localhost
DATABASE__PORT=5432
```

### Key-Value 格式

```
GET /api/apqcfg/export/kv
```

返回：
```
App:Name=MyApp
Database:Host=localhost
Database:Port=5432
```

## 安全建议

1. **生产环境必须启用认证**：不要在生产环境使用 `AuthenticationType.None`
2. **限制写入权限**：默认 `AllowWrite = false`，仅在需要时开启
3. **使用 HTTPS**：确保 API 通过 HTTPS 访问
4. **定期轮换 API Key**：定期更换 API Key 以提高安全性
5. **启用敏感值脱敏**：保持 `MaskSensitiveValues = true`
