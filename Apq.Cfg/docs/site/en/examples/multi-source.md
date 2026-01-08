# Multi-Source Examples

Combining multiple configuration sources with level-based priority.

## Default Levels

Each configuration source has a default level:

| Source Type | Default Level |
|-------------|---------------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## Local Files

```csharp
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                                    // Uses default level 0
    .AddJsonFile($"config.{env}.json", level: 10, optional: true)
    .AddJsonFile("config.local.json", level: 50, writeable: true, optional: true)
    .Build();
```

## With Remote Sources

```csharp
var cfg = new CfgBuilder()
    // Local base configuration (uses default level 0)
    .AddJsonFile("config.json")

    // Remote configuration center (uses default level 200)
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "myapp/config/";
    }, writeable: true, reloadOnChange: true)

    // Secrets from Vault (uses default level 300)
    .AddVault(options =>
    {
        options.Address = "http://vault:8200";
        options.SecretPath = "secret/myapp";
    })

    // Environment variable overrides (uses default level 400)
    .AddEnvironmentVariables(prefix: "MYAPP_")
    .Build();
```

## Mixed Formats

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                        // Uses default level 0
    .AddYamlFile("config.yaml", level: 10, optional: true)
    .AddTomlFile("config.toml", level: 20, optional: true)
    .Build();
```

## Next Steps

- [DI Integration](/en/examples/di-integration) - Dependency injection
- [Dynamic Reload](/en/examples/dynamic-reload) - Hot reload
