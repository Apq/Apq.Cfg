# 快速开始

本指南将帮助你在 5 分钟内开始使用 Apq.Cfg。

## 前置条件

- .NET 6.0 或更高版本
- 任意 IDE（推荐 Visual Studio、VS Code 或 Rider）

## 安装

使用 NuGet 安装核心包：

```bash
dotnet add package Apq.Cfg
```

根据需要安装配置源包：

```bash
# JSON 配置（内置支持）
# 无需额外安装

# YAML 配置
dotnet add package Apq.Cfg.Yaml

# TOML 配置
dotnet add package Apq.Cfg.Toml

# Redis 配置
dotnet add package Apq.Cfg.Redis
```

## 基本用法

### 1. 创建配置文件

创建 `appsettings.json`：

```json
{
  "App": {
    "Name": "MyApp",
    "Version": "1.0.0",
    "MaxRetries": 3
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=mydb",
    "Timeout": 30
  }
}
```

### 2. 读取配置

```csharp
using Apq.Cfg;

// 创建配置构建器
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// 读取简单值
var appName = cfg.Get<string>("App:Name");
var maxRetries = cfg.Get<int>("App:MaxRetries");

// 读取带默认值
var timeout = cfg.Get<int>("Database:Timeout", 60);

Console.WriteLine($"应用名称: {appName}");
Console.WriteLine($"最大重试次数: {maxRetries}");
Console.WriteLine($"超时时间: {timeout}秒");
```

### 3. 绑定到对象

定义配置类：

```csharp
public class AppSettings
{
    public string Name { get; set; }
    public string Version { get; set; }
    public int MaxRetries { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}
```

绑定配置：

```csharp
// 绑定到对象
var appSettings = cfg.GetSection("App").Bind<AppSettings>();
var dbSettings = cfg.GetSection("Database").Bind<DatabaseSettings>();

Console.WriteLine($"应用: {appSettings.Name} v{appSettings.Version}");
Console.WriteLine($"数据库: {dbSettings.ConnectionString}");
```

## 多配置源

Apq.Cfg 支持同时使用多个配置源：

```csharp
var cfg = new CfgBuilder()
    // 基础配置
    .AddJsonFile("appsettings.json")
    // 环境特定配置（覆盖基础配置）
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    // 环境变量（最高优先级）
    .AddEnvironmentVariables()
    .Build();
```

后添加的配置源具有更高的优先级，会覆盖之前的同名配置。

## 依赖注入

与 ASP.NET Core 集成：

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 注册配置
builder.Services.AddApqCfg(cfg =>
{
    cfg.AddJsonFile("appsettings.json");
    cfg.AddYamlFile("config.yaml", optional: true);
});

// 注册配置节
builder.Services.Configure<AppSettings>(cfg => cfg.GetSection("App"));
builder.Services.Configure<DatabaseSettings>(cfg => cfg.GetSection("Database"));

var app = builder.Build();
```

在服务中使用：

```csharp
public class MyService
{
    private readonly AppSettings _settings;

    public MyService(IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    public void DoSomething()
    {
        Console.WriteLine($"使用配置: {_settings.Name}");
    }
}
```

## 下一步

- [安装指南](/guide/installation) - 了解所有可用的包
- [配置源](/sources/) - 探索不同的配置源
- [类型转换](/guide/type-conversion) - 了解类型转换机制
- [动态重载](/guide/dynamic-reload) - 实现配置热更新
