# INI Configuration Source

INI file support for simple key-value configurations.

## Installation

```bash
dotnet add package Apq.Cfg.Ini
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Ini;

var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0)
    .Build();
```

## Configuration File

```ini
[App]
Name=MyApp
Port=8080

[Database]
Host=localhost
Port=5432
```

## Key Format

INI sections map to configuration paths:
- `[App]` + `Name` → `App:Name`
- `[Database]` + `Host` → `Database:Host`

## Next Steps

- [JSON](/en/config-sources/json) - JSON format
- [TOML](/en/config-sources/toml) - TOML format
