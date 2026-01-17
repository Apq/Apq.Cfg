# HOCON 配置源

HOCON (Human-Optimized Config Object Notation) 是一种对开发者和系统管理员友好的配置格式，支持层次结构、引用、拼接等高级特性。

## 安装

```bash
dotnet add package Apq.Cfg.Hcl
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Hcl` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddHclFile("config.hcl")

// 指定自定义层级
.AddHclFile("config.hcl", level: 5)
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Hcl;

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl")  // 使用默认层级 0
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", reloadOnChange: true)
    .AddHclFile("config.local.hcl", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["app:name"] = "NewName";
await cfg.SaveAsync();
```

## 方法签名

```csharp
public static CfgBuilder AddHclFile(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

## 参数说明

| 参数 | 说明 |
|------|------|
| `path` | HOCON 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写（默认 `false`） |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |

## HOCON 文件格式

### 基本结构

```hocon
# 这是注释
app_name = "MyApp"
version = "1.0.0"

database {
    host = "localhost"
    port = 5432
    name = "mydb"
    username = "admin"
    password = "secret"
}

logging {
    level = "INFO"
    enable_console = true
}
```

### 嵌套对象

```hocon
database {
    host = "localhost"
    connection {
        timeout = 30
        pool_size = 10
    }
}
```

### 数组配置

```hocon
# 简单数组
servers = ["server1.example.com", "server2.example.com"]

# 对象数组
services = [
    { name = "api", url = "https://api.example.com" },
    { name = "auth", url = "https://auth.example.com" }
]
```

### 配置引用

```hocon
# 引用其他配置值
base_url = "https://example.com"
api_url = ${base_url}"/api"
full_path = ${database.host}":"${database.port}
```

### 配置拼接

```hocon
# 合并数组
common_ports = [80, 443]
all_ports = ${common_ports} [8080, 8443]
```

### 包含文件

```hocon
# 包含其他配置文件
include "path/to/common.conf"
include "path/to/database.conf"
```

### 多行字符串

```hocon
description = """
This is a multi-line
string value that
preserves formatting.
"""
```

## 键路径映射

HOCON 结构映射为冒号分隔的键路径：

| HOCON 路径 | 配置键 |
|------------|--------|
| `app_name` | `app_name` |
| `database.host` | `database:host` |
| `database.connection.timeout` | `database:connection:timeout` |
| `services.0.name` | `services:0:name` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", encoding: options)
    .Build();
```

### 使用 Akka.Configuration 配置工厂

```csharp
using Akka.Configuration;

var hoconConfig = ConfigurationFactory.ParseString("""
    akka {
        actor {
            provider = "Akka.Remote.RemoteActorRefProvider, Akka.Remote"
        }
    }
    """);

var cfg = new CfgBuilder()
    .AddHclString(hoconConfig)
    .Build();
```

## 与其他格式混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddHclFile("config.hcl", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## 适用场景

- Akka.NET/Play Framework 项目配置（HOCON 是这些框架的默认格式）
- 需要配置引用和拼接的复杂配置
- 需要在配置中使用占位符的环境
- 多配置文件需要合并的场景
- 与 Java/Scala 项目共享配置

## 依赖

- [Akka.Configuration](https://www.nuget.org/packages/Akka.Configuration) - HOCON 解析库

## 下一步

- [JSON 配置源](/config-sources/json) - JSON 格式配置
- [YAML 配置源](/config-sources/yaml) - YAML 格式配置
- [TOML 配置源](/config-sources/toml) - TOML 格式配置
