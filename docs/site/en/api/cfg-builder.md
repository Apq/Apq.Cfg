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
    int level,
    bool writeable = false,
    bool optional = false,
    bool reloadOnChange = false,
    bool isPrimaryWriter = false,
    Encoding? encoding = null)
```

### AddYaml

Add a YAML configuration source (requires `Apq.Cfg.Yaml`).

```csharp
public CfgBuilder AddYaml(
    string path,
    int level,
    bool writeable = false,
    bool optional = false,
    bool reloadOnChange = false,
    bool isPrimaryWriter = false)
```

### AddEnvironmentVariables

Add environment variables as a configuration source.

```csharp
public CfgBuilder AddEnvironmentVariables(
    int level,
    string? prefix = null)
```

### AddSource

Add a custom configuration source.

```csharp
public CfgBuilder AddSource(ICfgSource source)
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
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

## Next Steps

- [ICfgRoot](/en/api/icfg-root) - Root interface
- [ICfgSection](/en/api/icfg-section) - Section interface
