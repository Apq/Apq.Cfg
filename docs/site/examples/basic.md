# 基础示例

本页展示 Apq.Cfg 的基础用法示例。

## 读取配置值

### 配置文件

```json
{
  "App": {
    "Name": "MyApplication",
    "Version": "1.0.0",
    "Debug": true
  },
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "mydb",
    "Timeout": 30
  },
  "Features": {
    "EnableCache": true,
    "CacheSize": 1000,
    "CacheExpiry": "00:30:00"
  }
}
```

### 读取字符串

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 使用默认层级 0
    .Build();

// 使用索引器
var appName = cfg["App:Name"];
Console.WriteLine($"应用名称: {appName}");

// 嵌套路径
var dbHost = cfg["Database:Host"];
Console.WriteLine($"数据库主机: {dbHost}");
```

### 读取类型化值

```csharp
// 整数
var port = cfg.GetValue<int>("Database:Port");
Console.WriteLine($"端口: {port}");

// 布尔值
var debug = cfg.GetValue<bool>("App:Debug");
Console.WriteLine($"调试模式: {debug}");

// 超时
var timeout = cfg.GetValue<int>("Database:Timeout");
Console.WriteLine($"超时: {timeout}秒");
```

## 配置节操作

### 获取配置节

```csharp
var db = cfg.GetSection("Database");

Console.WriteLine($"主机: {db["Host"]}");
Console.WriteLine($"端口: {db.GetValue<int>("Port")}");
Console.WriteLine($"数据库: {db["Database"]}");
```

### 遍历子键

```csharp
var app = cfg.GetSection("App");
foreach (var key in app.GetChildKeys())
{
    Console.WriteLine($"{key} = {app[key]}");
}
```

### 检查配置存在

```csharp
if (cfg.Exists("Optional"))
{
    Console.WriteLine("可选配置存在");
}
else
{
    Console.WriteLine("可选配置不存在");
}
```

## 强类型绑定

### 定义配置类

```csharp
public class AppConfig
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public bool Debug { get; set; }
}

public class DatabaseConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "";
    public int Timeout { get; set; } = 30;
}

public class FeaturesConfig
{
    public bool EnableCache { get; set; }
    public int CacheSize { get; set; }
    public TimeSpan CacheExpiry { get; set; }
}
```

### 绑定配置

```csharp
// 手动绑定
var app = cfg.GetSection("App");
var appConfig = new AppConfig
{
    Name = app["Name"] ?? "",
    Version = app["Version"] ?? "",
    Debug = app.GetValue<bool>("Debug")
};

var db = cfg.GetSection("Database");
var dbConfig = new DatabaseConfig
{
    Host = db["Host"] ?? "localhost",
    Port = db.GetValue<int>("Port"),
    Database = db["Database"] ?? "",
    Timeout = db.GetValue<int>("Timeout")
};

Console.WriteLine($"应用: {appConfig.Name} v{appConfig.Version}");
Console.WriteLine($"数据库: {dbConfig.Host}:{dbConfig.Port}/{dbConfig.Database}");
```

## 数组配置

### 配置文件

```json
{
  "Servers": [
    "server1.example.com",
    "server2.example.com",
    "server3.example.com"
  ],
  "Endpoints": [
    {
      "Name": "api",
      "Url": "https://api.example.com",
      "Timeout": 30
    },
    {
      "Name": "auth",
      "Url": "https://auth.example.com",
      "Timeout": 10
    }
  ]
}
```

### 读取数组

```csharp
// 字符串数组
var servers = cfg.GetSection("Servers");
foreach (var key in servers.GetChildKeys())
{
    Console.WriteLine($"服务器: {servers[key]}");
}

// 对象数组
public class EndpointConfig
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public int Timeout { get; set; }
}

var endpoints = cfg.GetSection("Endpoints");
foreach (var key in endpoints.GetChildKeys())
{
    var endpoint = endpoints.GetSection(key);
    Console.WriteLine($"端点: {endpoint["Name"]} -> {endpoint["Url"]}");
}

// 按索引访问
var firstServer = cfg["Servers:0"];
var firstEndpointName = cfg["Endpoints:0:Name"];
```

## 字典配置

### 配置文件

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=default;",
    "Readonly": "Server=readonly;Database=default;",
    "Analytics": "Server=analytics;Database=stats;"
  }
}
```

### 读取字典

```csharp
var conn = cfg.GetSection("ConnectionStrings");
foreach (var key in conn.GetChildKeys())
{
    Console.WriteLine($"{key}: {conn[key]}");
}
```

## 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", writeable: true, isPrimaryWriter: true)  // 使用默认层级 0
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
cfg["Database:Port"] = "5433";

// 保存到文件
await cfg.SaveAsync();
```

## 完整示例

```csharp
using Apq.Cfg;

// 创建配置
var cfg = new CfgBuilder()
    .AddJson("config.json")  // 使用默认层级 0
    .Build();

// 读取应用配置
var app = cfg.GetSection("App");
Console.WriteLine($"=== {app["Name"]} v{app["Version"]} ===");

// 读取数据库配置
var db = cfg.GetSection("Database");
Console.WriteLine($"数据库: {db["Host"]}:{db.GetValue<int>("Port")}");

// 读取功能配置
var features = cfg.GetSection("Features");
if (features.GetValue<bool>("EnableCache"))
{
    Console.WriteLine($"缓存已启用，大小: {features.GetValue<int>("CacheSize")}");
}

// 遍历所有顶级配置
Console.WriteLine("\n所有配置节:");
var root = cfg.GetSection("");
foreach (var key in root.GetChildKeys())
{
    Console.WriteLine($"  - {key}");
}
```

## 下一步

- [多配置源](/examples/multi-source) - 组合多个配置源
- [依赖注入](/examples/di-integration) - DI 集成示例
