# Dynamic Reload

Apq.Cfg supports hot reloading configuration without restarting your application.

## Enable Hot Reload

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();
```

## Subscribe to Changes

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"Configuration changed at {e.Timestamp}");

    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}");
        Console.WriteLine($"    Old: {change.OldValue}");
        Console.WriteLine($"    New: {change.NewValue}");
    }
});
```

## Change Types

| Type | Description |
|------|-------------|
| `Added` | New key added |
| `Modified` | Existing key value changed |
| `Removed` | Key removed |

## Debouncing

Multiple rapid changes are debounced into a single event (default: 100ms).

```csharp
// Changes within 100ms are combined
// File saved 3 times quickly → 1 change event
```

## Remote Source Reload

Remote sources like Consul and Nacos support automatic reload:

```csharp
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
    }, level: 10, writeable: true, reloadOnChange: true)
    .Build();
```

## Thread Safety

- Configuration reads are always thread-safe
- Change events are delivered on a background thread
- Use appropriate synchronization in your handlers

## Best Practices

1. **Keep handlers fast**: Don't block in change handlers
2. **Handle exceptions**: Wrap handler code in try-catch
3. **Consider caching**: Cache derived values and invalidate on change
4. **Test reload behavior**: Verify your app handles changes correctly

## Example: Refresh Service Configuration

```csharp
public class MyService
{
    private readonly ICfgRoot _cfg;
    private ServiceConfig _config;

    public MyService(ICfgRoot cfg)
    {
        _cfg = cfg;
        _config = LoadConfig();

        _cfg.ConfigChanges.Subscribe(_ =>
        {
            _config = LoadConfig();
            Console.WriteLine("Service configuration refreshed");
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

## Next Steps

- [Dynamic Reload Design](/en/guide/dynamic-reload-design) - Implementation details
- [Best Practices](/en/guide/best-practices) - Configuration best practices
