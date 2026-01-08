# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

.NET 统一配置组件库核心包，提供配置管理接口和基础实现。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg
```

## 快速开始

```csharp
using Apq.Cfg;

// 构建配置
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .Build();

// 读取配置
var host = cfg["Database:Host"];
var port = cfg.GetValue<int>("Database:Port");

// 使用配置节
var db = cfg.GetSection("Database");
var name = db["Name"];

// 修改并保存
cfg["App:LastRun"] = DateTime.Now.ToString();
await cfg.SaveAsync();
```

## 核心接口

### ICfgRoot

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    string? this[string key] { get; set; }
    T? GetValue<T>(string key);
    bool Exists(string key);
    ICfgSection GetSection(string path);
    void SetValue(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken ct = default);
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

### ICfgSection

```csharp
public interface ICfgSection
{
    string? this[string key] { get; set; }
    string Path { get; }
    T? GetValue<T>(string key);
    bool Exists(string key);
    void SetValue(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    ICfgSection GetSection(string path);
}
```

## 配置层级

| 层级 | 用途 | 配置源 |
|------|------|--------|
| 0-99 | 本地文件 | Json, Ini, Xml, Yaml, Toml |
| 100-199 | 远程存储 | Redis, Database |
| 200-299 | 配置中心 | Consul, Etcd, Nacos, Apollo, Zookeeper |
| 300-399 | 密钥管理 | Vault |
| 400+ | 环境变量 | Env, EnvironmentVariables |

## 扩展包

| 包名 | 说明 |
|------|------|
| `Apq.Cfg.Ini` | INI 格式 |
| `Apq.Cfg.Xml` | XML 格式 |
| `Apq.Cfg.Yaml` | YAML 格式 |
| `Apq.Cfg.Toml` | TOML 格式 |
| `Apq.Cfg.Env` | .env 文件 |
| `Apq.Cfg.Redis` | Redis 存储 |
| `Apq.Cfg.Database` | 数据库存储 |
| `Apq.Cfg.Consul` | Consul 配置中心 |
| `Apq.Cfg.Etcd` | Etcd 配置中心 |
| `Apq.Cfg.Nacos` | Nacos 配置中心 |
| `Apq.Cfg.Apollo` | Apollo 配置中心 |
| `Apq.Cfg.Zookeeper` | Zookeeper 配置中心 |
| `Apq.Cfg.Vault` | HashiCorp Vault |
| `Apq.Cfg.Crypto` | 配置加密脱敏 |
| `Apq.Cfg.SourceGenerator` | 源生成器 (Native AOT) |

## 许可证

MIT License
