# YAML Configuration Source

YAML provides a human-readable configuration format with support for complex hierarchies.

## Installation

```bash
dotnet add package Apq.Cfg.Yaml
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Yaml` (0).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 0
.AddYamlFile("config.yaml")

// Specify custom level
.AddYamlFile("config.yaml", level: 10)
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
    .AddYamlFile("config.yaml")  // Uses default level 0
    .Build();

var appName = cfg["App:Name"];
var port = cfg.GetValue<int>("App:Port");
```

## Configuration Options

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `path` | string | Required | File path |
| `level` | int | 0 | Priority level |
| `writeable` | bool | `false` | Enable write support |
| `optional` | bool | `false` | Allow missing file |
| `reloadOnChange` | bool | `false` | Enable hot reload |

## Next Steps

- [JSON](/en/config-sources/json) - JSON format
- [Config Merge](/en/guide/config-merge) - Multi-source merging
