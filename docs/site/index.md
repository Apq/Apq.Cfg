---
layout: home

hero:
  name: Apq.Cfg
  text: 高性能 .NET 配置组件库
  tagline: 支持多种配置源、动态重载、依赖注入集成
  image:
    src: /logo.svg
    alt: Apq.Cfg
  actions:
    - theme: brand
      text: 快速开始
      link: /guide/
    - theme: alt
      text: 在 Gitee 上查看
      link: https://gitee.com/apq/Apq.Cfg

features:
  - icon: 🚀
    title: 高性能
    details: 基于值缓存和快速集合优化，提供卓越的配置读取性能，支持高并发场景
  - icon: 📦
    title: 多配置源
    details: 支持 JSON、YAML、XML、INI、TOML 等本地格式，以及 Consul、Redis、Apollo、Vault 等远程配置中心
  - icon: 🔄
    title: 动态重载
    details: 支持配置变更监听和自动重载，无需重启应用即可更新配置，支持多种重载策略
  - icon: 💉
    title: 依赖注入
    details: 完美集成 Microsoft.Extensions.DependencyInjection，支持 IOptions/IOptionsSnapshot/IOptionsMonitor 模式
  - icon: 🔧
    title: 易于扩展
    details: 提供清晰的扩展接口，轻松实现自定义配置源，支持源生成器自动生成配置类
  - icon: 📝
    title: 类型安全
    details: 支持强类型配置绑定和源生成器，编译时检查配置错误，减少运行时异常
---

<div class="vp-doc" style="padding: 2rem;">

## 快速安装

::: code-group

```bash [.NET CLI]
# 安装核心包
dotnet add package Apq.Cfg

# 安装 YAML 支持
dotnet add package Apq.Cfg.Yaml

# 安装 Consul 支持
dotnet add package Apq.Cfg.Consul
```

```xml [PackageReference]
<PackageReference Include="Apq.Cfg" Version="1.0.*" />
<PackageReference Include="Apq.Cfg.Yaml" Version="1.0.*" />
<PackageReference Include="Apq.Cfg.Consul" Version="1.0.*" />
```

:::

## 简单示例

```csharp
using Apq.Cfg;

// 创建配置
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 读取配置
var connectionString = cfg["Database:ConnectionString"];
var timeout = cfg.GetValue<int>("Database:Timeout");

// 绑定到强类型对象
var dbConfig = cfg.GetSection("Database").Get<DatabaseConfig>();
```

## 支持的配置源

| 类型 | 配置源 | NuGet 包 |
|------|--------|----------|
| 本地 | JSON | Apq.Cfg (内置) |
| 本地 | YAML | Apq.Cfg.Yaml |
| 本地 | XML | Apq.Cfg.Xml |
| 本地 | INI | Apq.Cfg.Ini |
| 本地 | TOML | Apq.Cfg.Toml |
| 本地 | 环境变量 | Apq.Cfg (内置) |
| 远程 | Consul | Apq.Cfg.Consul |
| 远程 | Redis | Apq.Cfg.Redis |
| 远程 | Apollo | Apq.Cfg.Apollo |
| 远程 | Vault | Apq.Cfg.Vault |
| 远程 | Etcd | Apq.Cfg.Etcd |
| 远程 | Zookeeper | Apq.Cfg.Zookeeper |
| 远程 | Nacos | Apq.Cfg.Nacos |

</div>
