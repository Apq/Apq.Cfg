# Redis Configuration Source

Redis-based configuration storage.

## Installation

```bash
dotnet add package Apq.Cfg.Redis
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Redis;

var cfg = new CfgBuilder()
    .AddRedis(options =>
    {
        options.ConnectionString = "localhost:6379";
        options.KeyPrefix = "config:";
    }, level: 10, writeable: true)
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `ConnectionString` | string | Required | Redis connection string |
| `KeyPrefix` | string | `""` | Key prefix |
| `Database` | int | `0` | Redis database number |

## Next Steps

- [Consul](/en/config-sources/consul) - Consul configuration
- [Vault](/en/config-sources/vault) - Secrets management
