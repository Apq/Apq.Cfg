# CfgBuilder API

`CfgBuilder` 是配置构建器，用于组合多个配置源并创建配置实例。

## 构造函数

```csharp
public CfgBuilder()
```

创建一个新的配置构建器实例。

## 配置源方法

### AddSource

```csharp
public CfgBuilder AddSource(ICfgSource source, string? name = null)
```

添加自定义配置源。

**参数：**
- `source`: 配置源实例
- `name`: 配置源名称（可选，用于标识）

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
    int level = CfgSourceLevels.Json,  // 默认 0
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false,
    EncodingOptions? encoding = null,
    string? name = null)
```

添加 JSON 文件配置源。

**参数：**
- `path`: 文件路径
- `level`: 层级优先级（数值越大优先级越高，默认 0）
- `writeable`: 是否可写（默认 false）
- `optional`: 是否可选（默认 true）
- `reloadOnChange`: 是否在文件变更时重载（默认 true）
- `isPrimaryWriter`: 是否为主写入源（同层级只能有一个）
- `encoding`: 编码选项
- `name`: 配置源名称（可选）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    .Build();
```

### AddEnvironmentVariables

```csharp
public CfgBuilder AddEnvironmentVariables(
    int level = CfgSourceLevels.EnvironmentVariables,  // 默认 400
    string? prefix = null,
    string? name = null)
```

添加环境变量配置源。

**参数：**
- `level`: 层级优先级（默认 400）
- `prefix`: 环境变量前缀（可选，为 null 时加载所有环境变量）
- `name`: 配置源名称（可选）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

## 编码配置方法

### WithEncodingConfidenceThreshold

```csharp
public CfgBuilder WithEncodingConfidenceThreshold(float threshold)
```

设置编码检测置信度阈值。

**参数：**
- `threshold`: 置信度阈值（0.0 - 1.0，默认 0.6）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .WithEncodingConfidenceThreshold(0.8f)
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### AddReadEncodingMapping

```csharp
public CfgBuilder AddReadEncodingMapping(string filePath, Encoding encoding, int priority = 100)
```

添加读取编码映射（完整路径）。

**参数：**
- `filePath`: 文件完整路径
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
- `pattern`: 通配符模式（如 `*.ini`、`config/*.xml`、`**/*.txt`）
- `encoding`: 编码
- `priority`: 优先级（默认 0）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddReadEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GB2312"))
    .Build();
```

### AddReadEncodingMappingRegex

```csharp
public CfgBuilder AddReadEncodingMappingRegex(string regexPattern, Encoding encoding, int priority = 0)
```

添加读取编码映射（正则表达式）。

**参数：**
- `regexPattern`: 正则表达式模式
- `encoding`: 编码
- `priority`: 优先级（默认 0）

### AddWriteEncodingMapping / AddWriteEncodingMappingWildcard / AddWriteEncodingMappingRegex

与读取映射方法类似，用于配置写入编码映射。

```csharp
public CfgBuilder AddWriteEncodingMapping(string filePath, Encoding encoding, int priority = 100)
public CfgBuilder AddWriteEncodingMappingWildcard(string pattern, Encoding encoding, int priority = 0)
public CfgBuilder AddWriteEncodingMappingRegex(string regexPattern, Encoding encoding, int priority = 0)
```

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

### WithEncodingDetectionLogging

```csharp
public CfgBuilder WithEncodingDetectionLogging(Action<EncodingDetectionResult> handler)
```

启用编码检测日志。

**参数：**
- `handler`: 日志处理器回调

**示例：**
```csharp
var cfg = new CfgBuilder()
    .WithEncodingDetectionLogging(result =>
    {
        Console.WriteLine($"文件: {result.FilePath}");
        Console.WriteLine($"检测编码: {result.DetectedEncoding?.EncodingName}");
        Console.WriteLine($"置信度: {result.Confidence}");
    })
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

## 值转换器和脱敏器方法

### AddValueTransformer

```csharp
public CfgBuilder AddValueTransformer(IValueTransformer transformer)
```

添加值转换器（用于加密/解密配置值）。

**参数：**
- `transformer`: 值转换器实例，实现 `IValueTransformer` 接口

**返回：** 配置构建器实例，支持链式调用

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddValueTransformer(new EncryptionTransformer(cryptoProvider))
    .Build();
```

### AddValueMasker

```csharp
public CfgBuilder AddValueMasker(IValueMasker masker)
```

添加值脱敏器（用于日志输出时隐藏敏感信息）。

**参数：**
- `masker`: 值脱敏器实例，实现 `IValueMasker` 接口

**返回：** 配置构建器实例，支持链式调用

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddValueMasker(new SensitiveMasker())
    .Build();
```

### ConfigureValueTransformer

```csharp
public CfgBuilder ConfigureValueTransformer(Action<ValueTransformerOptions> configure)
```

配置值转换选项。

**参数：**
- `configure`: 配置委托

**返回：** 配置构建器实例，支持链式调用

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .ConfigureValueTransformer(options =>
    {
        options.EncryptedPrefix = "[ENCRYPTED]";
        options.SensitiveKeyPatterns.Add("*ApiSecret*");
        options.SensitiveKeyPatterns.Add("*Token*");
    })
    .AddValueTransformer(new EncryptionTransformer(cryptoProvider))
    .Build();
```

## 构建方法

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

### 基本配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddEnvironmentVariables(level: 1, prefix: "APP_")
    .Build();
```

### 多环境配置

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 环境特定配置
    .AddJson($"config.{environment}.json", level: 1, writeable: false, optional: true)
    // 本地覆盖（可写）
    .AddJson("config.local.json", level: 2, writeable: true, isPrimaryWriter: true, optional: true, reloadOnChange: true)
    // 环境变量（最高优先级）
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

### 带加密和脱敏的配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    // 配置值转换选项
    .ConfigureValueTransformer(options =>
    {
        options.EncryptedPrefix = "[ENC]";
        options.SensitiveKeyPatterns.Add("*Password*");
        options.SensitiveKeyPatterns.Add("*Secret*");
        options.SensitiveKeyPatterns.Add("*ConnectionString*");
    })
    // 添加加密转换器
    .AddValueTransformer(new EncryptionTransformer(cryptoProvider))
    // 添加脱敏器
    .AddValueMasker(new SensitiveMasker())
    .Build();

// 读取时自动解密
var password = cfg["Database:Password"];

// 日志输出时使用脱敏值
var maskedPassword = cfg.GetMasked("Database:Password");
Console.WriteLine($"密码: {maskedPassword}"); // 输出: 密码: ***
```

### 带远程配置源的配置

```csharp
var cfg = new CfgBuilder()
    // 基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 远程配置（需要安装对应的扩展包）
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false, optional: true))
    // 环境变量（最高优先级）
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    // 编码配置
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    .WithEncodingConfidenceThreshold(0.8f)
    .Build();
```

### 带编码检测日志的配置

```csharp
var cfg = new CfgBuilder()
    .WithEncodingDetectionLogging(result =>
    {
        if (result.Confidence < 0.8f)
        {
            Console.WriteLine($"警告: 文件 {result.FilePath} 编码检测置信度较低 ({result.Confidence:P0})");
        }
    })
    .AddReadEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GB2312"))
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

## 下一步

- [ICfgRoot API](/api/icfg-root) - 配置根接口
- [ICfgSection API](/api/icfg-section) - 配置节接口
- [扩展方法](/api/extensions) - 所有扩展方法
