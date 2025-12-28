# 配置源概览

Apq.Cfg 支持多种配置源，可以根据项目需求灵活选择和组合。

## 本地配置源

适用于单机部署或配置文件随应用分发的场景。

| 配置源 | 格式特点 | 适用场景 |
|-------|---------|---------|
| [JSON](/sources/json) | 结构化、广泛支持 | 通用配置，与 .NET 生态集成 |
| [YAML](/sources/yaml) | 人类友好、简洁 | DevOps、Kubernetes 配置 |
| [TOML](/sources/toml) | 简洁明了、类型明确 | Rust 生态、简单配置 |
| [XML](/sources/xml) | 结构严谨、支持 Schema | 传统 .NET 项目、企业应用 |
| [INI](/sources/ini) | 简单、轻量 | 简单键值配置 |
| [ENV](/sources/env) | 环境变量格式 | 容器化部署、12-Factor 应用 |

## 远程配置源

适用于分布式系统、微服务架构，需要集中管理配置的场景。

| 配置源 | 特点 | 适用场景 |
|-------|------|---------|
| [Redis](/sources/redis) | 高性能、支持发布订阅 | 高频访问配置、缓存配置 |
| [Database](/sources/database) | 持久化、支持审计 | 需要配置历史记录的场景 |
| [Etcd](/sources/etcd) | 强一致性、分布式 | Kubernetes 生态、服务发现 |
| [Consul](/sources/consul) | 服务发现、健康检查 | HashiCorp 生态、微服务 |
| [Nacos](/sources/nacos) | 阿里巴巴开源、功能丰富 | 阿里云生态、国内企业 |
| [Apollo](/sources/apollo) | 携程开源、灰度发布 | 大型企业、复杂配置管理 |
| [Vault](/sources/vault) | 密钥管理、动态凭证 | 敏感配置、安全要求高 |
| [Zookeeper](/sources/zookeeper) | 分布式协调、强一致 | 大数据生态、分布式系统 |

## 配置源优先级

当使用多个配置源时，后添加的配置源具有更高的优先级：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")           // 优先级 1（最低）
    .AddYamlFile("config.yaml")                // 优先级 2
    .AddEnvironmentVariables()                 // 优先级 3
    .AddConsul("http://consul:8500", "app/")   // 优先级 4（最高）
    .Build();
```

## 选择建议

### 开发环境
- 使用 JSON 或 YAML 本地文件
- 配合 `.gitignore` 管理敏感配置

### 测试环境
- 使用环境变量覆盖配置
- 可选使用 Consul/Nacos 进行集中管理

### 生产环境
- 使用远程配置中心（Nacos/Apollo/Consul）
- 敏感信息使用 Vault 管理
- 配合环境变量进行最终覆盖

详细的选择指南请参考 [配置源选择最佳实践](/best-practices/source-selection)。
