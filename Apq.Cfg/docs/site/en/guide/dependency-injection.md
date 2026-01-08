# Dependency Injection

Apq.Cfg integrates seamlessly with ASP.NET Core dependency injection.

## Basic Integration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Apq.Cfg
builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));

var app = builder.Build();
```

## Inject ICfgRoot

```csharp
public class MyService
{
    private readonly ICfgRoot _cfg;

    public MyService(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    public string GetAppName()
    {
        return _cfg["App:Name"] ?? "DefaultApp";
    }
}
```

## Options Pattern

### Configure Options

```csharp
public class DatabaseOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Name { get; set; } = "";
}

builder.Services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json", level: 0));

builder.Services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### Use Options

```csharp
public class DatabaseService
{
    private readonly DatabaseOptions _options;

    public DatabaseService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public string GetConnectionString()
    {
        return $"Host={_options.Host};Port={_options.Port};Database={_options.Name}";
    }
}
```

## IConfiguration Compatibility

```csharp
// Apq.Cfg also registers IConfiguration
public class LegacyService
{
    private readonly IConfiguration _config;

    public LegacyService(IConfiguration config)
    {
        _config = config;
    }
}
```

## Scoped Configuration

For request-scoped configuration:

```csharp
builder.Services.AddScoped<RequestConfig>(sp =>
{
    var cfg = sp.GetRequiredService<ICfgRoot>();
    return new RequestConfig
    {
        Timeout = cfg.GetValue<int>("Request:Timeout")
    };
});
```

## Next Steps

- [Best Practices](/en/guide/best-practices) - Configuration best practices
- [Dynamic Reload](/en/guide/dynamic-reload) - Hot reload with DI
