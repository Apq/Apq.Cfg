# Source Selection

This guide helps you choose the right configuration sources for your application.

## Decision Guide

### Local vs Remote

| Scenario | Recommended Source |
|----------|-------------------|
| Single application | Local files (JSON, YAML) |
| Microservices | Remote (Consul, Nacos) |
| Secrets management | Vault |
| Container deployment | Environment variables + Remote |

### File Format Selection

| Format | Best For |
|--------|----------|
| JSON | Most applications, good tooling support |
| YAML | Complex hierarchies, human-readable |
| TOML | Simple configs, Rust ecosystem |
| INI | Legacy systems, simple key-value |
| XML | Enterprise systems, schema validation |

## Common Patterns

### Development Environment

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.Development.json", level: 1, writeable: false, optional: true)
    .AddJson("config.local.json", level: 2, writeable: true, optional: true)
    .Build();
```

### Production with Remote Config

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "myapp/config/";
    }, level: 10, writeable: true, reloadOnChange: true)
    .AddVault(options =>
    {
        options.Address = "http://vault:8200";
        options.SecretPath = "secret/myapp";
    }, level: 15, writeable: false)
    .AddEnvironmentVariables(level: 20, prefix: "MYAPP_")
    .Build();
```

### Kubernetes Deployment

```csharp
var cfg = new CfgBuilder()
    .AddJson("/app/config/config.json", level: 0, writeable: false)
    .AddJson("/app/secrets/secrets.json", level: 5, writeable: false, optional: true)
    .AddEnvironmentVariables(level: 10, prefix: "APP_")
    .Build();
```

## Level Guidelines

| Level Range | Purpose | Examples |
|-------------|---------|----------|
| 0-9 | Base configuration | config.json, config.yaml |
| 10-19 | Remote configuration | Consul, Nacos, Etcd |
| 15-19 | Secrets | Vault |
| 20+ | Overrides | Environment variables |

## Next Steps

- [Config Merge](/en/guide/config-merge) - How merging works
- [Config Sources](/en/config-sources/) - All available sources
