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
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### 可选文件

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: false, optional: true)
    .Build();
```

### 启用重载

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg.Set("App:Name", "NewName");
await cfg.SaveAsync();
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
// 按索引访问
var firstServer = cfg.Get("Servers:0");
var firstEndpointName = cfg.Get("Endpoints:0:Name");

// 获取配置节
var serversSection = cfg.GetSection("Servers");
var endpointsSection = cfg.GetSection("Endpoints");
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
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false, encoding: options)
    .Build();
```

### 从流加载

```csharp
using var stream = File.OpenRead("config.json");
var cfg = new CfgBuilder()
    .AddSource(new JsonStreamCfgSource(stream, level: 0))
    .Build();
```

## 环境特定配置

### 典型模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson($"config.{environment}.json", level: 1, writeable: false, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### 配置文件示例

**config.json** (基础配置):
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

**config.Development.json** (开发环境):
```json
{
  "Logging": {
    "Level": "Debug"
  }
}
```

**config.Production.json** (生产环境):
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
