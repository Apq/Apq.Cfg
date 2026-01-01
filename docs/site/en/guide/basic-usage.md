# Basic Usage

This guide covers the fundamental usage patterns of Apq.Cfg.

## Building Configuration

Use `CfgBuilder` to create configuration instances:

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();
```

### Builder Parameters

| Parameter | Description |
|-----------|-------------|
| `level` | Priority level (higher overrides lower) |
| `writeable` | Whether the source supports writing |
| `optional` | Whether the source is optional |
| `reloadOnChange` | Enable hot reload |
| `isPrimaryWriter` | Mark as primary write target |

## Reading Values

### String Values

```csharp
// Get string value
string? value = cfg["App:Name"];

// With default value
string name = cfg["App:Name"] ?? "DefaultApp";
```

### Typed Values

```csharp
// Integer
int port = cfg.Get<int>("App:Port");

// Boolean
bool debug = cfg.Get<bool>("App:Debug");

// Double
double rate = cfg.Get<double>("App:Rate");

// DateTime
DateTime date = cfg.Get<DateTime>("App:StartDate");
```

### Check Existence

```csharp
if (cfg.Exists("App:Name"))
{
    var name = cfg["App:Name"];
}
```

## Configuration Sections

### Get Section

```csharp
var dbSection = cfg.GetSection("Database");

var host = dbSection["Host"];
var port = dbSection.Get<int>("Port");
```

### Nested Sections

```csharp
var connSection = cfg.GetSection("Database:Connection");
var timeout = connSection.Get<int>("Timeout");
```

### Enumerate Child Keys

```csharp
var section = cfg.GetSection("Servers");
foreach (var key in section.GetChildKeys())
{
    var serverConfig = section.GetSection(key);
    Console.WriteLine($"Server: {key}");
}
```

## Writing Values

### Set Values

```csharp
// Requires writeable source
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

// Set value
cfg["App:Name"] = "NewName";

// Set to specific level
cfg.Set("App:Port", "9090", targetLevel: 0);
```

### Remove Values

```csharp
cfg.Remove("App:TempKey");
```

### Save Changes

```csharp
// Save all changes
await cfg.SaveAsync();

// Save specific level
await cfg.SaveAsync(targetLevel: 0);
```

## Batch Operations

### Read Multiple Values

```csharp
var keys = new[] { "App:Name", "App:Port", "App:Debug" };
var values = cfg.GetMany(keys);

foreach (var (key, value) in values)
{
    Console.WriteLine($"{key}: {value}");
}
```

### Write Multiple Values

```csharp
var updates = new Dictionary<string, string?>
{
    ["App:Name"] = "NewApp",
    ["App:Port"] = "8080",
    ["App:Debug"] = "false"
};

cfg.SetMany(updates);
await cfg.SaveAsync();
```

## Microsoft.Extensions.Configuration Integration

```csharp
// Convert to IConfigurationRoot
IConfigurationRoot msConfig = cfg.ToMicrosoftConfiguration();

// Use with existing code
var value = msConfig["App:Name"];
```

## Next Steps

- [Config Merge](/en/guide/config-merge) - Learn about multi-source merging
- [Dynamic Reload](/en/guide/dynamic-reload) - Hot reload configuration
- [Dependency Injection](/en/guide/dependency-injection) - DI integration
