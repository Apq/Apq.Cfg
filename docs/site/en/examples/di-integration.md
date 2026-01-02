# DI Integration Examples

ASP.NET Core dependency injection integration.

## Basic Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json")                        // Uses default level 0
    .AddEnvironmentVariables(prefix: "APP_"));    // Uses default level 400

var app = builder.Build();
```

## Inject ICfgRoot

```csharp
public class MyController : ControllerBase
{
    private readonly ICfgRoot _cfg;

    public MyController(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    [HttpGet]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            AppName = _cfg["App:Name"],
            Port = _cfg.Get<int>("App:Port")
        });
    }
}
```

## Options Pattern

```csharp
public class DatabaseOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
}

// Registration
builder.Services.ConfigureApqCfg<DatabaseOptions>("Database");

// Usage
public class DatabaseService
{
    private readonly DatabaseOptions _options;

    public DatabaseService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
}
```

## Next Steps

- [Dynamic Reload](/en/examples/dynamic-reload) - Hot reload with DI
- [Complex Scenarios](/en/examples/complex-scenarios) - Enterprise patterns
