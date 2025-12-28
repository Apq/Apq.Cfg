# API 参考

本节提供 Apq.Cfg 的完整 API 文档。

## 核心类

### CfgBuilder

配置构建器，用于创建和配置 `CfgRoot` 实例。

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml", optional: true)
    .Build();
```

[查看详细文档](/api/cfg-builder)

### CfgRoot

配置根对象，提供配置读取和管理功能。

```csharp
// 获取值
var value = cfg.Get<string>("Key");
var valueWithDefault = cfg.Get<int>("Key", 10);

// 获取配置节
var section = cfg.GetSection("Section");

// 绑定到对象
var settings = cfg.Bind<AppSettings>();
```

[查看详细文档](/api/cfg-root)

### CfgSection

配置节，表示配置树中的一个节点。

```csharp
var section = cfg.GetSection("Database");
var connectionString = section.Get<string>("ConnectionString");
var settings = section.Bind<DatabaseSettings>();
```

[查看详细文档](/api/cfg-section)

## 扩展方法

### 配置源扩展

每个配置源包都提供相应的扩展方法：

```csharp
// JSON
builder.AddJsonFile("config.json");

// YAML
builder.AddYamlFile("config.yaml");

// TOML
builder.AddTomlFile("config.toml");

// Redis
builder.AddRedis(options => {
    options.ConnectionString = "localhost:6379";
    options.KeyPrefix = "config:";
});

// Consul
builder.AddConsul("http://localhost:8500", "app/config");
```

### 依赖注入扩展

```csharp
// 注册配置服务
services.AddApqCfg(builder => {
    builder.AddJsonFile("appsettings.json");
});

// 配置选项
services.Configure<AppSettings>(cfg => cfg.GetSection("App"));
```

## 接口

### ICfgSource

配置源接口，实现此接口可创建自定义配置源。

```csharp
public interface ICfgSource
{
    IEnumerable<KeyValuePair<string, string>> Load();
    void Reload();
    event EventHandler<CfgChangedEventArgs> Changed;
}
```

### ICfgProvider

配置提供程序接口。

```csharp
public interface ICfgProvider
{
    bool TryGet(string key, out string value);
    void Set(string key, string value);
    IEnumerable<string> GetChildKeys(string parentPath);
}
```

## 特性

### CfgSectionAttribute

用于源代码生成器的配置节特性。

```csharp
[CfgSection("Database")]
public partial class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}
```

## 下一步

- [CfgBuilder 详解](/api/cfg-builder)
- [CfgRoot 详解](/api/cfg-root)
- [CfgSection 详解](/api/cfg-section)
