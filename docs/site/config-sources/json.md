# JSON 配置源

JSON 是最常用的配置格式，Apq.Cfg 核心包内置支持。

## 安装

JSON 配置源包含在核心包中，无需额外安装：

```bash
dotnet add package Apq.Cfg
```

## 基本用法

### 加载 JSON 文件

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
```

### 可选文件

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", optional: true)
    .Build();
```

### 启用重载

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
```

## JSON 文件格式

### 基本结构

```json
{
  "App": {
    "Name": "MyApplication",
    "Version": "1.0.0"
  },
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "mydb",
    "Username": "admin",
    "Password": "secret"
  },
  "Logging": {
    "Level": "Information",
    "EnableConsole": true
  }
}
```

### 数组配置

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
// 读取字符串数组
var servers = cfg.GetSection("Servers").Get<List<string>>();

// 读取对象数组
var endpoints = cfg.GetSection("Endpoints").Get<List<EndpointConfig>>();

// 按索引访问
var firstServer = cfg["Servers:0"];
var firstEndpointName = cfg["Endpoints:0:Name"];
```

## 键路径映射

JSON 结构会被扁平化为冒号分隔的键路径：

| JSON 路径 | 配置键 |
|-----------|--------|
| `App.Name` | `App:Name` |
| `Database.Host` | `Database:Host` |
| `Servers[0]` | `Servers:0` |
| `Endpoints[0].Name` | `Endpoints:0:Name` |

## 高级选项

### 指定编码

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", encoding: Encoding.UTF8)
    .Build();
```

### JSON 解析选项

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", jsonOptions: new JsonDocumentOptions
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    })
    .Build();
```

### 从流加载

```csharp
using var stream = File.OpenRead("config.json");
var cfg = new CfgBuilder()
    .AddJsonStream(stream)
    .Build();
```

### 从字符串加载

```csharp
var json = @"{ ""Key"": ""Value"" }";
var cfg = new CfgBuilder()
    .AddJsonString(json)
    .Build();
```

## 环境特定配置

### 典型模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();
```

### 配置文件示例

**appsettings.json** (基础配置):
```json
{
  "Logging": {
    "Level": "Warning"
  },
  "Database": {
    "Host": "localhost",
    "Port": 5432
  }
}
```

**appsettings.Development.json** (开发环境):
```json
{
  "Logging": {
    "Level": "Debug"
  }
}
```

**appsettings.Production.json** (生产环境):
```json
{
  "Database": {
    "Host": "prod-db.example.com"
  }
}
```

## 下一步

- [YAML 配置源](/config-sources/yaml) - YAML 格式支持
- [环境变量](/config-sources/env) - 环境变量配置
