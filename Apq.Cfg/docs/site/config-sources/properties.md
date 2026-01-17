# Properties 配置源

Properties 是一种键值对格式的配置文件，广泛用于 Java 生态系统，也被许多其他应用程序支持。

## 安装

```bash
dotnet add package Apq.Cfg.Properties
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Properties` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddPropertiesFile("config.properties")

// 指定自定义层级
.AddPropertiesFile("config.properties", level: 5)
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Properties;

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties")  // 使用默认层级 0
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", reloadOnChange: true)
    .AddPropertiesFile("config.local.properties", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["App.Name"] = "NewName";
await cfg.SaveAsync();
```

## 方法签名

```csharp
public static CfgBuilder AddPropertiesFile(
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
| `path` | Properties 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写（默认 `false`） |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |

## Properties 文件格式

### 基本结构

```properties
# 这是注释
App.Name=MyApp
App.Version=1.0.0
Database.Host=localhost
Database.Port=5432
Database.Name=mydb
Database.Username=admin
Database.Password=secret
Logging.Level=INFO
Logging.EnableConsole=true
```

### 点分隔的命名空间

```properties
# 使用点号分隔命名空间
app.name=MyApp
app.database.host=localhost
app.database.port=5432
app.logging.level=INFO
```

### 使用等号或冒号分隔

```properties
# 使用等号
key1=value1

# 使用冒号
key2:value2
```

### 转义字符

```properties
# 换行符
description=Line1\nLine2

# 制表符
tabbed=value1\tvalue2

# 特殊字符转义
hash_comment=This is not a comment \#here
colon_value=key\:value
backslash=path\\to\\file
```

### 多行值

```properties
# 使用反斜杠继续
long_value=This is a very long value that \
          continues on the next line

# 使用三引号（在某些实现中）
query=SELECT * FROM users \
      WHERE active = true \
      ORDER BY created_at DESC
```

## 键路径映射

Properties 结构映射为点分隔的键路径：

| Properties 键 | 配置键 |
|---------------|--------|
| `App.Name` | `App.Name` |
| `Database.Host` | `Database.Host` |
| `Database.Connection.Timeout` | `Database.Connection.Timeout` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", encoding: options)
    .Build();
```

### 区段支持（INI 风格）

```properties
# 区段使用方括号
[app]
name=MyApp
version=1.0.0

[database]
host=localhost
port=5432

[logging]
level=INFO
```

读取时区段会作为前缀：

| Properties 键 | 配置键 |
|---------------|--------|
| `app.name` (无区段) | `app.name` |
| `[app]` 下的 `name` | `app.name` |
| `[database]` 下的 `host` | `database.host` |

## 与其他格式混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddPropertiesFile("config.properties", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## Properties vs 其他格式

| 特性 | Properties | JSON | YAML | TOML |
|------|-----------|------|------|------|
| 可读性 | ⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 注释支持 | ✅ | ❌ | ✅ | ✅ |
| 类型支持 | ❌ (字符串) | ✅ | ✅ | ✅ |
| 嵌套结构 | 有限 | ✅ | ✅ | ✅ |
| Java 生态 | ✅✅✅ | ✅ | ✅ | ✅ |
| 简单性 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |

## 适用场景

- Java/Spring 项目配置（Properties 是 Spring 的默认格式）
- 与遗留 Java 应用程序共享配置
- 简单的键值对配置需求
- 需要与系统属性（System.getProperty）保持一致
- 跨语言配置共享的简单格式

## 与 Spring Framework 集成

Spring Boot 项目的 `application.properties` 文件可以直接使用：

```properties
# Spring Boot application.properties
spring.datasource.url=jdbc:mysql://localhost:3306/mydb
spring.datasource.username=admin
spring.datasource.password=secret
server.port=8080
```

在 Apq.Cfg 中使用：

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("application.properties")
    .Build();

// 访问配置
var dbUrl = cfg["spring.datasource.url"];
var port = cfg.GetValue<int>("server.port");
```

## 依赖

无额外依赖（使用 .NET 内置功能解析 Properties 格式）。

## 下一步

- [JSON 配置源](/config-sources/json) - JSON 格式配置
- [INI 配置源](/config-sources/ini) - INI 格式配置
- [环境变量](/config-sources/env) - 环境变量配置
