# 快速开始

本教程将帮助您在 5 分钟内上手 Apq.Cfg。

## 创建项目

```bash
# 创建新的控制台项目
dotnet new console -n ApqCfgDemo
cd ApqCfgDemo

# 安装 Apq.Cfg
dotnet add package Apq.Cfg
```

## 创建配置文件

创建 `config.json`：

```json
{
  "App": {
    "Name": "My Application",
    "Version": "1.0.0"
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=mydb;",
    "MaxConnections": 100,
    "Timeout": 30
  },
  "Logging": {
    "Level": "Information",
    "EnableConsole": true
  }
}
```

## 读取配置

修改 `Program.cs`：

```csharp
using Apq.Cfg;

// 1. 创建配置构建器并加载配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();

// 2. 使用索引器读取配置
var appName = cfg["App:Name"];
Console.WriteLine($"应用名称: {appName}");

// 3. 读取类型化值
var maxConnections = cfg.Get<int>("Database:MaxConnections");
var enableConsole = cfg.Get<bool>("Logging:EnableConsole");
Console.WriteLine($"最大连接数: {maxConnections}");
Console.WriteLine($"启用控制台日志: {enableConsole}");

// 4. 使用配置节简化访问
var db = cfg.GetSection("Database");
Console.WriteLine($"连接字符串: {db["ConnectionString"]}");
Console.WriteLine($"超时时间: {db["Timeout"]}");
```

## 运行程序

```bash
dotnet run
```

输出：

```
应用名称: My Application
最大连接数: 100
启用控制台日志: True
连接字符串: Server=localhost;Database=mydb;
超时时间: 30
```

## 绑定到强类型对象

创建配置类：

```csharp
public class DatabaseConfig
{
    public string ConnectionString { get; set; } = "";
    public int MaxConnections { get; set; }
    public int Timeout { get; set; }
}

public class AppConfig
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
}
```

绑定配置（通过依赖注入）：

```csharp
// 在 DI 容器中注册
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0, writeable: false));

services.ConfigureApqCfg<DatabaseConfig>("Database");
services.ConfigureApqCfg<AppConfig>("App");

// 使用
public class MyService
{
    private readonly IOptions<DatabaseConfig> _dbConfig;
    private readonly IOptions<AppConfig> _appConfig;
    
    public MyService(IOptions<DatabaseConfig> dbConfig, IOptions<AppConfig> appConfig)
    {
        _dbConfig = dbConfig;
        _appConfig = appConfig;
    }
    
    public void PrintInfo()
    {
        Console.WriteLine($"应用: {_appConfig.Value.Name} v{_appConfig.Value.Version}");
        Console.WriteLine($"数据库超时: {_dbConfig.Value.Timeout}秒");
    }
}
```

## 多配置源

Apq.Cfg 支持从多个来源加载配置，数值越大的 level 优先级越高：

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置（level: 0，最低优先级）
    .AddJson("config.json", level: 0)
    // 环境特定配置（level: 1，覆盖基础配置）
    .AddJson($"config.{environment}.json", level: 1, optional: true)
    // 环境变量（level: 2，最高优先级）
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

## 下一步

- [基础用法](/guide/basic-usage) - 深入了解配置读取
- [配置合并](/guide/config-merge) - 学习多配置源合并
- [动态重载](/guide/dynamic-reload) - 实现配置热更新
