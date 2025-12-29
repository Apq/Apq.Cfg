# YAML Configuration Source

YAML provides a human-readable configuration format with support for complex hierarchies.

## Installation

```bash
dotnet add package Apq.Cfg.Yaml
```

## Basic Usage

### Configuration File

```yaml
App:
  Name: MyApp
  Port: 8080
  Debug: true

Database:
  Host: localhost
  Port: 5432
  Name: mydb
```

### Read Configuration

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;

var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: false)
    .Build();

var appName = cfg.Get("App:Name");
var port = cfg.Get<int>("App:Port");
```

## Configuration Options

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `path` | string | Required | File path |
| `level` | int | Required | Priority level |
| `writeable` | bool | `false` | Enable write support |
| `optional` | bool | `false` | Allow missing file |
| `reloadOnChange` | bool | `false` | Enable hot reload |

## Next Steps

- [JSON](/en/config-sources/json) - JSON format
- [Config Merge](/en/guide/config-merge) - Multi-source merging
