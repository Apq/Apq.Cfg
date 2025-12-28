# API 概述

本节提供 Apq.Cfg 的完整 API 参考文档。

## 核心类型

### CfgBuilder

配置构建器，用于创建配置实例。

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

[查看详细 API →](/api/cfg-builder)

### ICfgRoot

配置根接口，提供配置读写功能。

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 读取操作
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);
    ICfgSection GetSection(string key);
    
    // 写入操作
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken ct = default);
    
    // 批量操作
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);
    void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);
    
    // 转换与事件
    IConfigurationRoot ToMicrosoftConfiguration();
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

[查看详细 API →](/api/icfg-root)

### ICfgSection

配置节接口，表示配置的一个子节。

```csharp
public interface ICfgSection
{
    string Path { get; }
    string? Get(string key);
    T? Get<T>(string key);
    void Set(string key, string? value, int? targetLevel = null);
    ICfgSection GetSection(string key);
    IEnumerable<string> GetChildKeys();
}
```

[查看详细 API →](/api/icfg-section)

## 配置源接口

### ICfgSource

配置源接口，所有配置源都实现此接口。

```csharp
public interface ICfgSource
{
    int Level { get; }              // 层级优先级
    bool IsWriteable { get; }       // 是否可写
    bool IsPrimaryWriter { get; }   // 是否为主写入源
    IConfigurationSource BuildSource();
}
```

### IWritableCfgSource

可写配置源接口。

```csharp
public interface IWritableCfgSource : ICfgSource
{
    void Set(string key, string? value);
    void Remove(string key);
    Task SaveAsync(CancellationToken ct = default);
}
```

## 扩展方法

### CfgBuilder 扩展

| 方法 | 说明 |
|------|------|
| `AddJson()` | 添加 JSON 文件配置源 |
| `AddYaml()` | 添加 YAML 文件配置源 |
| `AddXml()` | 添加 XML 文件配置源 |
| `AddIni()` | 添加 INI 文件配置源 |
| `AddToml()` | 添加 TOML 文件配置源 |
| `AddEnvironmentVariables()` | 添加环境变量配置源 |
| `AddSource()` | 添加自定义配置源 |
| `AddReadEncodingMapping()` | 添加读取编码映射 |
| `AddWriteEncodingMapping()` | 添加写入编码映射 |
| `ConfigureEncodingMapping()` | 配置编码映射 |

[查看详细 API →](/api/extensions)

### ICfgRoot 扩展

| 方法 | 说明 |
|------|------|
| `Get()` | 获取配置值 |
| `Get<T>()` | 获取类型化配置值 |
| `GetSection()` | 获取配置节 |
| `Exists()` | 检查配置是否存在 |
| `Set()` | 设置配置值 |
| `Remove()` | 移除配置键 |
| `SaveAsync()` | 保存配置 |

### ICfgSection 扩展

| 方法 | 说明 |
|------|------|
| `Get()` | 获取配置值 |
| `Get<T>()` | 获取类型化值 |
| `GetSection()` | 获取子节 |
| `GetChildKeys()` | 获取子键列表 |

## 命名空间

| 命名空间 | 说明 |
|----------|------|
| `Apq.Cfg` | 核心类型和接口 |
| `Apq.Cfg.Sources` | 配置源基类和接口 |
| `Apq.Cfg.Changes` | 配置变更相关类型 |
| `Apq.Cfg.Encoding` | 编码处理相关类型 |
| `Apq.Cfg.DependencyInjection` | DI 集成 |
| `Apq.Cfg.Yaml` | YAML 配置源 |
| `Apq.Cfg.Xml` | XML 配置源 |
| `Apq.Cfg.Ini` | INI 配置源 |
| `Apq.Cfg.Toml` | TOML 配置源 |
| `Apq.Cfg.Consul` | Consul 配置源 |
| `Apq.Cfg.Redis` | Redis 配置源 |
| `Apq.Cfg.Apollo` | Apollo 配置源 |
| `Apq.Cfg.Vault` | Vault 配置源 |

## 下一步

- [CfgBuilder API](/api/cfg-builder) - 配置构建器详细 API
- [ICfgRoot API](/api/icfg-root) - 配置根接口详细 API
