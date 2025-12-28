# 示例概述

本节提供 Apq.Cfg 的各种使用示例。

## 示例列表

### 基础示例

- [基础示例](/examples/basic) - 基本配置读取和类型转换
- [多配置源](/examples/multi-source) - 组合多个配置源

### 集成示例

- [依赖注入](/examples/di-integration) - ASP.NET Core 集成
- [动态重载](/examples/dynamic-reload) - 配置热更新

### 高级示例

- [复杂场景](/examples/complex-scenarios) - 企业级应用配置

## 快速示例

### 最简单的用法

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var appName = cfg["App:Name"];
Console.WriteLine($"应用名称: {appName}");
```

### 多配置源

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddYamlFile("config.yaml", optional: true)
    .AddEnvironmentVariables()
    .Build();
```

### 强类型绑定

```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
}

var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
Console.WriteLine($"数据库: {dbConfig.Host}:{dbConfig.Port}");
```

### 依赖注入

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables());

builder.Services.Configure<DatabaseConfig>(
    builder.Configuration.GetSection("Database"));
```

### 动态重载

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .Build();

cfg.OnChange(changes =>
{
    Console.WriteLine("配置已更新!");
});
```

## 运行示例项目

项目中包含完整的示例代码：

```bash
cd Samples/Apq.Cfg.Samples
dotnet run
```

## 下一步

选择您感兴趣的示例深入了解：

- [基础示例](/examples/basic)
- [多配置源](/examples/multi-source)
- [依赖注入](/examples/di-integration)
