# Apq.Cfg.Redis

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Redis 配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg
- StackExchange.Redis 2.10.1

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Redis;

var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
        options.KeyPrefix = "config:";
        options.Database = 0;
    }, level: 1, isPrimaryWriter: true)
    .Build();

// 读取配置
var connStr = cfg.Get("Database:ConnectionString");
var timeout = cfg.Get<int>("Database:Timeout");
```

## 方法签名

```csharp
public static CfgBuilder AddRedis(
    this CfgBuilder builder,
    Action<RedisOptions> configure,
    int level,
    bool isPrimaryWriter = false)
```

## RedisOptions

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ConnectionString` | `string?` | - | Redis 连接字符串 |
| `KeyPrefix` | `string?` | - | 键前缀，用于过滤配置键 |
| `Database` | `int?` | null | Redis 数据库索引 |
| `Channel` | `string?` | - | 发布/订阅通道，配置变更时发送通知 |
| `ConnectTimeoutMs` | `int` | 5000 | 连接超时（毫秒） |
| `OperationTimeoutMs` | `int` | 5000 | 操作超时（毫秒） |
| `ScanPageSize` | `int` | 250 | SCAN 命令每次返回的键数量 |

## 示例

### 基本配置

```csharp
.AddRedis(options =>
{
    options.ConnectionString = "localhost:6379";
}, level: 0)
```

### 带前缀和通道

```csharp
.AddRedis(options =>
{
    options.ConnectionString = "localhost:6379,password=secret";
    options.KeyPrefix = "myapp:config:";
    options.Database = 1;
    options.Channel = "config-updates";
}, level: 1, isPrimaryWriter: true)
```

## 数据存储

配置以 Redis String 类型存储：

```
SET config:Database:ConnectionString "Server=localhost"
SET config:Logging:Level "Information"
```

读取时使用 SCAN 命令按前缀扫描所有键。

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
