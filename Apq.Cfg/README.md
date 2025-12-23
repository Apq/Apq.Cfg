# Apq.Cfg

统一配置管理系统核心库，提供配置管理接口和基础实现。

## 项目结构

```text
Apq.Cfg/
├── ICfgRoot.cs              # 配置根接口
├── MergedCfgRoot.cs         # 合并配置根实现
├── CfgBuilder.cs            # 配置构建器
├── CfgRootExtensions.cs     # 扩展方法
└── Sources/                 # 配置源
    ├── ICfgSource.cs        # 配置源接口
    ├── JsonFileCfgSource.cs # JSON 文件配置源
    ├── File/
    │   └── FileCfgSourceBase.cs  # 文件配置源基类
    └── Environment/
        └── EnvVarsCfgSource.cs   # 环境变量配置源
```

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、Redis、数据库
- **智能编码检测**：读取时自动检测文件编码（使用 UTF.Unknown 库）
- **统一写入编码**：写入时统一使用 UTF-8 无 BOM 编码
- **多层级配置**：支持配置源优先级，高层级覆盖低层级
- **可写配置**：支持配置修改并持久化到指定配置源
- **热重载**：文件配置源支持变更自动重载
- **Microsoft.Extensions.Configuration 兼容**：可无缝转换为标准配置接口

## 支持的框架

- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET 9.0

## 安装

```bash
# 核心库
dotnet add package Apq.Cfg

# 扩展包（按需安装）
dotnet add package Apq.Cfg.Ini
dotnet add package Apq.Cfg.Xml
dotnet add package Apq.Cfg.Yaml
dotnet add package Apq.Cfg.Toml
dotnet add package Apq.Cfg.Redis
dotnet add package Apq.Cfg.Database
```

## 扩展包

| 项目 | 说明 | 依赖 |
| ---- | ---- | ---- |
| `Apq.Cfg` | 核心库，包含 JSON 和环境变量支持 | UTF.Unknown |
| `Apq.Cfg.Ini` | INI 文件扩展 | Microsoft.Extensions.Configuration.Ini |
| `Apq.Cfg.Xml` | XML 文件扩展 | Microsoft.Extensions.Configuration.Xml |
| `Apq.Cfg.Yaml` | YAML 文件扩展 | YamlDotNet |
| `Apq.Cfg.Toml` | TOML 文件扩展 | Tomlyn |
| `Apq.Cfg.Redis` | Redis 扩展 | StackExchange.Redis |
| `Apq.Cfg.Database` | 数据库扩展 | SqlSugarCore |

## 快速开始

### 基本用法

```csharp
using Apq.Cfg;

// 构建配置
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
var connectionString = cfg.Get("Database:ConnectionString");
var timeout = cfg.Get<int>("Database:Timeout");

// 检查配置是否存在
if (cfg.Exists("Feature:Enabled"))
{
    // ...
}

// 修改配置（写入到 isPrimaryWriter 的配置源）
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();

// 转换为 Microsoft.Extensions.Configuration
IConfigurationRoot msConfig = cfg.ToMicrosoftConfiguration();
```

### 使用扩展包

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;
using Apq.Cfg.Redis;

var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: true)
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
        options.KeyPrefix = "config:";
    }, level: 1)
    .Build();
```

### 使用 INI 配置

```csharp
using Apq.Cfg;
using Apq.Cfg.Ini;

var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: true)
    .Build();

// INI 格式示例：
// [Database]
// ConnectionString=Server=localhost;Database=mydb
// Timeout=30
```

### 使用 TOML 配置

```csharp
using Apq.Cfg;
using Apq.Cfg.Toml;

var cfg = new CfgBuilder()
    .AddToml("config.toml", level: 0, writeable: true)
    .Build();

// TOML 格式示例：
// [database]
// connection_string = "Server=localhost;Database=mydb"
// timeout = 30
```

### 使用数据库配置

```csharp
using Apq.Cfg;
using Apq.Cfg.Database;

var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.ConnectionString = "Server=localhost;Database=config;...";
        options.TableName = "AppConfig";
        options.KeyColumn = "Key";
        options.ValueColumn = "Value";
    }, level: 0, writeable: true)
    .Build();
```

## 配置层级

配置源按 `level` 参数排序，数值越大优先级越高。相同键的配置值，高层级会覆盖低层级。

```csharp
// level 0: 基础配置（最低优先级）
// level 1: 本地覆盖配置
// level 2: 环境变量（最高优先级）

var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0)           // 基础配置
    .AddJson("appsettings.local.json", level: 1)     // 本地覆盖
    .AddEnvironmentVariables(level: 2)               // 环境变量优先
    .Build();
```

## 可写配置

设置 `writeable: true` 的配置源支持写入。设置 `isPrimaryWriter: true` 的配置源为默认写入目标。

```csharp
// 写入到默认配置源
cfg.Set("Key", "Value");
await cfg.SaveAsync();

// 写入到指定层级
cfg.Set("Key", "Value", targetLevel: 1);
await cfg.SaveAsync(targetLevel: 1);

// 删除配置
cfg.Remove("Key");
await cfg.SaveAsync();
```

## 编码处理

- **读取**：自动检测文件编码，支持 UTF-8、GBK、GB2312 等多种编码
- **写入**：统一使用 UTF-8 无 BOM 编码

可调整编码检测置信度阈值：

```csharp
using Apq.Cfg.Sources.File;

// 方式1：通过 CfgBuilder 设置（推荐）
var cfg = new CfgBuilder()
    .WithEncodingConfidenceThreshold(0.7f)
    .AddJson("appsettings.json", level: 0, writeable: false)
    .Build();

// 方式2：直接设置静态属性
// 默认 0.6，范围 0.0-1.0
FileCfgSourceBase.EncodingConfidenceThreshold = 0.7f;
```

也可以通过环境变量设置默认值（无需修改代码）：

```bash
# Windows
set APQ_CFG_ENCODING_CONFIDENCE=0.7

# Linux/macOS
export APQ_CFG_ENCODING_CONFIDENCE=0.7
```

## 热重载

文件配置源支持自动监听文件变更并重新加载配置：

```csharp
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, reloadOnChange: true)
    .Build();

// 配置文件变更后会自动重新加载
// 后续读取 cfg.Get() 将获取到最新的配置值
```

## 核心类型

### ICfgRoot

配置根接口，提供配置的读取、写入和保存功能。

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 读取
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);

    // 写入
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default);

    // 转换
    IConfigurationRoot ToMicrosoftConfiguration();
}
```

### CfgBuilder

配置构建器，用于组合多个配置源。

```csharp
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2)
    .Build();
```

### FileCfgSourceBase

文件配置源基类，提供编码检测和统一写入编码功能。

```csharp
// 写入编码：UTF-8 无 BOM
public static readonly Encoding WriteEncoding = new UTF8Encoding(false);

// 编码检测置信度阈值（可调整，默认 0.6）
// 也可通过环境变量 APQ_CFG_ENCODING_CONFIDENCE 设置
public static float EncodingConfidenceThreshold { get; set; } = 0.6f;

// 检测文件编码
public static Encoding DetectEncoding(string path);
```

## 扩展开发

实现 `ICfgSource` 或 `IWritableCfgSource` 接口创建自定义配置源：

```csharp
public interface ICfgSource
{
    int Level { get; }
    bool IsWriteable { get; }
    bool IsPrimaryWriter { get; }
    IConfigurationSource BuildSource();
}

public interface IWritableCfgSource : ICfgSource
{
    Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken);
}
```

通过 `CfgBuilder.AddSource()` 添加自定义配置源：

```csharp
builder.AddSource(new MyCustomCfgSource(...));
```

## 依赖项

| 包名 | 用途 |
| ---- | ---- |
| Microsoft.Extensions.Configuration | 配置基础设施 |
| Microsoft.Extensions.Configuration.Abstractions | 配置抽象接口 |
| Microsoft.Extensions.Configuration.Binder | 配置绑定功能 |
| Microsoft.Extensions.Configuration.Json | JSON 配置支持 |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 环境变量支持 |
| [UTF.Unknown](https://github.com/CharsetDetector/UTF-unknown) | 文件编码自动检测 |

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com
