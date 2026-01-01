# 动态重载示例

本页展示如何实现配置的动态重载（热更新）。

## 基本重载

### 文件变更重载

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();

// 监听变更
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("配置已更新:");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

### 远程配置重载

```csharp
var cfg = new CfgBuilder()
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, watch: true))
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("Consul 配置已更新");
});
```

## 变更事件处理

### 详细变更信息

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"配置变更时间: {e.Timestamp}");
    
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}");
        Console.WriteLine($"    旧值: {change.OldValue ?? "(null)"}");
        Console.WriteLine($"    新值: {change.NewValue ?? "(null)"}");
    }
});
```

### 按键过滤

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    // 只处理数据库配置变更
    var dbChanges = e.Changes.Where(c => c.Key.StartsWith("Database:"));
    
    if (dbChanges.Any())
    {
        Console.WriteLine("数据库配置已更新，需要重新连接");
        ReconnectDatabase();
    }
});
```

### 使用 Rx 操作符

```csharp
using System.Reactive.Linq;

// 防抖处理
cfg.ConfigChanges
    .Throttle(TimeSpan.FromSeconds(1))
    .Subscribe(e =>
    {
        Console.WriteLine("配置已更新（防抖后）");
    });

// 只关注特定键
cfg.ConfigChanges
    .Where(e => e.Changes.ContainsKey("App:Name"))
    .Subscribe(e =>
    {
        var change = e.Changes["App:Name"];
        Console.WriteLine($"应用名称已更改: {change.OldValue} -> {change.NewValue}");
    });
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

## 层级覆盖感知

动态重载会正确处理层级覆盖：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .AddJson("config.local.json", level: 1, reloadOnChange: true)
    .Build();

// 假设两个文件都有 "Timeout" 配置
// config.json: Timeout = 30
// config.local.json: Timeout = 60
// 最终值: 60 (level 1 覆盖 level 0)

cfg.ConfigChanges.Subscribe(e =>
{
    // 只有当最终合并值真正变化时才会触发
    // 如果 config.json 的 Timeout 从 30 改为 45，
    // 但 config.local.json 仍然是 60，
    // 则不会触发变更通知（因为最终值仍是 60）
});
```

## 可写配置的动态重载

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true, reloadOnChange: true)
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
await cfg.SaveAsync();

// 文件变更会触发重载，但由于是自己的修改，
// 系统会智能处理避免重复通知
```

## 完整示例

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置动态重载
builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .AddJson($"config.{builder.Environment.EnvironmentName}.json", level: 1, optional: true, reloadOnChange: true)
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, watch: true, optional: true))
    .AddEnvironmentVariables(level: 20, prefix: "APP_"));

// 配置选项
builder.Services.AddOptions<AppOptions>()
    .Bind(builder.Configuration.GetSection("App"));

// 注册配置监听服务
builder.Services.AddHostedService<ConfigWatcherService>();

var app = builder.Build();

// 获取配置实例并监听变更
var cfg = app.Services.GetRequiredService<ICfgRoot>();
cfg.ConfigChanges.Subscribe(e =>
{
    app.Logger.LogInformation("配置已更新: {Keys}",
        string.Join(", ", e.Changes.Keys));
});

app.Run();
```

## 配置监听服务示例

```csharp
public class ConfigWatcherService : IHostedService, IDisposable
{
    private readonly ICfgRoot _cfg;
    private readonly ILogger<ConfigWatcherService> _logger;
    private IDisposable? _subscription;
    
    public ConfigWatcherService(ICfgRoot cfg, ILogger<ConfigWatcherService> logger)
    {
        _cfg = cfg;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscription = _cfg.ConfigChanges.Subscribe(OnConfigChanged);
        _logger.LogInformation("配置监听服务已启动");
        return Task.CompletedTask;
    }
    
    private void OnConfigChanged(ConfigChangeEvent e)
    {
        _logger.LogInformation("检测到配置变更，共 {Count} 项", e.Changes.Count);
        
        foreach (var (key, change) in e.Changes)
        {
            _logger.LogInformation("  [{Type}] {Key}: {OldValue} -> {NewValue}",
                change.Type, key, change.OldValue ?? "(null)", change.NewValue ?? "(null)");
        }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        _logger.LogInformation("配置监听服务已停止");
        return Task.CompletedTask;
    }
    
    public void Dispose() => _subscription?.Dispose();
}
```

## 下一步

- [复杂场景](/examples/complex-scenarios) - 企业级应用配置
- [最佳实践](/guide/best-practices) - 最佳实践指南
