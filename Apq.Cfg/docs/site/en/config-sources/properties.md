# Properties Configuration Source

Properties is a key-value configuration file format widely used in the Java ecosystem and supported by many other applications.

## Installation

```bash
dotnet add package Apq.Cfg.Properties
```

## Default Level

This configuration source has a default level of `CfgSourceLevels.Properties` (0).

If the `level` parameter is not specified, the default level is used:

```csharp
// Use default level 0
.AddPropertiesFile("config.properties")

// Specify custom level
.AddPropertiesFile("config.properties", level: 5)
```

## Basic Usage

```csharp
using Apq.Cfg;
using Apq.Cfg.Properties;

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties")  // Use default level 0
    .Build();
```

### Optional Files and Reload

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", reloadOnChange: true)
    .AddPropertiesFile("config.local.properties", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### Writable Configuration

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", writeable: true, isPrimaryWriter: true)
    .Build();

// Modify configuration
cfg["App.Name"] = "NewName";
await cfg.SaveAsync();
```

## Method Signature

```csharp
public static CfgBuilder AddPropertiesFile(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

## Parameter Description

| Parameter | Description |
|-----------|-------------|
| `path` | Properties file path |
| `level` | Configuration level, higher values have higher priority |
| `writeable` | Whether writable (default `false`) |
| `optional` | Whether to ignore if file doesn't exist (default `true`) |
| `reloadOnChange` | Whether to automatically reload on file change (default `true`) |
| `isPrimaryWriter` | Whether this is the default write target (default `false`) |

## Properties File Format

### Basic Structure

```properties
# This is a comment
App.Name=MyApp
App.Version=1.0.0
Database.Host=localhost
Database.Port=5432
Database.Name=mydb
Database.Username=admin
Database.Password=secret
Logging.Level=INFO
Logging.EnableConsole=true
```

### Dot-separated Namespaces

```properties
# Use dots to separate namespaces
app.name=MyApp
app.database.host=localhost
app.database.port=5432
app.logging.level=INFO
```

### Using Equals or Colon

```properties
# Use equals
key1=value1

# Use colon
key2:value2
```

### Escape Characters

```properties
# Newline
description=Line1\nLine2

# Tab
tabbed=value1\tvalue2

# Special character escaping
hash_comment=This is not a comment \#here
colon_value=key\:value
backslash=path\\to\\file
```

### Multi-line Values

```properties
# Use backslash for continuation
long_value=This is a very long value that \
          continues on the next line
```

## Key Path Mapping

Properties structure maps to colon-separated key paths:

| Properties Key | Configuration Key |
|----------------|-------------------|
| `App.Name` | `App:Name` |
| `Database.Host` | `Database:Host` |
| `Database.Connection.Timeout` | `Database:Connection:Timeout` |

## Advanced Options

### Specify Encoding

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", encoding: options)
    .Build();
```

### Section Support (INI-style)

```properties
# Sections use square brackets
[app]
name=MyApp
version=1.0.0

[database]
host=localhost
port=5432

[logging]
level=INFO
```

When reading, sections act as prefixes:

| Properties Key | Configuration Key |
|----------------|-------------------|
| `app.name` (no section) | `app.name` |
| `name` under `[app]` | `app.name` |
| `host` under `[database]` | `database.host` |

## Mix with Other Formats

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddPropertiesFile("config.properties", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## Properties vs Other Formats

| Feature | Properties | JSON | YAML | TOML |
|---------|-----------|------|------|------|
| Readability | ⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Comments | ✅ | ❌ | ✅ | ✅ |
| Types | ❌ (string only) | ✅ | ✅ | ✅ |
| Nesting | Limited | ✅ | ✅ | ✅ |
| Java ecosystem | ✅✅✅ | ✅ | ✅ | ✅ |
| Simplicity | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |

## Use Cases

- Java/Spring projects (Properties is Spring's default format)
- Sharing configuration with legacy Java applications
- Simple key-value configuration needs
- Consistency with system properties (System.getProperty)
- Simple format for cross-language configuration sharing

## Spring Framework Integration

Spring Boot's `application.properties` files can be used directly:

```properties
# Spring Boot application.properties
spring.datasource.url=jdbc:mysql://localhost:3306/mydb
spring.datasource.username=admin
spring.datasource.password=secret
server.port=8080
```

Use in Apq.Cfg:

```csharp
var cfg = new CfgBuilder()
    .AddPropertiesFile("application.properties")
    .Build();

// Access configuration
var dbUrl = cfg["spring.datasource.url"];
var port = cfg.GetValue<int>("server.port");
```

## Dependencies

No additional dependencies (uses .NET built-in Properties format parsing).

## Next Steps

- [JSON Configuration Source](/en/config-sources/json) - JSON format
- [INI Configuration Source](/en/config-sources/ini) - INI format
- [Environment Variables](/en/config-sources/env) - Environment variables
