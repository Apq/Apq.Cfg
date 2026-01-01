# Multi-Source Examples

Combining multiple configuration sources with level-based priority.

## Local Files

```csharp
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson($"config.{env}.json", level: 1, optional: true)
    .AddJson("config.local.json", level: 2, writeable: true, optional: true)
    .Build();
```

## With Remote Sources

```csharp
var cfg = new CfgBuilder()
    // Local base configuration
    .AddJson("config.json", level: 0)

    // Remote configuration center
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "myapp/config/";
    }, level: 10, writeable: true, reloadOnChange: true)

    // Secrets from Vault
    .AddVault(options =>
    {
        options.Address = "http://vault:8200";
        options.SecretPath = "secret/myapp";
    }, level: 15)

    // Environment variable overrides
    .AddEnvironmentVariables(level: 20, prefix: "MYAPP_")
    .Build();
```

## Mixed Formats

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddYaml("config.yaml", level: 1, optional: true)
    .AddToml("config.toml", level: 2, optional: true)
    .Build();
```

## Next Steps

- [DI Integration](/en/examples/di-integration) - Dependency injection
- [Dynamic Reload](/en/examples/dynamic-reload) - Hot reload
