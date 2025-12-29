# Apollo Configuration Source

Ctrip Apollo configuration center integration.

## Installation

```bash
dotnet add package Apq.Cfg.Apollo
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Apollo;

var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = "myapp";
        options.MetaServer = "http://localhost:8080";
        options.Namespaces = new[] { "application", "common" };
    }, level: 10, reloadOnChange: true)
    .Build();
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `AppId` | string | Required | Application ID |
| `MetaServer` | string | Required | Meta server address |
| `Namespaces` | string[] | `["application"]` | Namespaces to load |
| `Cluster` | string | `"default"` | Cluster name |

## Next Steps

- [Nacos](/en/config-sources/nacos) - Nacos configuration
- [Consul](/en/config-sources/consul) - Consul configuration
