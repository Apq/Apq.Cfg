# Consul Configuration Source

HashiCorp Consul integration for distributed configuration.

## Installation

```bash
dotnet add package Apq.Cfg.Consul
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
    }, level: 10, writeable: true, reloadOnChange: true)
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
.AddConsul(options => { }, level: 10, reloadOnChange: true)
```

## Next Steps

- [Nacos](/en/config-sources/nacos) - Nacos configuration
- [Vault](/en/config-sources/vault) - Secrets management
