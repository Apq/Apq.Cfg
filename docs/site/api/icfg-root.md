# ICfgRoot API

`ICfgRoot` 是配置根接口，提供配置读取的核心功能。

## 接口定义

```csharp
public interface ICfgRoot : IDisposable
{
    /// <summary>
    /// 通过键获取配置值
    /// </summary>
    string this[string key] { get; }
    
    /// <summary>
    /// 获取配置节
    /// </summary>
    ICfgSection GetSection(string key);
    
    /// <summary>
    /// 获取所有子节
    /// </summary>
    IEnumerable<ICfgSection> GetChildren();
    
    /// <summary>
    /// 获取重载令牌
    /// </summary>
    IChangeToken GetReloadToken();
    
    /// <summary>
    /// 配置变更事件
    /// </summary>
    event EventHandler<ConfigChangeEvent> ConfigChanged;
}
```

## 属性

### 索引器 this[string key]

通过键路径获取配置值。

**参数：**
- `key`: 配置键路径，使用冒号 `:` 分隔层级

**返回：** 配置值字符串，如果不存在返回 `null`

**示例：**
```csharp
var host = cfg["Database:Host"];
var port = cfg["Database:Port"];
var nested = cfg["Services:Api:Url"];
```

## 方法

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
var host = dbSection["Host"];
var port = dbSection.GetValue<int>("Port");
```

### GetChildren

```csharp
IEnumerable<ICfgSection> GetChildren()
```

获取所有顶级配置节。

**返回：** 配置节集合

**示例：**
```csharp
foreach (var section in cfg.GetChildren())
{
    Console.WriteLine($"配置节: {section.Key}");
}
```

### GetReloadToken

```csharp
IChangeToken GetReloadToken()
```

获取配置重载令牌，用于监听配置变更。

**返回：** `IChangeToken` 变更令牌

**示例：**
```csharp
ChangeToken.OnChange(
    () => cfg.GetReloadToken(),
    () => Console.WriteLine("配置已重载"));
```

## 扩展方法

### GetValue&lt;T&gt;

```csharp
public static T GetValue<T>(this ICfgRoot cfg, string key, T defaultValue = default)
```

获取类型化的配置值。

**参数：**
- `key`: 配置键
- `defaultValue`: 默认值

**返回：** 转换后的值

**示例：**
```csharp
var port = cfg.GetValue<int>("Database:Port");
var timeout = cfg.GetValue<int>("Database:Timeout", 30);
var enabled = cfg.GetValue<bool>("Feature:Enabled");
```

### GetRequiredSection

```csharp
public static ICfgSection GetRequiredSection(this ICfgRoot cfg, string key)
```

获取必需的配置节，如果不存在则抛出异常。

**参数：**
- `key`: 配置节键

**返回：** `ICfgSection` 配置节

**异常：** `InvalidOperationException` 如果配置节不存在

**示例：**
```csharp
var dbSection = cfg.GetRequiredSection("Database");
```

### Exists

```csharp
public static bool Exists(this ICfgRoot cfg, string key)
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

## 事件

### ConfigChanged

配置变更时触发的事件。

**示例：**
```csharp
cfg.ConfigChanged += (sender, e) =>
{
    Console.WriteLine($"配置变更时间: {e.Timestamp}");
    foreach (var change in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {change.Key}");
    }
};
```

## 下一步

- [ICfgSection API](/api/icfg-section) - 配置节接口
- [扩展方法](/api/extensions) - 所有扩展方法
