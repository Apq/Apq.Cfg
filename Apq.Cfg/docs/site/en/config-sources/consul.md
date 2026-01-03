# Consul Configuration Source

HashiCorp Consul integration for distributed configuration.

## Installation

```bash
dotnet add package Apq.Cfg.Consul
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Consul` (200).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 200
.AddConsul(options => { ... })

// Specify custom level
.AddConsul(options => { ... }, level: 250)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Consul;

var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
    }, writeable: true, reloadOnChange: true)  // Uses default level 200
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Address` | string | Required | Consul server address |
| `KeyPrefix` | string | `""` | Key prefix filter |
| `Token` | string | `null` | ACL token |
| `Datacenter` | string | `null` | Target datacenter |

## Hot Reload

Consul supports automatic reload via long polling:

```csharp
.AddConsul(options => { }, reloadOnChange: true)  // Uses default level 200
```

## Next Steps

- [Nacos](/en/config-sources/nacos) - Nacos configuration
- [Vault](/en/config-sources/vault) - Secrets management
