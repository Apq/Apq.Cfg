# 多配置源示例

本页展示如何组合使用多个配置源。

## 基本组合

### 本地文件 + 环境变量

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
```

### 多种文件格式

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml", optional: true)
    .AddTomlFile("config.toml", optional: true)
    .Build();
```

## 环境特定配置

### 标准模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
    ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJsonFile("appsettings.json")
    // 环境特定配置
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    // 本地开发覆盖（不提交到版本控制）
    .AddJsonFile("appsettings.local.json", optional: true)
    // 环境变量（最高优先级）
    .AddEnvironmentVariables()
    .Build();
```

### 配置文件示例

**appsettings.json**（基础配置）:
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

**appsettings.Development.json**（开发环境）:
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

**appsettings.Production.json**（生产环境）:
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
    .AddJsonFile("appsettings.json")
    // 远程动态配置
    .AddConsul("http://consul:8500", "myapp/config", watch: true)
    // 环境变量覆盖
    .AddEnvironmentVariables()
    .Build();
```

### Redis 集成

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddRedis("localhost:6379", "config:myapp", subscribeChanges: true)
    .AddEnvironmentVariables()
    .Build();
```

### Apollo 集成

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddApollo("http://apollo:8080", "myapp", "application")
    .AddEnvironmentVariables()
    .Build();
```

## 敏感配置分离

### Vault 存储密钥

```csharp
var cfg = new CfgBuilder()
    // 普通配置
    .AddJsonFile("appsettings.json")
    .AddConsul("http://consul:8500", "myapp/config")
    // 敏感配置（密码、密钥等）
    .AddVault("https://vault:8200", "secret/myapp")
    // 环境变量
    .AddEnvironmentVariables()
    .Build();
```

### 配置结构

```
appsettings.json:
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
    // 主配置源
    .AddConsul("http://consul:8500", "myapp/config", optional: true)
    // 备用配置源
    .AddRedis("localhost:6379", "config:myapp", optional: true)
    // 本地回退
    .AddJsonFile("config.fallback.json")
    .Build();
```

### 多数据中心

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Addresses = new[]
        {
            "http://consul-dc1:8500",
            "http://consul-dc2:8500"
        };
        options.KeyPrefix = "myapp/config";
        options.Optional = true;
    })
    .AddJsonFile("config.fallback.json")
    .Build();
```

## 配置优先级示例

```csharp
// 优先级从低到高：
// 1. 基础配置
// 2. 环境配置
// 3. 远程配置
// 4. 本地覆盖
// 5. 环境变量
// 6. 命令行参数

var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")                    // 优先级 1
    .AddJsonFile($"appsettings.{env}.json", optional: true)  // 优先级 2
    .AddConsul("http://consul:8500", "myapp/config")    // 优先级 3
    .AddJsonFile("appsettings.local.json", optional: true)   // 优先级 4
    .AddEnvironmentVariables("MYAPP_")                  // 优先级 5
    .AddCommandLine(args)                               // 优先级 6
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
    .AddJsonFile("appsettings.json")
    .AddYamlFile("config.yaml", optional: true)
    
    // 环境特定
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    
    // 远程配置（生产环境）
    .AddConsul("http://consul:8500", "myapp/config", 
        watch: true, 
        optional: environment != "Production")
    
    // 本地覆盖（开发环境）
    .AddJsonFile("appsettings.local.json", optional: true)
    
    // 环境变量
    .AddEnvironmentVariables("MYAPP_")
    
    // 配置重载
    .ConfigureReload(options =>
    {
        options.DebounceDelay = 1000;
    })
    .Build();

// 监听变更
cfg.OnChange(keys =>
{
    Console.WriteLine($"配置已更新: {string.Join(", ", keys)}");
});

// 使用配置
var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
Console.WriteLine($"数据库: {dbConfig.Host}:{dbConfig.Port}");
```

## 下一步

- [依赖注入](/examples/di-integration) - DI 集成示例
- [动态重载](/examples/dynamic-reload) - 配置热更新示例
