# Environment Variables

Environment variables configuration source, included in the core package.

## Basic Usage

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

## Prefix Filtering

Use prefix to filter relevant variables:

```bash
# These will be included with prefix "APP_"
APP_NAME=MyApp
APP_PORT=8080
APP_DATABASE__HOST=localhost

# These will be excluded
OTHER_VAR=value
```

## Key Mapping

Environment variables use double underscore for hierarchy:

| Environment Variable | Configuration Key |
|---------------------|-------------------|
| `APP_NAME` | `Name` |
| `APP_DATABASE__HOST` | `Database:Host` |
| `APP_DATABASE__PORT` | `Database:Port` |

## Priority

Environment variables typically have the highest priority (level 20+) to allow runtime overrides.

## Next Steps

- [JSON](/en/config-sources/json) - File-based configuration
- [Config Merge](/en/guide/config-merge) - How merging works
