# 配置合并

Apq.Cfg 支持从多个配置源加载配置，并按优先级合并。

## 合并规则

### 优先级顺序

后添加的配置源优先级更高：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("base.json")           // 优先级 1（最低）
    .AddJsonFile("override.json")       // 优先级 2
    .AddEnvironmentVariables()          // 优先级 3（最高）
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
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
```

### 用户配置覆盖

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".myapp/config.json"), optional: true)
    .Build();
```

### 本地开发配置

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)  // gitignore
    .Build();
```

## 使用 MergedCfgRoot

对于需要显式控制合并行为的场景：

```csharp
var baseCfg = new CfgBuilder()
    .AddJsonFile("base.json")
    .Build();

var overrideCfg = new CfgBuilder()
    .AddJsonFile("override.json")
    .Build();

var merged = new MergedCfgRoot(baseCfg, overrideCfg);
```

## 下一步

- [动态重载](/guide/dynamic-reload) - 配置热更新
- [配置源](/config-sources/) - 了解所有配置源
