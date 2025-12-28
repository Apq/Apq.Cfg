# ICfgSection API

`ICfgSection` 是配置节接口，表示配置的一个子节。

## 接口定义

```csharp
public interface ICfgSection
{
    /// <summary>
    /// 配置节的键名
    /// </summary>
    string Key { get; }
    
    /// <summary>
    /// 配置节的值（如果是叶子节点）
    /// </summary>
    string? Value { get; }
    
    /// <summary>
    /// 配置节的完整路径
    /// </summary>
    string Path { get; }
    
    /// <summary>
    /// 通过键获取子配置值
    /// </summary>
    string this[string key] { get; }
    
    /// <summary>
    /// 获取子配置节
    /// </summary>
    ICfgSection GetSection(string key);
    
    /// <summary>
    /// 获取所有子节
    /// </summary>
    IEnumerable<ICfgSection> GetChildren();
}
```

## 属性

### Key

配置节的键名（不包含父路径）。

```csharp
var dbSection = cfg.GetSection("Database");
Console.WriteLine(dbSection.Key); // 输出: Database
```

### Value

配置节的值。对于叶子节点返回实际值，对于容器节点返回 `null`。

```csharp
var hostSection = cfg.GetSection("Database:Host");
Console.WriteLine(hostSection.Value); // 输出: localhost
```

### Path

配置节的完整路径。

```csharp
var portSection = cfg.GetSection("Database").GetSection("Port");
Console.WriteLine(portSection.Path); // 输出: Database:Port
```

### 索引器 this[string key]

获取子配置的值。

```csharp
var dbSection = cfg.GetSection("Database");
var host = dbSection["Host"];
var port = dbSection["Port"];
```

## 方法

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
var dbSection = cfg.GetSection("Database");
var connSection = dbSection.GetSection("Connection");
```

### GetChildren

```csharp
IEnumerable<ICfgSection> GetChildren()
```

获取所有直接子节。

**返回：** 子配置节集合

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
foreach (var child in dbSection.GetChildren())
{
    Console.WriteLine($"{child.Key} = {child.Value}");
}
```

## 扩展方法

### Get&lt;T&gt;

```csharp
public static T Get<T>(this ICfgSection section)
```

将配置节绑定到指定类型。

**返回：** 绑定后的对象实例

**示例：**
```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
}

var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
```

### GetValue&lt;T&gt;

```csharp
public static T GetValue<T>(this ICfgSection section, string key, T defaultValue = default)
```

获取子配置的类型化值。

**参数：**
- `key`: 子配置键
- `defaultValue`: 默认值

**返回：** 转换后的值

**示例：**
```csharp
var dbSection = cfg.GetSection("Database");
var port = dbSection.GetValue<int>("Port");
var timeout = dbSection.GetValue<int>("Timeout", 30);
```

### Exists

```csharp
public static bool Exists(this ICfgSection section)
```

检查配置节是否存在（有值或有子节）。

**返回：** 是否存在

**示例：**
```csharp
var optionalSection = cfg.GetSection("Optional");
if (optionalSection.Exists())
{
    // 处理可选配置
}
```

### Bind

```csharp
public static void Bind(this ICfgSection section, object instance)
```

将配置节绑定到现有对象实例。

**参数：**
- `instance`: 要绑定的对象实例

**示例：**
```csharp
var dbConfig = new DatabaseConfig();
cfg.GetSection("Database").Bind(dbConfig);
```

## 使用示例

### 基本用法

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// 获取配置节
var dbSection = cfg.GetSection("Database");

// 读取值
var host = dbSection["Host"];
var port = dbSection.GetValue<int>("Port");

// 遍历子节
foreach (var child in dbSection.GetChildren())
{
    Console.WriteLine($"{child.Key}: {child.Value}");
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

### 绑定到对象

```csharp
public class ServiceConfig
{
    public string Url { get; set; } = "";
    public int Timeout { get; set; } = 30;
    public bool Enabled { get; set; } = true;
}

var apiConfig = cfg.GetSection("Services:Api").Get<ServiceConfig>();
Console.WriteLine($"API URL: {apiConfig.Url}");
```

### 绑定到集合

```csharp
// JSON: { "Servers": ["s1", "s2", "s3"] }
var servers = cfg.GetSection("Servers").Get<List<string>>();

// JSON: { "Endpoints": [{ "Name": "api", "Url": "..." }, ...] }
var endpoints = cfg.GetSection("Endpoints").Get<List<EndpointConfig>>();
```

### 绑定到字典

```csharp
// JSON: { "ConnectionStrings": { "Default": "...", "Readonly": "..." } }
var connStrings = cfg.GetSection("ConnectionStrings")
    .Get<Dictionary<string, string>>();
```

## 下一步

- [扩展方法](/api/extensions) - 所有扩展方法参考
- [示例](/examples/) - 更多使用示例
