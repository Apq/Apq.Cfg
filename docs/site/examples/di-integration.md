# 依赖注入集成示例

本页展示如何将 Apq.Cfg 集成到 ASP.NET Core 依赖注入系统。

## 基本集成

### 注册配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 方式一：使用 AddApqCfg
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables());

var app = builder.Build();
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
    
    public string GetDatabaseHost()
    {
        return _cfg["Database:Host"];
    }
}
```

## IOptions 模式

### 定义配置类

```csharp
public class DatabaseOptions
{
    public const string SectionName = "Database";
    
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    
    public string ConnectionString => 
        $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
}

public class CacheOptions
{
    public const string SectionName = "Cache";
    
    public bool Enabled { get; set; } = true;
    public int MaxSize { get; set; } = 1000;
    public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(5);
}
```

### 注册选项

```csharp
var builder = WebApplication.CreateBuilder(args);

// 注册配置
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables());

// 绑定选项
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.SectionName));
```

### 使用 IOptions

```csharp
public class DatabaseService
{
    private readonly DatabaseOptions _options;
    
    public DatabaseService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
    
    public void Connect()
    {
        Console.WriteLine($"连接到: {_options.ConnectionString}");
    }
}
```

### 使用 IOptionsSnapshot

每次请求重新读取配置：

```csharp
public class CacheService
{
    private readonly IOptionsSnapshot<CacheOptions> _options;
    
    public CacheService(IOptionsSnapshot<CacheOptions> options)
    {
        _options = options;
    }
    
    public void DoWork()
    {
        // 每次访问获取最新配置
        if (_options.Value.Enabled)
        {
            Console.WriteLine($"缓存大小: {_options.Value.MaxSize}");
        }
    }
}
```

### 使用 IOptionsMonitor

支持配置变更通知：

```csharp
public class ConfigWatcherService : IHostedService, IDisposable
{
    private readonly IOptionsMonitor<DatabaseOptions> _options;
    private readonly IDisposable _changeListener;
    private readonly ILogger<ConfigWatcherService> _logger;
    
    public ConfigWatcherService(
        IOptionsMonitor<DatabaseOptions> options,
        ILogger<ConfigWatcherService> logger)
    {
        _options = options;
        _logger = logger;
        
        // 监听配置变更
        _changeListener = _options.OnChange(OnConfigChanged);
    }
    
    private void OnConfigChanged(DatabaseOptions newOptions)
    {
        _logger.LogInformation("数据库配置已更新: {Host}:{Port}", 
            newOptions.Host, newOptions.Port);
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("当前数据库: {Host}", _options.CurrentValue.Host);
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    public void Dispose() => _changeListener?.Dispose();
}
```

## 配置验证

### 数据注解验证

```csharp
using System.ComponentModel.DataAnnotations;

public class DatabaseOptions
{
    [Required(ErrorMessage = "数据库主机不能为空")]
    public string Host { get; set; } = "";
    
    [Range(1, 65535, ErrorMessage = "端口必须在 1-65535 之间")]
    public int Port { get; set; } = 5432;
    
    [Required]
    [MinLength(1, ErrorMessage = "数据库名不能为空")]
    public string Database { get; set; } = "";
}

// 注册时启用验证
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // 启动时验证
```

### 自定义验证

```csharp
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .Validate(options =>
    {
        // 自定义验证逻辑
        if (options.Host.Contains("localhost") && options.Port == 5432)
        {
            return true; // 开发环境
        }
        
        // 生产环境必须使用非默认端口
        return options.Port != 5432;
    }, "生产环境不能使用默认端口")
    .ValidateOnStart();
```

### 复杂验证

```csharp
public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    private readonly IHostEnvironment _environment;
    
    public DatabaseOptionsValidator(IHostEnvironment environment)
    {
        _environment = environment;
    }
    
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(options.Host))
            errors.Add("数据库主机不能为空");
        
        if (_environment.IsProduction())
        {
            if (options.Host.Contains("localhost"))
                errors.Add("生产环境不能使用 localhost");
            
            if (string.IsNullOrEmpty(options.Password))
                errors.Add("生产环境必须设置密码");
        }
        
        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}

// 注册验证器
builder.Services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
```

## 命名选项

```csharp
// appsettings.json
{
  "Databases": {
    "Primary": {
      "Host": "primary.db.local",
      "Port": 5432
    },
    "Readonly": {
      "Host": "readonly.db.local",
      "Port": 5432
    }
  }
}

// 注册命名选项
builder.Services.Configure<DatabaseOptions>("Primary",
    builder.Configuration.GetSection("Databases:Primary"));
builder.Services.Configure<DatabaseOptions>("Readonly",
    builder.Configuration.GetSection("Databases:Readonly"));

// 使用命名选项
public class MultiDatabaseService
{
    private readonly IOptionsSnapshot<DatabaseOptions> _options;
    
    public MultiDatabaseService(IOptionsSnapshot<DatabaseOptions> options)
    {
        _options = options;
    }
    
    public DatabaseOptions GetPrimaryDb() => _options.Get("Primary");
    public DatabaseOptions GetReadonlyDb() => _options.Get("Readonly");
}
```

## 完整示例

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 Apq.Cfg
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddConsul("http://consul:8500", "myapp/config", watch: true, optional: true)
    .AddEnvironmentVariables("MYAPP_"));

// 配置选项
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<CacheOptions>()
    .Bind(builder.Configuration.GetSection("Cache"));

// 注册服务
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddHostedService<ConfigWatcherService>();

var app = builder.Build();

app.MapGet("/config", (IOptions<DatabaseOptions> dbOptions, IOptions<CacheOptions> cacheOptions) =>
{
    return new
    {
        Database = new { dbOptions.Value.Host, dbOptions.Value.Port },
        Cache = new { cacheOptions.Value.Enabled, cacheOptions.Value.MaxSize }
    };
});

app.Run();
```

## 下一步

- [动态重载](/examples/dynamic-reload) - 配置热更新示例
- [复杂场景](/examples/complex-scenarios) - 企业级应用配置
