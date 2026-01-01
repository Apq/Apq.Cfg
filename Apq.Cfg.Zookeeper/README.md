# Apq.Cfg.Zookeeper

[![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Zookeeper)](https://www.nuget.org/packages/Apq.Cfg.Zookeeper)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apq.Cfg 的 Zookeeper 配置中心扩展，支持热重载。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet add package Apq.Cfg.Zookeeper
```

## 快速开始

### 基本用法（KeyValue 模式）

```csharp
using Apq.Cfg;
using Apq.Cfg.Zookeeper;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddZookeeper(options => {
        options.ConnectionString = "localhost:2181";
        options.RootPath = "/app/config";
        options.EnableHotReload = true;
    }, level: 10)
    .Build();

// 使用索引器访问
var dbHost = cfg["Database:Host"];
var dbPort = cfg.Get<int>("Database:Port");
```

### 简化用法

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddZookeeper("localhost:2181", "/app/config", level: 10)
    .Build();
```

### JSON 格式配置

```csharp
// 从单个节点读取 JSON 配置
var cfg = new CfgBuilder()
    .AddZookeeperJson("localhost:2181", "/app/config.json", level: 10)
    .Build();
```

### 多节点集群

```csharp
var cfg = new CfgBuilder()
    .AddZookeeper(options => {
        options.ConnectionString = "zk1:2181,zk2:2181,zk3:2181";
        options.RootPath = "/app/config";
        options.SessionTimeout = TimeSpan.FromSeconds(30);
    }, level: 10)
    .Build();
```

### 带认证

```csharp
var cfg = new CfgBuilder()
    .AddZookeeper(options => {
        options.ConnectionString = "localhost:2181";
        options.RootPath = "/app/config";
        options.AuthScheme = "digest";
        options.AuthInfo = "user:password";
    }, level: 10)
    .Build();
```

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ConnectionString` | string | `localhost:2181` | Zookeeper 连接字符串，支持多节点 |
| `SessionTimeout` | TimeSpan | 30秒 | 会话超时时间 |
| `ConnectTimeout` | TimeSpan | 10秒 | 连接超时时间 |
| `RootPath` | string | `/config` | 配置根路径 |
| `EnableHotReload` | bool | `true` | 是否启用热重载 |
| `ReconnectInterval` | TimeSpan | 5秒 | 重连间隔 |
| `DataFormat` | enum | `KeyValue` | 数据格式（KeyValue/Json） |
| `SingleNode` | string | `null` | JSON 模式下的节点路径 |
| `AuthScheme` | string | `null` | 认证方案（如 digest） |
| `AuthInfo` | string | `null` | 认证信息（如 user:password） |

## 数据格式

### KeyValue 模式（默认）

每个 ZNode 对应一个配置项，节点路径映射为配置键：

```
/app/config/
├── Database/
│   ├── Host       -> "localhost"
│   ├── Port       -> "5432"
│   └── Name       -> "mydb"
└── Logging/
    └── Level      -> "Information"
```

对应的配置键：
- `Database:Host` = "localhost"
- `Database:Port` = "5432"
- `Database:Name` = "mydb"
- `Logging:Level` = "Information"

### JSON 模式

单个节点存储完整的 JSON 配置：

```
/app/config.json -> {"Database":{"Host":"localhost","Port":5432},"Logging":{"Level":"Info"}}
```

## 写入配置

```csharp
// 写入单个配置
cfg.Set("Database:Host", "192.168.1.100");
await cfg.SaveAsync();

// 批量写入
cfg.SetMany(new Dictionary<string, string?>
{
    ["Database:Host"] = "192.168.1.100",
    ["Database:Port"] = "5433"
});
await cfg.SaveAsync();

// 删除配置
cfg.Remove("Database:Deprecated");
await cfg.SaveAsync();
```

## 热重载

启用热重载后，配置变更会自动同步：

```csharp
// 订阅配置变更
cfg.ConfigChanges.Subscribe(change => {
    Console.WriteLine($"配置变更: {change.Key} = {change.NewValue}");
});
```

## 与其他配置源组合

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)                        // 基础配置
    .AddJson("config.local.json", level: 1, writeable: true) // 本地覆盖
    .AddEnvironmentVariables(level: 2, prefix: "APP_")       // 环境变量
    .AddZookeeper("localhost:2181", "/app/config", level: 10) // Zookeeper 最高优先级
    .Build();
```

## 注意事项

1. **会话管理**：Zookeeper 使用会话机制，会话过期后会自动重连
2. **临时节点**：配置使用持久节点（PERSISTENT），不会因会话过期而删除
3. **Watch 机制**：热重载基于 Zookeeper Watch，节点变更会触发重新加载
4. **性能**：首次加载会递归遍历所有子节点，大量配置时可考虑使用 JSON 模式

## 依赖

- [ZooKeeperNetEx](https://www.nuget.org/packages/ZooKeeperNetEx) - Zookeeper .NET 客户端
