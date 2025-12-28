# 依赖注入

Apq.Cfg 完美集成 Microsoft.Extensions.DependencyInjection。

## 基本集成

### 注册配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 方式一：使用 AddApqCfg 扩展方法
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables());

// 方式二：手动注册
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.AddSingleton<ICfgRoot>(cfg);
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
    
    public string GetConnectionString()
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
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

// 注册配置
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));
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
    private readonly IDisposable _changeListener;
    
    public MyService(IOptionsMonitor<DatabaseOptions> options)
    {
        _options = options;
        
        // 监听配置变更
        _changeListener = _options.OnChange(newOptions =>
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

## 命名配置

支持同一类型的多个配置实例：

```csharp
// appsettings.json
{
  "Databases": {
    "Primary": { "Host": "primary.db.local" },
    "Readonly": { "Host": "readonly.db.local" }
  }
}

// 注册命名配置
builder.Services.Configure<DatabaseOptions>("Primary",
    builder.Configuration.GetSection("Databases:Primary"));
builder.Services.Configure<DatabaseOptions>("Readonly",
    builder.Configuration.GetSection("Databases:Readonly"));

// 使用命名配置
public class MyService
{
    private readonly IOptionsSnapshot<DatabaseOptions> _options;
    
    public MyService(IOptionsSnapshot<DatabaseOptions> options)
    {
        _options = options;
    }
    
    public DatabaseOptions GetPrimaryDb() => _options.Get("Primary");
    public DatabaseOptions GetReadonlyDb() => _options.Get("Readonly");
}
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
        if (string.IsNullOrEmpty(options.Host))
            return false;
        if (options.Port <= 0 || options.Port > 65535)
            return false;
        return true;
    }, "数据库配置无效");
```

## 配置后处理

```csharp
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .PostConfigure(options =>
    {
        // 设置默认值
        if (string.IsNullOrEmpty(options.Host))
            options.Host = "localhost";
        
        // 环境变量覆盖
        var envHost = Environment.GetEnvironmentVariable("DB_HOST");
        if (!string.IsNullOrEmpty(envHost))
            options.Host = envHost;
    });
```

## 下一步

- [编码处理](/guide/encoding) - 文件编码处理
- [性能优化](/guide/performance) - 性能调优
