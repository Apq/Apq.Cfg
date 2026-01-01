# 配置合并

Apq.Cfg 支持从多个配置源加载配置，并按层级（level）合并。

## 合并规则

### 层级优先级

使用 `level` 参数控制优先级，数值越大优先级越高：

```csharp
var cfg = new CfgBuilder()
    .AddJson("base.json", level: 0)                                // 优先级最低
    .AddJson("override.json", level: 1)                            // 优先级中
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
    .AddJson("config.json", level: 0)
    .AddJson($"config.{environment}.json", level: 1, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### 用户配置覆盖

```csharp
var userConfigPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    ".myapp/config.json");

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson(userConfigPath, level: 1, writeable: true, optional: true)
    .Build();
```

### 本地开发配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson("config.Development.json", level: 1, optional: true)
    .AddJson("config.local.json", level: 2, writeable: true, optional: true, isPrimaryWriter: true)  // gitignore
    .Build();
```

## 层级设计建议

推荐的层级分配：

| 层级范围 | 用途 | 示例 |
|----------|------|------|
| 0-2 | 基础配置 | config.json, config.{env}.json |
| 3-5 | 本地覆盖 | config.local.json, .env |
| 6-9 | 用户配置 | ~/.myapp/config.json |
| 10-19 | 远程配置 | Consul, Nacos, Apollo |
| 20+ | 环境变量 | 最高优先级覆盖 |

```csharp
var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0)
    .AddJson($"config.{env}.json", level: 1, optional: true)

    // 本地覆盖
    .AddJson("config.local.json", level: 3, writeable: true, optional: true, isPrimaryWriter: true)

    // 远程配置
    .AddSource(new ConsulCfgSource(/* ... */, level: 10))

    // 环境变量
    .AddEnvironmentVariables(level: 20, prefix: "APP_")

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
