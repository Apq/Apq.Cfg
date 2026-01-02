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
    /// 索引器：获取或设置子配置值
    /// </summary>
    string? this[string key] { get; set; }

    /// <summary>
    /// 获取类型化的子配置值
    /// </summary>
    T? GetValue<T>(string key);

    /// <summary>
    /// 检查配置键是否存在
    /// </summary>
    bool Exists(string key);

    /// <summary>
    /// 设置子配置值
    /// </summary>
    void SetValue(string key, string? value, int? targetLevel = null);

    /// <summary>
    /// 移除配置键
    /// </summary>
    void Remove(string key, int? targetLevel = null);

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

### 索引器

```csharp
string? this[string key] { get; set; }
```

获取或设置子配置的值。

**参数：**
- `key`: 子配置键（相对于此节的键名）

**返回：** 配置值，如果不存在返回 `null`

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
// 读取
var host = dbSection["Host"];      // 等同于 cfg["Database:Host"]
var port = dbSection["Port"];      // 等同于 cfg["Database:Port"]
// 写入
dbSection["Host"] = "new-host";
```

### GetValue&lt;T&gt;

```csharp
T? GetValue<T>(string key)
```

获取子配置的类型化值。

**参数：**
- `key`: 子配置键

**返回：** 转换后的值，不存在或转换失败时返回默认值

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
var port = dbSection.GetValue<int>("Port");       // 等同于 cfg.GetValue<int>("Database:Port")
var timeout = dbSection.GetValue<int>("Timeout"); // 等同于 cfg.GetValue<int>("Database:Timeout")
```

### Exists

```csharp
bool Exists(string key)
```

检查配置键是否存在。

**参数：**
- `key`: 子配置键（相对于此节的键名）

**返回：** 存在返回 `true`，否则返回 `false`

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
if (dbSection.Exists("ConnectionString"))
{
    var connStr = dbSection["ConnectionString"];
    // 处理连接字符串
}

if (dbSection.Exists("Password"))
{
    // 使用密码
}
else
{
    // 使用默认认证
}
```

### Set

```csharp
void SetValue(string key, string? value, int? targetLevel = null)
```

设置子配置值。

**参数：**
- `key`: 子配置键
- `value`: 配置值
- `targetLevel`: 目标层级（可选，默认写入可写的最高层级）

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
dbSection.SetValue("Host", "new-host");   // 等同于 cfg.SetValue("Database:Host", "new-host")
dbSection.SetValue("Port", "5433");       // 等同于 cfg.SetValue("Database:Port", "5433")
```

### Remove

```csharp
void Remove(string key, int? targetLevel = null)
```

移除配置键。

**参数：**
- `key`: 子配置键（相对于此节的键名）
- `targetLevel`: 目标层级（可选，为 null 时从所有层级移除）

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
dbSection.Remove("OldSetting");              // 等同于 cfg.Remove("Database:OldSetting")
dbSection.Remove("TempValue", targetLevel: 1); // 只从层级1移除
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
var apiSection = servicesSection.GetSection("Api");  // 等同于 cfg.GetSection("Services:Api")
var url = apiSection["Url"];
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
    Console.WriteLine($"{key} = {dbSection[key]}");
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
var host = dbSection["Host"];
var port = dbSection.GetValue<int>("Port");

// 检查键是否存在
if (dbSection.Exists("Password"))
{
    var password = dbSection["Password"];
}

// 遍历子键
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection[key]}");
}
```

### 嵌套配置节

```csharp
// JSON: { "Services": { "Api": { "Url": "...", "Timeout": 30 } } }

var apiSection = cfg.GetSection("Services:Api");
// 或
var apiSection = cfg.GetSection("Services").GetSection("Api");

var url = apiSection["Url"];
var timeout = apiSection.GetValue<int>("Timeout");
```

### 修改配置节

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

var dbSection = cfg.GetSection("Database");

// 设置值
dbSection.SetValue("Host", "new-host");
dbSection.SetValue("Port", "5433");

// 移除旧配置
dbSection.Remove("DeprecatedSetting");

await cfg.SaveAsync();
```

### 处理数组配置

```csharp
// JSON: { "Servers": ["s1", "s2", "s3"] }
var serversSection = cfg.GetSection("Servers");
foreach (var key in serversSection.GetChildKeys())
{
    var server = serversSection[key];
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
    var name = endpoint["Name"];
    var url = endpoint["Url"];
    Console.WriteLine($"Endpoint {name}: {url}");
}
```

### 检查配置节是否存在

```csharp
var optionalSection = cfg.GetSection("Optional");
var childKeys = optionalSection.GetChildKeys().ToList();
if (childKeys.Count > 0 || optionalSection.Exists("Value"))
{
    // 配置节存在且有内容
}
```

### 条件配置处理

```csharp
var featureSection = cfg.GetSection("Features");

// 检查功能是否启用
if (featureSection.Exists("NewUI") && featureSection.GetValue<bool>("NewUI"))
{
    // 启用新 UI
}

// 获取可选配置，带默认值
var maxRetries = featureSection.Exists("MaxRetries") 
    ? featureSection.GetValue<int>("MaxRetries") 
    : 3;
```

## 下一步

- [扩展方法](/api/extensions) - 所有扩展方法参考
- [示例](/examples/) - 更多使用示例
