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
    .AddRedis("localhost:6379", "config:myapp")
    .Build();
```

### 启用订阅

```csharp
var cfg = new CfgBuilder()
    .AddRedis("localhost:6379", "config:myapp", subscribeChanges: true)
    .Build();
```

## 配置选项

### 完整配置

```csharp
var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379,password=secret";
        options.KeyPrefix = "config:myapp";
        options.Database = 0;
        options.SubscribeChanges = true;
        options.ChangeChannel = "config:myapp:changes";
        options.Optional = false;
    })
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
    .AddRedis("localhost:6379", "config:myapp", parseJson: true)
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
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
        options.KeyPrefix = "config:myapp";
        options.SubscribeChanges = true;
        options.ChangeChannel = "config:myapp:changes";
    })
    .Build();

cfg.OnChange(changes =>
{
    Console.WriteLine("Redis 配置已更新");
});
```

## 高可用配置

### Redis Sentinel

```csharp
var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "sentinel1:26379,sentinel2:26379,serviceName=mymaster";
        options.KeyPrefix = "config:myapp";
    })
    .Build();
```

### Redis Cluster

```csharp
var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "node1:6379,node2:6379,node3:6379";
        options.KeyPrefix = "config:myapp";
    })
    .Build();
```

## 认证配置

### 密码认证

```csharp
var cfg = new CfgBuilder()
    .AddRedis("localhost:6379,password=your-password", "config:myapp")
    .Build();
```

### SSL/TLS

```csharp
var cfg = new CfgBuilder()
    .AddRedis("localhost:6379,ssl=true,sslHost=redis.example.com", "config:myapp")
    .Build();
```

## 下一步

- [Apollo 配置源](/config-sources/apollo) - Apollo 配置中心
- [Vault 配置源](/config-sources/vault) - HashiCorp Vault
