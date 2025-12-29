# Etcd Configuration Source

Etcd distributed key-value store integration.

## Installation

```bash
dotnet add package Apq.Cfg.Etcd
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
    }, level: 10, writeable: true, reloadOnChange: true)
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
