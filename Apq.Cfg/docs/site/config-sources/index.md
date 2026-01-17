# 配置源概述

Apq.Cfg 支持多种配置源，可以灵活组合使用。

## 默认层级

每种配置源都有默认层级，如果不指定 `level` 参数，将使用默认值：

| 配置源类型 | 默认层级 |
|------------|----------|
| Json, Ini, Xml, Yaml, Toml, Hcl, Properties | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## 配置源分类

### 本地配置源

从本地文件或环境读取配置：

| 配置源 | NuGet 包 | 默认层级 | 说明 |
|--------|----------|----------|------|
| [JSON](/config-sources/json) | Apq.Cfg (内置) | 0 | JSON 格式配置文件 |
| [YAML](/config-sources/yaml) | Apq.Cfg.Yaml | 0 | YAML 格式配置文件 |
| [XML](/config-sources/xml) | Apq.Cfg.Xml | 0 | XML 格式配置文件 |
| [INI](/config-sources/ini) | Apq.Cfg.Ini | 0 | INI 格式配置文件 |
| [TOML](/config-sources/toml) | Apq.Cfg.Toml | 0 | TOML 格式配置文件 |
| [HOCON](/config-sources/hcl) | Apq.Cfg.Hcl | 0 | HOCON 格式配置文件 |
| [Properties](/config-sources/properties) | Apq.Cfg.Properties | 0 | Java Properties 格式配置文件 |
| [.env 文件](/config-sources/env#env-文件支持) | Apq.Cfg.Env | 400 | .env 格式配置文件 |
| [环境变量](/config-sources/env) | Apq.Cfg (内置) | 400 | 系统环境变量 |

### 数据存储配置源

从数据存储系统读取配置：

| 配置源 | NuGet 包 | 默认层级 | 说明 |
|--------|----------|----------|------|
| [Redis](/config-sources/redis) | Apq.Cfg.Redis | 100 | Redis 键值存储 |
| [Database](/config-sources/database) | Apq.Cfg.Database | 100 | 数据库配置源（支持多种数据库） |

### 远程配置中心

从远程配置中心读取配置：

| 配置源 | NuGet 包 | 默认层级 | 说明 |
|--------|----------|----------|------|
| [Consul](/config-sources/consul) | Apq.Cfg.Consul | 200 | HashiCorp Consul |
| [Etcd](/config-sources/etcd) | Apq.Cfg.Etcd | 200 | Etcd 分布式键值存储 |
| [Nacos](/config-sources/nacos) | Apq.Cfg.Nacos | 200 | 阿里巴巴 Nacos |
| [Apollo](/config-sources/apollo) | Apq.Cfg.Apollo | 200 | 携程 Apollo 配置中心 |
| [Zookeeper](/config-sources/zookeeper) | Apq.Cfg.Zookeeper | 200 | Apache Zookeeper |
| [Vault](/config-sources/vault) | Apq.Cfg.Vault | 300 | HashiCorp Vault |

## 配置源选择指南

### 开发环境

推荐使用本地文件配置：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                                    // 使用默认层级 0
    .AddJsonFile("config.Development.json", level: 10, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")                   // 使用默认层级 400
    .Build();
```

### 生产环境

推荐使用远程配置中心：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")                    // 使用默认层级 0
    .AddConsul(options => { ... })             // 使用默认层级 200
    .AddVault(options => { ... })              // 使用默认层级 300
    .Build();
```

### 混合部署

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置（使用默认层级 0）
    .AddJsonFile("config.json")
    // 环境特定配置
    .AddJsonFile($"config.{env}.json", level: 10, optional: true)
    // 远程配置（使用默认层级 200）
    .AddConsul(options => { ... })
    // 环境变量覆盖（使用默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
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

使用 `level` 参数控制优先级，数值越大优先级越高：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("base.json", level: 0, writeable: false)           // 优先级 0（最低）
    .AddYamlFile("override.yaml", level: 1, writeable: false)       // 优先级 1
    .AddSource(new ConsulCfgSource(..., level: 10))             // 优先级 10
    .AddEnvironmentVariables(level: 20, prefix: "APP_")         // 优先级 20（最高）
    .Build();
```

## 下一步

选择您需要的配置源了解详细用法：

- [JSON 配置源](/config-sources/json)
- [YAML 配置源](/config-sources/yaml)
- [TOML 配置源](/config-sources/toml)
- [HOCON 配置源](/config-sources/hcl)
- [Properties 配置源](/config-sources/properties)
- [Consul 配置源](/config-sources/consul)
- [Redis 配置源](/config-sources/redis)
