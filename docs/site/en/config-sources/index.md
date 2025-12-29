# Config Sources Overview

Apq.Cfg supports 14+ configuration sources, divided into local and remote categories.

## Local Configuration Sources

| Source | Package | Description |
|--------|---------|-------------|
| [JSON](/en/config-sources/json) | `Apq.Cfg` | JSON file configuration |
| [YAML](/en/config-sources/yaml) | `Apq.Cfg.Yaml` | YAML file configuration |
| [XML](/en/config-sources/xml) | `Apq.Cfg.Xml` | XML file configuration |
| [INI](/en/config-sources/ini) | `Apq.Cfg.Ini` | INI file configuration |
| [TOML](/en/config-sources/toml) | `Apq.Cfg.Toml` | TOML file configuration |
| [Environment Variables](/en/config-sources/env) | `Apq.Cfg` | Environment variables |

## Remote Configuration Sources

| Source | Package | Description |
|--------|---------|-------------|
| [Consul](/en/config-sources/consul) | `Apq.Cfg.Consul` | HashiCorp Consul |
| [Redis](/en/config-sources/redis) | `Apq.Cfg.Redis` | Redis key-value store |
| [Apollo](/en/config-sources/apollo) | `Apq.Cfg.Apollo` | Ctrip Apollo |
| [Nacos](/en/config-sources/nacos) | `Apq.Cfg.Nacos` | Alibaba Nacos |
| [Vault](/en/config-sources/vault) | `Apq.Cfg.Vault` | HashiCorp Vault |
| [Etcd](/en/config-sources/etcd) | `Apq.Cfg.Etcd` | Etcd key-value store |
| [Zookeeper](/en/config-sources/zookeeper) | `Apq.Cfg.Zookeeper` | Apache Zookeeper |

## Quick Example

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    // Local sources
    .AddJson("config.json", level: 0, writeable: false)
    .AddYaml("config.yaml", level: 1, writeable: false, optional: true)

    // Remote sources
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
    }, level: 10, writeable: true)

    // Environment variables (highest priority)
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

## Level Priority

Configuration sources are merged by level, higher levels override lower levels:

```
Level 0:  config.json          (base configuration)
Level 1:  config.{env}.json    (environment-specific)
Level 2:  config.local.json    (local overrides)
Level 10: Consul/Nacos/Etcd    (remote configuration)
Level 15: Vault                (secrets)
Level 20: Environment Variables (highest priority)
```

## Next Steps

Choose a configuration source to learn more:

- [JSON](/en/config-sources/json) - Most common format
- [YAML](/en/config-sources/yaml) - Human-readable format
- [Consul](/en/config-sources/consul) - Service discovery and configuration
- [Vault](/en/config-sources/vault) - Secrets management
