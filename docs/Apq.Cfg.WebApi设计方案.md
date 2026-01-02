# Apq.Cfg.WebApi 设计方案

## 概述

Apq.Cfg.WebApi 是一个 ASP.NET Core 扩展项目，用于通过 RESTful API 暴露合并后的配置内容。

## 功能特性

- 提供 RESTful API 读写配置
- 支持启用/禁用开关
- 支持虚拟目录（自定义路由前缀）
- 支持多种认证方式（无认证、API Key、JWT Bearer）
- 支持敏感值脱敏
- 支持嵌入式 WebUI
- 支持 CORS 跨域

## 项目结构

```
Apq.Cfg.WebApi/
├── Apq.Cfg.WebApi.csproj
├── README.md
├── WebApiOptions.cs                    # 配置选项
├── JwtOptions.cs                       # JWT 配置
├── AuthenticationType.cs               # 认证类型枚举
├── Controllers/
│   └── ConfigController.cs             # API 控制器
├── Authentication/
│   ├── ApiKeyAuthenticationHandler.cs  # API Key 认证处理器
│   ├── ApiKeyAuthenticationOptions.cs  # API Key 认证选项
│   └── ApiKeyDefaults.cs               # 认证方案常量
├── Middleware/
│   └── ConfigApiMiddleware.cs          # 启用检查中间件
├── Models/
│   ├── ConfigValueResponse.cs          # 配置值响应
│   ├── ConfigTreeNode.cs               # 配置树节点
│   ├── BatchUpdateRequest.cs           # 批量更新请求
│   └── ApiResponse.cs                  # 统一响应格式
├── Services/
│   ├── IConfigApiService.cs            # 服务接口
│   └── ConfigApiService.cs             # 服务实现
├── Extensions/
│   ├── ServiceCollectionExtensions.cs  # DI 扩展
│   └── ApplicationBuilderExtensions.cs # 中间件扩展
└── wwwroot/                            # 嵌入式 WebUI 静态文件
```

## 项目文件

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <RootNamespace>Apq.Cfg.WebApi</RootNamespace>
        <Description>Apq.Cfg 的 Web API 扩展，提供 RESTful API 暴露配置内容</Description>
        <PackageTags>configuration;config;webapi;rest</PackageTags>
    </PropertyGroup>

    <ItemGroup>
        <FrameworkReference Include="Microsoft.AspNetCore.App" />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer"
                          Version="$(MicrosoftAspNetCoreVersion)" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="..\Apq.Cfg\Apq.Cfg.csproj" />
    </ItemGroup>

    <!-- 嵌入 wwwroot 静态文件 -->
    <ItemGroup>
        <EmbeddedResource Include="wwwroot\**\*" />
    </ItemGroup>

</Project>
```

## 配置选项

### WebApiOptions

```csharp
namespace Apq.Cfg.WebApi;

/// <summary>
/// Web API 配置选项
/// </summary>
public sealed class WebApiOptions
{
    /// <summary>
    /// 是否启用 API，默认 true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// API 路由前缀，默认 "/api/config"
    /// </summary>
    public string RoutePrefix { get; set; } = "/api/config";

    /// <summary>
    /// 认证方式，默认无认证
    /// </summary>
    public AuthenticationType Authentication { get; set; } = AuthenticationType.None;

    /// <summary>
    /// API Key（当 Authentication 为 ApiKey 时使用）
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// API Key 请求头名称，默认 "X-Api-Key"
    /// </summary>
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";

    /// <summary>
    /// JWT 配置（当 Authentication 为 JwtBearer 时使用）
    /// </summary>
    public JwtOptions? Jwt { get; set; }

    /// <summary>
    /// 是否允许读取配置，默认 true
    /// </summary>
    public bool AllowRead { get; set; } = true;

    /// <summary>
    /// 是否允许写入配置，默认 false
    /// </summary>
    public bool AllowWrite { get; set; } = false;

    /// <summary>
    /// 是否允许删除配置，默认 false
    /// </summary>
    public bool AllowDelete { get; set; } = false;

    /// <summary>
    /// 是否脱敏敏感值，默认 true
    /// </summary>
    public bool MaskSensitiveValues { get; set; } = true;

    /// <summary>
    /// 敏感键模式（支持通配符）
    /// </summary>
    public string[] SensitiveKeyPatterns { get; set; } =
        ["*Password*", "*Secret*", "*Key*", "*Token*", "*ConnectionString*"];

    /// <summary>
    /// 是否启用嵌入式 WebUI，默认 false
    /// </summary>
    public bool EnableWebUI { get; set; } = false;

    /// <summary>
    /// WebUI 路径前缀，默认 "/config-ui"
    /// </summary>
    public string WebUIPath { get; set; } = "/config-ui";

    /// <summary>
    /// 是否启用 CORS，默认 false
    /// </summary>
    public bool EnableCors { get; set; } = false;

    /// <summary>
    /// CORS 允许的源
    /// </summary>
    public string[] CorsOrigins { get; set; } = ["*"];
}
```

### AuthenticationType

```csharp
namespace Apq.Cfg.WebApi;

/// <summary>
/// 认证类型
/// </summary>
public enum AuthenticationType
{
    /// <summary>
    /// 无认证
    /// </summary>
    None,

    /// <summary>
    /// API Key 认证
    /// </summary>
    ApiKey,

    /// <summary>
    /// JWT Bearer 认证
    /// </summary>
    JwtBearer
}
```

### JwtOptions

```csharp
namespace Apq.Cfg.WebApi;

/// <summary>
/// JWT 认证配置选项
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// 认证服务器地址
    /// </summary>
    public string Authority { get; set; } = "";

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = "";

    /// <summary>
    /// 是否要求 HTTPS 元数据，默认 true
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// 是否验证颁发者，默认 true
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// 是否验证受众，默认 true
    /// </summary>
    public bool ValidateAudience { get; set; } = true;
}
```

## API 端点设计

| 方法 | 路由 | 说明 | 权限 |
|------|------|------|------|
| GET | `{prefix}` | 获取所有配置（扁平化键值对） | AllowRead |
| GET | `{prefix}/tree` | 获取配置树结构 | AllowRead |
| GET | `{prefix}/keys/{*key}` | 获取单个配置值 | AllowRead |
| GET | `{prefix}/sections/{*section}` | 获取配置节 | AllowRead |
| PUT | `{prefix}/keys/{*key}` | 设置单个配置值 | AllowWrite |
| PUT | `{prefix}/batch` | 批量设置配置 | AllowWrite |
| DELETE | `{prefix}/keys/{*key}` | 删除配置 | AllowDelete |
| POST | `{prefix}/save` | 保存配置到持久化存储 | AllowWrite |
| POST | `{prefix}/reload` | 重新加载配置 | AllowWrite |
| GET | `{prefix}/export` | 导出配置快照（JSON） | AllowRead |
| GET | `{prefix}/export/{format}` | 导出指定格式（json/env/keyvalue） | AllowRead |

## 响应模型

### ApiResponse

```csharp
namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 统一 API 响应格式
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string error, string? errorCode = null)
        => new() { Success = false, Error = error, ErrorCode = errorCode };
}
```

### ConfigValueResponse

```csharp
namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 配置值响应
/// </summary>
public sealed class ConfigValueResponse
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public bool Exists { get; set; }
    public bool IsMasked { get; set; }
}
```

### ConfigTreeNode

```csharp
namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 配置树节点
/// </summary>
public sealed class ConfigTreeNode
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public bool HasValue { get; set; }
    public bool IsMasked { get; set; }
    public List<ConfigTreeNode> Children { get; set; } = new();
}
```

### BatchUpdateRequest

```csharp
namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 批量更新请求
/// </summary>
public sealed class BatchUpdateRequest
{
    public Dictionary<string, string?> Values { get; set; } = new();
    public int? TargetLevel { get; set; }
}
```

## 扩展方法

### ServiceCollectionExtensions

```csharp
namespace Apq.Cfg.WebApi;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Apq.Cfg Web API 服务
    /// </summary>
    public static IServiceCollection AddApqCfgWebApi(
        this IServiceCollection services,
        Action<WebApiOptions>? configure = null)
    {
        // 配置选项
        // 注册服务
        // 配置认证
        // 配置 CORS
        // 添加控制器
    }
}
```

### ApplicationBuilderExtensions

```csharp
namespace Apq.Cfg.WebApi;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用 Apq.Cfg Web API
    /// </summary>
    public static IApplicationBuilder UseApqCfgWebApi(this IApplicationBuilder app)
    {
        // 启用检查中间件
        // CORS
        // 认证
        // 嵌入式 WebUI
    }

    /// <summary>
    /// 映射 Apq.Cfg Web API 端点
    /// </summary>
    public static IEndpointRouteBuilder MapApqCfgWebApi(this IEndpointRouteBuilder endpoints)
    {
        // 映射控制器路由
    }
}
```

## 使用示例

### 基本使用

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加 Apq.Cfg
builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json")
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true));

// 添加 Web API（只读，无认证）
builder.Services.AddApqCfgWebApi();

var app = builder.Build();

app.UseApqCfgWebApi();
app.MapApqCfgWebApi();

app.Run();
```

### 启用写入和 API Key 认证

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.RoutePrefix = "/api/config";
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "my-secret-api-key";
    options.AllowWrite = true;
    options.AllowDelete = true;
});
```

### 启用 JWT 认证

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.Authentication = AuthenticationType.JwtBearer;
    options.Jwt = new JwtOptions
    {
        Authority = "https://auth.example.com",
        Audience = "config-api"
    };
    options.AllowWrite = true;
});
```

### 启用嵌入式 WebUI

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.EnableWebUI = true;
    options.WebUIPath = "/config-ui";
    options.EnableCors = true;
});
```

### 自定义虚拟目录

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.RoutePrefix = "/admin/configuration";
    options.WebUIPath = "/admin/config-ui";
});
```

## API 调用示例

### 获取所有配置

```http
GET /api/config HTTP/1.1
X-Api-Key: my-secret-api-key
```

响应：
```json
{
  "success": true,
  "data": {
    "App:Name": "MyApp",
    "App:Version": "1.0.0",
    "Database:Host": "localhost",
    "Database:Port": "5432",
    "Database:Password": "***"
  }
}
```

### 获取配置树

```http
GET /api/config/tree HTTP/1.1
```

响应：
```json
{
  "success": true,
  "data": {
    "key": "",
    "children": [
      {
        "key": "App",
        "children": [
          { "key": "Name", "value": "MyApp", "hasValue": true },
          { "key": "Version", "value": "1.0.0", "hasValue": true }
        ]
      },
      {
        "key": "Database",
        "children": [
          { "key": "Host", "value": "localhost", "hasValue": true },
          { "key": "Password", "value": "***", "hasValue": true, "isMasked": true }
        ]
      }
    ]
  }
}
```

### 设置配置值

```http
PUT /api/config/keys/App:Version HTTP/1.1
Content-Type: application/json
X-Api-Key: my-secret-api-key

"2.0.0"
```

### 批量更新

```http
PUT /api/config/batch HTTP/1.1
Content-Type: application/json
X-Api-Key: my-secret-api-key

{
  "values": {
    "App:Name": "NewApp",
    "App:Version": "2.0.0"
  },
  "targetLevel": 1
}
```

### 保存配置

```http
POST /api/config/save HTTP/1.1
X-Api-Key: my-secret-api-key
```

### 导出配置

```http
GET /api/config/export/env HTTP/1.1
```

响应：
```
APP__NAME=MyApp
APP__VERSION=1.0.0
DATABASE__HOST=localhost
DATABASE__PORT=5432
```
