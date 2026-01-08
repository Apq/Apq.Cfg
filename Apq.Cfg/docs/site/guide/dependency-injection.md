# 依赖注入

Apq.Cfg 完美集成 Microsoft.Extensions.DependencyInjection。

## 基本集成

### 注册配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 方式一：使用 AddApqCfg 扩展方法
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));

// 方式二：使用工厂方法（可访问其他服务）
builder.Services.AddApqCfg(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new CfgBuilder()
        .AddJsonFile("config.json", level: 0)
        .AddJsonFile($"config.{env.EnvironmentName}.json", level: 1, optional: true)
        .AddEnvironmentVariables(level: 2, prefix: "APP_")
        .Build();
});
```

### 注入使用

```csharp
public class MyService
{
    private readonly ICfgRoot _cfg;

    public MyService(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    public string? GetConnectionString()
    {
        return _cfg["Database:ConnectionString"];
    }
}
```

## IOptions 模式

### 配置绑定

```csharp
// 定义配置类
public class DatabaseOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "";
}

// 注册配置并绑定
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0));

builder.Services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### 一步完成注册和绑定

```csharp
builder.Services.AddApqCfg<DatabaseOptions>(
    cfg => cfg.AddJsonFile("config.json", level: 0),
    "Database");
```

### IOptions&lt;T&gt;

单例配置，应用启动时读取一次：

```csharp
public class MyService
{
    private readonly DatabaseOptions _options;
    
    public MyService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
}
```

### IOptionsSnapshot&lt;T&gt;

作用域配置，每次请求重新读取：

```csharp
public class MyService
{
    private readonly IOptionsSnapshot<DatabaseOptions> _options;
    
    public MyService(IOptionsSnapshot<DatabaseOptions> options)
    {
        _options = options;
    }
    
    public void DoWork()
    {
        // 每次访问获取最新值
        var host = _options.Value.Host;
    }
}
```

### IOptionsMonitor&lt;T&gt;

支持变更通知的配置：

```csharp
public class MyService : IDisposable
{
    private readonly IOptionsMonitor<DatabaseOptions> _options;
    private readonly IDisposable? _changeListener;
    
    public MyService(IOptionsMonitor<DatabaseOptions> options)
    {
        _options = options;
        
        // 监听配置变更
        _changeListener = _options.OnChange((newOptions, name) =>
        {
            Console.WriteLine($"数据库主机已更改为: {newOptions.Host}");
        });
    }
    
    public DatabaseOptions CurrentOptions => _options.CurrentValue;
    
    public void Dispose()
    {
        _changeListener?.Dispose();
    }
}
```

## 配置变更回调

注册配置时同时添加变更回调：

```csharp
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0, writeable: false, reloadOnChange: true));

builder.Services.ConfigureApqCfg<DatabaseOptions>("Database", options =>
{
    Console.WriteLine($"数据库配置已更新: {options.Host}:{options.Port}");
    // 执行必要的重新连接逻辑
});
```

## 嵌套对象绑定

支持嵌套对象和集合的自动绑定：

```csharp
public class DatabaseOptions
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
    public RetryOptions Retry { get; set; } = new();
}

public class RetryOptions
{
    public int Count { get; set; } = 3;
    public int Delay { get; set; } = 1000;
}

// 配置 JSON
// {
//   "Database": {
//     "ConnectionString": "...",
//     "Timeout": 60,
//     "Retry": {
//       "Count": 5,
//       "Delay": 2000
//     }
//   }
// }

builder.Services.ConfigureApqCfg<DatabaseOptions>("Database");

// 使用
var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
var retryCount = dbOptions.Retry.Count; // 5
```

## 配置验证

### 数据注解验证

```csharp
public class DatabaseOptions
{
    [Required]
    public string Host { get; set; } = "";
    
    [Range(1, 65535)]
    public int Port { get; set; } = 5432;
    
    [Required]
    [MinLength(1)]
    public string Database { get; set; } = "";
}

// 启用验证
builder.Services.AddOptions<DatabaseOptions>()
    .Configure<ICfgRoot>((options, cfg) =>
    {
        var section = cfg.GetSection("Database");
        // 手动绑定或使用 ObjectBinder
    })
    .ValidateDataAnnotations()
    .ValidateOnStart(); // 启动时验证
```

## 与 Microsoft Configuration 集成

Apq.Cfg 可以转换为 `IConfigurationRoot`，与现有代码无缝集成：

```csharp
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0, writeable: false));

// AddApqCfg 会自动注册 IConfigurationRoot
// 可以直接使用 Microsoft.Extensions.Configuration 的 API
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));
```

## 生命周期

| 接口 | 生命周期 | 说明 |
|------|----------|------|
| `ICfgRoot` | Singleton | 配置根，整个应用共享 |
| `IConfigurationRoot` | Singleton | Microsoft Configuration 根 |
| `IOptions<T>` | Singleton | 配置快照，启动时读取 |
| `IOptionsSnapshot<T>` | Scoped | 每次请求重新绑定 |
| `IOptionsMonitor<T>` | Singleton | 支持变更通知 |

## 下一步

- [编码处理](/guide/encoding) - 文件编码处理
- [性能优化](/guide/performance) - 性能调优
