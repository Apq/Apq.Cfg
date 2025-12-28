# ICfgSection API

`ICfgSection` 是配置节接口，表示配置的一个子节。

## 接口定义

```csharp
public interface ICfgSection
{
    /// <summary>
    /// 配置节的完整路径
    /// </summary>
    string Path { get; }
    
    /// <summary>
    /// 获取子配置值
    /// </summary>
    string? Get(string key);
    
    /// <summary>
    /// 获取类型化的子配置值
    /// </summary>
    T? Get<T>(string key);
    
    /// <summary>
    /// 设置子配置值
    /// </summary>
    void Set(string key, string? value, int? targetLevel = null);
    
    /// <summary>
    /// 获取子配置节
    /// </summary>
    ICfgSection GetSection(string key);
    
    /// <summary>
    /// 获取所有子键
    /// </summary>
    IEnumerable<string> GetChildKeys();
}
```

## 属性

### Path

配置节的完整路径。

```csharp
var dbSection = cfg.GetSection("Database");
Console.WriteLine(dbSection.Path); // 输出: Database

var portSection = dbSection.GetSection("Port");
Console.WriteLine(portSection.Path); // 输出: Database:Port
```

## 方法

### Get

```csharp
string? Get(string key)
```

获取子配置的值。

**参数：**
- `key`: 子配置键

**返回：** 配置值，如果不存在返回 `null`

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host");
var port = dbSection.Get("Port");
```

### Get&lt;T&gt;

```csharp
T? Get<T>(string key)
```

获取子配置的类型化值。

**参数：**
- `key`: 子配置键

**返回：** 转换后的值

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
var port = dbSection.Get<int>("Port");
var timeout = dbSection.Get<int>("Timeout");
```

### Set

```csharp
void Set(string key, string? value, int? targetLevel = null)
```

设置子配置值。

**参数：**
- `key`: 子配置键
- `value`: 配置值
- `targetLevel`: 目标层级（可选）

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
dbSection.Set("Host", "new-host");
dbSection.Set("Port", "5433");
```

### GetSection

```csharp
ICfgSection GetSection(string key)
```

获取子配置节。

**参数：**
- `key`: 子配置节键

**返回：** `ICfgSection` 子配置节

**示例：**
```csharp
var servicesSection = cfg.GetSection("Services");
var apiSection = servicesSection.GetSection("Api");
var url = apiSection.Get("Url");
```

### GetChildKeys

```csharp
IEnumerable<string> GetChildKeys()
```

获取所有直接子键。

**返回：** 子键集合

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key} = {dbSection.Get(key)}");
}
// 输出:
// Host = localhost
// Port = 5432
// Database = mydb
```

## 使用示例

### 基本用法

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// 获取配置节
var dbSection = cfg.GetSection("Database");

// 读取值
var host = dbSection.Get("Host");
var port = dbSection.Get<int>("Port");

// 遍历子键
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection.Get(key)}");
}
```

### 嵌套配置节

```csharp
// JSON: { "Services": { "Api": { "Url": "...", "Timeout": 30 } } }

var apiSection = cfg.GetSection("Services:Api");
// 或
var apiSection = cfg.GetSection("Services").GetSection("Api");

var url = apiSection.Get("Url");
var timeout = apiSection.Get<int>("Timeout");
```

### 修改配置节

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

var dbSection = cfg.GetSection("Database");
dbSection.Set("Host", "new-host");
dbSection.Set("Port", "5433");

await cfg.SaveAsync();
```

### 处理数组配置

```csharp
// JSON: { "Servers": ["s1", "s2", "s3"] }
var serversSection = cfg.GetSection("Servers");
foreach (var key in serversSection.GetChildKeys())
{
    var server = serversSection.Get(key);
    Console.WriteLine($"Server {key}: {server}");
}
// 输出:
// Server 0: s1
// Server 1: s2
// Server 2: s3
```

### 处理对象数组

```csharp
// JSON: { "Endpoints": [{ "Name": "api", "Url": "..." }, { "Name": "auth", "Url": "..." }] }
var endpointsSection = cfg.GetSection("Endpoints");
foreach (var key in endpointsSection.GetChildKeys())
{
    var endpoint = endpointsSection.GetSection(key);
    var name = endpoint.Get("Name");
    var url = endpoint.Get("Url");
    Console.WriteLine($"Endpoint {name}: {url}");
}
```

### 检查配置节是否存在

```csharp
var optionalSection = cfg.GetSection("Optional");
var childKeys = optionalSection.GetChildKeys().ToList();
if (childKeys.Count > 0 || optionalSection.Get("Value") != null)
{
    // 配置节存在
}
```

## 下一步

- [扩展方法](/api/extensions) - 所有扩展方法参考
- [示例](/examples/) - 更多使用示例
