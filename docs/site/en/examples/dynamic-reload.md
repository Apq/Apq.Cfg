# Dynamic Reload Examples

Configuration hot reload examples.

## Basic Hot Reload

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"Configuration changed at {e.Timestamp}");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## Refresh Service Configuration

```csharp
public class ConfigurableService
{
    private readonly ICfgRoot _cfg;
    private ServiceConfig _config;

    public ConfigurableService(ICfgRoot cfg)
    {
        _cfg = cfg;
        _config = LoadConfig();

        _cfg.ConfigChanges.Subscribe(_ =>
        {
            _config = LoadConfig();
            Console.WriteLine("Configuration refreshed");
        });
    }

    private ServiceConfig LoadConfig()
    {
        return new ServiceConfig
        {
            Timeout = _cfg.Get<int>("Service:Timeout"),
            RetryCount = _cfg.Get<int>("Service:RetryCount")
        };
    }
}
```

## Remote Source Reload

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
    }, level: 10, reloadOnChange: true)
    .Build();
```

## Next Steps

- [Complex Scenarios](/en/examples/complex-scenarios) - Enterprise patterns
- [Dynamic Reload Guide](/en/guide/dynamic-reload) - Detailed guide
