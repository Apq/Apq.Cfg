# 基础用法

本页详细介绍 Apq.Cfg 的基础用法。

## 配置构建器

`CfgBuilder` 是创建配置的入口点：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 使用默认层级 0
    .Build();
```

### 链式调用

支持链式调用添加多个配置源：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")                                           // 默认层级 0
    .AddJson("config.local.json", level: 50, writeable: true, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")                          // 默认层级 400
    .Build();
```

### 配置源优先级

使用 `level` 参数控制优先级，数值越大优先级越高。每种配置源都有默认层级，如果不指定则使用默认值：

| 配置源类型 | 默认层级 |
|------------|----------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

```csharp
// config.json: { "Key": "value1" }
// config.local.json: { "Key": "value2" }

var cfg = new CfgBuilder()
    .AddJson("config.json")                    // 使用默认层级 0
    .AddJson("config.local.json", level: 50)   // 自定义层级 50
    .AddEnvironmentVariables()                 // 使用默认层级 400
    .Build();

Console.WriteLine(cfg["Key"]); // 输出: value2（来自 config.local.json）
```

## 读取配置值

### 索引器

使用索引器是最简洁的方式：

```csharp
// JSON: { "Database": { "Host": "localhost" } }
var host = cfg["Database:Host"];
```

### Get 方法

使用冒号 `:` 分隔的键路径：

```csharp
// JSON: { "Database": { "Host": "localhost" } }
var host = cfg["Database:Host"];
```

### 泛型 Get 方法

获取类型化的值：

```csharp
// 基本类型
var port = cfg.GetValue<int>("Database:Port");
var enabled = cfg.GetValue<bool>("Feature:Enabled");
var ratio = cfg.GetValue<double>("Settings:Ratio");

// 可空类型
var maxSize = cfg.GetValue<int?>("Cache:MaxSize");
```

### 扩展方法

```csharp
// 带默认值
var timeout = cfg.GetValue<int>("Database:Timeout", 30);

// 必需的配置（不存在时抛出异常）
var connectionString = cfg.GetRequired<string>("Database:ConnectionString");

// 尝试获取
if (cfg.TryGetValue<int>("Database:Port", out var port))
{
    Console.WriteLine($"端口: {port}");
}
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
var db = cfg.GetSection("Database");

// 使用索引器读取子节的值
var host = db["Host"];
var port = db.GetValue<int>("Port");
```

### 嵌套配置节

```csharp
// JSON: { "Services": { "Api": { "Url": "..." } } }
var apiSection = cfg.GetSection("Services:Api");
var url = apiSection["Url"];

// 或者直接访问
var url = cfg["Services:Api:Url"];
```

### 配置节属性

```csharp
var db = cfg.GetSection("Database");

// 获取节的路径
Console.WriteLine(db.Path); // 输出: Database

// 检查键是否存在
if (db.Exists("ConnectionString"))
{
    // 配置存在
}
```

## 检查配置存在

```csharp
// 检查键是否存在
if (cfg.Exists("OptionalFeature:Enabled"))
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

## 获取所有子键

```csharp
// 获取顶级键
foreach (var key in cfg.GetChildKeys())
{
    Console.WriteLine($"顶级键: {key}");
}

// 获取配置节的子键
var db = cfg.GetSection("Database");
foreach (var key in db.GetChildKeys())
{
    Console.WriteLine($"{key} = {db[key]}");
}
```

## 批量操作

### 批量获取（返回字典）

```csharp
var keys = new[] { "Database:Host", "Database:Port", "Database:Name" };
var values = cfg.GetMany(keys);

foreach (var (key, value) in values)
{
    Console.WriteLine($"{key} = {value}");
}
```

### 批量获取（回调方式，零堆分配）

```csharp
cfg.GetMany(new[] { "Database:Host", "Database:Port" }, (key, value) =>
{
    Console.WriteLine($"{key} = {value}");
});
```

### 批量设置

```csharp
cfg.SetManyValues(new Dictionary<string, string?>
{
    ["Database:Host"] = "newhost",
    ["Database:Port"] = "5433"
});

await cfg.SaveAsync();
```

## 写入配置

### 使用索引器设置值

```csharp
cfg["Database:Timeout"] = "60";
await cfg.SaveAsync();
```

### 使用 Set 方法

```csharp
cfg.SetValue("Database:Timeout", "60");
await cfg.SaveAsync();
```

### 移除配置

```csharp
cfg.Remove("Database:OldSetting");
await cfg.SaveAsync();
```

### 指定目标层级

```csharp
// 写入到特定层级
cfg.SetValue("Database:Timeout", "60", targetLevel: 1);
await cfg.SaveAsync(targetLevel: 1);
```

## 下一步

- [配置合并](/guide/config-merge) - 多配置源合并策略
- [动态重载](/guide/dynamic-reload) - 配置热更新
- [依赖注入](/guide/dependency-injection) - DI 集成
