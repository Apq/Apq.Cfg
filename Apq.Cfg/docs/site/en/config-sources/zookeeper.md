# Zookeeper Configuration Source

Apache Zookeeper integration for distributed configuration.

## Installation

```bash
dotnet add package Apq.Cfg.Zookeeper
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Zookeeper` (200).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 200
.AddZookeeper(options => { ... })

// Specify custom level
.AddZookeeper(options => { ... }, level: 250)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Zookeeper;

var cfg = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = "localhost:2181";
        options.BasePath = "/config/myapp";
    }, writeable: true, reloadOnChange: true)  // Uses default level 200
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `ConnectionString` | string | Required | Zookeeper connection string |
| `BasePath` | string | `"/"` | Base path for configuration |
| `SessionTimeout` | TimeSpan | `30s` | Session timeout |

## Next Steps

- [Etcd](/en/config-sources/etcd) - Etcd configuration
- [Consul](/en/config-sources/consul) - Consul configuration
