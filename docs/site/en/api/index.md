# API Overview

This section provides the complete API reference for Apq.Cfg.

## Core Interfaces

| Interface | Description |
|-----------|-------------|
| [ICfgRoot](/en/api/icfg-root) | Configuration root interface, main entry point |
| [ICfgSection](/en/api/icfg-section) | Configuration section interface |

## Core Classes

| Class | Description |
|-------|-------------|
| [CfgBuilder](/en/api/cfg-builder) | Fluent API builder for creating configuration |

## Extension Methods

| Category | Description |
|----------|-------------|
| [Extensions](/en/api/extensions) | Extension methods for various configuration sources |

## Quick Reference

### Creating Configuration

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### Reading Values

```csharp
// String value
string? value = cfg.Get("App:Name");

// Typed value
int port = cfg.GetValue<int>("App:Port");

// Check existence
bool exists = cfg.Exists("App:Name");

// Get section
ICfgSection section = cfg.GetSection("Database");
```

### Writing Values

```csharp
// Set value
cfg.Set("App:Name", "NewName");

// Remove value
cfg.Remove("App:TempKey");

// Save changes
await cfg.SaveAsync();
```

### Batch Operations

```csharp
// Read multiple
var values = cfg.GetMany(new[] { "Key1", "Key2", "Key3" });

// Write multiple
cfg.SetMany(new Dictionary<string, string?>
{
    ["Key1"] = "Value1",
    ["Key2"] = "Value2"
});
```

### Configuration Changes

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}");
    }
});
```

## Next Steps

- [CfgBuilder](/en/api/cfg-builder) - Builder API reference
- [ICfgRoot](/en/api/icfg-root) - Root interface reference
- [ICfgSection](/en/api/icfg-section) - Section interface reference
