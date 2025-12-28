# 基础用法

本页详细介绍 Apq.Cfg 的基础用法。

## 配置构建器

`CfgBuilder` 是创建配置的入口点：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .Build();
```

### 链式调用

支持链式调用添加多个配置源：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddYamlFile("config.yaml", optional: true)
    .AddEnvironmentVariables()
    .Build();
```

### 配置源优先级

后添加的配置源优先级更高，会覆盖先添加的同名配置：

```csharp
// appsettings.json: { "Key": "value1" }
// appsettings.local.json: { "Key": "value2" }

var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")        // 优先级低
    .AddJsonFile("appsettings.local.json")  // 优先级高
    .Build();

Console.WriteLine(cfg["Key"]); // 输出: value2
```

## 读取配置值

### 索引器访问

使用冒号 `:` 分隔的键路径：

```csharp
// JSON: { "Database": { "Host": "localhost" } }
var host = cfg["Database:Host"];
```

### GetValue 方法

获取类型化的值：

```csharp
// 基本类型
var port = cfg.GetValue<int>("Database:Port");
var enabled = cfg.GetValue<bool>("Feature:Enabled");
var ratio = cfg.GetValue<double>("Settings:Ratio");

// 带默认值
var timeout = cfg.GetValue<int>("Database:Timeout", defaultValue: 30);

// 可空类型
var maxSize = cfg.GetValue<int?>("Cache:MaxSize");
```

### 支持的类型转换

| 类型 | 示例 |
|------|------|
| `string` | `"hello"` |
| `int`, `long` | `123`, `-456` |
| `float`, `double`, `decimal` | `3.14`, `-2.5` |
| `bool` | `true`, `false`, `1`, `0` |
| `DateTime` | `"2024-01-01"`, `"2024-01-01T12:00:00"` |
| `TimeSpan` | `"00:30:00"`, `"1.02:03:04"` |
| `Guid` | `"550e8400-e29b-41d4-a716-446655440000"` |
| `Enum` | `"Warning"`, `2` |
| `Uri` | `"https://example.com"` |

## 配置节

### GetSection 方法

获取配置的子节：

```csharp
var dbSection = cfg.GetSection("Database");

// 读取子节的值
var host = dbSection["Host"];
var port = dbSection.GetValue<int>("Port");
```

### 嵌套配置节

```csharp
// JSON: { "Services": { "Api": { "Url": "..." } } }
var apiSection = cfg.GetSection("Services:Api");
var url = apiSection["Url"];

// 或者
var url = cfg["Services:Api:Url"];
```

### 绑定到对象

```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
```

### 绑定到集合

```csharp
// JSON: { "Servers": ["server1", "server2", "server3"] }
var servers = cfg.GetSection("Servers").Get<List<string>>();

// JSON: { "Endpoints": [{ "Name": "api", "Url": "..." }, ...] }
var endpoints = cfg.GetSection("Endpoints").Get<List<EndpointConfig>>();
```

### 绑定到字典

```csharp
// JSON: { "ConnectionStrings": { "Default": "...", "Readonly": "..." } }
var connStrings = cfg.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
```

## 检查配置存在

```csharp
// 检查键是否存在
if (cfg.GetSection("OptionalFeature").Exists())
{
    // 配置存在
}

// 检查值是否为空
var value = cfg["SomeKey"];
if (!string.IsNullOrEmpty(value))
{
    // 值存在且不为空
}
```

## 获取所有子节

```csharp
var dbSection = cfg.GetSection("Database");
foreach (var child in dbSection.GetChildren())
{
    Console.WriteLine($"{child.Key} = {child.Value}");
}
```

## 下一步

- [配置合并](/guide/config-merge) - 多配置源合并策略
- [动态重载](/guide/dynamic-reload) - 配置热更新
- [依赖注入](/guide/dependency-injection) - DI 集成
