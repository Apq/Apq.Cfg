# CfgBuilder API

`CfgBuilder` 是配置构建器，用于组合多个配置源并创建配置实例。

## 构造函数

```csharp
public CfgBuilder()
```

创建一个新的配置构建器实例。

## 方法

### AddSource

```csharp
public CfgBuilder AddSource(ICfgSource source)
```

添加自定义配置源。

**参数：**
- `source`: 配置源实例

**返回：** 当前构建器实例（支持链式调用）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddSource(new MyCustomSource(level: 5, writeable: false))
    .Build();
```

### AddJson

```csharp
public CfgBuilder AddJson(
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 JSON 文件配置源。

**参数：**
- `path`: 文件路径
- `level`: 层级优先级（数值越大优先级越高）
- `writeable`: 是否可写
- `isPrimaryWriter`: 是否为主写入源（同层级只能有一个）
- `optional`: 是否可选
- `reloadOnChange`: 是否在文件变更时重载
- `encoding`: 编码选项

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    .Build();
```

### AddYaml

```csharp
public CfgBuilder AddYaml(
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 YAML 文件配置源。参数与 `AddJson` 相同。

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: false)
    .Build();
```

### AddEnvironmentVariables

```csharp
public CfgBuilder AddEnvironmentVariables(int level, string? prefix = null)
```

添加环境变量配置源。

**参数：**
- `level`: 层级优先级
- `prefix`: 环境变量前缀（可选）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

### AddReadEncodingMapping

```csharp
public CfgBuilder AddReadEncodingMapping(string path, Encoding encoding, int priority = 100)
```

添加读取编码映射（完整路径）。

**参数：**
- `path`: 文件完整路径
- `encoding`: 编码
- `priority`: 优先级（默认 100）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddReadEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### AddReadEncodingMappingWildcard

```csharp
public CfgBuilder AddReadEncodingMappingWildcard(string pattern, Encoding encoding, int priority = 0)
```

添加读取编码映射（通配符）。

**参数：**
- `pattern`: 通配符模式（如 `*.ini`）
- `encoding`: 编码
- `priority`: 优先级（默认 0）

### AddReadEncodingMappingRegex

```csharp
public CfgBuilder AddReadEncodingMappingRegex(string pattern, Encoding encoding, int priority = 0)
```

添加读取编码映射（正则表达式）。

**参数：**
- `pattern`: 正则表达式模式
- `encoding`: 编码
- `priority`: 优先级（默认 0）

### AddWriteEncodingMapping / AddWriteEncodingMappingWildcard / AddWriteEncodingMappingRegex

与读取映射方法类似，用于配置写入编码映射。

### ConfigureEncodingMapping

```csharp
public CfgBuilder ConfigureEncodingMapping(Action<EncodingMappingConfig> configure)
```

高级编码映射配置。

**参数：**
- `configure`: 配置委托

**示例：**
```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        config.AddReadMapping("*.xml", EncodingMappingType.Wildcard, Encoding.UTF8, priority: 50);
        config.AddWriteMapping("**/*.txt", EncodingMappingType.Wildcard, new UTF8Encoding(true), priority: 10);
        config.ClearWriteMappings();
    })
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### WithEncodingConfidenceThreshold

```csharp
public CfgBuilder WithEncodingConfidenceThreshold(float threshold)
```

设置编码检测置信度阈值。

**参数：**
- `threshold`: 置信度阈值（0.0 - 1.0，默认 0.6）

### WithEncodingDetectionLogging

```csharp
public CfgBuilder WithEncodingDetectionLogging(Action<EncodingDetectionResult> logger)
```

启用编码检测日志。

**参数：**
- `logger`: 日志回调

### Build

```csharp
public ICfgRoot Build()
```

构建配置实例。

**返回：** `ICfgRoot` 配置根实例

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

## 完整示例

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 环境特定配置
    .AddJson($"config.{environment}.json", level: 1, writeable: false, optional: true)
    // 本地覆盖（可写）
    .AddJson("config.local.json", level: 2, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    // 远程配置
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false, optional: true))
    // 环境变量（最高优先级）
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    // 编码配置
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    .WithEncodingConfidenceThreshold(0.8f)
    .Build();
```

## 下一步

- [ICfgRoot API](/api/icfg-root) - 配置根接口
- [ICfgSection API](/api/icfg-section) - 配置节接口
