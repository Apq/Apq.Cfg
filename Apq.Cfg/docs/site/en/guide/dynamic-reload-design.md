# Dynamic Reload Design

This document describes the implementation of hot reload in Apq.Cfg.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ChangeCoordinator                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ Debounce    │  │ Incremental │  │ Change Notification │  │
│  │ Processing  │  │ Update      │  │ (ConfigChanges)     │  │
│  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘  │
└─────────┼────────────────┼─────────────────────┼────────────┘
          │                │                     │
          ▼                ▼                     ▼
    File Change       Source Reload        Rx Observable
```

## Reload Flow

1. **File Watching**: FileSystemWatcher monitors config files
2. **Debouncing**: Multiple rapid changes merged (default 100ms)
3. **Incremental Update**: Only reload changed sources
4. **Diff Calculation**: Compare old and new configs
5. **Notify Subscribers**: Publish via `ConfigChanges`

## Debounce Implementation

```csharp
// Multiple changes within debounce window are combined
private readonly Subject<Unit> _reloadTrigger = new();

_reloadTrigger
    .Throttle(TimeSpan.FromMilliseconds(100))
    .Subscribe(_ => ProcessReload());
```

## Change Detection

```csharp
public class ConfigChangeEvent
{
    public DateTime Timestamp { get; }
    public IReadOnlyDictionary<string, ConfigChange> Changes { get; }
}

public class ConfigChange
{
    public ConfigChangeType Type { get; }  // Added, Modified, Removed
    public string? OldValue { get; }
    public string? NewValue { get; }
}
```

## Remote Source Reload

Remote sources (Consul, Nacos, etc.) use different mechanisms:

| Source | Mechanism |
|--------|-----------|
| Consul | Long polling / Watch |
| Nacos | Long polling |
| Apollo | Long polling |
| Etcd | Watch |
| Zookeeper | Watch |

## Thread Safety

- **Atomic replacement**: New config replaces old atomically
- **Immutable snapshots**: Readers see consistent state
- **Background processing**: Reload happens on background thread

## Configuration

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.DebounceMs = 100;  // Debounce window
        options.NotifyOnReload = true;  // Enable notifications
    })
    .Build();
```

## Next Steps

- [Dynamic Reload](/en/guide/dynamic-reload) - Usage guide
- [Architecture](/en/guide/architecture) - Overall architecture
