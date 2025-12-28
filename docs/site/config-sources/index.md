# 配置源概述

Apq.Cfg 支持多种配置源，可以灵活组合使用。

## 配置源分类

### 本地配置源

从本地文件或环境读取配置：

| 配置源 | NuGet 包 | 说明 |
|--------|----------|------|
| [JSON](/config-sources/json) | Apq.Cfg (内置) | JSON 格式配置文件 |
| [YAML](/config-sources/yaml) | Apq.Cfg.Yaml | YAML 格式配置文件 |
| [XML](/config-sources/xml) | Apq.Cfg.Xml | XML 格式配置文件 |
| [INI](/config-sources/ini) | Apq.Cfg.Ini | INI 格式配置文件 |
| [TOML](/config-sources/toml) | Apq.Cfg.Toml | TOML 格式配置文件 |
| [环境变量](/config-sources/env) | Apq.Cfg (内置) | 系统环境变量 |

### 远程配置源

从远程配置中心读取配置：

| 配置源 | NuGet 包 | 说明 |
|--------|----------|------|
| [Consul](/config-sources/consul) | Apq.Cfg.Consul | HashiCorp Consul |
| [Redis](/config-sources/redis) | Apq.Cfg.Redis | Redis 键值存储 |
| [Apollo](/config-sources/apollo) | Apq.Cfg.Apollo | 携程 Apollo 配置中心 |
| [Vault](/config-sources/vault) | Apq.Cfg.Vault | HashiCorp Vault |
| [Etcd](/config-sources/etcd) | Apq.Cfg.Etcd | Etcd 分布式键值存储 |
| [Zookeeper](/config-sources/zookeeper) | Apq.Cfg.Zookeeper | Apache Zookeeper |
| [Nacos](/config-sources/nacos) | Apq.Cfg.Nacos | 阿里巴巴 Nacos |

## 配置源选择指南

### 开发环境

推荐使用本地文件配置：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
```

### 生产环境

推荐使用远程配置中心：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")  // 基础配置
    .AddConsul("http://consul:8500", "myapp/config")  // 动态配置
    .AddVault("https://vault:8200", "secret/myapp")   // 敏感配置
    .Build();
```

### 混合部署

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置
    .AddJsonFile("appsettings.json")
    // 环境特定配置
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    // 远程配置（可选，用于动态更新）
    .AddConsul("http://consul:8500", "myapp/config", optional: true)
    // 环境变量覆盖
    .AddEnvironmentVariables()
    .Build();
```

## 配置源特性对比

| 特性 | JSON | YAML | Consul | Redis | Apollo |
|------|------|------|--------|-------|--------|
| 动态重载 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 分布式 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 版本控制 | ❌ | ❌ | ✅ | ❌ | ✅ |
| 灰度发布 | ❌ | ❌ | ❌ | ❌ | ✅ |
| 权限控制 | ❌ | ❌ | ✅ | ❌ | ✅ |
| 审计日志 | ❌ | ❌ | ✅ | ❌ | ✅ |

## 配置源优先级

后添加的配置源优先级更高：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("base.json")           // 优先级 1
    .AddYamlFile("override.yaml")       // 优先级 2
    .AddConsul("...", "...")            // 优先级 3
    .AddEnvironmentVariables()          // 优先级 4（最高）
    .Build();
```

## 下一步

选择您需要的配置源了解详细用法：

- [JSON 配置源](/config-sources/json)
- [YAML 配置源](/config-sources/yaml)
- [Consul 配置源](/config-sources/consul)
- [Redis 配置源](/config-sources/redis)
