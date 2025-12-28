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
    
    // 写入操作
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken ct = default);
    
    // 批量操作
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);
    void GetMany(IEnumerable<string> keys, Action<string, string?> onValue);
    void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);
    
    // 转换与事件
    IConfigurationRoot ToMicrosoftConfiguration();
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

### GetMany

```csharp
IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys)
void GetMany(IEnumerable<string> keys, Action<string, string?> onValue)
```

批量获取多个配置值。

**示例：**
```csharp
// 返回字典
var values = cfg.GetMany(new[] { "Database:Host", "Database:Port", "App:Name" });

// 使用回调（性能更好）
cfg.GetMany(new[] { "Database:Host", "Database:Port" }, (key, value) =>
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
- `targetLevel`: 目标层级（可选，默认写入最高优先级的可写源）

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
- `targetLevel`: 目标层级（可选）

**示例：**
```csharp
cfg.Remove("DeprecatedSetting");
```

### SetMany

```csharp
void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null)
```

批量设置多个配置值。

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
Task SaveAsync(int? targetLevel = null, CancellationToken ct = default)
```

保存配置到持久化存储。

**参数：**
- `targetLevel`: 目标层级（可选，默认保存所有层级）
- `ct`: 取消令牌

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
```

转换为 Microsoft.Extensions.Configuration 的 `IConfigurationRoot`。

**返回：** `IConfigurationRoot` 实例

**示例：**
```csharp
var msConfig = cfg.ToMicrosoftConfiguration();

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

// 获取配置节
var dbSection = cfg.GetSection("Database");
var connString = $"Host={dbSection.Get("Host")};Port={dbSection.Get("Port")}";

// 批量读取
var values = cfg.GetMany(new[] { "App:Name", "App:Version" });

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
```

## 下一步

- [ICfgSection API](/api/icfg-section) - 配置节接口
- [扩展方法](/api/extensions) - 所有扩展方法
