# 多配置源示例

本页展示如何组合使用多个配置源。

## 默认层级

每种配置源都有默认层级，如果不指定 `level` 参数，将使用默认值：

| 配置源类型 | 默认层级 |
|------------|----------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## 基本组合

### 本地文件 + 环境变量

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 默认层级 0
    .AddEnvironmentVariables(prefix: "APP_")  // 默认层级 400
    .Build();
```

### 多种文件格式

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 默认层级 0
    .AddYaml("config.yaml", optional: true)  // 默认层级 0
    .AddToml("config.toml", optional: true)  // 默认层级 0
    .Build();
```

## 环境特定配置

### 标准模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置（默认层级 0）
    .AddJson("config.json")
    // 环境特定配置（指定层级 1）
    .AddJson($"config.{environment}.json", level: 1, optional: true)
    // 本地开发覆盖（不提交到版本控制）
    .AddJson("config.local.json", level: 2, writeable: true, isPrimaryWriter: true, optional: true)
    // 环境变量（默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
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
    // 本地基础配置（默认层级 0）
    .AddJson("config.json")
    // 远程动态配置（默认层级 200）
    .AddConsul("http://consul:8500", "myapp/config/", enableHotReload: true)
    // 环境变量覆盖（默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

### Redis 集成

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 默认层级 0
    .AddRedis("localhost:6379", "config:myapp", enableHotReload: true)  // 默认层级 100
    .AddEnvironmentVariables(prefix: "APP_")  // 默认层级 400
    .Build();
```

## 敏感配置分离

### Vault 存储密钥

```csharp
var cfg = new CfgBuilder()
    // 普通配置（默认层级 0）
    .AddJson("config.json")
    // 远程配置（默认层级 200）
    .AddConsul("http://consul:8500", "myapp/config/", enableHotReload: true)
    // 敏感配置（默认层级 300）
    .AddVaultV2("https://vault:8200", "s.token", "kv", "myapp/secrets")
    // 环境变量（默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
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
    // 本地回退（默认层级 0）
    .AddJson("config.fallback.json")
    // 备用配置源（指定层级 5）
    .AddRedis("localhost:6379", "config:myapp", level: 5, optional: true)
    // 主配置源（指定层级 10）
    .AddConsul("http://consul:8500", "myapp/config/", level: 10, optional: true)
    .Build();
```

### 多数据中心

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://consul-dc1:8500";
        options.KeyPrefix = "myapp/config/";
        options.EnableHotReload = true;
    }, optional: true)  // 默认层级 200
    .AddJson("config.fallback.json")  // 默认层级 0
    .Build();
```

## 配置优先级示例

```csharp
// 使用默认层级时的优先级（从低到高）：
// 层级 0: 本地文件配置 (Json, Yaml, Toml, Ini, Xml)
// 层级 100: 数据存储配置 (Redis, Database)
// 层级 200: 配置中心 (Consul, Etcd, Nacos, Apollo, Zookeeper)
// 层级 300: 密钥管理 (Vault)
// 层级 400: 环境变量 (.env, EnvironmentVariables)

var cfg = new CfgBuilder()
    .AddJson("config.json")                                                    // 层级 0
    .AddJson($"config.{env}.json", level: 1, optional: true)                   // 层级 1（自定义）
    .AddConsul("http://consul:8500", "myapp/config/", enableHotReload: true)   // 层级 200
    .AddJson("config.local.json", level: 250, writeable: true, isPrimaryWriter: true, optional: true)  // 层级 250（自定义）
    .AddEnvironmentVariables(prefix: "APP_")                                   // 层级 400
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
    // 基础配置（默认层级 0）
    .AddJson("config.json")
    .AddYaml("config.yaml", optional: true)

    // 环境特定（指定层级 2）
    .AddJson($"config.{environment}.json", level: 2, optional: true)

    // 远程配置（默认层级 200，生产环境）
    .AddConsul(
        address: "http://consul:8500",
        keyPrefix: "myapp/config/",
        enableHotReload: true,
        optional: environment != "Production")

    // 本地覆盖（指定层级 250，开发环境）
    .AddJson("config.local.json", level: 250, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)

    // 环境变量（默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();

// 监听变更
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"配置已更新: {string.Join(", ", e.Changes.Keys)}");
});

// 使用配置
var db = cfg.GetSection("Database");
Console.WriteLine($"数据库: {db["Host"]}:{db.Get<int>("Port")}");
```

## 下一步

- [依赖注入](/examples/di-integration) - DI 集成示例
- [动态重载](/examples/dynamic-reload) - 配置热更新示例
