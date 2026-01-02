# 从 Microsoft.Extensions.Configuration 迁移

本指南帮助您从 `Microsoft.Extensions.Configuration` 迁移到 Apq.Cfg。

## 为什么迁移？

| 特性 | Microsoft.Extensions.Configuration | Apq.Cfg |
|------|-----------------------------------|---------|
| 配置模板/变量替换 | ❌ 不支持 | ✅ 支持 |
| 配置加密脱敏 | ❌ 需第三方库 | ✅ 内置支持 |
| 配置验证 | ⚠️ 需额外配置 | ✅ 内置支持 |
| 配置快照导出 | ❌ 不支持 | ✅ 支持 |
| 批量操作 | ❌ 不支持 | ✅ 支持（零堆分配） |
| 远程配置中心 | ⚠️ 需第三方库 | ✅ 内置 6+ 配置中心 |
| 可写配置 | ❌ 只读 | ✅ 支持写入和持久化 |

## 1. 替换 ConfigurationBuilder

### 基本配置

```csharp
// 之前 (Microsoft.Extensions.Configuration)
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 之后 (Apq.Cfg)
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson($"config.{env}.json", level: 1, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### 带热重载的配置

```csharp
// 之前
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", reloadOnChange: true)
    .Build();

// 之后
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();
```

## 2. 替换配置读取

### 基本读取

```csharp
// 之前
var value = config["Section:Key"];
var typedValue = config.GetValue<int>("Section:Key");
var section = config.GetSection("Section");

// 之后
var value = cfg["Section:Key"];
var typedValue = cfg.Get<int>("Section:Key");
var section = cfg.GetSection("Section");
```

### 带默认值的读取

```csharp
// 之前
var port = config.GetValue<int>("Server:Port", 8080);

// 之后
var port = cfg.GetOrDefault("Server:Port", 8080);
```

### 安全读取（TryGet）

```csharp
// 之前
var value = config["Key"];
if (value != null)
{
    // 使用 value
}

// 之后
if (cfg.TryGet<int>("Key", out var value))
{
    // 使用 value
}
```

## 3. 替换依赖注入

### 注册配置

```csharp
// 之前
services.AddSingleton<IConfiguration>(config);
services.Configure<DatabaseOptions>(config.GetSection("Database"));

// 之后
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));

services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### 使用 IOptions

```csharp
// 之前和之后相同
public class MyService
{
    private readonly DatabaseOptions _options;

    public MyService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
}
```

## 4. 替换配置绑定

### 绑定到对象

```csharp
// 之前
var options = new DatabaseOptions();
config.GetSection("Database").Bind(options);

// 之后（使用源生成器，零反射）
[CfgSection("Database")]
public partial class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}

var options = DatabaseOptions.BindFrom(cfg.GetSection("Database"));
```

## 5. 新增功能

迁移后，您可以使用 Apq.Cfg 的独有功能：

### 配置模板

```csharp
// config.json: { "App:Name": "MyApp", "App:LogPath": "${App:Name}/logs" }
var logPath = cfg.GetResolved("App:LogPath");
// 返回: "MyApp/logs"
```

### 配置加密

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true)
    .AddAesGcmEncryptionFromEnv()
    .Build();

// 自动解密 {ENC} 前缀的值
var password = cfg.Get("Database:Password");
```

### 配置验证

```csharp
var (cfg, result) = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Server:Port", 1, 65535))
    .BuildAndValidate();
```

### 可写配置

```csharp
cfg["App:LastRun"] = DateTime.Now.ToString();
await cfg.SaveAsync();
```

### 批量操作

```csharp
// 高性能批量读取（零堆分配）
cfg.GetMany(new[] { "Key1", "Key2", "Key3" }, (key, value) =>
{
    Console.WriteLine($"{key}: {value}");
});
```

## 6. API 对照表

| Microsoft.Extensions.Configuration | Apq.Cfg | 说明 |
|-----------------------------------|---------|------|
| `config["Key"]` | `cfg["Key"]` | 相同 |
| `config.GetValue<T>("Key")` | `cfg.Get<T>("Key")` | 方法名更简洁 |
| `config.GetValue<T>("Key", default)` | `cfg.GetOrDefault("Key", default)` | 更明确的语义 |
| `config.GetSection("Path")` | `cfg.GetSection("Path")` | 相同 |
| `config.GetChildren()` | `cfg.GetChildKeys()` | 返回键名列表 |
| `config.GetReloadToken()` | `cfg.ConfigChanges` | Rx 订阅方式 |
| N/A | `cfg.GetResolved("Key")` | 变量替换 |
| N/A | `cfg.Set("Key", "Value")` | 可写配置 |
| N/A | `cfg.SaveAsync()` | 持久化 |
| N/A | `cfg.GetMasked("Key")` | 脱敏输出 |
| N/A | `cfg.ExportSnapshot()` | 快照导出 |

## 7. 注意事项

### 配置文件命名

Apq.Cfg 推荐使用 `config.json` 而非 `appsettings.json`：

```csharp
// 推荐
.AddJson("config.json", level: 0)
.AddJson("config.local.json", level: 1)

// 不推荐
.AddJson("appsettings.json", level: 0)
```

### 层级设计

Apq.Cfg 使用 `level` 参数控制配置优先级，数值越大优先级越高：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddEnvironmentVariables(level: 2)          // 环境变量最高
    .Build();
```

### 资源释放

Apq.Cfg 的 `ICfgRoot` 实现了 `IDisposable` 和 `IAsyncDisposable`：

```csharp
// 推荐使用 using
using var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();

// 或在 DI 中自动管理生命周期
services.AddApqCfg(cfg => cfg.AddJson("config.json", level: 0));
```

## 下一步

- [基本用法](/guide/basic-usage) - 了解更多基础功能
- [配置模板](/guide/template) - 使用变量替换
- [加密脱敏](/guide/encryption-masking) - 保护敏感配置
- [最佳实践](/guide/best-practices) - 配置设计建议
