# 扩展方法

本页列出 Apq.Cfg 提供的所有扩展方法。

## CfgBuilder 扩展

### 本地配置源

#### AddJsonFile

```csharp
public static CfgBuilder AddJsonFile(
    this CfgBuilder builder,
    string path,
    bool optional = false,
    bool reloadOnChange = false,
    Encoding? encoding = null)
```

添加 JSON 文件配置源。

#### AddYamlFile

```csharp
public static CfgBuilder AddYamlFile(
    this CfgBuilder builder,
    string path,
    bool optional = false,
    bool reloadOnChange = false,
    Encoding? encoding = null)
```

添加 YAML 文件配置源。需要 `Apq.Cfg.Yaml` 包。

#### AddXmlFile

```csharp
public static CfgBuilder AddXmlFile(
    this CfgBuilder builder,
    string path,
    bool optional = false,
    bool reloadOnChange = false,
    Encoding? encoding = null)
```

添加 XML 文件配置源。需要 `Apq.Cfg.Xml` 包。

#### AddIniFile

```csharp
public static CfgBuilder AddIniFile(
    this CfgBuilder builder,
    string path,
    bool optional = false,
    bool reloadOnChange = false,
    Encoding? encoding = null)
```

添加 INI 文件配置源。需要 `Apq.Cfg.Ini` 包。

#### AddTomlFile

```csharp
public static CfgBuilder AddTomlFile(
    this CfgBuilder builder,
    string path,
    bool optional = false,
    bool reloadOnChange = false,
    Encoding? encoding = null)
```

添加 TOML 文件配置源。需要 `Apq.Cfg.Toml` 包。

#### AddEnvironmentVariables

```csharp
public static CfgBuilder AddEnvironmentVariables(
    this CfgBuilder builder,
    string? prefix = null)
```

添加环境变量配置源。

#### AddInMemory

```csharp
public static CfgBuilder AddInMemory(
    this CfgBuilder builder,
    IDictionary<string, string> data)
```

添加内存配置源。

### 远程配置源

#### AddConsul

```csharp
public static CfgBuilder AddConsul(
    this CfgBuilder builder,
    string address,
    string keyPrefix,
    bool watch = false,
    bool optional = false)

public static CfgBuilder AddConsul(
    this CfgBuilder builder,
    Action<ConsulCfgOptions> configure)
```

添加 Consul 配置源。需要 `Apq.Cfg.Consul` 包。

#### AddRedis

```csharp
public static CfgBuilder AddRedis(
    this CfgBuilder builder,
    string connectionString,
    string keyPrefix,
    bool subscribeChanges = false,
    bool optional = false)

public static CfgBuilder AddRedis(
    this CfgBuilder builder,
    Action<RedisOptions> configure)
```

添加 Redis 配置源。需要 `Apq.Cfg.Redis` 包。

#### AddApollo

```csharp
public static CfgBuilder AddApollo(
    this CfgBuilder builder,
    string configServerUrl,
    string appId,
    string? namespace = null,
    bool optional = false)

public static CfgBuilder AddApollo(
    this CfgBuilder builder,
    Action<ApolloCfgOptions> configure)
```

添加 Apollo 配置源。需要 `Apq.Cfg.Apollo` 包。

#### AddVault

```csharp
public static CfgBuilder AddVault(
    this CfgBuilder builder,
    string address,
    string secretPath,
    string? token = null,
    bool optional = false)

public static CfgBuilder AddVault(
    this CfgBuilder builder,
    Action<VaultCfgOptions> configure)
```

添加 Vault 配置源。需要 `Apq.Cfg.Vault` 包。

#### AddEtcd

```csharp
public static CfgBuilder AddEtcd(
    this CfgBuilder builder,
    string connectionString,
    string keyPrefix,
    bool watch = false,
    bool optional = false)
```

添加 Etcd 配置源。需要 `Apq.Cfg.Etcd` 包。

#### AddZookeeper

```csharp
public static CfgBuilder AddZookeeper(
    this CfgBuilder builder,
    string connectionString,
    string path,
    bool watch = false,
    bool optional = false)

public static CfgBuilder AddZookeeper(
    this CfgBuilder builder,
    Action<ZookeeperCfgOptions> configure)
```

添加 Zookeeper 配置源。需要 `Apq.Cfg.Zookeeper` 包。

## ICfgRoot 扩展

### GetValue&lt;T&gt;

```csharp
public static T GetValue<T>(
    this ICfgRoot cfg,
    string key,
    T defaultValue = default)
```

获取类型化配置值。

**支持的类型：**
- 基本类型：`string`, `int`, `long`, `float`, `double`, `decimal`, `bool`
- 日期时间：`DateTime`, `DateTimeOffset`, `TimeSpan`
- 其他：`Guid`, `Uri`, `Enum`

### GetRequiredSection

```csharp
public static ICfgSection GetRequiredSection(
    this ICfgRoot cfg,
    string key)
```

获取必需的配置节，不存在时抛出异常。

### Exists

```csharp
public static bool Exists(
    this ICfgRoot cfg,
    string key)
```

检查配置键是否存在。

### OnChange

```csharp
public static IDisposable OnChange(
    this ICfgRoot cfg,
    Action<IEnumerable<string>> callback)
```

注册配置变更回调。

## ICfgSection 扩展

### Get&lt;T&gt;

```csharp
public static T Get<T>(this ICfgSection section)
```

将配置节绑定到类型。

### GetValue&lt;T&gt;

```csharp
public static T GetValue<T>(
    this ICfgSection section,
    string key,
    T defaultValue = default)
```

获取子配置的类型化值。

### Exists

```csharp
public static bool Exists(this ICfgSection section)
```

检查配置节是否存在。

### Bind

```csharp
public static void Bind(
    this ICfgSection section,
    object instance)
```

将配置节绑定到现有对象。

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
    .AddJsonFile("config.json")
    .AddEnvironmentVariables());
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
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml", optional: true)
    .AddConsul("http://consul:8500", "myapp/config", watch: true)
    .AddEnvironmentVariables("MYAPP_")
    .Build();

// 读取配置
var host = cfg.GetValue<string>("Database:Host");
var port = cfg.GetValue<int>("Database:Port", 5432);

// 绑定配置
var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();

// 监听变更
cfg.OnChange(keys =>
{
    Console.WriteLine($"配置已更新: {string.Join(", ", keys)}");
});

// 依赖注入
services.AddApqCfg(builder => builder
    .AddJsonFile("config.json"));
services.ConfigureApqCfg<DatabaseOptions>("Database");
```

## 下一步

- [示例](/examples/) - 更多使用示例
- [最佳实践](/guide/best-practices) - 最佳实践指南
