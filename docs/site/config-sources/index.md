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
| [.env 文件](/config-sources/env#env-文件支持) | Apq.Cfg.Env | .env 格式配置文件 |
| [环境变量](/config-sources/env) | Apq.Cfg (内置) | 系统环境变量 |

### 数据存储配置源

从数据存储系统读取配置：

| 配置源 | NuGet 包 | 说明 |
|--------|----------|------|
| [Redis](/config-sources/redis) | Apq.Cfg.Redis | Redis 键值存储 |
| [Database](/config-sources/database) | Apq.Cfg.Database | 数据库配置源（支持多种数据库） |

### 远程配置中心

从远程配置中心读取配置：

| 配置源 | NuGet 包 | 说明 |
|--------|----------|------|
| [Consul](/config-sources/consul) | Apq.Cfg.Consul | HashiCorp Consul |
| [Apollo](/config-sources/apollo) | Apq.Cfg.Apollo | 携程 Apollo 配置中心 |
| [Nacos](/config-sources/nacos) | Apq.Cfg.Nacos | 阿里巴巴 Nacos |
| [Vault](/config-sources/vault) | Apq.Cfg.Vault | HashiCorp Vault |
| [Etcd](/config-sources/etcd) | Apq.Cfg.Etcd | Etcd 分布式键值存储 |
| [Zookeeper](/config-sources/zookeeper) | Apq.Cfg.Zookeeper | Apache Zookeeper |

## 配置源选择指南

### 开发环境

推荐使用本地文件配置：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.Development.json", level: 1, writeable: false, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### 生产环境

推荐使用远程配置中心：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)  // 基础配置
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10))  // 动态配置
    .AddSource(new VaultCfgSource("https://vault:8200", "secret/myapp", level: 15))   // 敏感配置
    .Build();
```

### 混合部署

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置
    .AddJson("config.json", level: 0, writeable: false)
    // 环境特定配置
    .AddJson($"config.{env}.json", level: 1, writeable: false, optional: true)
    // 远程配置（可选，用于动态更新）
    .AddSource(new ConsulCfgSource("http://consul:8500", "myapp/config", level: 10, optional: true))
    // 环境变量覆盖
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
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
    .AddJson("base.json", level: 0, writeable: false)           // 优先级 0（最低）
    .AddYaml("override.yaml", level: 1, writeable: false)       // 优先级 1
    .AddSource(new ConsulCfgSource(..., level: 10))             // 优先级 10
    .AddEnvironmentVariables(level: 20, prefix: "APP_")         // 优先级 20（最高）
    .Build();
```

## 下一步

选择您需要的配置源了解详细用法：

- [JSON 配置源](/config-sources/json)
- [YAML 配置源](/config-sources/yaml)
- [Consul 配置源](/config-sources/consul)
- [Redis 配置源](/config-sources/redis)
