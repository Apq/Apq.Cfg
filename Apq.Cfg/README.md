# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

统一配置管理系统核心库，提供配置管理接口和基础实现。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、Redis、数据库
- **智能编码检测**：读取时自动检测文件编码（BOM 优先，UTF.Unknown 库辅助）
- **灵活编码映射**：支持完整路径、通配符、正则表达式三种匹配方式
- **多层级配置**：支持配置源优先级，高层级覆盖低层级
- **可写配置**：支持配置修改并持久化到指定配置源
- **热重载**：文件配置源支持变更自动重载
- **动态配置重载**：支持文件变更自动检测、防抖、增量更新
- **配置节**：支持按路径获取配置子节，简化嵌套配置访问
- **依赖注入集成**：提供 `AddApqCfg` 和 `ConfigureApqCfg<T>` 扩展方法
- **Rx 支持**：通过 `ConfigChanges` 订阅配置变更事件
- **Microsoft.Extensions.Configuration 兼容**：可无缝转换为标准配置接口

## 支持的框架

- .NET 6.0 / 7.0 / 8.0 / 9.0

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

## 快速开始

```csharp
using Apq.Cfg;

// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
var connectionString = cfg.Get("Database:ConnectionString");
var timeout = cfg.Get<int>("Database:Timeout");

// 使用配置节简化嵌套访问
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host");
var port = dbSection.Get<int>("Port");

// 修改配置（写入到 isPrimaryWriter 的配置源）
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();
```

## 配置层级

配置源按 `level` 参数排序，数值越大优先级越高。相同键的配置值，高层级会覆盖低层级。

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置（最低优先级）
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddEnvironmentVariables(level: 2)               // 环境变量（最高优先级）
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

### 读取编码检测

读取文件时按以下顺序检测编码：

1. **用户指定编码**：`EncodingOptions.ReadStrategy = Specified`
2. **读取映射配置**：通过 `AddReadEncodingMapping` 等方法配置
3. **缓存结果**：如果启用缓存且文件未修改
4. **BOM 检测**：支持 UTF-8、UTF-16 LE/BE、UTF-32 LE/BE
5. **UTF.Unknown 库检测**：置信度高于阈值时使用
6. **回退编码**：默认 UTF-8

### 写入编码

写入文件时按以下顺序确定编码：

1. **EncodingOptions 策略**：`Utf8NoBom`（默认）、`Utf8WithBom`、`Specified`、`Preserve`
2. **写入映射配置**：通过 `AddWriteEncodingMapping` 等方法配置
3. **默认编码**：UTF-8 无 BOM

### 编码映射

支持为特定文件或文件模式指定读取/写入编码：

```csharp
using System.Text;
using Apq.Cfg;
using Apq.Cfg.EncodingSupport;

var cfg = new CfgBuilder()
    // 完整路径映射：特定文件使用 GB2312 读取
    .AddReadEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))

    // 通配符映射：所有 PS1 文件写入时使用 UTF-8 BOM
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))

    // 正则表达式映射
    .AddWriteEncodingMappingRegex(@"logs[/\\].*\.log$", Encoding.Unicode)

    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

#### 通配符语法

| 符号   | 含义                           | 示例                              |
| ------ | ------------------------------ | --------------------------------- |
| `*`    | 匹配任意字符（不含路径分隔符） | `*.json` 匹配 `config.json`       |
| `**`   | 匹配任意字符（含路径分隔符）   | `**/*.txt` 匹配 `a/b/c.txt`       |
| `?`    | 匹配单个字符                   | `config?.json` 匹配 `config1.json` |

#### 映射优先级

| 匹配类型          | 默认优先级 | 说明                              |
| ----------------- | ---------- | --------------------------------- |
| ExactPath         | 100        | 完整路径精确匹配                  |
| Wildcard          | 0          | 通配符匹配                        |
| Regex             | 0          | 正则表达式匹配                    |
| 内置 PowerShell   | -100       | `*.ps1`, `*.psm1`, `*.psd1`       |

#### 高级配置

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        // 添加多条规则
        config.AddReadMapping("*.xml", EncodingMappingType.Wildcard,
            Encoding.UTF8, priority: 50);
        config.AddWriteMapping("**/*.txt", EncodingMappingType.Wildcard,
            new UTF8Encoding(true), priority: 10);

        // 清除默认规则
        config.ClearWriteMappings();
    })
    .WithEncodingConfidenceThreshold(0.8f)  // 提高检测置信度阈值
    .WithEncodingDetectionLogging(result =>  // 启用日志
    {
        Console.WriteLine($"检测到编码: {result}");
    })
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

#### 单文件编码选项

可以为单个配置源指定编码选项：

```csharp
// 使用预定义的 PowerShell 选项（UTF-8 带 BOM）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true,
        encoding: EncodingOptions.PowerShell)
    .Build();

// 保持原编码
var options = new EncodingOptions
{
    WriteStrategy = EncodingWriteStrategy.Preserve
};

var cfg = new CfgBuilder()
    .AddJson("legacy.json", level: 0, writeable: true, encoding: options)
    .Build();
```

#### 置信度阈值

```csharp
// 方式1：通过 CfgBuilder 设置（推荐）
var cfg = new CfgBuilder()
    .WithEncodingConfidenceThreshold(0.7f)
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// 方式2：直接设置静态属性
FileCfgSourceBase.EncodingConfidenceThreshold = 0.7f;

// 方式3：通过环境变量设置（无需修改代码）
// Windows: set APQ_CFG_ENCODING_CONFIDENCE=0.7
// Linux/macOS: export APQ_CFG_ENCODING_CONFIDENCE=0.7
```

详细的编码处理流程请参阅 [编码处理流程文档](../docs/编码处理流程.md)。

## 热重载

文件配置源支持自动监听文件变更并重新加载配置：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();

// 配置文件变更后会自动重新加载
// 后续读取 cfg.Get() 将获取到最新的配置值
```

### 动态重载与变更订阅

```csharp
using Apq.Cfg;
using Apq.Cfg.Changes;
using Microsoft.Extensions.Primitives;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .AddJson("config.local.json", level: 1, reloadOnChange: true)
    .Build();

// 获取支持动态重载的 Microsoft Configuration
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 100,           // 防抖时间窗口（毫秒）
    EnableDynamicReload = true  // 启用动态重载
});

// 方式1：使用 IChangeToken 监听变更
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));

// 方式2：使用 Rx 订阅配置变更事件
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

动态重载特性：
- **防抖处理**：批量文件保存时，多次快速变化合并为一次处理
- **增量更新**：只重新加载发生变化的配置源
- **层级覆盖感知**：只有当最终合并值真正发生变化时才触发通知

## 依赖注入集成

```csharp
using Apq.Cfg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();

// 注册 Apq.Cfg 配置
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");

var provider = services.BuildServiceProvider();
var cfgRoot = provider.GetRequiredService<ICfgRoot>();
var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}
```

## 扩展包

| 项目               | 说明                           | 依赖                                          |
| ------------------ | ------------------------------ | --------------------------------------------- |
| `Apq.Cfg`          | 核心库，JSON 和环境变量支持    | UTF.Unknown, System.Reactive                  |
| `Apq.Cfg.Ini`      | INI 文件扩展                   | Microsoft.Extensions.Configuration.Ini        |
| `Apq.Cfg.Xml`      | XML 文件扩展                   | Microsoft.Extensions.Configuration.Xml        |
| `Apq.Cfg.Yaml`     | YAML 文件扩展                  | YamlDotNet                                    |
| `Apq.Cfg.Toml`     | TOML 文件扩展                  | Tomlyn                                        |
| `Apq.Cfg.Redis`    | Redis 扩展                     | StackExchange.Redis                           |
| `Apq.Cfg.Database` | 数据库扩展                     | SqlSugarCore                                  |

### 使用扩展包示例

```csharp
// INI 配置
using Apq.Cfg.Ini;
var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: true)
    .Build();

// YAML 配置
using Apq.Cfg.Yaml;
var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: true)
    .Build();

// TOML 配置
using Apq.Cfg.Toml;
var cfg = new CfgBuilder()
    .AddToml("config.toml", level: 0, writeable: true)
    .Build();

// Redis 配置
using Apq.Cfg.Redis;
var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
        options.KeyPrefix = "config:";
    }, level: 1)
    .Build();

// 数据库配置
using Apq.Cfg.Database;
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "MySql"; // SqlServer/MySql/PostgreSql/Oracle/SQLite
        options.ConnectionString = "Server=localhost;Database=config;...";
        options.Table = "AppConfig";
        options.KeyColumn = "Key";
        options.ValueColumn = "Value";
    }, level: 0, isPrimaryWriter: true)
    .Build();
```

## API 参考

### ICfgRoot

配置根接口，提供配置的读取、写入和保存功能。

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 读取
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);

    // 配置节
    ICfgSection GetSection(string path);
    IEnumerable<string> GetChildKeys();

    // 写入
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default);

    // 转换
    IConfigurationRoot ToMicrosoftConfiguration();
    IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options);

    // 配置变更事件
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

### ICfgSection

配置节接口，提供对配置子节的访问。

```csharp
public interface ICfgSection
{
    string Path { get; }
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);
    void Set(string key, string? value);
    void Remove(string key);
    ICfgSection GetSection(string path);
    IEnumerable<string> GetChildKeys();
}
```

### CfgBuilder

配置构建器方法：

| 方法                                                          | 说明                     |
| ------------------------------------------------------------- | ------------------------ |
| `AddJson(path, level, writeable, ...)`                        | 添加 JSON 配置源         |
| `AddEnvironmentVariables(level, prefix)`                      | 添加环境变量配置源       |
| `AddSource(ICfgSource)`                                       | 添加自定义配置源         |
| `AddReadEncodingMapping(path, encoding, priority)`            | 添加读取映射（完整路径） |
| `AddReadEncodingMappingWildcard(pattern, encoding, priority)` | 添加读取映射（通配符）   |
| `AddReadEncodingMappingRegex(pattern, encoding, priority)`    | 添加读取映射（正则）     |
| `AddWriteEncodingMapping(path, encoding, priority)`           | 添加写入映射（完整路径） |
| `AddWriteEncodingMappingWildcard(pattern, encoding, priority)`| 添加写入映射（通配符）   |
| `AddWriteEncodingMappingRegex(pattern, encoding, priority)`   | 添加写入映射（正则）     |
| `ConfigureEncodingMapping(Action<EncodingMappingConfig>)`     | 高级编码映射配置         |
| `WithEncodingConfidenceThreshold(float)`                      | 设置置信度阈值           |
| `WithEncodingDetectionLogging(Action<EncodingDetectionResult>)`| 启用检测日志            |
| `Build()`                                                     | 构建配置根实例           |

### EncodingOptions

编码选项配置：

```csharp
public sealed class EncodingOptions
{
    // 预定义选项
    public static readonly EncodingOptions Default;      // 默认配置
    public static readonly EncodingOptions PowerShell;   // PowerShell 脚本配置（UTF-8 BOM）

    // 读取策略
    public EncodingReadStrategy ReadStrategy { get; set; }  // AutoDetect, Specified, Preserve

    // 写入策略
    public EncodingWriteStrategy WriteStrategy { get; set; } // Utf8NoBom, Utf8WithBom, Preserve, Specified

    // 指定的读取/写入编码
    public Encoding? ReadEncoding { get; set; }
    public Encoding? WriteEncoding { get; set; }

    // 回退编码（自动检测失败时使用），默认 UTF-8
    public Encoding FallbackEncoding { get; set; }

    // 编码检测置信度阈值（0.0-1.0），默认 0.6
    public float ConfidenceThreshold { get; set; }

    // 是否启用编码检测缓存，默认 true
    public bool EnableCache { get; set; }

    // 是否启用编码检测日志，默认 false
    public bool EnableLogging { get; set; }
}
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

## 项目结构

```text
Apq.Cfg/
├── ICfgRoot.cs              # 配置根接口
├── MergedCfgRoot.cs         # 合并配置根实现
├── CfgBuilder.cs            # 配置构建器
├── CfgRootExtensions.cs     # 扩展方法
├── Changes/                 # 配置变更相关
│   ├── ChangeType.cs
│   ├── ConfigChange.cs
│   ├── ConfigChangeEvent.cs
│   └── DynamicReloadOptions.cs
├── EncodingSupport/         # 编码支持
│   ├── EncodingDetector.cs      # 编码检测器
│   ├── EncodingDetectionResult.cs
│   ├── EncodingMapping.cs       # 编码映射规则
│   └── EncodingOptions.cs       # 编码选项配置
├── Internal/                # 内部实现
│   ├── ChangeCoordinator.cs
│   ├── MergedConfigurationProvider.cs
│   └── MergedConfigurationSource.cs
└── Sources/                 # 配置源
    ├── ICfgSource.cs
    ├── JsonFileCfgSource.cs
    ├── File/
    │   └── FileCfgSourceBase.cs
    └── Environment/
        └── EnvVarsCfgSource.cs
```

## 依赖项

| 包名                                                | 用途                 |
| --------------------------------------------------- | -------------------- |
| Microsoft.Extensions.Configuration                  | 配置基础设施         |
| Microsoft.Extensions.Configuration.Abstractions     | 配置抽象接口         |
| Microsoft.Extensions.Configuration.Binder           | 配置绑定功能         |
| Microsoft.Extensions.Configuration.Json             | JSON 配置支持        |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 环境变量支持     |
| [UTF.Unknown](https://github.com/CharsetDetector/UTF-unknown) | 文件编码自动检测 |
| [System.Reactive](https://github.com/dotnet/reactive) | 配置变更订阅       |

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
