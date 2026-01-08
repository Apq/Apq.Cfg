# 配置合并

Apq.Cfg 支持从多个配置源加载配置，并按层级（level）合并。

## 合并规则

### 层级优先级

使用 `level` 参数控制优先级，数值越大优先级越高：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("base.json", level: 0)                                // 优先级最低
    .AddJsonFile("override.json", level: 1)                            // 优先级中
    .AddEnvironmentVariables(level: 2, prefix: "APP_")             // 优先级最高
    .Build();
```

### 值覆盖

相同键的值会被高优先级配置覆盖：

```json
// base.json
{
  "Database": {
    "Host": "localhost",
    "Port": 5432
  }
}

// override.json
{
  "Database": {
    "Host": "production-server"
  }
}
```

结果：
- `Database:Host` = `"production-server"` (被覆盖)
- `Database:Port` = `5432` (保留)

### 深度合并

嵌套对象会进行深度合并，而不是整体替换：

```json
// base.json
{
  "Logging": {
    "Level": "Warning",
    "Console": { "Enabled": true },
    "File": { "Path": "/var/log/app.log" }
  }
}

// override.json
{
  "Logging": {
    "Level": "Debug",
    "Console": { "Format": "json" }
  }
}
```

合并结果：
```json
{
  "Logging": {
    "Level": "Debug",
    "Console": { "Enabled": true, "Format": "json" },
    "File": { "Path": "/var/log/app.log" }
  }
}
```

## 典型场景

### 环境特定配置

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddJsonFile($"config.{environment}.json", level: 1, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### 用户配置覆盖

```csharp
var userConfigPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    ".myapp/config.json");

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddJsonFile(userConfigPath, level: 1, writeable: true, optional: true)
    .Build();
```

### 本地开发配置

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddJsonFile("config.Development.json", level: 1, optional: true)
    .AddJsonFile("config.local.json", level: 2, writeable: true, optional: true, isPrimaryWriter: true)  // gitignore
    .Build();
```

## 默认层级

Apq.Cfg 为每种配置源定义了默认层级，如果不指定 `level` 参数，将使用默认值：

| 层级范围 | 用途 | 配置源 | 默认值 |
|----------|------|--------|--------|
| 0-99 | 基础配置文件 | Json, Ini, Xml, Yaml, Toml | 0 |
| 100-199 | 远程存储 | Redis, Database | 100 |
| 200-299 | 配置中心 | Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| 300-399 | 密钥管理 | Vault | 300 |
| 400+ | 环境相关 | .env, EnvironmentVariables | 400 |

> 层级间隔 100，方便用户在中间插入自定义层级。

### 使用默认层级

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                    // 使用默认层级 0
    .AddJsonFile("config.local.json", level: 50)   // 自定义层级 50
    .AddConsul(options => { ... })             // 使用默认层级 200
    .AddEnvironmentVariables()                 // 使用默认层级 400
    .Build();
```

## 层级设计建议

推荐的层级分配：

| 层级 | 用途 | 示例 |
|------|------|------|
| 0 | 基础配置 | config.json |
| 10-50 | 环境配置 | config.Production.json |
| 50-99 | 本地覆盖 | config.local.json |
| 100 | 远程存储 | Redis, Database |
| 200 | 配置中心 | Consul, Nacos, Apollo |
| 300 | 密钥管理 | Vault |
| 400 | 环境变量 | 最高优先级覆盖 |

```csharp
var cfg = new CfgBuilder()
    // 基础配置（使用默认层级 0）
    .AddJsonFile("config.json")
    .AddJsonFile($"config.{env}.json", level: 10, optional: true)

    // 本地覆盖
    .AddJsonFile("config.local.json", level: 50, writeable: true, optional: true, isPrimaryWriter: true)

    // 远程配置（使用默认层级 200）
    .AddConsul(options => { ... })

    // 环境变量（使用默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")

    .Build();
```

## 写入配置

当配置有多个可写源时，可以指定写入目标：

```csharp
// 写入到默认的可写源（isPrimaryWriter: true 的源）
cfg["Database:Timeout"] = "60";
await cfg.SaveAsync();

// 写入到特定层级
cfg["Database:Timeout"] = "60";
await cfg.SaveAsync(targetLevel: 3);
```

## 下一步

- [动态重载](/guide/dynamic-reload) - 配置热更新
- [配置源](/config-sources/) - 了解所有配置源
