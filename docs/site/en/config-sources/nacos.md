# Nacos Configuration Source

Alibaba Nacos configuration center integration.

## Installation

```bash
dotnet add package Apq.Cfg.Nacos
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
    }, level: 10, reloadOnChange: true)
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
