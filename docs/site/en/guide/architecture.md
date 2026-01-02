# Architecture

This document describes the architecture design and core components of Apq.Cfg.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        Application Layer                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────┐  │
│  │ cfg.Get()   │  │ cfg.Set()   │  │ cfg.ConfigChanges       │  │
│  │ cfg.GetSection() │ cfg.SaveAsync() │ (Rx Observable)    │  │
│  └──────┬──────┘  └──────┬──────┘  └───────────┬─────────────┘  │
└─────────┼────────────────┼─────────────────────┼────────────────┘
          │                │                     │
┌─────────▼────────────────▼─────────────────────▼────────────────┐
│                      ICfgRoot Interface                          │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │                   MergedCfgRoot                              ││
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  ││
│  │  │ Level Merge │  │ Pending     │  │ ChangeCoordinator   │  ││
│  │  │             │  │ Queue       │  │                     │  ││
│  │  └─────────────┘  └─────────────┘  └─────────────────────┘  ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
          │                │                     │
┌─────────▼────────────────▼─────────────────────▼────────────────┐
│                      Source Layer (ICfgSource)                   │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│  │ JSON    │ │ YAML    │ │ Consul  │ │ Nacos   │ │ Vault   │   │
│  │ Level:0 │ │ Level:1 │ │ Level:10│ │ Level:10│ │ Level:15│   │
│  └────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘   │
└───────┼──────────┼──────────┼──────────┼──────────┼─────────────┘
        │          │          │          │          │
┌───────▼──────────▼──────────▼──────────▼──────────▼─────────────┐
│                Microsoft.Extensions.Configuration                │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │              IConfigurationRoot (Merged Config)              ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

## Core Components

### ICfgRoot Interface

The main entry point for the configuration system.

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // Read operations
    string? Get(string key);
    T? GetValue<T>(string key);
    bool Exists(string key);
    ICfgSection GetSection(string key);

    // Write operations
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken ct = default);

    // Batch operations
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);
    void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);

    // Events
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

### ICfgSource Interface

Base interface for all configuration sources.

```csharp
public interface ICfgSource
{
    int Level { get; }
    bool IsWriteable { get; }
    bool IsPrimaryWriter { get; }
    IConfigurationSource BuildSource();
}

public interface IWritableCfgSource : ICfgSource
{
    void Set(string key, string? value);
    void Remove(string key);
    Task SaveAsync(CancellationToken ct = default);
}
```

## Level Merge Mechanism

Configuration sources are merged by level, with higher levels taking precedence:

```
Level 0:  config.json          (base configuration)
Level 1:  config.{env}.json    (environment-specific)
Level 2:  config.local.json    (local overrides)
Level 10: Consul/Nacos/Etcd    (remote configuration)
Level 15: Vault                (secrets)
Level 20: Environment Variables (highest priority)
```

## Hot Reload Mechanism

```
┌─────────────────────────────────────────────────────────────┐
│                    ChangeCoordinator                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ Debounce    │  │ Incremental │  │ Change Notification │  │
│  │ (100ms)     │  │ Update      │  │ (ConfigChanges)     │  │
│  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘  │
└─────────┼────────────────┼─────────────────────┼────────────┘
          │                │                     │
          ▼                ▼                     ▼
    File Change       Source Reload        Rx Observable
```

## Thread Safety

1. **Read operations**: Lock-free, using immutable data structures
2. **Write operations**: ConcurrentDictionary for pending data
3. **Save operations**: Per-source locking
4. **Hot reload**: Debounce + atomic replacement

## Next Steps

- [Encryption Design](/en/guide/encryption-masking-design) - Encryption architecture
- [Extension Development](/en/guide/extension) - Create custom sources
