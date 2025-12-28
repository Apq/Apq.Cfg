# 性能优化

本指南介绍 Apq.Cfg 的性能优化策略和最佳实践。

## 性能基准

| 场景 | 性能指标 | 说明 |
|------|----------|------|
| 基本读写 | 17-22 ns | Get/Set 操作纳秒级响应 |
| 类型转换 | 67-136 ns | 支持所有标准类型 |
| 批量操作（回调） | 零堆分配 | 比返回字典版本快 43-50% |
| 并发读取 | 14-19 μs (16线程) | 高并发场景性能提升 19% |
| 缓存命中 | 1.5-1.7 μs | 缓存性能提升 12% |
| 配置节 | 18-29 ns | GetSection 操作 |
| 源生成器 | 2.1-2.7 μs | 比反射绑定快约 100 倍 |
| DI 解析 | 6-12 ns | Scoped 解析性能极佳 |

## 优化策略

### 1. 使用高性能批量操作

#### ❌ 低效方式：多次单独调用

```csharp
// 每次调用都有锁开销
var host = cfg.Get("Database:Host");
var port = cfg.Get<int>("Database:Port");
var name = cfg.Get("Database:Name");
var timeout = cfg.Get<int>("Database:Timeout");
```

#### ✅ 高效方式：批量获取（返回字典）

```csharp
var keys = new[] { "Database:Host", "Database:Port", "Database:Name", "Database:Timeout" };
var values = cfg.GetMany(keys);

var host = values["Database:Host"];
var port = int.Parse(values["Database:Port"] ?? "5432");
```

#### ✅✅ 最高效方式：批量获取（回调，零堆分配）

```csharp
string? host = null;
int port = 5432;
string? name = null;
int timeout = 30;

cfg.GetMany(new[] { "Database:Host", "Database:Port", "Database:Name", "Database:Timeout" }, 
    (key, value) =>
    {
        switch (key)
        {
            case "Database:Host": host = value; break;
            case "Database:Port": port = int.TryParse(value, out var p) ? p : 5432; break;
            case "Database:Name": name = value; break;
            case "Database:Timeout": timeout = int.TryParse(value, out var t) ? t : 30; break;
        }
    });
```

### 2. 使用配置节减少键路径解析

#### ❌ 低效方式：重复的长键路径

```csharp
var host = cfg.Get("Application:Services:Database:Connection:Host");
var port = cfg.Get<int>("Application:Services:Database:Connection:Port");
var name = cfg.Get("Application:Services:Database:Connection:Name");
```

#### ✅ 高效方式：使用配置节

```csharp
var dbSection = cfg.GetSection("Application:Services:Database:Connection");
var host = dbSection.Get("Host");
var port = dbSection.Get<int>("Port");
var name = dbSection.Get("Name");
```

### 3. 使用源生成器（Native AOT 支持）

#### ❌ 低效方式：反射绑定

```csharp
// 运行时反射，性能较差
var options = cfg.GetSection("Database").Get<DatabaseOptions>();
```

#### ✅ 高效方式：源生成器（编译时生成）

```csharp
// 1. 定义配置类
[CfgSection("Database")]
public partial class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 5432;
    public string? Name { get; set; }
}

// 2. 使用生成的绑定方法（零反射）
var options = DatabaseOptions.BindFrom(cfg.GetSection("Database"));
```

**性能对比**：源生成器比反射绑定快约 **100 倍**。

### 4. 优化热重载配置

#### 调整防抖时间

```csharp
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 200,  // 增加防抖时间，减少重载频率
    EnableDynamicReload = true
});
```

#### 按需启用热重载

```csharp
var cfg = new CfgBuilder()
    // 基础配置不需要热重载
    .AddJson("config.json", level: 0, reloadOnChange: false)
    // 只对需要动态更新的配置启用热重载
    .AddJson("dynamic-config.json", level: 1, reloadOnChange: true)
    .Build();
```

### 5. 减少配置源数量

#### ❌ 低效方式：过多配置源

```csharp
var cfg = new CfgBuilder()
    .AddJson("config1.json", level: 0)
    .AddJson("config2.json", level: 1)
    .AddJson("config3.json", level: 2)
    .AddJson("config4.json", level: 3)
    .AddJson("config5.json", level: 4)
    // ... 更多配置源
    .Build();
```

#### ✅ 高效方式：合理组织配置源

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson($"config.{env}.json", level: 1)    // 环境配置
    .AddEnvironmentVariables(prefix: "APP_", level: 2) // 覆盖配置
    .Build();
```

### 6. 缓存配置值

#### 对于频繁访问的配置

```csharp
public class ConfigCache
{
    private readonly ICfgRoot _cfg;
    private string? _cachedConnectionString;
    private int _cachedTimeout;
    private bool _initialized;
    
    public ConfigCache(ICfgRoot cfg)
    {
        _cfg = cfg;
        RefreshCache();
        
        // 订阅变更，自动刷新缓存
        _cfg.ConfigChanges.Subscribe(_ => RefreshCache());
    }
    
    private void RefreshCache()
    {
        _cachedConnectionString = _cfg.Get("Database:ConnectionString");
        _cachedTimeout = _cfg.Get<int>("Database:Timeout");
        _initialized = true;
    }
    
    public string? ConnectionString => _cachedConnectionString;
    public int Timeout => _cachedTimeout;
}
```

### 7. 选择合适的运行时

| 运行时 | 相对性能 | 建议 |
|--------|----------|------|
| .NET 6.0 | 基准 | 生产可用 |
| .NET 8.0 | +35-45% | 推荐 |
| .NET 9.0 | +40-55% | 最佳性能 |

**建议**：优先使用 .NET 8.0 或 .NET 9.0 以获得最佳性能。

## 场景优化建议

### 高并发读取场景

```csharp
// 1. 使用批量操作减少锁竞争
cfg.GetMany(keys, (key, value) => { /* 处理 */ });

// 2. 考虑使用本地缓存
var cache = new ConcurrentDictionary<string, string?>();
cfg.ConfigChanges.Subscribe(_ => cache.Clear());

// 3. 使用源生成器避免反射开销
var options = AppConfig.BindFrom(cfg.GetSection("App"));
```

### 大量配置项场景

```csharp
// 1. 使用 JSON 模式而非 KV 模式（远程配置中心）
.AddConsul(options =>
{
    options.DataFormat = ConsulDataFormat.Json;
    options.SingleKey = "app-config";
})

// 2. 合理划分配置节
var dbSection = cfg.GetSection("Database");
var loggingSection = cfg.GetSection("Logging");
```

### 启动性能优化

```csharp
// 1. 延迟加载远程配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)  // 本地配置先加载
    .Build();

// 应用启动后异步加载远程配置
_ = Task.Run(async () =>
{
    await LoadRemoteConfigAsync();
});

// 2. 使用可选配置源
.AddJson("optional-config.json", level: 1, optional: true)
```

### 内存优化

```csharp
// 1. 使用回调方式批量获取（零堆分配）
cfg.GetMany(keys, (key, value) => { /* 处理 */ });

// 2. 及时释放不再使用的配置
await using var cfg = new CfgBuilder()
    .AddJson("config.json")
    .Build();

// 3. 避免频繁创建配置实例
// ❌ 每次请求创建新实例
// ✅ 使用单例或 Scoped 生命周期
```

## 性能监控

### 添加性能日志

```csharp
var sw = Stopwatch.StartNew();
var value = cfg.Get("SomeKey");
sw.Stop();

if (sw.ElapsedMilliseconds > 10)
{
    _logger.LogWarning("配置读取耗时过长: {Key} - {ElapsedMs}ms", "SomeKey", sw.ElapsedMilliseconds);
}
```

### 监控热重载频率

```csharp
var reloadCount = 0;
cfg.ConfigChanges.Subscribe(_ =>
{
    Interlocked.Increment(ref reloadCount);
    _logger.LogInformation("配置重载次数: {Count}", reloadCount);
});
```

## 性能测试

运行性能基准测试：

```bash
cd benchmarks/Apq.Cfg.Benchmarks
dotnet run -c Release
```

主要测试类：
- `GetSectionBenchmarks` - 配置节性能
- `BatchOperationBenchmarks` - 批量操作性能
- `ConcurrencyBenchmarks` - 并发性能
- `SourceGeneratorBenchmarks` - 源生成器性能
- `TypeConversionBenchmarks` - 类型转换性能

## 常见性能问题

### 问题 1：配置读取缓慢

**可能原因**：
- 配置源过多
- 未使用批量操作
- 热重载过于频繁

**解决方案**：
- 减少配置源数量
- 使用 `GetMany` 批量获取
- 增加防抖时间

### 问题 2：内存占用过高

**可能原因**：
- 大量配置项
- 频繁创建配置实例
- 未释放配置资源

**解决方案**：
- 使用 JSON 模式存储配置
- 使用单例模式
- 正确实现 `IDisposable`

### 问题 3：启动时间过长

**可能原因**：
- 远程配置中心连接慢
- 大量配置文件

**解决方案**：
- 本地配置优先加载
- 异步加载远程配置
- 使用可选配置源

## 下一步

- [最佳实践](/guide/best-practices) - 最佳实践指南
- [扩展开发](/guide/extension) - 自定义配置源
