# ICfgRoot API

`ICfgRoot` 是配置根接口，提供配置读写的核心功能。

## 接口定义

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 读取操作
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);
    ICfgSection GetSection(string key);
    IEnumerable<string> GetChildKeys();
    
    // 写入操作
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default);
    
    // 批量操作
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);
    IReadOnlyDictionary<string, T?> GetMany<T>(IEnumerable<string> keys);
    void GetMany(IEnumerable<string> keys, Action<string, string?> onValue);
    void GetMany<T>(IEnumerable<string> keys, Action<string, T?> onValue);
    void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);
    
    // 转换与事件
    IConfigurationRoot ToMicrosoftConfiguration();
    IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options);
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

## 读取方法

### Get

```csharp
string? Get(string key)
```

通过键路径获取配置值。

**参数：**
- `key`: 配置键路径，使用冒号 `:` 分隔层级

**返回：** 配置值字符串，如果不存在返回 `null`

**示例：**
```csharp
var host = cfg.Get("Database:Host");
var port = cfg.Get("Database:Port");
var nested = cfg.Get("Services:Api:Url");
```

### Get&lt;T&gt;

```csharp
T? Get<T>(string key)
```

获取类型化的配置值。

**参数：**
- `key`: 配置键

**返回：** 转换后的值，如果不存在或转换失败返回 `default(T)`

**示例：**
```csharp
var port = cfg.Get<int>("Database:Port");
var timeout = cfg.Get<int>("Database:Timeout");
var enabled = cfg.Get<bool>("Feature:Enabled");
```

### Exists

```csharp
bool Exists(string key)
```

检查配置键是否存在。

**参数：**
- `key`: 配置键

**返回：** 是否存在

**示例：**
```csharp
if (cfg.Exists("OptionalFeature"))
{
    // 处理可选功能
}
```

### GetSection

```csharp
ICfgSection GetSection(string key)
```

获取指定键的配置节。

**参数：**
- `key`: 配置节键路径

**返回：** `ICfgSection` 配置节实例

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host");
var port = dbSection.Get<int>("Port");
```

### GetChildKeys

```csharp
IEnumerable<string> GetChildKeys()
```

获取所有顶级配置键。

**返回：** 顶级键名集合

**示例：**
```csharp
foreach (var key in cfg.GetChildKeys())
{
    Console.WriteLine($"顶级配置: {key}");
}
// 输出:
// 顶级配置: App
// 顶级配置: Database
// 顶级配置: Logging
```

### GetMany

```csharp
IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys)
IReadOnlyDictionary<string, T?> GetMany<T>(IEnumerable<string> keys)
void GetMany(IEnumerable<string> keys, Action<string, string?> onValue)
void GetMany<T>(IEnumerable<string> keys, Action<string, T?> onValue)
```

批量获取多个配置值，减少锁竞争。

**示例：**
```csharp
// 返回字典
var values = cfg.GetMany(new[] { "Database:Host", "Database:Port", "App:Name" });

// 返回类型化字典
var intValues = cfg.GetMany<int>(new[] { "Database:Port", "Database:Timeout" });

// 使用回调（零堆分配，性能更好）
cfg.GetMany(new[] { "Database:Host", "Database:Port" }, (key, value) =>
{
    Console.WriteLine($"{key} = {value}");
});

// 类型化回调
cfg.GetMany<int>(new[] { "Database:Port", "Database:Timeout" }, (key, value) =>
{
    Console.WriteLine($"{key} = {value}");
});
```

## 写入方法

### Set

```csharp
void Set(string key, string? value, int? targetLevel = null)
```

设置配置值。

**参数：**
- `key`: 配置键
- `value`: 配置值
- `targetLevel`: 目标层级（可选，默认写入可写的最高层级）

**示例：**
```csharp
cfg.Set("App:Name", "NewName");
cfg.Set("Database:Port", "5433", targetLevel: 2);
```

### Remove

```csharp
void Remove(string key, int? targetLevel = null)
```

移除配置键。

**参数：**
- `key`: 配置键
- `targetLevel`: 目标层级（可选，为 null 时从所有层级移除）

**示例：**
```csharp
cfg.Remove("DeprecatedSetting");
cfg.Remove("TempSetting", targetLevel: 1);
```

### SetMany

```csharp
void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null)
```

批量设置多个配置值，减少锁竞争。

**示例：**
```csharp
cfg.SetMany(new Dictionary<string, string?>
{
    ["Database:Host"] = "new-host",
    ["Database:Port"] = "5433"
});
```

### SaveAsync

```csharp
Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default)
```

保存配置到持久化存储。

**参数：**
- `targetLevel`: 目标层级（可选，默认保存所有可写层级）
- `cancellationToken`: 取消令牌

**示例：**
```csharp
cfg.Set("App:Name", "NewName");
await cfg.SaveAsync();

// 只保存特定层级
await cfg.SaveAsync(targetLevel: 2);
```

## 转换与事件

### ToMicrosoftConfiguration

```csharp
IConfigurationRoot ToMicrosoftConfiguration()
IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options)
```

转换为 Microsoft.Extensions.Configuration 的 `IConfigurationRoot`。

**参数：**
- `options`: 动态重载选项（可选），为 null 时返回静态快照

**返回：** `IConfigurationRoot` 实例

**示例：**
```csharp
// 静态快照
var msConfig = cfg.ToMicrosoftConfiguration();

// 支持动态重载
var dynamicConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    ReloadOnChange = true,
    DebounceInterval = TimeSpan.FromMilliseconds(500)
});

// 用于依赖注入
services.AddSingleton<IConfiguration>(msConfig);
```

### ConfigChanges

```csharp
IObservable<ConfigChangeEvent> ConfigChanges { get; }
```

配置变更的可观察序列（Rx Observable）。

**示例：**
```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"配置变更时间: {e.Timestamp}");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## 扩展方法

ICfgRoot 还提供了一些便捷的扩展方法：

### TryGetValue

```csharp
bool TryGetValue<T>(this ICfgRoot root, string key, out T? value)
```

尝试获取配置值。

**示例：**
```csharp
if (cfg.TryGetValue<int>("Database:Port", out var port))
{
    Console.WriteLine($"端口: {port}");
}
```

### GetRequired

```csharp
T GetRequired<T>(this ICfgRoot root, string key)
```

获取必需的配置值，如果不存在则抛出异常。

**示例：**
```csharp
var connectionString = cfg.GetRequired<string>("Database:ConnectionString");
```

### GetOrDefault

```csharp
T? GetOrDefault<T>(this ICfgRoot root, string key, T? defaultValue = default)
```

获取配置值，如果不存在则返回默认值。

**示例：**
```csharp
var timeout = cfg.GetOrDefault("Database:Timeout", 30);
var retryCount = cfg.GetOrDefault<int>("Database:RetryCount", 3);
```

### GetMasked

```csharp
string GetMasked(this ICfgRoot cfg, string key)
```

获取脱敏后的配置值（用于日志输出）。

**示例：**
```csharp
// 日志输出时使用脱敏值
logger.LogInformation("连接字符串: {ConnectionString}", cfg.GetMasked("Database:ConnectionString"));
// 输出: 连接字符串: Ser***ion
```

### GetMaskedSnapshot

```csharp
IReadOnlyDictionary<string, string> GetMaskedSnapshot(this ICfgRoot cfg)
```

获取所有配置的脱敏快照（用于调试）。

**示例：**
```csharp
var snapshot = cfg.GetMaskedSnapshot();
foreach (var (key, value) in snapshot)
{
    Console.WriteLine($"{key}: {value}");
}
```

## 完整示例

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    .Build();

// 读取配置
var host = cfg.Get("Database:Host");
var port = cfg.Get<int>("Database:Port");

// 检查配置是否存在
if (cfg.Exists("OptionalFeature:Enabled"))
{
    var enabled = cfg.Get<bool>("OptionalFeature:Enabled");
}

// 获取所有顶级键
foreach (var key in cfg.GetChildKeys())
{
    Console.WriteLine($"配置节: {key}");
}

// 获取配置节
var dbSection = cfg.GetSection("Database");
var connString = $"Host={dbSection.Get("Host")};Port={dbSection.Get("Port")}";

// 批量读取
var values = cfg.GetMany(new[] { "App:Name", "App:Version" });

// 使用扩展方法
var timeout = cfg.GetOrDefault("Database:Timeout", 30);
var connStr = cfg.GetRequired<string>("Database:ConnectionString");

// 修改配置
cfg.Set("App:Name", "NewName");
cfg.SetMany(new Dictionary<string, string?>
{
    ["Database:Host"] = "new-host",
    ["Database:Port"] = "5433"
});

// 保存配置
await cfg.SaveAsync();

// 订阅配置变更
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("配置已更新");
});

// 转换为 Microsoft Configuration
var msConfig = cfg.ToMicrosoftConfiguration();

// 获取脱敏快照用于调试
var maskedSnapshot = cfg.GetMaskedSnapshot();
```

## 下一步

- [ICfgSection API](/api/icfg-section) - 配置节接口
- [扩展方法](/api/extensions) - 所有扩展方法
