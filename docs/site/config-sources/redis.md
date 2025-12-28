# Redis 配置源

Redis 是高性能的键值存储，适合作为配置中心使用。

## 安装

```bash
dotnet add package Apq.Cfg.Redis
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Redis;

var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(
        connectionString: "localhost:6379",
        keyPrefix: "config:myapp",
        level: 10,
        writeable: false))
    .Build();
```

### 启用订阅

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(
        connectionString: "localhost:6379",
        keyPrefix: "config:myapp",
        level: 10,
        writeable: false,
        subscribeChanges: true))
    .Build();
```

## 配置选项

### 完整配置

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(new RedisCfgOptions
    {
        ConnectionString = "localhost:6379,password=secret",
        KeyPrefix = "config:myapp",
        Database = 0,
        SubscribeChanges = true,
        ChangeChannel = "config:myapp:changes",
        Optional = false
    }, level: 10, writeable: false))
    .Build();
```

### 选项说明

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ConnectionString` | string | - | Redis 连接字符串 |
| `KeyPrefix` | string | - | 配置键前缀 |
| `Database` | int | 0 | Redis 数据库索引 |
| `SubscribeChanges` | bool | false | 是否订阅变更 |
| `ChangeChannel` | string | null | 变更通知频道 |
| `Optional` | bool | false | 是否可选 |

## Redis 配置结构

### Hash 存储（推荐）

```bash
# 使用 Redis CLI
HSET config:myapp Database:Host "localhost"
HSET config:myapp Database:Port "5432"
HSET config:myapp App:Name "MyApplication"

# 批量设置
HMSET config:myapp \
  Database:Host "localhost" \
  Database:Port "5432" \
  App:Name "MyApplication"
```

### 字符串存储

```bash
SET config:myapp:Database:Host "localhost"
SET config:myapp:Database:Port "5432"
```

### JSON 存储

```bash
SET config:myapp '{"Database":{"Host":"localhost","Port":5432}}'
```

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(new RedisCfgOptions
    {
        ConnectionString = "localhost:6379",
        KeyPrefix = "config:myapp",
        ParseJson = true
    }, level: 10, writeable: false))
    .Build();
```

## 配置变更通知

### 发布变更

```bash
# 修改配置后发布通知
HSET config:myapp Database:Host "new-host"
PUBLISH config:myapp:changes "Database:Host"
```

### 订阅变更

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(new RedisCfgOptions
    {
        ConnectionString = "localhost:6379",
        KeyPrefix = "config:myapp",
        SubscribeChanges = true,
        ChangeChannel = "config:myapp:changes"
    }, level: 10, writeable: false))
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("Redis 配置已更新:");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## 高可用配置

### Redis Sentinel

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(new RedisCfgOptions
    {
        ConnectionString = "sentinel1:26379,sentinel2:26379,serviceName=mymaster",
        KeyPrefix = "config:myapp"
    }, level: 10, writeable: false))
    .Build();
```

### Redis Cluster

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(new RedisCfgOptions
    {
        ConnectionString = "node1:6379,node2:6379,node3:6379",
        KeyPrefix = "config:myapp"
    }, level: 10, writeable: false))
    .Build();
```

## 认证配置

### 密码认证

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(
        connectionString: "localhost:6379,password=your-password",
        keyPrefix: "config:myapp",
        level: 10,
        writeable: false))
    .Build();
```

### SSL/TLS

```csharp
var cfg = new CfgBuilder()
    .AddSource(new RedisCfgSource(
        connectionString: "localhost:6379,ssl=true,sslHost=redis.example.com",
        keyPrefix: "config:myapp",
        level: 10,
        writeable: false))
    .Build();
```

## 与本地配置混合使用

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // Redis 远程配置（高优先级）
    .AddSource(new RedisCfgSource("localhost:6379", "config:myapp", level: 10, writeable: false, subscribeChanges: true))
    // 环境变量（最高优先级）
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

## 下一步

- [Apollo 配置源](/config-sources/apollo) - Apollo 配置中心
- [Vault 配置源](/config-sources/vault) - HashiCorp Vault
