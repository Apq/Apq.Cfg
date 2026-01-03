# 配置源选择指南

本文档帮助您根据不同场景选择合适的配置源。

## 配置源分类

### 本地文件配置源

| 配置源 | 适用场景 | 优点 | 缺点 |
|--------|----------|------|------|
| **JSON** | 通用配置、结构化数据 | 可读性好、支持嵌套、IDE 支持好 | 不支持注释（标准 JSON） |
| **YAML** | DevOps 配置、K8s 配置 | 支持注释、简洁、可读性好 | 缩进敏感、解析较慢 |
| **TOML** | Rust/Go 项目、简单配置 | 支持注释、类型明确 | 嵌套结构不够直观 |
| **INI** | 传统应用、简单键值对 | 简单易懂、广泛支持 | 不支持深层嵌套 |
| **XML** | 企业应用、.NET 传统项目 | 结构严谨、支持 Schema 验证 | 冗长、可读性差 |
| **.env** | 开发环境、Docker 配置 | 简单、环境变量兼容 | 仅支持字符串、无嵌套 |

### 数据存储配置源

| 配置源 | 适用场景 | 优点 | 缺点 |
|--------|----------|------|------|
| **Redis** | 高性能缓存、分布式配置 | 高性能、支持发布订阅 | 需要 Redis 服务 |
| **Database** | 配置审计、多租户 | 支持事务、可审计 | 性能较低、需要数据库 |

### 远程配置中心

| 配置中心 | 适用场景 | 优点 | 缺点 |
|----------|----------|------|------|
| **Consul** | 服务发现 + 配置管理 | 多功能、健康检查 | 功能复杂 |
| **Etcd** | K8s 生态、强一致性需求 | 强一致性、高可用 | 资源占用较高 |
| **Nacos** | 阿里云生态、微服务 | 功能丰富、中文文档好 | 依赖较重 |
| **Apollo** | 大型企业、多环境管理 | 权限管理、灰度发布 | 部署复杂、只读 |
| **Zookeeper** | 分布式协调、传统项目 | 成熟稳定、广泛使用 | 配置管理非主要功能 |
| **Vault** | 密钥管理、安全敏感配置 | 安全性高、审计完善 | 学习曲线陡峭 |

## 场景选择建议

### 1. 单体应用 / 小型项目

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson($"config.{env}.json", level: 1, writeable: false, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

**理由**：简单、无外部依赖、与 ASP.NET Core 配置模式一致。

### 2. 微服务架构

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new ConsulCfgSource(options => {
        options.Address = "http://consul:8500";
        options.KeyPrefix = $"services/{serviceName}/";
        options.EnableHotReload = true;
    }, level: 10))
    .Build();
```

**理由**：Consul 同时提供服务发现和配置管理，减少基础设施复杂度。

### 3. Kubernetes 环境

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new YamlFileCfgSource("/config/app-config.yaml", level: 1, 
        writeable: false, optional: false, reloadOnChange: true))
    .AddEnvironmentVariables(level: 2)
    .Build();
```

**理由**：ConfigMap 挂载为 YAML 文件，环境变量注入敏感配置。

### 4. 阿里云 / 国内云环境

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new NacosCfgSource(options => {
        options.ServerAddresses = "mse-xxx.nacos.mse.aliyuncs.com:8848";
        options.AccessKey = Environment.GetEnvironmentVariable("NACOS_AK");
        options.SecretKey = Environment.GetEnvironmentVariable("NACOS_SK");
        options.DataId = "app-config";
        options.EnableHotReload = true;
    }, level: 10))
    .Build();
```

**理由**：Nacos 是阿里云 MSE 原生支持的配置中心，中文文档完善。

### 5. 多环境 / 灰度发布

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new ApolloCfgSource(options => {
        options.AppId = "my-app";
        options.MetaServer = "http://apollo-meta:8080";
        options.Cluster = Environment.GetEnvironmentVariable("APOLLO_CLUSTER") ?? "default";
        options.Namespaces = new[] { "application", "common" };
        options.EnableHotReload = true;
    }, level: 10))
    .Build();
```

**理由**：Apollo 提供完善的多环境、多集群、灰度发布支持。

### 6. 安全敏感配置 / 密钥管理

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSource(new VaultCfgSource(
        address: "https://vault.example.com:8200",
        roleId: Environment.GetEnvironmentVariable("VAULT_ROLE_ID")!,
        roleSecret: Environment.GetEnvironmentVariable("VAULT_SECRET_ID")!,
        enginePath: "kv",
        path: "apps/my-app/secrets",
        kvVersion: 2,
        level: 10
    ))
    .Build();
```

**理由**：Vault 专为密钥管理设计，提供审计、轮换、访问控制等安全特性。

### 7. 传统企业应用 / 遗留系统

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddSource(new XmlFileCfgSource("app.config", level: 0, writeable: false))
    .AddSource(new IniFileCfgSource("settings.ini", level: 1, writeable: false))
    .AddSource(new DatabaseCfgSource(options => {
        options.Provider = "SqlServer";
        options.ConnectionString = connectionString;
        options.Table = "AppConfig";
    }, level: 2))
    .Build();
```

**理由**：兼容现有配置格式，数据库配置便于审计和管理。

### 8. 开发环境 / 本地调试

**推荐配置**：
```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.Development.json", level: 1, writeable: false, optional: true)
    .AddSource(new EnvFileCfgSource(".env", level: 2, optional: true))
    .AddSource(new EnvFileCfgSource(".env.local", level: 3, optional: true))
    .AddEnvironmentVariables(level: 4, prefix: "APP_")
    .Build();
```

**理由**：.env 文件便于本地开发，不会提交到版本控制。

## 配置源组合最佳实践

### 层级设计原则

```
level 0-9:   本地文件配置（基础配置）
level 10-19: 远程配置中心（动态配置）
level 20+:   环境变量/命令行（覆盖配置）
```

### 推荐的通用模式

```csharp
var cfg = new CfgBuilder()
    // 基础配置（打包在应用中）
    .AddJson("config.json", level: 0, writeable: false)
    
    // 环境特定配置（可选）
    .AddJson($"config.{env}.json", level: 1, writeable: false, optional: true)
    
    // 本地覆盖（开发用，不提交版本控制）
    .AddJson("config.local.json", level: 2, writeable: true, optional: true, isPrimaryWriter: true)
    
    // 远程配置中心（生产环境）
    .AddSource(new ConsulCfgSource(options => { /* ... */ }, level: 10))
    
    // 密钥管理（敏感配置）
    .AddSource(new VaultCfgSource(options => { /* ... */ }, level: 15))
    
    // 环境变量（最高优先级覆盖）
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    
    .Build();
```

## 性能对比

| 配置源 | 首次加载 | 读取性能 | 热重载延迟 |
|--------|----------|----------|------------|
| JSON | ~1ms | ~20ns | ~100ms |
| YAML | ~5ms | ~20ns | ~100ms |
| Redis | ~10ms | ~100μs | ~10ms |
| Consul | ~50ms | ~20ns | ~100ms |
| Etcd | ~50ms | ~20ns | ~50ms |
| Nacos | ~100ms | ~20ns | ~100ms |
| Vault | ~100ms | ~20ns | ~30s (轮询) |

## 决策流程图

```
需要配置管理？
├── 单体应用 → JSON + 环境变量
├── 微服务
│   ├── 已有 Consul → Consul
│   ├── K8s 环境 → Etcd 或 ConfigMap
│   └── 阿里云 → Nacos
├── 需要灰度发布 → Apollo
├── 需要密钥管理 → Vault
└── 传统企业应用 → Database
```

## 迁移建议

### 从 config.json 迁移到配置中心

1. **保持本地配置作为默认值**
2. **配置中心只存储需要动态修改的配置**
3. **敏感配置使用 Vault 或环境变量**
4. **逐步迁移，先在测试环境验证**

### 从其他配置库迁移

Apq.Cfg 兼容 `Microsoft.Extensions.Configuration`，可以通过 `ToMicrosoftConfiguration()` 方法与现有代码集成：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// 转换为 IConfiguration，兼容现有代码
IConfigurationRoot msConfig = cfg.ToMicrosoftConfiguration();
```

## 下一步

- [配置源概述](/config-sources/) - 了解各配置源详情
- [最佳实践](/guide/best-practices) - 配置管理最佳实践
- [复杂场景示例](/examples/complex-scenarios) - 企业级应用示例
