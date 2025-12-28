# 动态重载

Apq.Cfg 支持配置的动态重载，无需重启应用即可更新配置。

## 启用动态重载

### 文件配置源

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
    .AddYamlFile("config.yaml", optional: true, reloadOnChange: true)
    .Build();
```

### 远程配置源

远程配置源通常内置了变更监听：

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config", watch: true)
    .AddRedis("localhost:6379", "config:myapp", subscribeChanges: true)
    .Build();
```

## 监听配置变更

### 使用 OnChange 回调

```csharp
cfg.OnChange((changedKeys) =>
{
    Console.WriteLine("配置已更新:");
    foreach (var key in changedKeys)
    {
        Console.WriteLine($"  {key} = {cfg[key]}");
    }
});
```

### 使用 ChangeToken

```csharp
ChangeToken.OnChange(
    () => cfg.GetReloadToken(),
    () =>
    {
        Console.WriteLine("配置已重新加载");
        // 重新读取配置
        var newValue = cfg["SomeKey"];
    });
```

## 重载策略

### 配置重载选项

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        // 防抖延迟（毫秒）
        options.DebounceDelay = 500;
        
        // 重载策略
        options.Strategy = ReloadStrategy.Immediate;
        
        // 错误处理
        options.OnError = (ex) =>
        {
            Console.WriteLine($"重载失败: {ex.Message}");
        };
    })
    .Build();
```

### 重载策略类型

| 策略 | 说明 |
|------|------|
| `Immediate` | 立即重载 |
| `Debounced` | 防抖重载，合并短时间内的多次变更 |
| `Scheduled` | 定时重载 |

## 配置变更事件

### ConfigChangeEvent

```csharp
cfg.ConfigChanged += (sender, e) =>
{
    foreach (var change in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {change.Key}: {change.OldValue} -> {change.NewValue}");
    }
};
```

### 变更类型

| 类型 | 说明 |
|------|------|
| `Added` | 新增配置项 |
| `Modified` | 修改配置项 |
| `Removed` | 删除配置项 |

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
    
    public MyService(IOptionsMonitor<DatabaseConfig> options)
    {
        _options = options;
        
        // 监听变更
        _options.OnChange(config =>
        {
            Console.WriteLine($"数据库配置已更新: {config.Host}");
        });
    }
}
```

## 最佳实践

### 1. 使用防抖

避免频繁重载：

```csharp
.ConfigureReload(options =>
{
    options.DebounceDelay = 1000; // 1秒防抖
})
```

### 2. 错误处理

配置重载失败时保留旧配置：

```csharp
.ConfigureReload(options =>
{
    options.OnError = (ex) =>
    {
        _logger.LogError(ex, "配置重载失败，保留当前配置");
    };
})
```

### 3. 验证新配置

重载前验证配置有效性：

```csharp
.ConfigureReload(options =>
{
    options.Validator = (newConfig) =>
    {
        // 返回 false 将拒绝此次重载
        return !string.IsNullOrEmpty(newConfig["Database:ConnectionString"]);
    };
})
```

## 下一步

- [动态重载设计](/guide/dynamic-reload-design) - 深入了解动态重载的设计与实现
- [依赖注入](/guide/dependency-injection) - DI 集成详解
- [性能优化](/guide/performance) - 性能调优指南
