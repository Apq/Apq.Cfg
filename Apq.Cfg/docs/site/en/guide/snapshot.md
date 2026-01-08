# Configuration Snapshot Export

Apq.Cfg provides configuration snapshot export functionality, supporting exporting the current configuration state to multiple formats for debugging, backup, and migration purposes.

## Basic Usage

### Export to JSON

```csharp
using Apq.Cfg;
using Apq.Cfg.Snapshot;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .Build();

// Export to JSON (default format)
var json = cfg.ExportSnapshot();

// Export to formatted JSON
var jsonIndented = cfg.ExportSnapshotAsJson(indented: true);
```

### Export to Environment Variable Format

```csharp
// Export to environment variable format
var env = cfg.ExportSnapshotAsEnv();
// Output: APP__NAME=MyApp
//         DATABASE__HOST=localhost

// Add prefix
var envWithPrefix = cfg.ExportSnapshotAsEnv(prefix: "MYAPP_");
// Output: MYAPP_APP__NAME=MyApp
//         MYAPP_DATABASE__HOST=localhost
```

### Export to Dictionary

```csharp
// Export to flattened dictionary
var dict = cfg.ExportSnapshotAsDictionary();

foreach (var (key, value) in dict)
{
    Console.WriteLine($"{key} = {value}");
}
// Output: App:Name = MyApp
//         Database:Host = localhost
```

## Export Options

Use the `ExportOptions` class to customize export behavior:

```csharp
var options = new ExportOptions
{
    // Whether to indent (JSON only)
    Indented = true,

    // Whether to include metadata (export time, key count, etc.)
    IncludeMetadata = true,

    // Whether to mask sensitive values
    MaskSensitiveValues = true,

    // Only include specified keys (supports wildcard *)
    IncludeKeys = new[] { "App:*", "Database:Host" },

    // Exclude specified keys (supports wildcard *)
    ExcludeKeys = new[] { "Secrets:*", "Database:Password" },

    // Key prefix for environment variable format
    EnvPrefix = "MYAPP_"
};

// Use built-in exporters
var json = cfg.ExportSnapshot(SnapshotExporters.Json, options);
var env = cfg.ExportSnapshot(SnapshotExporters.Env, options);
```

### Using Builder Pattern

```csharp
var json = cfg.ExportSnapshot(options =>
{
    options.IncludeMetadata = true;
    options.ExcludeKeys = new[] { "Secrets:*" };
});
```

## Export Formats

### JSON Format

Exports to nested JSON structure:

```json
{
  "App": {
    "Name": "MyApp",
    "Version": "1.0.0"
  },
  "Database": {
    "Host": "localhost",
    "Port": "5432"
  }
}
```

With metadata enabled:

```json
{
  "App": {
    "Name": "MyApp"
  },
  "__metadata": {
    "exportedAt": "2026-01-02T10:30:00.0000000Z",
    "format": "Apq.Cfg.Snapshot",
    "version": "1.0",
    "keyCount": 5
  }
}
```

### KeyValue Format

Exports to flat key-value pair format:

```
App:Name=MyApp
App:Version=1.0.0
Database:Host=localhost
Database:Port=5432
```

### Env Format

Exports to environment variable format (keys converted to uppercase, colons replaced with double underscores):

```bash
APP__NAME=MyApp
APP__VERSION=1.0.0
DATABASE__HOST=localhost
DATABASE__PORT=5432
```

## Export to File

### Async Export to File

```csharp
// Export to file
await cfg.ExportSnapshotToFileAsync("config-snapshot.json");

// With custom options
await cfg.ExportSnapshotToFileAsync("config-snapshot.json", new ExportOptions
{
    Indented = true,
    MaskSensitiveValues = true
});
```

### Export to Stream

```csharp
using var stream = new MemoryStream();
await cfg.ExportSnapshotAsync(stream, new ExportOptions
{
    MaskSensitiveValues = false
});
```

## Filtering Configuration

### Include Specific Keys

```csharp
// Only export App and Database:Host
var filtered = cfg.ExportSnapshot(new ExportOptions
{
    IncludeKeys = new[] { "App:*", "Database:Host" }
});
```

### Exclude Specific Keys

```csharp
// Exclude sensitive configuration
var safe = cfg.ExportSnapshot(new ExportOptions
{
    ExcludeKeys = new[] { "Secrets:*", "Database:Password", "Api:Key" }
});
```

### Wildcard Support

- `*` matches any characters
- `?` matches a single character

```csharp
// Match all keys starting with Database
IncludeKeys = new[] { "Database:*" }

// Match App:Name and App:Version
IncludeKeys = new[] { "App:Name", "App:Version" }

// Exclude all passwords and keys
ExcludeKeys = new[] { "*:Password", "*:Secret", "*:Key" }
```

## Sensitive Value Masking

By default, export masks sensitive values:

```csharp
// Enable masking (default)
var masked = cfg.ExportSnapshot(new ExportOptions
{
    MaskSensitiveValues = true
});
// Output: Database:Password = sec***123

// Disable masking (for debugging only)
var unmasked = cfg.ExportSnapshot(new ExportOptions
{
    MaskSensitiveValues = false
});
// Output: Database:Password = secret123
```

## Static Methods

You can also use the `ConfigExporter` static class directly:

```csharp
using Apq.Cfg.Snapshot;

// Export to string
var json = ConfigExporter.Export(cfg);

// Export to dictionary
var dict = ConfigExporter.ExportToDictionary(cfg);

// Export to file
await ConfigExporter.ExportToFileAsync(cfg, "snapshot.json");

// Export to stream
await ConfigExporter.ExportAsync(cfg, stream);
```

## Custom Exporters

Use the `SnapshotExporter` delegate to create custom export formats:

```csharp
using Apq.Cfg.Snapshot;

// Use lambda expression to create custom exporter
var yaml = cfg.ExportSnapshot((data, ctx) =>
{
    var sb = new StringBuilder();

    if (ctx.IncludeMetadata)
    {
        sb.AppendLine($"# Exported at: {ctx.ExportedAt:O}");
        sb.AppendLine($"# Key count: {ctx.KeyCount}");
        sb.AppendLine();
    }

    foreach (var (key, value) in data.OrderBy(x => x.Key))
    {
        // Simple YAML format
        sb.AppendLine($"{key}: \"{value}\"");
    }

    return sb.ToString();
});

// Export to file
await cfg.ExportSnapshotToFileAsync((data, ctx) =>
{
    // Custom format logic
    return string.Join("\n", data.Select(x => $"{x.Key}={x.Value}"));
}, "config.txt");
```

### ExportContext Properties

Custom exporters can use `ExportContext` to get export context information:

| Property | Type | Description |
|----------|------|-------------|
| `ExportedAt` | `DateTime` | Export time (UTC) |
| `IncludeMetadata` | `bool` | Whether to include metadata |
| `Indented` | `bool` | Whether to indent format |
| `EnvPrefix` | `string?` | Environment variable prefix |
| `KeyCount` | `int` | Configuration key count |
| `Properties` | `IDictionary<string, object?>` | Custom properties |

### Built-in Exporters

| Exporter | Description |
|----------|-------------|
| `SnapshotExporters.Json` | Nested JSON structure |
| `SnapshotExporters.KeyValue` | Flat key-value pairs |
| `SnapshotExporters.Env` | Environment variable format |

```csharp
// Use built-in exporters directly
var json = cfg.ExportSnapshot(SnapshotExporters.Json);
var kv = cfg.ExportSnapshot(SnapshotExporters.KeyValue);
var env = cfg.ExportSnapshot(SnapshotExporters.Env);
```

## Use Cases

### Debugging Configuration Issues

```csharp
// Export configuration snapshot at application startup
var snapshot = cfg.ExportSnapshotAsJson(maskSensitive: true);
logger.LogDebug("Configuration snapshot:\n{Snapshot}", snapshot);
```

### Configuration Backup

```csharp
// Periodically backup configuration
var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
await cfg.ExportSnapshotToFileAsync($"backup/config_{timestamp}.json", new ExportOptions
{
    Indented = true,
    MaskSensitiveValues = false  // Backup needs complete values
});
```

### Configuration Migration

```csharp
// Export to environment variable format for container deployment
var envContent = cfg.ExportSnapshotAsEnv(prefix: "APP_");
await File.WriteAllTextAsync(".env.production", envContent);
```

### Configuration Comparison

```csharp
// Export configurations from two environments for comparison
var devConfig = devCfg.ExportSnapshotAsDictionary();
var prodConfig = prodCfg.ExportSnapshotAsDictionary();

var differences = devConfig.Keys
    .Where(k => !prodConfig.ContainsKey(k) || devConfig[k] != prodConfig[k])
    .ToList();
```

## API Reference

### ExportOptions Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Indented` | `bool` | `true` | Whether to indent JSON |
| `IncludeMetadata` | `bool` | `false` | Whether to include metadata |
| `MaskSensitiveValues` | `bool` | `true` | Whether to mask sensitive values |
| `IncludeKeys` | `string[]?` | `null` | Keys to include (supports wildcards) |
| `ExcludeKeys` | `string[]?` | `null` | Keys to exclude (supports wildcards) |
| `EnvPrefix` | `string?` | `null` | Prefix for environment variable format |

### Extension Methods

| Method | Description |
|--------|-------------|
| `ExportSnapshot()` | Export to string |
| `ExportSnapshot(options)` | Export with options |
| `ExportSnapshot(configure)` | Export using builder pattern |
| `ExportSnapshotAsJson()` | Export to JSON |
| `ExportSnapshotAsEnv()` | Export to environment variable format |
| `ExportSnapshotAsDictionary()` | Export to dictionary |
| `ExportSnapshotToFileAsync()` | Export to file |
| `ExportSnapshotAsync()` | Export to stream |
