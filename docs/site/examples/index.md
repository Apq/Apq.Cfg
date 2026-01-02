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
- [加密脱敏](/guide/encryption-masking) - 配置加密与脱敏

## 默认层级

每种配置源都有默认层级，如果不指定 `level` 参数，将使用默认值：

| 配置源类型 | 默认层级 |
|------------|----------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## 快速示例

### 最简单的用法

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json")  // 使用默认层级 0
    .Build();

var appName = cfg.Get("App:Name");
Console.WriteLine($"应用名称: {appName}");
```

### 多配置源

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 默认层级 0
    .AddYaml("config.yaml", optional: true)  // 默认层级 0
    .AddEnvironmentVariables(prefix: "APP_")  // 默认层级 400
    .Build();
```

### 强类型绑定

```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
}

var dbSection = cfg.GetSection("Database");
var dbConfig = new DatabaseConfig
{
    Host = dbSection.Get("Host") ?? "localhost",
    Port = dbSection.Get<int>("Port")
};
Console.WriteLine($"数据库: {dbConfig.Host}:{dbConfig.Port}");
```

### 依赖注入

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json")  // 默认层级 0
    .AddEnvironmentVariables(prefix: "APP_"));  // 默认层级 400

builder.Services.ConfigureApqCfg<DatabaseConfig>("Database");
```

### 动态重载

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", reloadOnChange: true)  // 默认层级 0
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("配置已更新!");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}");
    }
});
```

### 加密脱敏

```csharp
using Apq.Cfg.Crypto;

var cfg = new CfgBuilder()
    .AddJson("config.json")  // 默认层级 0
    .AddAesGcmEncryptionFromEnv()  // 从环境变量读取密钥
    .AddSensitiveMasking()          // 添加脱敏支持
    .Build();

// 配置文件中的加密值: { "Database": { "Password": "{ENC}base64..." } }
// 读取时自动解密
var password = cfg.Get("Database:Password");

// 日志输出时使用脱敏值
Console.WriteLine($"密码: {cfg.GetMasked("Database:Password")}");
// 输出: 密码: myS***ord
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", writeable: true, isPrimaryWriter: true)  // 默认层级 0
    .Build();

// 修改配置
cfg.Set("App:Name", "NewName");
cfg.Set("Database:Port", "5433");

// 保存到文件
await cfg.SaveAsync();
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
