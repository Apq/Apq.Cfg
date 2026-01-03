# API 参考

本节提供 Apq.Cfg 的完整 API 参考文档。

> 文档基于 .NET 10.0 生成，API 与 .NET 8.0 版本完全兼容。

## 命名空间

### 核心

- [Apq.Cfg](Apq.Cfg.html) - 核心配置组件
- [Apq.Cfg.Sources](Apq.Cfg.Sources.html) - 配置源接口
- [Apq.Cfg.Changes](Apq.Cfg.Changes.html) - 动态重载
- [Apq.Cfg.Security](Apq.Cfg.Security.html) - 安全接口
- [Apq.Cfg.DependencyInjection](Apq.Cfg.DependencyInjection.html) - 依赖注入
- [Apq.Cfg.EncodingSupport](Apq.Cfg.EncodingSupport.html) - 编码支持

### 配置验证

- [Apq.Cfg.Validation](Apq.Cfg.Validation.html) - 配置验证核心
- [Apq.Cfg.Validation.Rules](Apq.Cfg.Validation.Rules.html) - 验证规则（Required、Range、Regex、OneOf、Length、DependsOn、Custom）

### 模板引擎

- [Apq.Cfg.Template](Apq.Cfg.Template.html) - 变量替换（支持 `${Key}`、`${ENV:Name}`、`${SYS:Property}` 语法）

### 快照导出

- [Apq.Cfg.Snapshot](Apq.Cfg.Snapshot.html) - 配置快照导出（JSON、YAML、TOML、INI、XML、Env）

### 加密脱敏

- [Apq.Cfg.Crypto](Apq.Cfg.Crypto.html) - 加密脱敏核心
- [Apq.Cfg.Crypto.Providers](Apq.Cfg.Crypto.Providers.html) - 加密提供程序
- [Apq.Cfg.Crypto.DataProtection](Apq.Cfg.Crypto.DataProtection.html) - ASP.NET Core Data Protection 实现

### 命令行工具

- [Apq.Cfg.Crypto.Tool](Apq.Cfg.Crypto.Tool.html) - 配置加密 CLI 工具 (apqenc)

### 本地配置源

- [Apq.Cfg.Ini](Apq.Cfg.Ini.html) - INI 格式
- [Apq.Cfg.Xml](Apq.Cfg.Xml.html) - XML 格式
- [Apq.Cfg.Yaml](Apq.Cfg.Yaml.html) - YAML 格式
- [Apq.Cfg.Toml](Apq.Cfg.Toml.html) - TOML 格式
- [Apq.Cfg.Env](Apq.Cfg.Env.html) - 环境变量/.env 文件

### 数据存储配置源

- [Apq.Cfg.Redis](Apq.Cfg.Redis.html) - Redis 配置源
- [Apq.Cfg.Database](Apq.Cfg.Database.html) - 数据库配置源

### 远程配置中心

- [Apq.Cfg.Consul](Apq.Cfg.Consul.html) - Consul 配置中心
- [Apq.Cfg.Etcd](Apq.Cfg.Etcd.html) - Etcd 配置中心
- [Apq.Cfg.Nacos](Apq.Cfg.Nacos.html) - Nacos 配置中心
- [Apq.Cfg.Apollo](Apq.Cfg.Apollo.html) - Apollo 配置中心
- [Apq.Cfg.Zookeeper](Apq.Cfg.Zookeeper.html) - Zookeeper 配置中心
- [Apq.Cfg.Vault](Apq.Cfg.Vault.html) - HashiCorp Vault

### 源生成器

- [Apq.Cfg.SourceGenerator](Apq.Cfg.SourceGenerator.html) - 编译时强类型配置绑定（Native AOT）
