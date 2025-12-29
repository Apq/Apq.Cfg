# Zookeeper Configuration Source

Apache Zookeeper integration for distributed configuration.

## Installation

```bash
dotnet add package Apq.Cfg.Zookeeper
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
    }, level: 10, writeable: true, reloadOnChange: true)
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
