# JSON Configuration Source

JSON is the most commonly used configuration format, included in the core `Apq.Cfg` package.

## Installation

```bash
dotnet add package Apq.Cfg
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Json` (0).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 0
.AddJsonFile("config.json")

// Specify custom level
.AddJsonFile("config.json", level: 10)
```

## Basic Usage

### Configuration File

```json
{
    "App": {
        "Name": "MyApp",
        "Port": 8080,
        "Debug": true
    },
    "Database": {
        "Host": "localhost",
        "Port": 5432,
        "Name": "mydb",
        "ConnectionString": "Host=localhost;Database=mydb"
    },
    "Logging": {
        "Level": "Information",
        "Providers": ["Console", "File"]
    }
}
```

### Read Configuration

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")  // Uses default level 0
    .Build();

// Read values
var appName = cfg["App:Name"];
var port = cfg.GetValue<int>("App:Port");
var debug = cfg.GetValue<bool>("App:Debug");
```

## Configuration Options

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `path` | string | Required | File path |
| `level` | int | 0 | Priority level |
| `writeable` | bool | `false` | Enable write support |
| `optional` | bool | `false` | Allow missing file |
| `reloadOnChange` | bool | `false` | Enable hot reload |
| `isPrimaryWriter` | bool | `false` | Mark as primary write target |

## Multi-Environment Configuration

```csharp
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                                    // Uses default level 0
    .AddJsonFile($"config.{env}.json", level: 10, optional: true)
    .AddJsonFile("config.local.json", level: 50, optional: true)
    .Build();
```

## Writable Configuration

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", writeable: true, isPrimaryWriter: true)  // Uses default level 0
    .Build();

// Modify values
cfg["App:Name"] = "NewAppName";
cfg["App:Port"] = "9090";

// Save to file
await cfg.SaveAsync();
```

## Hot Reload

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)  // Uses default level 0
    .Build();

// Subscribe to changes
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("Configuration changed!");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## Array Support

### Configuration

```json
{
    "Servers": [
        { "Host": "server1.example.com", "Port": 8080 },
        { "Host": "server2.example.com", "Port": 8081 }
    ]
}
```

### Reading Arrays

```csharp
// Access by index
var host1 = cfg["Servers:0:Host"];
var port1 = cfg.GetValue<int>("Servers:0:Port");

// Enumerate
var serversSection = cfg.GetSection("Servers");
foreach (var key in serversSection.GetChildKeys())
{
    var server = serversSection.GetSection(key);
    Console.WriteLine($"{server["Host"]}:{server.GetValue<int>("Port")}");
}
```

## Next Steps

- [YAML](/en/config-sources/yaml) - Alternative format
- [Config Merge](/en/guide/config-merge) - Multi-source merging
- [Dynamic Reload](/en/guide/dynamic-reload) - Hot reload details
