# Redis Configuration Source

Redis-based configuration storage.

## Installation

```bash
dotnet add package Apq.Cfg.Redis
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Redis` (100).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 100
.AddRedis(options => { ... })

// Specify custom level
.AddRedis(options => { ... }, level: 150)
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
    }, writeable: true)  // Uses default level 100
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
