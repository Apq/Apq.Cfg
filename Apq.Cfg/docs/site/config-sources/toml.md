# TOML 配置源

TOML (Tom's Obvious, Minimal Language) 是一种易于阅读的配置格式，语义明确。

## 安装

```bash
dotnet add package Apq.Cfg.Toml
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Toml` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddTomlFile("config.toml")

// 指定自定义层级
.AddTomlFile("config.toml", level: 5)
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Toml;

var cfg = new CfgBuilder()
    .AddTomlFile("config.toml")  // 使用默认层级 0
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddTomlFile("config.toml", reloadOnChange: true)
    .AddTomlFile("config.local.toml", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddTomlFile("config.toml", writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
await cfg.SaveAsync();
```

## 方法签名

```csharp
public static CfgBuilder AddToml(
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
| `path` | TOML 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写（默认 `false`） |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |

## TOML 文件格式

### 基本结构

```toml
# 这是注释
AppName = "MyApp"
Version = "1.0.0"

[Database]
Host = "localhost"
Port = 5432
Name = "mydb"
Username = "admin"
Password = "secret"

[Logging]
Level = "Information"
EnableConsole = true
```

### 数组配置

```toml
# 简单数组
Servers = ["server1.example.com", "server2.example.com", "server3.example.com"]

# 表数组
[[Endpoints]]
Name = "api"
Url = "https://api.example.com"
Timeout = 30

[[Endpoints]]
Name = "auth"
Url = "https://auth.example.com"
Timeout = 10
```

### 内联表

```toml
[Database]
Connection = { Host = "localhost", Port = 5432, Name = "mydb" }
```

### 多行字符串

```toml
[Query]
SQL = """
SELECT * FROM users
WHERE active = true
ORDER BY created_at DESC
"""

# 字面量字符串（不处理转义）
Regex = '''\\d+\\.\\d+'''
```

### 日期时间

```toml
[Schedule]
StartDate = 2024-01-01
StartTime = 08:00:00
StartDateTime = 2024-01-01T08:00:00
```

## 键路径映射

TOML 结构映射为冒号分隔的键路径：

| TOML 路径 | 配置键 |
|-----------|--------|
| `AppName` | `AppName` |
| `[Database] Host` | `Database:Host` |
| `[[Endpoints]][0] Name` | `Endpoints:0:Name` |
| `Servers[0]` | `Servers:0` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddTomlFile("config.toml", encoding: options)
    .Build();
```

## 与其他格式混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddTomlFile("config.toml", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## TOML vs 其他格式

| 特性 | TOML | JSON | YAML | INI |
|------|------|------|------|-----|
| 可读性 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| 注释支持 | ✅ | ❌ | ✅ | ✅ |
| 类型支持 | ✅ | ✅ | ✅ | ❌ |
| 日期时间 | ✅ | ❌ | ❌ | ❌ |
| 多行字符串 | ✅ | ❌ | ✅ | ❌ |
| 嵌套支持 | ✅ | ✅ | ✅ | ❌ |

## 适用场景

- Rust/Go 项目配置（TOML 是这些语言的常用配置格式）
- 需要日期时间类型的配置
- 需要注释的配置文件
- 需要多行字符串的配置

## 依赖

- [Tomlyn](https://www.nuget.org/packages/Tomlyn) - TOML 解析库

## 下一步

- [环境变量](/config-sources/env) - 环境变量配置
- [Consul](/config-sources/consul) - Consul 配置中心
