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
cfg.Set("App:Name", "NewName");
await cfg.SaveAsync();
```

### Extensible

Clean interface design for easy custom configuration source implementation:

```csharp
public class MyCustomSource : ICfgSource
{
    public int Level { get; }
    public bool IsWriteable { get; }

    public Task<IDictionary<string, string?>> LoadAsync(CancellationToken cancellationToken)
    {
        // Implement custom loading logic
    }
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
var password = cfg.Get("Database:Password");

// Mask for logging
logger.LogInfo("Password: {0}", cfg.GetMasked("Database:Password"));
// Output: Password: myS***ord
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

var appName = cfg.Get("App:Name");
var port = cfg.Get<int>("App:Port");
```

## Supported Platforms

| Platform | Versions |
|----------|----------|
| .NET | 6.0, 7.0, 8.0, 9.0 |
| .NET Standard | 2.0, 2.1 |

## Next Steps

- [Installation Guide](/en/guide/installation) - Detailed installation instructions
- [Quick Start](/en/guide/quick-start) - 5-minute tutorial
- [Config Sources](/en/config-sources/) - Learn about all supported sources
- [Encryption & Masking](/en/guide/encryption-masking) - Protect sensitive configs
- [API Reference](/en/api/) - Complete API documentation
