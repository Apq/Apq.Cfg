# 动态重载

Apq.Cfg 支持配置的动态重载，无需重启应用即可更新配置。

## 启用动态重载

### 文件配置源

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false, optional: false, reloadOnChange: true)
    .Build();
```

### 远程配置源

远程配置源通常内置了变更监听：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddSource(new ConsulCfgSource(options => {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "myapp/config";
        options.EnableHotReload = true;
    }, level: 10))
    .Build();
```

## 监听配置变更

### 使用 ConfigChanges 可观察序列

```csharp
cfg.ConfigChanges.Subscribe(changeEvent =>
{
    Console.WriteLine($"配置变更批次: {changeEvent.BatchId}");
    Console.WriteLine($"变更时间: {changeEvent.Timestamp}");
    
    foreach (var (key, change) in changeEvent.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

### 变更类型

| 类型 | 说明 |
|------|------|
| `Added` | 新增配置项 |
| `Modified` | 修改配置项 |
| `Removed` | 删除配置项 |

## 转换为 Microsoft Configuration

### 静态快照

```csharp
// 创建静态快照，不支持动态重载
IConfigurationRoot msConfig = cfg.ToMicrosoftConfiguration();
```

### 支持动态重载

```csharp
// 创建支持动态重载的配置
IConfigurationRoot msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    EnableDynamicReload = true,
    DebounceMs = 100,
    Strategy = ReloadStrategy.Eager
});
```

### DynamicReloadOptions 选项

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `EnableDynamicReload` | `bool` | `true` | 是否启用动态重载 |
| `DebounceMs` | `int` | `100` | 防抖时间窗口（毫秒） |
| `Strategy` | `ReloadStrategy` | `Eager` | 重载策略 |
| `KeyPrefixFilters` | `IReadOnlyList<string>?` | `null` | 键前缀过滤器 |
| `RollbackOnError` | `bool` | `true` | 重载失败时是否回滚 |
| `HistorySize` | `int` | `0` | 保留的变更历史记录数量 |

### 重载策略

| 策略 | 说明 |
|------|------|
| `Eager` | 立即重载（默认） |
| `Lazy` | 延迟重载，下次访问时生效 |
| `Scheduled` | 定时重载 |

## 与依赖注入集成

### IOptionsSnapshot

每次请求获取最新配置：

```csharp
public class MyService
{
    private readonly IOptionsSnapshot<DatabaseConfig> _options;
    
    public MyService(IOptionsSnapshot<DatabaseConfig> options)
    {
        _options = options;
    }
    
    public void DoWork()
    {
        // 每次访问都获取最新配置
        var config = _options.Value;
    }
}
```

### IOptionsMonitor

监听配置变更：

```csharp
public class MyService
{
    private readonly IOptionsMonitor<DatabaseConfig> _options;
    private readonly IDisposable? _changeListener;
    
    public MyService(IOptionsMonitor<DatabaseConfig> options)
    {
        _options = options;
        
        // 监听变更
        _changeListener = _options.OnChange((config, name) =>
        {
            Console.WriteLine($"数据库配置已更新: {config.Host}");
        });
    }
    
    public void Dispose()
    {
        _changeListener?.Dispose();
    }
}
```

### 注册带变更回调的配置

```csharp
services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0, writeable: false, reloadOnChange: true));

services.ConfigureApqCfg<DatabaseConfig>("Database", config =>
{
    Console.WriteLine($"数据库配置已更新: {config.Host}");
    // 执行必要的重新连接逻辑
});
```

## 最佳实践

### 1. 使用防抖

避免频繁重载：

```csharp
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 500 // 500毫秒防抖
});
```

### 2. 按需启用热重载

只对需要动态更新的配置启用热重载：

```csharp
var cfg = new CfgBuilder()
    // 基础配置不需要热重载
    .AddJsonFile("config.json", level: 0, writeable: false, reloadOnChange: false)
    // 只对需要动态更新的配置启用热重载
    .AddJsonFile("dynamic-config.json", level: 1, writeable: true, reloadOnChange: true)
    .Build();
```

### 3. 错误处理

配置重载失败时保留旧配置：

```csharp
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    RollbackOnError = true // 重载失败时回滚到之前的配置
});
```

### 4. 键前缀过滤

只监听特定前缀的配置变更：

```csharp
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    KeyPrefixFilters = new[] { "Database:", "Cache:" }
});
```

## 下一步

- [动态重载设计](/guide/dynamic-reload-design) - 深入了解动态重载的设计与实现
- [依赖注入](/guide/dependency-injection) - DI 集成详解
- [性能优化](/guide/performance) - 性能调优指南
