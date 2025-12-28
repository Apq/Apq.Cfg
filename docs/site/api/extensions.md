# 扩展方法

本页列出 Apq.Cfg 提供的所有扩展方法。

## CfgBuilder 扩展

### 本地配置源

#### AddJson

```csharp
public static CfgBuilder AddJson(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 JSON 文件配置源。

#### AddYaml

```csharp
public static CfgBuilder AddYaml(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 YAML 文件配置源。需要 `Apq.Cfg.Yaml` 包。

#### AddXml

```csharp
public static CfgBuilder AddXml(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 XML 文件配置源。需要 `Apq.Cfg.Xml` 包。

#### AddIni

```csharp
public static CfgBuilder AddIni(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 INI 文件配置源。需要 `Apq.Cfg.Ini` 包。

#### AddToml

```csharp
public static CfgBuilder AddToml(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable,
    bool isPrimaryWriter = false,
    bool optional = false,
    bool reloadOnChange = false,
    EncodingOptions? encoding = null)
```

添加 TOML 文件配置源。需要 `Apq.Cfg.Toml` 包。

#### AddEnvironmentVariables

```csharp
public static CfgBuilder AddEnvironmentVariables(
    this CfgBuilder builder,
    int level,
    string? prefix = null)
```

添加环境变量配置源。

#### AddSource

```csharp
public static CfgBuilder AddSource(
    this CfgBuilder builder,
    ICfgSource source)
```

添加自定义配置源。

### 编码映射

#### AddReadEncodingMapping

```csharp
public static CfgBuilder AddReadEncodingMapping(
    this CfgBuilder builder,
    string path,
    Encoding encoding,
    int priority = 100)
```

添加读取编码映射（完整路径）。

#### AddReadEncodingMappingWildcard

```csharp
public static CfgBuilder AddReadEncodingMappingWildcard(
    this CfgBuilder builder,
    string pattern,
    Encoding encoding,
    int priority = 0)
```

添加读取编码映射（通配符）。

#### AddReadEncodingMappingRegex

```csharp
public static CfgBuilder AddReadEncodingMappingRegex(
    this CfgBuilder builder,
    string pattern,
    Encoding encoding,
    int priority = 0)
```

添加读取编码映射（正则表达式）。

#### AddWriteEncodingMapping / AddWriteEncodingMappingWildcard / AddWriteEncodingMappingRegex

与读取映射方法类似，用于配置写入编码映射。

#### ConfigureEncodingMapping

```csharp
public static CfgBuilder ConfigureEncodingMapping(
    this CfgBuilder builder,
    Action<EncodingMappingConfig> configure)
```

高级编码映射配置。

#### WithEncodingConfidenceThreshold

```csharp
public static CfgBuilder WithEncodingConfidenceThreshold(
    this CfgBuilder builder,
    float threshold)
```

设置编码检测置信度阈值。

#### WithEncodingDetectionLogging

```csharp
public static CfgBuilder WithEncodingDetectionLogging(
    this CfgBuilder builder,
    Action<EncodingDetectionResult> logger)
```

启用编码检测日志。

## ServiceCollection 扩展

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

### ConfigureApqCfg

```csharp
public static IServiceCollection ConfigureApqCfg<TOptions>(
    this IServiceCollection services,
    string sectionKey) where TOptions : class
```

配置选项绑定。

**示例：**
```csharp
services.ConfigureApqCfg<DatabaseOptions>("Database");
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
var host = cfg.Get("Database:Host");
var port = cfg.Get<int>("Database:Port");

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
