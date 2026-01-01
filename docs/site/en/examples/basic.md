# Basic Examples

Basic configuration reading and type conversion examples.

## Simple Configuration

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();

// String value
var name = cfg["App:Name"];

// Integer value
var port = cfg.Get<int>("App:Port");

// Boolean value
var debug = cfg.Get<bool>("App:Debug");

// With default value
var timeout = cfg.Get<int?>("App:Timeout") ?? 30;
```

## Configuration Sections

```csharp
var dbSection = cfg.GetSection("Database");

var host = dbSection["Host"];
var port = dbSection.Get<int>("Port");
var name = dbSection["Name"];

Console.WriteLine($"Database: {host}:{port}/{name}");
```

## Check Existence

```csharp
if (cfg.Exists("App:OptionalFeature"))
{
    var feature = cfg["App:OptionalFeature"];
    // Use feature
}
```

## Next Steps

- [Multi-Source](/en/examples/multi-source) - Multiple configuration sources
- [DI Integration](/en/examples/di-integration) - Dependency injection
