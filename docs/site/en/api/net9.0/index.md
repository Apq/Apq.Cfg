# API 参考 (.NET 9.0)

本节包含 Apq.Cfg 所有公开 API 的详细文档，由代码注释自动生成。

> 当前文档基于 .NET 9.0 版本生成。各版本 API 基本一致，仅内部实现有差异。

## 核心库

- [Apq.Cfg](./core/) - 核心配置库（JSON、环境变量、DI 集成）

## 加密脱敏

- [Apq.Cfg.Crypto](./crypto/) - 配置加密脱敏
- [Apq.Cfg.Crypto.DataProtection](./crypto-dp/) - ASP.NET Core DataProtection 集成

## 本地配置源

- [Apq.Cfg.Ini](./ini/) - INI 格式支持
- [Apq.Cfg.Xml](./xml/) - XML 格式支持
- [Apq.Cfg.Yaml](./yaml/) - YAML 格式支持
- [Apq.Cfg.Toml](./toml/) - TOML 格式支持
- [Apq.Cfg.Env](./env/) - .env 文件支持

## 远程配置源

- [Apq.Cfg.Redis](./redis/) - Redis 配置源
- [Apq.Cfg.Consul](./consul/) - Consul 配置中心
- [Apq.Cfg.Etcd](./etcd/) - Etcd 配置中心
- [Apq.Cfg.Nacos](./nacos/) - Nacos 配置中心
- [Apq.Cfg.Apollo](./apollo/) - Apollo 配置中心
- [Apq.Cfg.Zookeeper](./zookeeper/) - Zookeeper 配置中心
- [Apq.Cfg.Vault](./vault/) - HashiCorp Vault
- [Apq.Cfg.Database](./database/) - 数据库配置源