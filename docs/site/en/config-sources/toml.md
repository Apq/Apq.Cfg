# TOML Configuration Source

TOML configuration support with clear syntax.

## Installation

```bash
dotnet add package Apq.Cfg.Toml
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Toml;

var cfg = new CfgBuilder()
    .AddToml("config.toml", level: 0)
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
