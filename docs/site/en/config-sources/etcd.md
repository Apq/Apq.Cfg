# Etcd Configuration Source

Etcd distributed key-value store integration.

## Installation

```bash
dotnet add package Apq.Cfg.Etcd
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Etcd` (200).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 200
.AddEtcd(options => { ... })

// Specify custom level
.AddEtcd(options => { ... }, level: 250)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Etcd;

var cfg = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { "http://localhost:2379" };
        options.KeyPrefix = "/config/myapp/";
    }, writeable: true, reloadOnChange: true)  // Uses default level 200
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Endpoints` | string[] | Required | Etcd endpoints |
| `KeyPrefix` | string | `""` | Key prefix |
| `Username` | string | `null` | Authentication username |
| `Password` | string | `null` | Authentication password |

## Next Steps

- [Consul](/en/config-sources/consul) - Consul configuration
- [Zookeeper](/en/config-sources/zookeeper) - Zookeeper configuration
