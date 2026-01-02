# Configuration Templates and Variable Substitution

Apq.Cfg supports variable references in configuration values, enabling dynamic composition and reuse of configurations.

## Basic Usage

### Referencing Other Configurations

```csharp
// config.json
{
    "App": {
        "Name": "MyApp",
        "LogPath": "${App:Name}/logs",
        "DataPath": "${App:Name}/data"
    }
}
```

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// Use GetResolved to get the resolved value
var logPath = cfg.GetResolved("App:LogPath");
// Returns: "MyApp/logs"

var dataPath = cfg.GetResolved("App:DataPath");
// Returns: "MyApp/data"
```

### Referencing Environment Variables

Use `${ENV:VariableName}` syntax to reference environment variables:

```csharp
// config.json
{
    "Paths": {
        "Home": "${ENV:USERPROFILE}",
        "Temp": "${ENV:TEMP}",
        "AppData": "${ENV:APPDATA}/MyApp"
    }
}
```

```csharp
var homePath = cfg.GetResolved("Paths:Home");
// Returns: "C:\Users\username"
```

### Referencing System Properties

Use `${SYS:PropertyName}` syntax to reference system properties:

```csharp
// config.json
{
    "System": {
        "Machine": "${SYS:MachineName}",
        "User": "${SYS:UserName}",
        "LogFile": "logs/${SYS:MachineName}_${SYS:Today}.log"
    }
}
```

```csharp
var logFile = cfg.GetResolved("System:LogFile");
// Returns: "logs/SERVER01_2026-01-02.log"
```

#### Supported System Properties

| Property | Description |
|----------|-------------|
| `MachineName` | Computer name |
| `UserName` | Current user name |
| `UserDomainName` | User domain name |
| `OSVersion` | Operating system version |
| `ProcessId` | Current process ID |
| `CurrentDirectory` | Current working directory |
| `SystemDirectory` | System directory |
| `ProcessorCount` | Number of processors |
| `Is64BitProcess` | Whether 64-bit process |
| `Is64BitOperatingSystem` | Whether 64-bit OS |
| `CLRVersion` | CLR version |
| `Now` | Current time (ISO 8601 format) |
| `UtcNow` | Current UTC time |
| `Today` | Current date (yyyy-MM-dd) |

## Nested References

Variables can be nested, supporting multi-level resolution:

```csharp
// config.json
{
    "App": {
        "Name": "MyApp",
        "Version": "1.0.0"
    },
    "Paths": {
        "Base": "${ENV:APPDATA}/${App:Name}",
        "Data": "${Paths:Base}/data/${App:Version}"
    }
}
```

```csharp
var dataPath = cfg.GetResolved("Paths:Data");
// Returns: "C:\Users\username\AppData\Roaming\MyApp/data/1.0.0"
```

## Resolving Template Strings

Besides getting configuration values, you can directly resolve any template string:

```csharp
var template = "Application ${App:Name} v${App:Version} running on ${SYS:MachineName}";
var result = cfg.ResolveVariables(template);
// Returns: "Application MyApp v1.0.0 running on SERVER01"
```

## Type Conversion

The `GetResolved<T>` method supports converting resolved values to specified types:

```csharp
// config.json
{
    "Settings": {
        "BasePort": "8080",
        "Port": "${Settings:BasePort}"
    }
}
```

```csharp
var port = cfg.GetResolved<int>("Settings:Port");
// Returns: 8080 (int)
```

## Batch Retrieval

```csharp
var keys = new[] { "App:LogPath", "App:DataPath", "Paths:Home" };
var values = cfg.GetManyResolved(keys);

foreach (var (key, value) in values)
{
    Console.WriteLine($"{key} = {value}");
}
```

## Custom Resolution Options

### Modify Variable Syntax

```csharp
var options = new VariableResolutionOptions
{
    VariablePrefix = "#{",    // Default "${"
    VariableSuffix = "}#",    // Default "}"
    PrefixSeparator = "."     // Default ":"
};
options.Resolvers.Add(VariableResolvers.Config);

// Use custom syntax: #{App.Name}#
var result = cfg.GetResolved("Key", options);
```

### Control Recursion Depth

```csharp
var options = new VariableResolutionOptions
{
    MaxRecursionDepth = 5  // Default 10
};
```

### Handling Unresolved Variables

```csharp
var options = new VariableResolutionOptions
{
    // Keep: Keep original expression (default)
    // Empty: Replace with empty string
    // Throw: Throw exception
    UnresolvedBehavior = UnresolvedVariableBehavior.Throw
};
```

## Configuration via CfgBuilder

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .ConfigureVariableResolution(options =>
    {
        options.MaxRecursionDepth = 5;
        options.UnresolvedBehavior = UnresolvedVariableBehavior.Empty;
    })
    .Build();
```

### Adding Custom Resolvers

```csharp
// Custom resolver
public class CustomResolver : IVariableResolver
{
    public string? Prefix => "CUSTOM";

    public string? Resolve(string variableName, ICfgRoot cfg)
    {
        return variableName switch
        {
            "Timestamp" => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            "Guid" => Guid.NewGuid().ToString(),
            _ => null
        };
    }
}

// Register custom resolver
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddVariableResolver(new CustomResolver())
    .Build();

// Usage: ${CUSTOM:Timestamp}
```

## Circular Reference Detection

The template engine automatically detects circular references and throws an exception:

```csharp
// config.json - circular reference
{
    "A": "${B}",
    "B": "${A}"
}
```

```csharp
// Throws InvalidOperationException: Circular reference detected: A
cfg.GetResolved("A");
```

## API Reference

### Extension Methods

| Method | Description |
|--------|-------------|
| `GetResolved(key)` | Get resolved configuration value |
| `GetResolved<T>(key)` | Get resolved value with type conversion |
| `GetResolved(key, options)` | Get resolved value with custom options |
| `TryGetResolved(key, out value)` | Try to get resolved value |
| `TryGetResolved<T>(key, out value)` | Try to get resolved value with type conversion |
| `GetManyResolved(keys)` | Batch get resolved values |
| `ResolveVariables(template)` | Resolve variables in template string |
| `ResolveVariables(template, options)` | Resolve template with custom options |

### VariableResolutionOptions Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `VariablePrefix` | `string` | `"${"` | Variable prefix |
| `VariableSuffix` | `string` | `"}"` | Variable suffix |
| `PrefixSeparator` | `string` | `":"` | Prefix separator |
| `MaxRecursionDepth` | `int` | `10` | Maximum recursion depth |
| `UnresolvedBehavior` | `enum` | `Keep` | How to handle unresolved variables |
| `CacheResults` | `bool` | `true` | Whether to cache results |
| `InvalidateCacheOnChange` | `bool` | `true` | Clear cache on config change |
| `Resolvers` | `IList` | Built-in resolvers | Variable resolver list |

### Built-in Resolvers

| Resolver | Prefix | Description |
|----------|--------|-------------|
| `VariableResolvers.Config` | None | Reference other config keys |
| `VariableResolvers.Environment` | `ENV` | Reference environment variables |
| `VariableResolvers.System` | `SYS` | Reference system properties |
