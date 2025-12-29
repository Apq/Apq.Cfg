# Config Merge

Apq.Cfg supports merging multiple configuration sources with level-based priority.

## How It Works

Configuration sources are merged by their `level` value. Higher levels override lower levels.

```
Level 20: Environment Variables  ─┐
Level 15: Vault                   │
Level 10: Consul                  ├─► Merged Configuration
Level 1:  config.local.json       │
Level 0:  config.json            ─┘
```

## Example

### Configuration Files

**config.json** (Level 0):
```json
{
    "App": {
        "Name": "MyApp",
        "Port": 8080,
        "Debug": false
    }
}
```

**config.local.json** (Level 1):
```json
{
    "App": {
        "Debug": true
    }
}
```

### Code

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: false, optional: true)
    .Build();

// Results:
// App:Name = "MyApp"      (from level 0)
// App:Port = 8080         (from level 0)
// App:Debug = true        (from level 1, overrides level 0)
```

## Merge Rules

1. **Higher level wins**: Values from higher levels override lower levels
2. **Key-by-key merge**: Only matching keys are overridden
3. **Null handling**: Setting a value to `null` removes it from the merged result
4. **Array handling**: Arrays are replaced entirely, not merged

## Write Target Selection

When writing values, you can specify the target level:

```csharp
// Write to default (highest writable level)
cfg.Set("App:Name", "NewName");

// Write to specific level
cfg.Set("App:Name", "NewName", targetLevel: 0);
```

## Best Practices

1. **Use level 0 for defaults**: Base configuration that rarely changes
2. **Use level 1-9 for environment-specific**: Development, staging, production
3. **Use level 10+ for remote**: Dynamic configuration from config centers
4. **Use highest levels for overrides**: Environment variables, command-line args

## Next Steps

- [Dynamic Reload](/en/guide/dynamic-reload) - Hot reload configuration
- [Source Selection](/en/guide/source-selection) - Choose the right sources
