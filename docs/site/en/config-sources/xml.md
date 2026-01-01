# XML Configuration Source

XML configuration support for enterprise applications.

## Installation

```bash
dotnet add package Apq.Cfg.Xml
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Xml;

var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0)
    .Build();
```

## Configuration File

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <App>
    <Name>MyApp</Name>
    <Port>8080</Port>
  </App>
  <Database>
    <Host>localhost</Host>
    <Port>5432</Port>
  </Database>
</configuration>
```

## Next Steps

- [JSON](/en/config-sources/json) - JSON format
- [YAML](/en/config-sources/yaml) - YAML format
