# 扩展方法

本页列出 Apq.Cfg 提供的所有扩展方法。

## CfgBuilder 方法

以下是 `CfgBuilder` 类提供的配置方法。

### 本地配置源

#### AddJson

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

添加 JSON 文件配置源。这是 `CfgBuilder` 的实例方法。

**参数：**
- `path`: JSON 文件路径
- `level`: 配置层级，数值越大优先级越高
- `writeable`: 是否可写
- `optional`: 是否为可选文件（默认 `true`）
- `reloadOnChange`: 文件变更时是否自动重载（默认 `true`）
- `isPrimaryWriter`: 是否为主要写入器（默认 `false`）
- `encoding`: 编码选项（默认 `null`）

#### AddYaml（扩展方法）

```csharp
public static CfgBuilder AddYaml(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

添加 YAML 文件配置源。需要 `Apq.Cfg.Yaml` 包。

#### AddXml（扩展方法）

```csharp
public static CfgBuilder AddXml(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

添加 XML 文件配置源。需要 `Apq.Cfg.Xml` 包。

#### AddIni（扩展方法）

```csharp
public static CfgBuilder AddIni(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

添加 INI 文件配置源。需要 `Apq.Cfg.Ini` 包。

#### AddToml（扩展方法）

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

添加 TOML 文件配置源。需要 `Apq.Cfg.Toml` 包。

#### AddEnvironmentVariables

```csharp
public CfgBuilder AddEnvironmentVariables(
    int level,
    string? prefix = null)
```

添加环境变量配置源。

**参数：**
- `level`: 配置层级，数值越大优先级越高
- `prefix`: 环境变量前缀，为 `null` 时加载所有环境变量

#### AddSource

```csharp
public CfgBuilder AddSource(ICfgSource source)
```

添加自定义配置源。

### 编码映射

#### AddReadEncodingMapping

```csharp
public CfgBuilder AddReadEncodingMapping(
    string filePath,
    Encoding encoding,
    int priority = 100)
```

添加读取编码映射（完整路径）。

#### AddReadEncodingMappingWildcard

```csharp
public CfgBuilder AddReadEncodingMappingWildcard(
    string pattern,
    Encoding encoding,
    int priority = 0)
```

添加读取编码映射（通配符）。

#### AddReadEncodingMappingRegex

```csharp
public CfgBuilder AddReadEncodingMappingRegex(
    string regexPattern,
    Encoding encoding,
    int priority = 0)
```

添加读取编码映射（正则表达式）。

#### AddWriteEncodingMapping / AddWriteEncodingMappingWildcard / AddWriteEncodingMappingRegex

与读取映射方法类似，用于配置写入编码映射。

```csharp
public CfgBuilder AddWriteEncodingMapping(string filePath, Encoding encoding, int priority = 100)
public CfgBuilder AddWriteEncodingMappingWildcard(string pattern, Encoding encoding, int priority = 0)
public CfgBuilder AddWriteEncodingMappingRegex(string regexPattern, Encoding encoding, int priority = 0)
```

#### ConfigureEncodingMapping

```csharp
public CfgBuilder ConfigureEncodingMapping(
    Action<EncodingMappingConfig> configure)
```

高级编码映射配置。

#### WithEncodingConfidenceThreshold

```csharp
public CfgBuilder WithEncodingConfidenceThreshold(float threshold)
```

设置编码检测置信度阈值（0.0-1.0，默认 0.6）。

#### WithEncodingDetectionLogging

```csharp
public CfgBuilder WithEncodingDetectionLogging(
    Action<EncodingDetectionResult> handler)
```

启用编码检测日志。

### 值转换器和脱敏器

#### AddValueTransformer

```csharp
public CfgBuilder AddValueTransformer(IValueTransformer transformer)
```

添加值转换器（用于加密/解密配置值）。

#### AddValueMasker

```csharp
public CfgBuilder AddValueMasker(IValueMasker masker)
```

添加值脱敏器（用于日志输出时隐藏敏感信息）。

#### ConfigureValueTransformer

```csharp
public CfgBuilder ConfigureValueTransformer(Action<ValueTransformerOptions> configure)
```

配置值转换选项。

## ICfgRoot 扩展方法

### TryGetValue

```csharp
public static bool TryGetValue<T>(this ICfgRoot root, string key, out T? value)
```

尝试获取配置值。

**示例：**
```csharp
if (cfg.TryGetValue<int>("Database:Port", out var port))
{
    Console.WriteLine($"端口: {port}");
}
```

### GetRequired

```csharp
public static T GetRequired<T>(this ICfgRoot root, string key)
```

获取必需的配置值，如果不存在则抛出 `InvalidOperationException` 异常。

**示例：**
```csharp
var connectionString = cfg.GetRequired<string>("Database:ConnectionString");
```

### GetOrDefault

```csharp
public static T? GetOrDefault<T>(this ICfgRoot root, string key, T? defaultValue = default)
```

获取配置值，如果不存在则返回默认值。

**示例：**
```csharp
var timeout = cfg.GetOrDefault("Database:Timeout", 30);
var retryCount = cfg.GetOrDefault<int>("Database:RetryCount", 3);
```

### GetMasked

```csharp
public static string GetMasked(this ICfgRoot cfg, string key)
```

获取脱敏后的配置值（用于日志输出）。

**示例：**
```csharp
logger.LogInformation("连接字符串: {ConnectionString}", cfg.GetMasked("Database:ConnectionString"));
// 输出: 连接字符串: Ser***ion
```

### GetMaskedSnapshot

```csharp
public static IReadOnlyDictionary<string, string> GetMaskedSnapshot(this ICfgRoot cfg)
```

获取所有配置的脱敏快照（用于调试）。

**示例：**
```csharp
var snapshot = cfg.GetMaskedSnapshot();
foreach (var (key, value) in snapshot)
{
    Console.WriteLine($"{key}: {value}");
}
```

## ServiceCollection 扩展方法

### AddApqCfg

```csharp
public static IServiceCollection AddApqCfg(
    this IServiceCollection services,
    Action<CfgBuilder> configure)
```

注册 Apq.Cfg 到依赖注入容器。

**示例：**
```csharp
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0, writeable: false)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));
```

### AddApqCfg（支持访问 IServiceProvider）

```csharp
public static IServiceCollection AddApqCfg(
    this IServiceCollection services,
    Action<CfgBuilder, IServiceProvider> configure)
```

注册 Apq.Cfg 到依赖注入容器，支持在配置过程中访问已注册的服务。

**示例：**
```csharp
services.AddDataProtection();
services.AddApqCfg((builder, sp) => builder
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddDataProtectionEncryption(sp.GetRequiredService<IDataProtectionProvider>())
    .AddSensitiveMasking());
```

### AddApqCfg（工厂方法）

```csharp
public static IServiceCollection AddApqCfg(
    this IServiceCollection services,
    Func<IServiceProvider, ICfgRoot> factory)
```

使用工厂方法注册 Apq.Cfg。

**示例：**
```csharp
services.AddApqCfg(sp => {
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new CfgBuilder()
        .AddJson("config.json", level: 0, writeable: false)
        .AddJson($"config.{env.EnvironmentName}.json", level: 1, writeable: false, optional: true)
        .AddEnvironmentVariables(level: 2, prefix: "APP_")
        .Build();
});
```

### AddApqCfg&lt;TOptions&gt;

```csharp
public static IServiceCollection AddApqCfg<TOptions>(
    this IServiceCollection services,
    Action<CfgBuilder> configure,
    string sectionKey)
    where TOptions : class, new()
```

添加 Apq.Cfg 配置服务并绑定强类型配置。

**示例：**
```csharp
services.AddApqCfg<DatabaseOptions>(cfg => cfg
    .AddJson("config.json", level: 0, writeable: false)
    .AddEnvironmentVariables(level: 2, prefix: "APP_"),
    "Database");

// 使用配置
var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
```

### ConfigureApqCfg

```csharp
public static IServiceCollection ConfigureApqCfg<TOptions>(
    this IServiceCollection services,
    string sectionKey)
    where TOptions : class, new()
```

配置选项绑定，支持嵌套对象和集合。

**示例：**
```csharp
services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### ConfigureApqCfg（带变更回调）

```csharp
public static IServiceCollection ConfigureApqCfg<TOptions>(
    this IServiceCollection services,
    string sectionKey,
    Action<TOptions> onChange)
    where TOptions : class, new()
```

配置选项绑定并启用配置变更监听。

**示例：**
```csharp
services.ConfigureApqCfg<DatabaseOptions>("Database", options => {
    Console.WriteLine($"数据库连接字符串已更新: {options.ConnectionString}");
    // 执行必要的重新连接逻辑
});
```

## 使用示例

```csharp
// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddYaml("config.yaml", level: 1, writeable: false, optional: true)
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, writeable: false, watch: true))
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    .Build();

// 读取配置
var host = cfg["Database:Host"];
var port = cfg.GetValue<int>("Database:Port");

// 使用扩展方法
var timeout = cfg.GetOrDefault("Database:Timeout", 30);
var connStr = cfg.GetRequired<string>("Database:ConnectionString");

// 检查配置是否存在
if (cfg.Exists("OptionalFeature"))
{
    // 处理可选功能
}

// 获取配置节
var dbSection = cfg.GetSection("Database");

// 监听变更
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"配置已更新: {string.Join(", ", e.Changes.Keys)}");
});

// 依赖注入
services.AddApqCfg(builder => builder
    .AddJson("config.json", level: 0, writeable: false));
services.ConfigureApqCfg<DatabaseOptions>("Database");
```

## 下一步

- [示例](/examples/) - 更多使用示例
- [最佳实践](/guide/best-practices) - 最佳实践指南
