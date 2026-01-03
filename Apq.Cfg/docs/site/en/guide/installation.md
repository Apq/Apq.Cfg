# Installation

## NuGet Packages

### Core Package

```bash
dotnet add package Apq.Cfg
```

The core package includes:
- JSON configuration source
- Environment variables support
- Dependency injection integration

### Extension Packages

Install additional packages based on your needs:

| Package | Description |
|---------|-------------|
| `Apq.Cfg.Yaml` | YAML configuration support |
| `Apq.Cfg.Xml` | XML configuration support |
| `Apq.Cfg.Ini` | INI configuration support |
| `Apq.Cfg.Toml` | TOML configuration support |
| `Apq.Cfg.Env` | .env file support |
| `Apq.Cfg.Consul` | Consul configuration center |
| `Apq.Cfg.Nacos` | Nacos configuration center |
| `Apq.Cfg.Apollo` | Apollo configuration center |
| `Apq.Cfg.Etcd` | Etcd configuration center |
| `Apq.Cfg.Zookeeper` | Zookeeper configuration center |
| `Apq.Cfg.Redis` | Redis configuration source |
| `Apq.Cfg.Vault` | HashiCorp Vault secrets |
| `Apq.Cfg.Database` | Database configuration source |
| `Apq.Cfg.Crypto` | Encryption and masking |
| `Apq.Cfg.SourceGenerator` | Source generator for AOT |

### Installation Examples

```bash
# YAML support
dotnet add package Apq.Cfg.Yaml

# Consul integration
dotnet add package Apq.Cfg.Consul

# Encryption support
dotnet add package Apq.Cfg.Crypto
```

## Supported Platforms

| Platform | Versions |
|----------|----------|
| .NET | 8.0, 10.0 (LTS) |

## IDE Support

Apq.Cfg works with all major .NET IDEs:

- Visual Studio 2022+
- Visual Studio Code with C# extension
- JetBrains Rider
- Visual Studio for Mac

## Next Steps

- [Quick Start](/en/guide/quick-start) - Get started in 5 minutes
- [Basic Usage](/en/guide/basic-usage) - Learn the basics
