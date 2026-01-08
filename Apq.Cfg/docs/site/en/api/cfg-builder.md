# CfgBuilder

The fluent API builder for creating configuration instances.

## Constructor

```csharp
public CfgBuilder()
```

## Methods

### AddJson

Add a JSON configuration source.

```csharp
public CfgBuilder AddJson(
    string path,
    int level = CfgSourceLevels.Json,  // default 0
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false,
    EncodingOptions? encoding = null,
    string? name = null)
```

### AddYaml

Add a YAML configuration source (requires `Apq.Cfg.Yaml`).

```csharp
public static CfgBuilder AddYaml(
    this CfgBuilder builder,
    string path,
    int level = CfgSourceLevels.Yaml,  // default 0
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

### AddEnvironmentVariables

Add environment variables as a configuration source.

```csharp
public CfgBuilder AddEnvironmentVariables(
    int level = CfgSourceLevels.EnvironmentVariables,  // default 400
    string? prefix = null,
    string? name = null)
```

### AddSource

Add a custom configuration source.

```csharp
public CfgBuilder AddSource(ICfgSource source, string? name = null)
```

### ConfigureEncodingMapping

Configure encoding detection rules.

```csharp
public CfgBuilder ConfigureEncodingMapping(
    Action<EncodingMappingConfig> configure)
```

### Build

Build the configuration instance.

```csharp
public ICfgRoot Build()
```

## Example

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddJsonFile("config.local.json", level: 1, writeable: true, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

## Next Steps

- [ICfgRoot](/en/api/icfg-root) - Root interface
- [ICfgSection](/en/api/icfg-section) - Section interface
