# 动态重载示例

本页展示如何实现配置的动态重载（热更新）。

## 基本重载

### 文件变更重载

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
    .Build();

// 监听变更
cfg.OnChange(changedKeys =>
{
    Console.WriteLine("配置已更新:");
    foreach (var key in changedKeys)
    {
        Console.WriteLine($"  {key} = {cfg[key]}");
    }
});
```

### 远程配置重载

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://consul:8500", "myapp/config", watch: true)
    .Build();

cfg.OnChange(changedKeys =>
{
    Console.WriteLine("Consul 配置已更新");
});
```

## 重载策略

### 防抖重载

避免频繁重载：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.Strategy = ReloadStrategy.Debounced;
        options.DebounceDelay = 1000; // 1秒内的变更合并
    })
    .Build();
```

### 节流重载

限制重载频率：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.Strategy = ReloadStrategy.Throttled;
        options.ThrottleInterval = TimeSpan.FromSeconds(5); // 最多每5秒重载一次
    })
    .Build();
```

### 立即重载

每次变更立即重载：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.Strategy = ReloadStrategy.Immediate;
    })
    .Build();
```

## 变更事件处理

### 详细变更信息

```csharp
cfg.ConfigChanged += (sender, e) =>
{
    Console.WriteLine($"配置变更时间: {e.Timestamp}");
    Console.WriteLine($"变更来源: {e.Source}");
    
    foreach (var change in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {change.Key}");
        Console.WriteLine($"    旧值: {change.OldValue}");
        Console.WriteLine($"    新值: {change.NewValue}");
    }
};
```

### 按键过滤

```csharp
cfg.ConfigChanged += (sender, e) =>
{
    // 只处理数据库配置变更
    var dbChanges = e.Changes.Where(c => c.Key.StartsWith("Database:"));
    
    if (dbChanges.Any())
    {
        Console.WriteLine("数据库配置已更新，需要重新连接");
        ReconnectDatabase();
    }
};
```

## 与 IOptionsMonitor 集成

### 自动更新

```csharp
public class ConfigurableService : IDisposable
{
    private readonly IOptionsMonitor<ServiceOptions> _options;
    private readonly IDisposable _changeListener;
    private ServiceOptions _currentOptions;
    
    public ConfigurableService(IOptionsMonitor<ServiceOptions> options)
    {
        _options = options;
        _currentOptions = options.CurrentValue;
        
        _changeListener = _options.OnChange(OnOptionsChanged);
    }
    
    private void OnOptionsChanged(ServiceOptions newOptions)
    {
        Console.WriteLine("服务配置已更新");
        
        // 比较变更
        if (newOptions.Timeout != _currentOptions.Timeout)
        {
            Console.WriteLine($"超时时间: {_currentOptions.Timeout} -> {newOptions.Timeout}");
        }
        
        _currentOptions = newOptions;
        
        // 应用新配置
        ApplyConfiguration(newOptions);
    }
    
    private void ApplyConfiguration(ServiceOptions options)
    {
        // 应用配置变更
    }
    
    public void Dispose() => _changeListener?.Dispose();
}
```

### 后台服务

```csharp
public class DynamicConfigService : BackgroundService
{
    private readonly IOptionsMonitor<WorkerOptions> _options;
    private readonly ILogger<DynamicConfigService> _logger;
    
    public DynamicConfigService(
        IOptionsMonitor<WorkerOptions> options,
        ILogger<DynamicConfigService> logger)
    {
        _options = options;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var options = _options.CurrentValue;
            
            _logger.LogInformation("执行任务，间隔: {Interval}ms", options.Interval);
            
            // 执行工作...
            
            await Task.Delay(options.Interval, stoppingToken);
        }
    }
}
```

## 重载错误处理

### 错误回调

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.OnReloadError = (source, exception) =>
        {
            Console.WriteLine($"配置重载失败: {source}");
            Console.WriteLine($"错误: {exception.Message}");
            
            // 可以选择：
            // 1. 记录日志
            // 2. 发送告警
            // 3. 使用缓存的旧配置
        };
    })
    .Build();
```

### 重试策略

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://consul:8500", "myapp/config", watch: true)
    .ConfigureReload(options =>
    {
        options.RetryCount = 3;
        options.RetryDelay = TimeSpan.FromSeconds(5);
        options.OnRetry = (attempt, exception) =>
        {
            Console.WriteLine($"重试 {attempt}/3: {exception.Message}");
        };
    })
    .Build();
```

## 手动重载

### 触发重载

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .Build();

// 手动触发重载
await cfg.ReloadAsync();
```

### 定时重载

```csharp
public class ConfigReloadService : BackgroundService
{
    private readonly ICfgRoot _cfg;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    
    public ConfigReloadService(ICfgRoot cfg)
    {
        _cfg = cfg;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            
            try
            {
                await _cfg.ReloadAsync();
                Console.WriteLine("配置已定时重载");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"定时重载失败: {ex.Message}");
            }
        }
    }
}
```

## 完整示例

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置动态重载
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json", reloadOnChange: true)
    .AddConsul("http://consul:8500", "myapp/config", watch: true, optional: true)
    .ConfigureReload(options =>
    {
        options.Strategy = ReloadStrategy.Debounced;
        options.DebounceDelay = 1000;
        options.OnReloadError = (source, ex) =>
        {
            Console.WriteLine($"重载失败 [{source}]: {ex.Message}");
        };
    }));

// 配置选项
builder.Services.AddOptions<AppOptions>()
    .Bind(builder.Configuration.GetSection("App"));

// 注册配置监听服务
builder.Services.AddHostedService<ConfigWatcherService>();

var app = builder.Build();

// 获取配置实例并监听变更
var cfg = app.Services.GetRequiredService<ICfgRoot>();
cfg.ConfigChanged += (sender, e) =>
{
    app.Logger.LogInformation("配置已更新: {Keys}", 
        string.Join(", ", e.Changes.Select(c => c.Key)));
};

app.Run();
```

## 下一步

- [复杂场景](/examples/complex-scenarios) - 企业级应用配置
- [最佳实践](/guide/best-practices) - 最佳实践指南
