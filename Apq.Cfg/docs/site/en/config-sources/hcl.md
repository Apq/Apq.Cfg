# HOCON Configuration Source

HOCON (Human-Optimized Config Object Notation) is a developer and administrator-friendly configuration format that supports advanced features like hierarchies, references, and concatenation.

## Installation

```bash
dotnet add package Apq.Cfg.Hcl
```

## Default Level

This configuration source has a default level of `CfgSourceLevels.Hcl` (0).

If the `level` parameter is not specified, the default level is used:

```csharp
// Use default level 0
.AddHclFile("config.hcl")

// Specify custom level
.AddHclFile("config.hcl", level: 5)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Hcl;

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl")  // Use default level 0
    .Build();
```

### Optional Files and Reload

```csharp
var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", reloadOnChange: true)
    .AddHclFile("config.local.hcl", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### Writable Configuration

```csharp
var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", writeable: true, isPrimaryWriter: true)
    .Build();

// Modify configuration
cfg["app:name"] = "NewName";
await cfg.SaveAsync();
```

## Method Signature

```csharp
public static CfgBuilder AddHclFile(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

## Parameter Description

| Parameter | Description |
|-----------|-------------|
| `path` | HOCON file path |
| `level` | Configuration level, higher values have higher priority |
| `writeable` | Whether writable (default `false`) |
| `optional` | Whether to ignore if file doesn't exist (default `true`) |
| `reloadOnChange` | Whether to automatically reload on file change (default `true`) |
| `isPrimaryWriter` | Whether this is the default write target (default `false`) |

## HOCON File Format

### Basic Structure

```hocon
# This is a comment
app_name = "MyApp"
version = "1.0.0"

database {
    host = "localhost"
    port = 5432
    name = "mydb"
    username = "admin"
    password = "secret"
}

logging {
    level = "INFO"
    enable_console = true
}
```

### Nested Objects

```hocon
database {
    host = "localhost"
    connection {
        timeout = 30
        pool_size = 10
    }
}
```

### Array Configuration

```hocon
# Simple array
servers = ["server1.example.com", "server2.example.com"]

# Array of objects
services = [
    { name = "api", url = "https://api.example.com" },
    { name = "auth", url = "https://auth.example.com" }
]
```

### Configuration References

```hocon
# Reference other configuration values
base_url = "https://example.com"
api_url = ${base_url}"/api"
full_path = ${database.host}":"${database.port}
```

### Configuration Concatenation

```hocon
# Merge arrays
common_ports = [80, 443]
all_ports = ${common_ports} [8080, 8443]
```

### Include Files

```hocon
# Include other configuration files
include "path/to/common.conf"
include "path/to/database.conf"
```

### Multi-line Strings

```hocon
description = """
This is a multi-line
string value that
preserves formatting.
"""
```

## Key Path Mapping

HOCON structure maps to colon-separated key paths:

| HOCON Path | Configuration Key |
|------------|-------------------|
| `app_name` | `app_name` |
| `database.host` | `database:host` |
| `database.connection.timeout` | `database:connection:timeout` |
| `services.0.name` | `services:0:name` |

## Advanced Options

### Specify Encoding

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", encoding: options)
    .Build();
```

### Use Akka.Configuration Factory

```csharp
using Akka.Configuration;

var hoconConfig = ConfigurationFactory.ParseString("""
    akka {
        actor {
            provider = "Akka.Remote.RemoteActorRefProvider, Akka.Remote"
        }
    }
    """);

var cfg = new CfgBuilder()
    .AddHclString(hoconConfig)
    .Build();
```

## Mix with Other Formats

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddHclFile("config.hcl", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## Use Cases

- Akka.NET/Play Framework projects (HOCON is the default format for these frameworks)
- Complex configurations requiring references and concatenation
- Environments using placeholders in configuration
- Scenarios needing multiple config files merged
- Sharing configuration with Java/Scala projects

## Dependencies

- [Akka.Configuration](https://www.nuget.org/packages/Akka.Configuration) - HOCON parsing library

## Next Steps

- [JSON Configuration Source](/en/config-sources/json) - JSON format
- [YAML Configuration Source](/en/config-sources/yaml) - YAML format
- [TOML Configuration Source](/en/config-sources/toml) - TOML format
