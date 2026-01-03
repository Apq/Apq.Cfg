# Apq.Cfg.Consul

[![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Consul)](https://www.nuget.org/packages/Apq.Cfg.Consul)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Consul 配置中心支持，为 Apq.Cfg 提供从 Consul KV 存储读取配置的能力，支持热重载。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg.Consul
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Consul` (200)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 200
.AddConsul(options => { ... })

// 指定自定义层级
.AddConsul(options => { ... }, level: 250)
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.Consul;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
        options.EnableHotReload = true;
    }, level: 10)
    .Build();

// 使用索引器访问
var host = cfg["Database:Host"];
```

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Address` | string | `http://localhost:8500` | Consul 服务地址 |
| `Token` | string? | null | ACL Token（可选） |
| `Datacenter` | string? | null | 数据中心名称（可选） |
| `KeyPrefix` | string | `config/` | KV 键前缀 |
| `EnableHotReload` | bool | true | 是否启用热重载 |
| `WaitTime` | TimeSpan | 5 分钟 | Blocking Query 等待时间 |
| `ConnectTimeout` | TimeSpan | 10 秒 | 连接超时时间 |
| `ReconnectInterval` | TimeSpan | 5 秒 | 重连间隔 |
| `DataFormat` | ConsulDataFormat | KeyValue | 配置数据格式 |
| `SingleKey` | string? | null | 当 DataFormat 为 Json/Yaml 时的单个 key |

## 数据格式

### KeyValue 模式（默认）

每个 Consul KV 键对应一个配置项：

```
config/Database/Host = "localhost"
config/Database/Port = "5432"
config/App/Name = "MyApp"
```

```csharp
.AddConsul(options =>
{
    options.KeyPrefix = "config/";
    options.DataFormat = ConsulDataFormat.KeyValue;
})
```

### JSON 模式

单个 key 存储 JSON 格式的配置：

```
config/app-config = {"Database":{"Host":"localhost","Port":5432}}
```

```csharp
.AddConsul(options =>
{
    options.KeyPrefix = "config/";
    options.DataFormat = ConsulDataFormat.Json;
    options.SingleKey = "app-config";
})
```

### YAML 模式

单个 key 存储 YAML 格式的配置：

```csharp
.AddConsul(options =>
{
    options.DataFormat = ConsulDataFormat.Yaml;
    options.SingleKey = "app-config";
})
```

## 热重载

启用 `EnableHotReload` 后，配置源会使用 Consul 的 Blocking Query 机制监听配置变更，当配置发生变化时自动更新。

```csharp
// 启用热重载
.AddConsul(options =>
{
    options.EnableHotReload = true;
    options.WaitTime = TimeSpan.FromMinutes(5);  // Blocking Query 超时时间
})
```

## 认证

### ACL Token

```csharp
.AddConsul(options =>
{
    options.Address = "http://localhost:8500";
    options.Token = "your-acl-token";
})
```

## 多层级配置

Consul 配置源可以与其他配置源组合使用，通过 `level` 参数控制优先级：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddConsul(options =>                        // Consul 远程配置（最高优先级）
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "myapp/";
    }, level: 10)
    .Build();
```

## 依赖

- [Consul](https://www.nuget.org/packages/Consul) - Consul .NET 客户端
