# Config Sources Overview

Apq.Cfg supports 14+ configuration sources, divided into local and remote categories.

## Default Levels

Each configuration source has a default level. If not specified, the default level is used:

| Source Type | Default Level |
|-------------|---------------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## Local Configuration Sources

| Source | Package | Default Level | Description |
|--------|---------|---------------|-------------|
| [JSON](/en/config-sources/json) | `Apq.Cfg` | 0 | JSON file configuration |
| [YAML](/en/config-sources/yaml) | `Apq.Cfg.Yaml` | 0 | YAML file configuration |
| [XML](/en/config-sources/xml) | `Apq.Cfg.Xml` | 0 | XML file configuration |
| [INI](/en/config-sources/ini) | `Apq.Cfg.Ini` | 0 | INI file configuration |
| [TOML](/en/config-sources/toml) | `Apq.Cfg.Toml` | 0 | TOML file configuration |
| [.env](/en/config-sources/env) | `Apq.Cfg.Env` | 400 | .env file configuration |
| [Environment Variables](/en/config-sources/env) | `Apq.Cfg` | 400 | Environment variables |

## Remote Configuration Sources

| Source | Package | Default Level | Description |
|--------|---------|---------------|-------------|
| [Redis](/en/config-sources/redis) | `Apq.Cfg.Redis` | 100 | Redis key-value store |
| [Database](/en/config-sources/database) | `Apq.Cfg.Database` | 100 | Database configuration |
| [Consul](/en/config-sources/consul) | `Apq.Cfg.Consul` | 200 | HashiCorp Consul |
| [Etcd](/en/config-sources/etcd) | `Apq.Cfg.Etcd` | 200 | Etcd key-value store |
| [Nacos](/en/config-sources/nacos) | `Apq.Cfg.Nacos` | 200 | Alibaba Nacos |
| [Apollo](/en/config-sources/apollo) | `Apq.Cfg.Apollo` | 200 | Ctrip Apollo |
| [Zookeeper](/en/config-sources/zookeeper) | `Apq.Cfg.Zookeeper` | 200 | Apache Zookeeper |
| [Vault](/en/config-sources/vault) | `Apq.Cfg.Vault` | 300 | HashiCorp Vault |

## Quick Example

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    // Local sources (uses default level 0)
    .AddJson("config.json")
    .AddYaml("config.yaml", level: 10, optional: true)

    // Remote sources (uses default level 200)
    .AddConsul(options =>
    {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
    }, writeable: true)

    // Environment variables (uses default level 400)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## Level Priority

Configuration sources are merged by level, higher levels override lower levels:

```
Level 0:   config.json          (base configuration)
Level 10:  config.{env}.json    (environment-specific)
Level 50:  config.local.json    (local overrides)
Level 100: Redis/Database       (remote storage)
Level 200: Consul/Nacos/Etcd    (config centers)
Level 300: Vault                (secrets)
Level 400: Environment Variables (highest priority)
```

## Next Steps

Choose a configuration source to learn more:

- [JSON](/en/config-sources/json) - Most common format
- [YAML](/en/config-sources/yaml) - Human-readable format
- [Consul](/en/config-sources/consul) - Service discovery and configuration
- [Vault](/en/config-sources/vault) - Secrets management
