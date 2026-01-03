# Nacos Configuration Source

Alibaba Nacos configuration center integration.

## Installation

```bash
dotnet add package Apq.Cfg.Nacos
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Nacos` (200).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 200
.AddNacos(options => { ... })

// Specify custom level
.AddNacos(options => { ... }, level: 250)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Nacos;

var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = new[] { "http://localhost:8848" };
        options.Namespace = "public";
        options.DataId = "myapp";
        options.Group = "DEFAULT_GROUP";
    }, reloadOnChange: true)  // Uses default level 200
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `ServerAddresses` | string[] | Required | Nacos server addresses |
| `Namespace` | string | `"public"` | Namespace |
| `DataId` | string | Required | Data ID |
| `Group` | string | `"DEFAULT_GROUP"` | Group name |

## Hot Reload

Nacos supports automatic reload via long polling.

## Next Steps

- [Apollo](/en/config-sources/apollo) - Apollo configuration
- [Consul](/en/config-sources/consul) - Consul configuration
