# Consul 配置源

Consul 是 HashiCorp 的分布式服务发现和配置管理工具。

## 安装

```bash
dotnet add package Apq.Cfg.Consul
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Consul;

var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config")
    .Build();
```

### 启用监听

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", watch: true)
    .Build();
```

## 配置选项

### 完整配置

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "myapp/config";
        options.Token = "your-acl-token";
        options.Datacenter = "dc1";
        options.WaitTime = TimeSpan.FromMinutes(5);
        options.Watch = true;
        options.Optional = false;
    })
    .Build();
```

### 选项说明

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Address` | string | - | Consul 服务地址 |
| `KeyPrefix` | string | - | 配置键前缀 |
| `Token` | string | null | ACL Token |
| `Datacenter` | string | null | 数据中心 |
| `WaitTime` | TimeSpan | 5分钟 | 长轮询等待时间 |
| `Watch` | bool | false | 是否监听变更 |
| `Optional` | bool | false | 是否可选 |

## Consul 配置结构

### 键值存储

在 Consul 中存储配置：

```bash
# 使用 Consul CLI
consul kv put myapp/config/Database/Host "localhost"
consul kv put myapp/config/Database/Port "5432"
consul kv put myapp/config/App/Name "MyApplication"

# 或使用 HTTP API
curl -X PUT -d 'localhost' http://localhost:8500/v1/kv/myapp/config/Database/Host
```

### JSON 格式值

支持将整个 JSON 对象存储为单个键的值：

```bash
consul kv put myapp/config/Database '{"Host":"localhost","Port":5432}'
```

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", parseJson: true)
    .Build();
```

## 键路径映射

Consul 键路径映射为配置键：

| Consul 键 | 配置键 |
|-----------|--------|
| `myapp/config/Database/Host` | `Database:Host` |
| `myapp/config/Database/Port` | `Database:Port` |
| `myapp/config/App/Name` | `App:Name` |

## 高可用配置

### 多节点

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Addresses = new[]
        {
            "http://consul1:8500",
            "http://consul2:8500",
            "http://consul3:8500"
        };
        options.KeyPrefix = "myapp/config";
    })
    .Build();
```

### 故障转移

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://consul:8500", "myapp/config")
    .AddJsonFile("config.fallback.json", fallback: true)
    .Build();
```

## 监听配置变更

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", watch: true)
    .Build();

cfg.OnChange(changes =>
{
    Console.WriteLine("Consul 配置已更新:");
    foreach (var key in changes)
    {
        Console.WriteLine($"  {key} = {cfg[key]}");
    }
});
```

## ACL 配置

### 创建策略

```hcl
# consul-policy.hcl
key_prefix "myapp/config" {
  policy = "read"
}
```

```bash
consul acl policy create -name myapp-config -rules @consul-policy.hcl
consul acl token create -policy-name myapp-config
```

### 使用 Token

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "myapp/config";
        options.Token = Environment.GetEnvironmentVariable("CONSUL_TOKEN");
    })
    .Build();
```

## 下一步

- [Redis 配置源](/config-sources/redis) - Redis 配置支持
- [Apollo 配置源](/config-sources/apollo) - Apollo 配置中心
