# Introduction

Apq.Cfg is a powerful, flexible, and high-performance .NET configuration library.

## Features

### Multi-Source Support

Supports 14+ configuration sources, including local files and remote configuration centers:

- **Local**: JSON, YAML, XML, INI, TOML, .env, Environment Variables
- **Remote**: Consul, Nacos, Apollo, Etcd, Zookeeper, Redis, Vault, Database

### Hot Reload

Real-time configuration updates without application restart:

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"Config changed: {e.Key}");
});
```

### Writable Configuration

Supports configuration modification and persistence:

```csharp
cfg.SetValue("App:Name", "NewName");
await cfg.SaveAsync();
```

### Extensible

Clean interface design for easy custom configuration source implementation:

```csharp
public class MyCustomSource : ICfgSource
{
    public string Name { get; set; } = "Custom";
    public int Level { get; }
    public string Type => "Custom";
    public bool IsWriteable { get; }
    public bool IsPrimaryWriter { get; }
    public int KeyCount => /* ... */;
    public int TopLevelKeyCount => /* ... */;

    public IConfigurationSource BuildSource() => /* ... */;
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues() => /* ... */;
}
```

### Encryption & Masking

Built-in configuration encryption and masking to protect sensitive information:

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv()  // Auto-decrypt {ENC} prefixed values
    .AddSensitiveMasking()          // Auto-mask in log output
    .Build();

// Auto-decrypt on read
var password = cfg["Database:Password"];

// Mask for logging
logger.LogInfo("Password: {0}", cfg.GetMasked("Database:Password"));
// Output: Password: myS***ord
```

### Config Templates

Support variable references for dynamic configuration composition (`Microsoft.Extensions.Configuration` does not support this feature):

```csharp
// config.json: { "App:Name": "MyApp", "App:LogPath": "${App:Name}/logs" }
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();

// Resolve variable references
var logPath = cfg.GetResolved("App:LogPath");
// Returns: "MyApp/logs"

// Reference environment variables and system properties
var home = cfg.GetResolved("Paths:Home");     // ${ENV:USERPROFILE}
var machine = cfg.GetResolved("Paths:Machine"); // ${SYS:MachineName}
```

## Quick Start

### Installation

```bash
dotnet add package Apq.Cfg
```

### Basic Usage

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

var appName = cfg["App:Name"];
var port = cfg.GetValue<int>("App:Port");
```

## Supported Platforms

| Platform | Versions |
|----------|----------|
| .NET | 8.0, 10.0 (LTS) |

## Next Steps

- [Installation Guide](/en/guide/installation) - Detailed installation instructions
- [Quick Start](/en/guide/quick-start) - 5-minute tutorial
- [Migration Guide](/en/guide/migration) - Migrate from Microsoft.Extensions.Configuration
- [Config Sources](/en/config-sources/) - Learn about all supported sources
- [Config Templates](/en/guide/template) - Variable references and dynamic config
- [Encryption & Masking](/en/guide/encryption-masking) - Protect sensitive configs
- [API Reference](/en/api/) - Complete API documentation
