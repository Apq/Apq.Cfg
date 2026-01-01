# Apq.Cfg.Apollo

[![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Apollo)](https://www.nuget.org/packages/Apq.Cfg.Apollo)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apollo 配置中心支持，为 Apq.Cfg 提供从 Apollo 读取配置的能力，支持热重载。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg.Apollo
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.Apollo;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddApollo(options =>
    {
        options.AppId = "my-app";
        options.MetaServer = "http://localhost:8080";
        options.Namespaces = new[] { "application" };
        options.EnableHotReload = true;
    }, level: 10)
    .Build();

// 使用索引器访问
var host = cfg["Database:Host"];
```

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `AppId` | string | `""` | Apollo 应用 ID |
| `MetaServer` | string | `http://localhost:8080` | Meta Server 地址 |
| `Cluster` | string | `default` | 集群名称 |
| `Namespaces` | string[] | `["application"]` | 命名空间列表 |
| `Secret` | string? | null | 访问密钥（可选） |
| `EnableHotReload` | bool | true | 是否启用热重载 |
| `ConnectTimeout` | TimeSpan | 10 秒 | 连接超时时间 |
| `LongPollingTimeout` | TimeSpan | 90 秒 | 长轮询超时时间 |
| `DataFormat` | ApolloDataFormat | Properties | 配置数据格式 |

## 多命名空间

Apollo 支持从多个命名空间读取配置：

```csharp
.AddApollo(options =>
{
    options.AppId = "my-app";
    options.MetaServer = "http://localhost:8080";
    options.Namespaces = new[] { "application", "common", "database" };
})
```

当配置多个命名空间时，非 `application` 命名空间的配置键会自动添加命名空间前缀：

```csharp
// application 命名空间的配置
var appName = cfg.Get("App:Name");

// common 命名空间的配置
var logLevel = cfg.Get("common:Logging:Level");

// database 命名空间的配置
var connStr = cfg.Get("database:ConnectionString");
```

## 热重载

启用 `EnableHotReload` 后，配置源会使用 Apollo 的长轮询通知机制监听配置变更，当配置发生变化时自动更新。

```csharp
// 启用热重载
.AddApollo(options =>
{
    options.EnableHotReload = true;
    options.LongPollingTimeout = TimeSpan.FromSeconds(90);
})
```

## 认证

### Secret 签名认证

```csharp
.AddApollo(options =>
{
    options.AppId = "my-app";
    options.MetaServer = "http://localhost:8080";
    options.Secret = "your-app-secret";
})
```

## 多层级配置

Apollo 配置源可以与其他配置源组合使用，通过 `level` 参数控制优先级：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddApollo(options =>                        // Apollo 远程配置（最高优先级）
    {
        options.AppId = "my-app";
        options.MetaServer = "http://apollo:8080";
    }, level: 10)
    .Build();
```

## 配置写入

> **注意**：Apollo 配置源**不支持**通过 API 写入配置。如需修改配置，请通过 Apollo 管理界面操作。

```csharp
// 以下代码会抛出 NotSupportedException
cfg.Set("Key", "Value");
await cfg.SaveAsync();  // 抛出异常
```

## 简化用法

```csharp
// 使用简化的扩展方法
var cfg = new CfgBuilder()
    .AddApollo("my-app", "http://localhost:8080",
        namespaces: new[] { "application", "common" },
        level: 10)
    .Build();
```

## 与其他配置中心对比

| 特性 | Apollo | Nacos | Consul | Etcd |
|------|--------|-------|--------|------|
| 写入支持 | ❌ | ✅ | ✅ | ✅ |
| 多命名空间 | ✅ | ❌ | ❌ | ❌ |
| 多集群 | ✅ | ❌ | ✅ | ❌ |
| 灰度发布 | ✅ | ✅ | ❌ | ❌ |
| 本地缓存 | ✅（内置） | ❌ | ❌ | ❌ |

## 依赖

无外部依赖，使用 HTTP API 直接与 Apollo 通信。
