# 性能优化

本指南介绍 Apq.Cfg 的性能优化策略和最佳实践。

## 内置优化

### 值缓存

Apq.Cfg 内置值缓存机制，避免重复解析：

```csharp
// 第一次访问：解析并缓存
var value1 = cfg.GetValue<int>("Database:Port");

// 后续访问：直接从缓存读取
var value2 = cfg.GetValue<int>("Database:Port"); // 更快
```

### 快速集合

内部使用优化的数据结构：

- 键查找：O(1) 哈希表
- 子节遍历：优化的迭代器
- 内存分配：对象池复用

## 配置缓存策略

### 启用缓存

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .ConfigureCache(options =>
    {
        options.Enabled = true;
        options.MaxSize = 1000;        // 最大缓存条目
        options.ExpireAfter = TimeSpan.FromMinutes(5);
    })
    .Build();
```

### 缓存预热

启动时预加载常用配置：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .ConfigureCache(options =>
    {
        options.PreloadKeys = new[]
        {
            "Database:ConnectionString",
            "Database:Timeout",
            "App:Name"
        };
    })
    .Build();
```

## 延迟加载

### 按需加载配置源

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("base.json")
    .AddJsonFile("optional.json", optional: true, lazy: true) // 延迟加载
    .Build();
```

### 延迟绑定

```csharp
// 延迟绑定，首次访问时才解析
var lazyConfig = new Lazy<DatabaseConfig>(() => 
    cfg.GetSection("Database").Get<DatabaseConfig>());

// 需要时才访问
if (needDatabase)
{
    var config = lazyConfig.Value;
}
```

## 批量操作

### 批量读取

```csharp
// 低效：多次访问
var host = cfg["Database:Host"];
var port = cfg["Database:Port"];
var database = cfg["Database:Database"];

// 高效：一次绑定
var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
```

### 批量监听

```csharp
// 低效：多个监听器
cfg.OnChange("Database:Host", _ => { });
cfg.OnChange("Database:Port", _ => { });

// 高效：监听整个节
cfg.OnChange("Database", changes => { });
```

## 内存优化

### 配置对象复用

```csharp
// 使用 IOptionsMonitor 复用配置对象
public class MyService
{
    private readonly IOptionsMonitor<DatabaseConfig> _options;
    
    public MyService(IOptionsMonitor<DatabaseConfig> options)
    {
        _options = options;
    }
    
    // CurrentValue 返回缓存的对象
    public DatabaseConfig Config => _options.CurrentValue;
}
```

### 避免频繁绑定

```csharp
// 避免在循环中绑定
// ❌ 低效
for (int i = 0; i < 1000; i++)
{
    var config = cfg.GetSection("Database").Get<DatabaseConfig>();
    // 使用 config
}

// ✅ 高效
var config = cfg.GetSection("Database").Get<DatabaseConfig>();
for (int i = 0; i < 1000; i++)
{
    // 使用 config
}
```

## 远程配置优化

### 连接池

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", options =>
    {
        options.MaxConnections = 10;
        options.ConnectionTimeout = TimeSpan.FromSeconds(5);
    })
    .Build();
```

### 本地缓存

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", options =>
    {
        options.LocalCacheEnabled = true;
        options.LocalCachePath = "./config-cache";
        options.LocalCacheExpiry = TimeSpan.FromHours(1);
    })
    .Build();
```

### 故障转移

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://consul1:8500", "myapp/config")
    .AddConsul("http://consul2:8500", "myapp/config", fallback: true)
    .AddJsonFile("config.fallback.json", fallback: true)
    .Build();
```

## 性能监控

### 启用指标

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .ConfigureMetrics(options =>
    {
        options.Enabled = true;
        options.OnMetrics = metrics =>
        {
            Console.WriteLine($"缓存命中率: {metrics.CacheHitRate:P}");
            Console.WriteLine($"平均读取时间: {metrics.AverageReadTime.TotalMicroseconds}μs");
        };
    })
    .Build();
```

## 基准测试结果

| 操作 | 耗时 | 内存分配 |
|------|------|----------|
| 索引器读取（缓存命中） | ~50ns | 0 |
| GetValue&lt;int&gt;（缓存命中） | ~100ns | 0 |
| GetSection | ~200ns | 24B |
| Get&lt;T&gt; 绑定（小对象） | ~1μs | 64B |
| Get&lt;T&gt; 绑定（大对象） | ~5μs | 256B |

## 下一步

- [最佳实践](/guide/best-practices) - 最佳实践指南
- [扩展开发](/guide/extension) - 自定义配置源
