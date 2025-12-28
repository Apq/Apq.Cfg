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
    .AddJsonFile("config.json")
    .Build();

// 2. 读取简单值
var appName = cfg["App:Name"];
Console.WriteLine($"应用名称: {appName}");

// 3. 读取类型化值
var maxConnections = cfg.GetValue<int>("Database:MaxConnections");
var enableConsole = cfg.GetValue<bool>("Logging:EnableConsole");
Console.WriteLine($"最大连接数: {maxConnections}");
Console.WriteLine($"启用控制台日志: {enableConsole}");

// 4. 读取配置节
var dbSection = cfg.GetSection("Database");
Console.WriteLine($"连接字符串: {dbSection["ConnectionString"]}");
Console.WriteLine($"超时时间: {dbSection["Timeout"]}");
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

绑定配置：

```csharp
// 绑定到强类型对象
var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
var appConfig = cfg.GetSection("App").Get<AppConfig>();

Console.WriteLine($"应用: {appConfig.Name} v{appConfig.Version}");
Console.WriteLine($"数据库超时: {dbConfig.Timeout}秒");
```

## 多配置源

Apq.Cfg 支持从多个来源加载配置：

```csharp
var cfg = new CfgBuilder()
    // 基础配置
    .AddJsonFile("config.json")
    // 环境特定配置（覆盖基础配置）
    .AddJsonFile($"config.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    // 环境变量（最高优先级）
    .AddEnvironmentVariables()
    .Build();
```

## 下一步

- [基础用法](/guide/basic-usage) - 深入了解配置读取
- [配置合并](/guide/config-merge) - 学习多配置源合并
- [动态重载](/guide/dynamic-reload) - 实现配置热更新
