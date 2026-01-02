# Quick Start

This guide will help you get started with Apq.Cfg in 5 minutes.

## Installation

```bash
dotnet add package Apq.Cfg
```

## Create Configuration File

Create a `config.json` file:

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
        "Name": "mydb"
    }
}
```

## Read Configuration

```csharp
using Apq.Cfg;

// Build configuration
var cfg = new CfgBuilder()
    .AddJson("config.json")  // Uses default level 0
    .Build();

// Read string value
var appName = cfg["App:Name"];
Console.WriteLine($"App Name: {appName}");

// Read typed value
var port = cfg.Get<int>("App:Port");
Console.WriteLine($"Port: {port}");

// Read boolean value
var debug = cfg.Get<bool>("App:Debug");
Console.WriteLine($"Debug: {debug}");
```

## Use Configuration Sections

```csharp
// Get configuration section
var dbSection = cfg.GetSection("Database");

// Read values from section
var host = dbSection["Host"];
var dbPort = dbSection.Get<int>("Port");
var dbName = dbSection["Name"];

Console.WriteLine($"Database: {host}:{dbPort}/{dbName}");
```

## Multi-Source Configuration

Each configuration source has a default level. Higher level values override lower level values:

| Source Type | Default Level |
|-------------|---------------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")                              // Uses default level 0
    .AddJson("config.local.json", level: 50, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")             // Uses default level 400
    .Build();
```

## Hot Reload

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", reloadOnChange: true)  // Uses default level 0
    .Build();

// Subscribe to configuration changes
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("Configuration updated!");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## Writable Configuration

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", writeable: true, isPrimaryWriter: true)  // Uses default level 0
    .Build();

// Modify configuration
cfg["App:Name"] = "NewAppName";
cfg["App:Port"] = "9090";

// Save to file
await cfg.SaveAsync();
```

## Next Steps

- [Installation](/en/guide/installation) - Detailed installation guide
- [Basic Usage](/en/guide/basic-usage) - Learn more usage patterns
- [Config Sources](/en/config-sources/) - Explore all configuration sources
