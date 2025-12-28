# API 概述

本节提供 Apq.Cfg 的完整 API 参考文档。

## 核心类型

### CfgBuilder

配置构建器，用于创建配置实例。

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .Build();
```

[查看详细 API →](/api/cfg-builder)

### ICfgRoot

配置根接口，提供配置读取功能。

```csharp
public interface ICfgRoot
{
    string this[string key] { get; }
    T GetValue<T>(string key, T defaultValue = default);
    ICfgSection GetSection(string key);
    IChangeToken GetReloadToken();
}
```

[查看详细 API →](/api/icfg-root)

### ICfgSection

配置节接口，表示配置的一个子节。

```csharp
public interface ICfgSection
{
    string Key { get; }
    string Value { get; }
    string Path { get; }
    T Get<T>();
    IEnumerable<ICfgSection> GetChildren();
}
```

[查看详细 API →](/api/icfg-section)

## 配置源接口

### ICfgSource

配置源接口，所有配置源都实现此接口。

```csharp
public interface ICfgSource
{
    string Name { get; }
    bool SupportsReload { get; }
    Task<IDictionary<string, string>> LoadAsync(CancellationToken ct = default);
    IChangeToken? GetReloadToken();
}
```

## 扩展方法

### CfgBuilder 扩展

| 方法 | 说明 |
|------|------|
| `AddJsonFile()` | 添加 JSON 文件配置源 |
| `AddYamlFile()` | 添加 YAML 文件配置源 |
| `AddXmlFile()` | 添加 XML 文件配置源 |
| `AddIniFile()` | 添加 INI 文件配置源 |
| `AddTomlFile()` | 添加 TOML 文件配置源 |
| `AddEnvironmentVariables()` | 添加环境变量配置源 |
| `AddConsul()` | 添加 Consul 配置源 |
| `AddRedis()` | 添加 Redis 配置源 |
| `AddApollo()` | 添加 Apollo 配置源 |
| `AddVault()` | 添加 Vault 配置源 |

[查看详细 API →](/api/extensions)

### ICfgRoot 扩展

| 方法 | 说明 |
|------|------|
| `GetValue<T>()` | 获取类型化配置值 |
| `GetSection()` | 获取配置节 |
| `GetRequiredSection()` | 获取必需的配置节 |
| `GetChildren()` | 获取所有子节 |
| `Exists()` | 检查配置是否存在 |

### ICfgSection 扩展

| 方法 | 说明 |
|------|------|
| `Get<T>()` | 绑定到类型 |
| `GetValue<T>()` | 获取类型化值 |
| `GetChildren()` | 获取子节 |
| `Exists()` | 检查是否存在 |

## 命名空间

| 命名空间 | 说明 |
|----------|------|
| `Apq.Cfg` | 核心类型和接口 |
| `Apq.Cfg.Sources` | 配置源基类和接口 |
| `Apq.Cfg.Changes` | 配置变更相关类型 |
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
