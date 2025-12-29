# Vault Configuration Source

HashiCorp Vault secrets management integration.

## Installation

```bash
dotnet add package Apq.Cfg.Vault
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Vault;

var cfg = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = "http://localhost:8200";
        options.Token = Environment.GetEnvironmentVariable("VAULT_TOKEN");
        options.SecretPath = "secret/data/myapp";
    }, level: 15)
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Address` | string | Required | Vault server address |
| `Token` | string | Required | Authentication token |
| `SecretPath` | string | Required | Secret path |
| `MountPoint` | string | `"secret"` | Secrets engine mount point |

## Authentication Methods

- Token authentication
- AppRole authentication
- Kubernetes authentication

## Next Steps

- [Consul](/en/config-sources/consul) - Consul configuration
- [Encryption & Masking](/en/guide/encryption-masking) - Local encryption
