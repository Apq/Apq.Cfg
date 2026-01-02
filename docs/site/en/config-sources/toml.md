# TOML Configuration Source

TOML configuration support with clear syntax.

## Installation

```bash
dotnet add package Apq.Cfg.Toml
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Toml` (0).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 0
.AddToml("config.toml")

// Specify custom level
.AddToml("config.toml", level: 10)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Toml;

var cfg = new CfgBuilder()
    .AddToml("config.toml")  // Uses default level 0
    .Build();
```

## Configuration File

```toml
[App]
Name = "MyApp"
Port = 8080
Debug = true

[Database]
Host = "localhost"
Port = 5432
```

## Next Steps

- [JSON](/en/config-sources/json) - JSON format
- [INI](/en/config-sources/ini) - INI format
