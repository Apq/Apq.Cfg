# Apq.Cfg.Etcd

[![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Etcd)](https://www.nuget.org/packages/Apq.Cfg.Etcd)

Etcd 配置中心支持，为 Apq.Cfg 提供从 Etcd KV 存储读取配置的能力，支持热重载。

## 安装

```bash
dotnet add package Apq.Cfg.Etcd
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.Etcd;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { "localhost:2379" };
        options.KeyPrefix = "/app/config/";
        options.EnableHotReload = true;
    }, level: 10)
    .Build();

// 读取配置
var value = cfg.Get("Database:Host");

// 订阅配置变更
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($"配置变更: {change.Key} = {change.NewValue}");
});
```

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Endpoints` | string[] | `["localhost:2379"]` | Etcd 服务端点列表 |
| `Username` | string? | null | 用户名（可选） |
| `Password` | string? | null | 密码（可选） |
| `CaCertPath` | string? | null | CA 证书路径（用于 TLS） |
| `ClientCertPath` | string? | null | 客户端证书路径（用于 mTLS） |
| `ClientKeyPath` | string? | null | 客户端私钥路径（用于 mTLS） |
| `KeyPrefix` | string | `/config/` | KV 键前缀 |
| `EnableHotReload` | bool | true | 是否启用热重载 |
| `ConnectTimeout` | TimeSpan | 10 秒 | 连接超时时间 |
| `ReconnectInterval` | TimeSpan | 5 秒 | 重连间隔 |
| `DataFormat` | EtcdDataFormat | KeyValue | 配置数据格式 |
| `SingleKey` | string? | null | 当 DataFormat 为 Json 时的单个 key |

## 数据格式

### KeyValue 模式（默认）

每个 Etcd KV 键对应一个配置项：

```
/config/Database/Host = "localhost"
/config/Database/Port = "5432"
/config/App/Name = "MyApp"
```

```csharp
.AddEtcd(options =>
{
    options.KeyPrefix = "/config/";
    options.DataFormat = EtcdDataFormat.KeyValue;
})
```

### JSON 模式

单个 key 存储 JSON 格式的配置：

```
/config/app-config = {"Database":{"Host":"localhost","Port":5432}}
```

```csharp
.AddEtcd(options =>
{
    options.KeyPrefix = "/config/";
    options.DataFormat = EtcdDataFormat.Json;
    options.SingleKey = "app-config";
})
```

## 热重载

启用 `EnableHotReload` 后，配置源会使用 Etcd 的 Watch API 监听配置变更，当配置发生变化时自动更新。

```csharp
// 启用热重载
.AddEtcd(options =>
{
    options.EnableHotReload = true;
})
```

## 认证

### 用户名/密码认证

```csharp
.AddEtcd(options =>
{
    options.Endpoints = new[] { "localhost:2379" };
    options.Username = "root";
    options.Password = "your-password";
})
```

### TLS 认证

```csharp
.AddEtcd(options =>
{
    options.Endpoints = new[] { "localhost:2379" };
    options.CaCertPath = "/path/to/ca.crt";
})
```

### mTLS 认证

```csharp
.AddEtcd(options =>
{
    options.Endpoints = new[] { "localhost:2379" };
    options.CaCertPath = "/path/to/ca.crt";
    options.ClientCertPath = "/path/to/client.crt";
    options.ClientKeyPath = "/path/to/client.key";
})
```

## 多端点高可用

支持配置多个 Etcd 端点实现高可用：

```csharp
.AddEtcd(options =>
{
    options.Endpoints = new[]
    {
        "etcd1:2379",
        "etcd2:2379",
        "etcd3:2379"
    };
})
```

## 多层级配置

Etcd 配置源可以与其他配置源组合使用，通过 `level` 参数控制优先级：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddEtcd(options =>                          // Etcd 远程配置（最高优先级）
    {
        options.Endpoints = new[] { "etcd:2379" };
        options.KeyPrefix = "/myapp/";
    }, level: 10)
    .Build();
```

## 依赖

- [dotnet-etcd](https://www.nuget.org/packages/dotnet-etcd) - Etcd .NET 客户端
