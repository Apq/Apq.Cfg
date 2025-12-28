# 多配置源示例

本页展示如何组合使用多个配置源。

## 基本组合

### 本地文件 + 环境变量

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddEnvironmentVariables(level: 1, prefix: "APP_")
    .Build();
```

### 多种文件格式

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddYaml("config.yaml", level: 1, writeable: false, optional: true)
    .AddToml("config.toml", level: 2, writeable: false, optional: true)
    .Build();
```

## 环境特定配置

### 标准模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
    ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 环境特定配置
    .AddJson($"config.{environment}.json", level: 1, writeable: false, optional: true)
    // 本地开发覆盖（不提交到版本控制）
    .AddJson("config.local.json", level: 2, writeable: true, isPrimaryWriter: true, optional: true)
    // 环境变量（最高优先级）
    .AddEnvironmentVariables(level: 3, prefix: "APP_")
    .Build();
```

### 配置文件示例

**config.json**（基础配置）:
```json
{
  "App": {
    "Name": "MyApplication"
  },
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "mydb"
  },
  "Logging": {
    "Level": "Warning"
  }
}
```

**config.Development.json**（开发环境）:
```json
{
  "Logging": {
    "Level": "Debug"
  },
  "Database": {
    "Database": "mydb_dev"
  }
}
```

**config.Production.json**（生产环境）:
```json
{
  "Database": {
    "Host": "prod-db.example.com"
  }
}
```

## 本地 + 远程配置

### Consul 集成

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 远程动态配置
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false, watch: true))
    // 环境变量覆盖
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

### Redis 集成

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new RedisCfgSource("localhost:6379", "config:myapp", level: 10, writeable: false, subscribeChanges: true))
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

## 敏感配置分离

### Vault 存储密钥

```csharp
var cfg = new CfgBuilder()
    // 普通配置
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false))
    // 敏感配置（密码、密钥等）
    .AddSource(new VaultCfgSource("https://vault:8200", "secret/myapp", level: 15, writeable: false))
    // 环境变量
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

### 配置结构

```
config.json:
{
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "mydb"
  }
}

Vault (secret/myapp):
{
  "Database": {
    "Username": "admin",
    "Password": "secret123"
  }
}
```

## 故障转移配置

### 远程配置回退

```csharp
var cfg = new CfgBuilder()
    // 本地回退（最低优先级）
    .AddJson("config.fallback.json", level: 0, writeable: false)
    // 主配置源
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false, optional: true))
    // 备用配置源
    .AddSource(new RedisCfgSource("localhost:6379", "config:myapp", level: 5, writeable: false, optional: true))
    .Build();
```

### 多数据中心

```csharp
var cfg = new CfgBuilder()
    .AddSource(new ConsulCfgSource(new ConsulCfgOptions
    {
        Addresses = new[]
        {
            "http://consul-dc1:8500",
            "http://consul-dc2:8500"
        },
        KeyPrefix = "myapp/config",
        Optional = true
    }, level: 10, writeable: false))
    .AddJson("config.fallback.json", level: 0, writeable: false)
    .Build();
```

## 配置优先级示例

```csharp
// 优先级从低到高（level 数值越大优先级越高）：
// level 0: 基础配置
// level 1: 环境配置
// level 10: 远程配置
// level 15: 本地覆盖
// level 20: 环境变量

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)                           // 优先级 0
    .AddJson($"config.{env}.json", level: 1, writeable: false, optional: true)    // 优先级 1
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false))  // 优先级 10
    .AddJson("config.local.json", level: 15, writeable: true, isPrimaryWriter: true, optional: true)    // 优先级 15
    .AddEnvironmentVariables(level: 20, prefix: "APP_")                           // 优先级 20
    .Build();
```

## 完整示例

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;
using Apq.Cfg.Consul;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
    ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0, writeable: false)
    .AddYaml("config.yaml", level: 1, writeable: false, optional: true)
    
    // 环境特定
    .AddJson($"config.{environment}.json", level: 2, writeable: false, optional: true)
    
    // 远程配置（生产环境）
    .AddSource(new ConsulCfgSource(
        address: "http://consul:8500",
        keyPrefix: "myapp/config",
        level: 10,
        writeable: false,
        watch: true,
        optional: environment != "Production"))
    
    // 本地覆盖（开发环境）
    .AddJson("config.local.json", level: 15, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    
    // 环境变量
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();

// 监听变更
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"配置已更新: {string.Join(", ", e.Changes.Keys)}");
});

// 使用配置
var dbSection = cfg.GetSection("Database");
Console.WriteLine($"数据库: {dbSection.Get("Host")}:{dbSection.Get<int>("Port")}");
```

## 下一步

- [依赖注入](/examples/di-integration) - DI 集成示例
- [动态重载](/examples/dynamic-reload) - 配置热更新示例
