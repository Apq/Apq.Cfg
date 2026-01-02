# Apq.Cfg.WebApi 设计方案

## 概述

Apq.Cfg.WebApi 是一个 ASP.NET Core 扩展项目，用于通过 RESTful API 暴露配置内容。

**核心特性：**
- 提供**合并后**的配置（merged）- 所有配置源合并后的最终结果
- 提供**合并前**的配置（sources）- 各个配置源的原始内容

## 前置依赖：Apq.Cfg 核心项目修改

本项目依赖 Apq.Cfg 核心项目的以下修改：

### 1. ICfgSource 接口添加 Name 属性

```csharp
public interface ICfgSource
{
    /// <summary>
    /// 配置源名称（同一层级内唯一）
    /// </summary>
    string Name { get; }

    // ... 其他现有属性
}
```

### 2. CfgBuilder.AddSource 方法添加可选 name 参数

```csharp
public CfgBuilder AddSource(ICfgSource source, int level = 0, string? name = null)
{
    // 如果未指定名称，自动生成
    var sourceName = name ?? GenerateSourceName(source);

    // 验证同一层级内名称唯一
    if (_sources.Any(s => s.Level == level && s.Name == sourceName))
    {
        throw new InvalidOperationException(
            $"配置源名称 '{sourceName}' 在层级 {level} 中已存在");
    }

    source.Name = sourceName;
    source.Level = level;
    _sources.Add(source);
    return this;
}

private string GenerateSourceName(ICfgSource source)
{
    return source switch
    {
        // 文件配置源：使用文件名（带后缀）
        IFileCfgSource fileSource => Path.GetFileName(fileSource.FilePath),

        // 环境变量
        EnvVarsCfgSource => "env",

        // 命令行参数
        CommandLineCfgSource => "commandline",

        // 内存配置源
        MemoryCfgSource => "memory",

        // 远程配置源：使用类型名小写（去掉 CfgSource 后缀）
        _ when source.GetType().Name.EndsWith("CfgSource") =>
            source.GetType().Name[..^9].ToLowerInvariant(),

        // 其他：使用类型名小写
        _ => source.GetType().Name.ToLowerInvariant()
    };
}
```

### 3. 名称自动生成规则

| 配置源类型 | 自动生成名称 | 示例 |
|-----------|-------------|------|
| 文件配置源 | 文件名（带后缀） | `config.json`、`config.local.json` |
| 环境变量 | 固定 `env` | `env` |
| 命令行参数 | 固定 `commandline` | `commandline` |
| 内存配置源 | 固定 `memory` | `memory` |
| 远程配置源 | 类型名小写（去掉 CfgSource 后缀） | `redis`、`consul`、`nacos` |

### 4. 名称唯一性验证

- 同一层级（Level）内，配置源名称必须唯一
- 添加配置源时如果名称重复，抛出 `InvalidOperationException`
- 不同层级可以有相同名称的配置源

### 5. ConfigSourceInfo 类

```csharp
namespace Apq.Cfg;

/// <summary>
/// 配置源信息（用于查询和展示）
/// </summary>
public sealed class ConfigSourceInfo
{
    /// <summary>
    /// 配置源层级（优先级，数字越大优先级越高）
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 配置源名称（同一层级内唯一）
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 配置源类型名称
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// 是否可写
    /// </summary>
    public bool IsWriteable { get; set; }

    /// <summary>
    /// 是否为主写入源
    /// </summary>
    public bool IsPrimaryWriter { get; set; }

    /// <summary>
    /// 配置项数量
    /// </summary>
    public int KeyCount { get; set; }
}
```

### 6. ICfgRoot 添加获取配置源信息的方法

```csharp
public interface ICfgRoot
{
    // ... 现有成员

    /// <summary>
    /// 获取所有配置源信息
    /// </summary>
    IReadOnlyList<ConfigSourceInfo> GetSourceInfos();

    /// <summary>
    /// 根据层级和名称获取配置源
    /// </summary>
    ICfgSource? GetSource(int level, string name);
}
```

---

## 功能特性

- 提供 RESTful API 读写配置
- **同时暴露合并后和合并前的配置**
- 支持启用/禁用开关
- 支持虚拟目录（自定义路由前缀）
- 支持多种认证方式（无认证、API Key、JWT Bearer）
- 支持敏感值脱敏
- 支持 CORS 跨域
- **支持从配置文件读取选项**

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
```

> **注意**：`ConfigSourceInfo` 类位于 Apq.Cfg 核心项目中。

## 项目文件

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
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
    /// 配置节名称，用于从 IConfiguration 绑定
    /// </summary>
    public const string SectionName = "ApqCfg:WebApi";

    /// <summary>
    /// 是否启用 API，默认 true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// API 路由前缀，默认 "/api/apqcfg"
    /// </summary>
    public string RoutePrefix { get; set; } = "/api/apqcfg";

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

---

## API 端点设计

### 合并后配置（Merged）

| 方法 | 路由 | 说明 | 权限 |
|------|------|------|------|
| GET | `{prefix}/merged` | 获取合并后的所有配置（扁平化键值对） | AllowRead |
| GET | `{prefix}/merged/tree` | 获取合并后的配置树结构 | AllowRead |
| GET | `{prefix}/merged/keys/{*key}` | 获取合并后的单个配置值 | AllowRead |
| GET | `{prefix}/merged/sections/{*section}` | 获取合并后的配置节 | AllowRead |

### 合并前配置（Sources）

| 方法 | 路由 | 说明 | 权限 |
|------|------|------|------|
| GET | `{prefix}/sources` | 获取所有配置源列表 | AllowRead |
| GET | `{prefix}/sources/{level}/{name}` | 获取指定配置源内容 | AllowRead |
| GET | `{prefix}/sources/{level}/{name}/tree` | 获取指定配置源的配置树结构 | AllowRead |
| GET | `{prefix}/sources/{level}/{name}/keys/{*key}` | 获取指定配置源的单个配置值 | AllowRead |

> **说明**：`{level}` 是配置源层级，`{name}` 是配置源名称，两者组合作为唯一标识符。同一层级内名称必须唯一。

### 写入操作

| 方法 | 路由 | 说明 | 权限 |
|------|------|------|------|
| PUT | `{prefix}/keys/{*key}` | 设置配置值（写入主写入源） | AllowWrite |
| PUT | `{prefix}/sources/{level}/{name}/keys/{*key}` | 设置指定配置源的配置值 | AllowWrite |
| PUT | `{prefix}/batch` | 批量设置配置 | AllowWrite |
| DELETE | `{prefix}/keys/{*key}` | 删除配置 | AllowDelete |
| DELETE | `{prefix}/sources/{level}/{name}/keys/{*key}` | 删除指定配置源的配置 | AllowDelete |

### 管理操作

| 方法 | 路由 | 说明 | 权限 |
|------|------|------|------|
| POST | `{prefix}/save` | 保存配置到持久化存储 | AllowWrite |
| POST | `{prefix}/reload` | 重新加载配置 | AllowWrite |
| GET | `{prefix}/export/{format}` | 导出合并后配置（json/env/keyvalue） | AllowRead |
| GET | `{prefix}/sources/{level}/{name}/export/{format}` | 导出指定配置源 | AllowRead |

---

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

> **注意**：`ConfigSourceInfo` 类已移至 Apq.Cfg 核心项目，WebApi 直接使用 `Apq.Cfg.ConfigSourceInfo`。

### BatchUpdateRequest

```csharp
namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 批量更新请求
/// </summary>
public sealed class BatchUpdateRequest
{
    public Dictionary<string, string?> Values { get; set; } = new();

    /// <summary>
    /// 目标配置源的层级（可选，不指定则写入主写入源）
    /// </summary>
    public int? TargetLevel { get; set; }

    /// <summary>
    /// 目标配置源的名称（可选，不指定则写入主写入源）
    /// </summary>
    public string? TargetName { get; set; }
}
```

---

## 服务层

### IConfigApiService

```csharp
namespace Apq.Cfg.WebApi.Services;

/// <summary>
/// 配置 API 服务接口
/// </summary>
public interface IConfigApiService
{
    // ========== 合并后配置（Merged）==========

    /// <summary>
    /// 获取合并后的所有配置（扁平化键值对）
    /// </summary>
    Dictionary<string, string?> GetMergedConfig();

    /// <summary>
    /// 获取合并后的配置树结构
    /// </summary>
    ConfigTreeNode GetMergedTree();

    /// <summary>
    /// 获取合并后的单个配置值
    /// </summary>
    ConfigValueResponse GetMergedValue(string key);

    /// <summary>
    /// 获取合并后的配置节
    /// </summary>
    Dictionary<string, string?> GetMergedSection(string section);

    // ========== 合并前配置（Sources）==========

    /// <summary>
    /// 获取所有配置源列表
    /// </summary>
    List<ConfigSourceInfo> GetSources();

    /// <summary>
    /// 获取指定配置源的内容
    /// </summary>
    Dictionary<string, string?>? GetSourceConfig(int level, string name);

    /// <summary>
    /// 获取指定配置源的配置树
    /// </summary>
    ConfigTreeNode? GetSourceTree(int level, string name);

    /// <summary>
    /// 获取指定配置源的单个配置值
    /// </summary>
    ConfigValueResponse? GetSourceValue(int level, string name, string key);

    // ========== 写入操作 ==========

    /// <summary>
    /// 设置配置值（写入主写入源）
    /// </summary>
    bool SetValue(string key, string? value);

    /// <summary>
    /// 设置指定配置源的配置值
    /// </summary>
    bool SetSourceValue(int level, string name, string key, string? value);

    /// <summary>
    /// 批量设置配置
    /// </summary>
    bool BatchUpdate(BatchUpdateRequest request);

    /// <summary>
    /// 删除配置
    /// </summary>
    bool DeleteKey(string key);

    /// <summary>
    /// 删除指定配置源的配置
    /// </summary>
    bool DeleteSourceKey(int level, string name, string key);

    // ========== 管理操作 ==========

    /// <summary>
    /// 保存配置到持久化存储
    /// </summary>
    Task<bool> SaveAsync();

    /// <summary>
    /// 重新加载配置
    /// </summary>
    Task<bool> ReloadAsync();

    /// <summary>
    /// 导出配置
    /// </summary>
    string Export(string format, int? level = null, string? name = null);
}
```

### ConfigApiService

```csharp
namespace Apq.Cfg.WebApi.Services;

/// <summary>
/// 配置 API 服务实现
/// </summary>
public sealed class ConfigApiService : IConfigApiService
{
    private readonly ICfgRoot _cfgRoot;
    private readonly IOptions<WebApiOptions> _options;

    public ConfigApiService(ICfgRoot cfgRoot, IOptions<WebApiOptions> options)
    {
        _cfgRoot = cfgRoot;
        _options = options;
    }

    // ========== 合并后配置（Merged）==========

    public Dictionary<string, string?> GetMergedConfig()
    {
        var result = new Dictionary<string, string?>();
        foreach (var (key, value) in _cfgRoot.GetAllValues())
        {
            result[key] = MaskIfSensitive(key, value);
        }
        return result;
    }

    public ConfigTreeNode GetMergedTree()
    {
        return BuildTree(_cfgRoot.GetAllValues());
    }

    public ConfigValueResponse GetMergedValue(string key)
    {
        var value = _cfgRoot[key];
        return new ConfigValueResponse
        {
            Key = key,
            Value = MaskIfSensitive(key, value),
            Exists = value != null,
            IsMasked = IsSensitiveKey(key) && value != null
        };
    }

    // ========== 合并前配置（Sources）==========

    public List<ConfigSourceInfo> GetSources()
    {
        // 直接使用核心项目提供的方法
        return _cfgRoot.GetSourceInfos().ToList();
    }

    public Dictionary<string, string?>? GetSourceConfig(int level, string name)
    {
        var source = _cfgRoot.GetSource(level, name);
        if (source == null) return null;

        var result = new Dictionary<string, string?>();
        foreach (var (key, value) in source.GetAllValues())
        {
            result[key] = MaskIfSensitive(key, value);
        }
        return result;
    }

    // ... 其他方法实现

    // ========== 辅助方法 ==========

    private bool IsSensitiveKey(string key)
    {
        if (!_options.Value.MaskSensitiveValues) return false;

        return _options.Value.SensitiveKeyPatterns.Any(pattern =>
            MatchWildcard(key, pattern));
    }

    private string? MaskIfSensitive(string key, string? value)
    {
        if (value == null || !IsSensitiveKey(key)) return value;
        return "***";
    }

    private static bool MatchWildcard(string text, string pattern)
    {
        // 简单的通配符匹配实现
        var regex = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
    }

    private ConfigTreeNode BuildTree(IEnumerable<KeyValuePair<string, string?>> values)
    {
        // 构建配置树的实现
        // ...
    }
}
```

---

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
        var optionsBuilder = services.AddOptions<WebApiOptions>();

        // 支持从配置文件绑定
        services.AddOptions<WebApiOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                config.GetSection(WebApiOptions.SectionName).Bind(options);
            });

        // 代码配置覆盖配置文件
        if (configure != null)
        {
            services.Configure(configure);
        }

        // 注册服务
        services.AddSingleton<IConfigApiService, ConfigApiService>();

        // 添加控制器
        services.AddControllers()
            .AddApplicationPart(typeof(ConfigController).Assembly);

        return services;
    }

    /// <summary>
    /// 添加 Apq.Cfg Web API 服务（从配置节绑定）
    /// </summary>
    public static IServiceCollection AddApqCfgWebApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WebApiOptions>? configure = null)
    {
        services.Configure<WebApiOptions>(configuration.GetSection(WebApiOptions.SectionName));

        if (configure != null)
        {
            services.PostConfigure(configure);
        }

        services.AddSingleton<IConfigApiService, ConfigApiService>();
        services.AddControllers()
            .AddApplicationPart(typeof(ConfigController).Assembly);

        return services;
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
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<WebApiOptions>>().Value;

        if (!options.Enabled)
            return app;

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
```

---

## 配置文件示例

### config.json

```json
{
  "ApqCfg": {
    "WebApi": {
      "Enabled": true,
      "RoutePrefix": "/api/apqcfg",
      "Authentication": "ApiKey",
      "ApiKey": "my-secret-api-key",
      "ApiKeyHeaderName": "X-Api-Key",
      "AllowRead": true,
      "AllowWrite": true,
      "AllowDelete": false,
      "MaskSensitiveValues": true,
      "SensitiveKeyPatterns": [
        "*Password*",
        "*Secret*",
        "*Key*",
        "*Token*",
        "*ConnectionString*"
      ],
      "EnableCors": true,
      "CorsOrigins": ["http://localhost:5173", "https://admin.example.com"],
      "Jwt": {
        "Authority": "https://auth.example.com",
        "Audience": "config-api",
        "RequireHttpsMetadata": true,
        "ValidateIssuer": true,
        "ValidateAudience": true
      }
    }
  }
}
```

---

## 使用示例

### 基本使用（从配置文件读取）

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加 Apq.Cfg
builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json")
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true));

// 添加 Web API（从配置文件读取选项）
builder.Services.AddApqCfgWebApi();

var app = builder.Build();

app.UseApqCfgWebApi();
app.MapApqCfgWebApi();

app.Run();
```

### 代码配置（覆盖配置文件）

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.RoutePrefix = "/api/apqcfg";
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "my-secret-api-key";
    options.AllowWrite = true;
    options.AllowDelete = true;
});
```

### 混合配置（配置文件 + 代码）

```csharp
// 先从配置文件读取，再用代码覆盖部分选项
builder.Services.AddApqCfgWebApi(builder.Configuration, options =>
{
    // 覆盖 API Key（例如从环境变量）
    options.ApiKey = Environment.GetEnvironmentVariable("APQCFG_API_KEY");
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

---

## API 调用示例

### 获取合并后的所有配置

```http
GET /api/apqcfg/merged HTTP/1.1
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

### 获取合并后的配置树

```http
GET /api/apqcfg/merged/tree HTTP/1.1
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

### 获取所有配置源列表

```http
GET /api/apqcfg/sources HTTP/1.1
```

响应：
```json
{
  "success": true,
  "data": [
    {
      "level": 0,
      "name": "config.json",
      "type": "JsonCfgSource",
      "isWriteable": false,
      "isPrimaryWriter": false,
      "keyCount": 10
    },
    {
      "level": 1,
      "name": "config.local.json",
      "type": "JsonCfgSource",
      "isWriteable": true,
      "isPrimaryWriter": true,
      "keyCount": 3
    },
    {
      "level": 2,
      "name": "env",
      "type": "EnvCfgSource",
      "isWriteable": false,
      "isPrimaryWriter": false,
      "keyCount": 5
    }
  ]
}
```

### 获取指定配置源内容

```http
GET /api/apqcfg/sources/1/config.local.json HTTP/1.1
```

响应：
```json
{
  "success": true,
  "data": {
    "App:Version": "2.0.0",
    "Database:Host": "192.168.1.100",
    "Feature:Debug": "true"
  }
}
```

### 获取指定配置源的配置树

```http
GET /api/apqcfg/sources/1/config.local.json/tree HTTP/1.1
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
          { "key": "Version", "value": "2.0.0", "hasValue": true }
        ]
      },
      {
        "key": "Database",
        "children": [
          { "key": "Host", "value": "192.168.1.100", "hasValue": true }
        ]
      }
    ]
  }
}
```

### 设置配置值（写入主写入源）

```http
PUT /api/apqcfg/keys/App:Version HTTP/1.1
Content-Type: application/json
X-Api-Key: my-secret-api-key

"2.0.0"
```

### 设置指定配置源的配置值

```http
PUT /api/apqcfg/sources/1/config.local.json/keys/App:Version HTTP/1.1
Content-Type: application/json
X-Api-Key: my-secret-api-key

"2.0.0"
```

### 批量更新

```http
PUT /api/apqcfg/batch HTTP/1.1
Content-Type: application/json
X-Api-Key: my-secret-api-key

{
  "values": {
    "App:Name": "NewApp",
    "App:Version": "2.0.0"
  },
  "targetLevel": 1,
  "targetName": "config.local.json"
}
```

### 保存配置

```http
POST /api/apqcfg/save HTTP/1.1
X-Api-Key: my-secret-api-key
```

### 导出合并后配置

```http
GET /api/apqcfg/export/env HTTP/1.1
```

响应：
```
APP__NAME=MyApp
APP__VERSION=1.0.0
DATABASE__HOST=localhost
DATABASE__PORT=5432
```

### 导出指定配置源

```http
GET /api/apqcfg/sources/1/config.local.json/export/json HTTP/1.1
```

响应：
```json
{
  "App": {
    "Version": "2.0.0"
  },
  "Database": {
    "Host": "192.168.1.100"
  }
}
```
