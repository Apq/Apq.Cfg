# JSON 配置源

JSON 是最常用的配置格式，Apq.Cfg 核心包内置支持。

## 安装

JSON 配置源包含在核心包中，无需额外安装：

```bash
dotnet add package Apq.Cfg
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Json` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddJsonFile("config.json")

// 指定自定义层级
.AddJsonFile("config.json", level: 5)
```

## 基本用法

### 加载 JSON 文件

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")  // 使用默认层级 0
    .Build();
```

### 可选文件

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile("config.local.json", level: 1, optional: true)
    .Build();
```

### 启用重载

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
await cfg.SaveAsync();
```

## 方法签名

```csharp
public CfgBuilder AddJson(
    string path,
    int level,
    bool writeable,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false,
    EncodingOptions? encoding = null)
```

## 参数说明

| 参数 | 说明 |
|------|------|
| `path` | JSON 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |
| `encoding` | 编码选项（默认 `null`，自动检测） |

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
var firstServer = cfg["Servers:0"];
var firstEndpointName = cfg["Endpoints:0:Name"];

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
    .AddJsonFile("config.json", encoding: options)
    .Build();
```

### 从流加载

```csharp
using var stream = File.OpenRead("config.json");
var cfg = new CfgBuilder()
    .AddSource(new JsonStreamCfgSource(stream))
    .Build();
```

## 环境特定配置

### 典型模式

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile($"config.{environment}.json", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
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
