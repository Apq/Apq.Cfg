# Config Merge

Apq.Cfg supports merging multiple configuration sources with level-based priority.

## How It Works

Configuration sources are merged by their `level` value. Higher levels override lower levels.

Each configuration source has a default level. If not specified, the default level is used:

| Source Type | Default Level |
|-------------|---------------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

```
Level 400: Environment Variables  ─┐
Level 300: Vault                   │
Level 200: Consul                  ├─► Merged Configuration
Level 50:  config.local.json       │
Level 0:   config.json            ─┘
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

**config.local.json** (Level 50):
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
    .AddJson("config.json")                              // Uses default level 0
    .AddJson("config.local.json", level: 50, optional: true)
    .Build();

// Results:
// App:Name = "MyApp"      (from level 0)
// App:Port = 8080         (from level 0)
// App:Debug = true        (from level 50, overrides level 0)
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
cfg["App:Name"] = "NewName";

// Write to specific level
cfg.Set("App:Name", "NewName", targetLevel: 0);
```

## Best Practices

### Recommended Level Design

| Level Range | Purpose | Example |
|-------------|---------|---------|
| 0 | Base configuration | config.json |
| 10-50 | Environment-specific | config.Production.json |
| 50-99 | Local overrides | config.local.json |
| 100 | Remote storage | Redis, Database |
| 200 | Config centers | Consul, Nacos, Apollo |
| 300 | Secret management | Vault |
| 400 | Environment variables | Highest priority overrides |

> Level intervals of 100 allow flexibility for custom levels in between.

### Typical Scenarios

#### Environment-Specific Configuration

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJson("config.json")                                    // Uses default level 0
    .AddJson($"config.{environment}.json", level: 10, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")                   // Uses default level 400
    .Build();
```

#### Local Development Configuration

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")                                                           // Uses default level 0
    .AddJson("config.Development.json", level: 10, optional: true)
    .AddJson("config.local.json", level: 50, writeable: true, optional: true, isPrimaryWriter: true)  // gitignore
    .Build();
```

#### With Remote Config Center

```csharp
var cfg = new CfgBuilder()
    // Base configuration (uses default level 0)
    .AddJson("config.json")
    .AddJson($"config.{env}.json", level: 10, optional: true)

    // Local override
    .AddJson("config.local.json", level: 50, writeable: true, optional: true, isPrimaryWriter: true)

    // Remote configuration (uses default level 200)
    .AddConsul(options => { ... })

    // Environment variables (uses default level 400)
    .AddEnvironmentVariables(prefix: "APP_")

    .Build();
```

## Next Steps

- [Dynamic Reload](/en/guide/dynamic-reload) - Hot reload configuration
- [Source Selection](/en/guide/source-selection) - Choose the right sources
