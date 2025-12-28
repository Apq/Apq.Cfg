---
layout: home

hero:
  name: "Apq.Cfg"
  text: ".NET 配置管理框架"
  tagline: 强大、灵活、可扩展的统一配置解决方案
  image:
    src: /logo.svg
    alt: Apq.Cfg
  actions:
    - theme: brand
      text: 快速开始
      link: /guide/getting-started
    - theme: alt
      text: 在 Gitee 上查看
      link: https://gitee.com/AlanPoon/Apq.Cfg

features:
  - icon: 🔌
    title: 多配置源支持
    details: 支持 JSON、YAML、TOML、XML、INI、ENV 等本地格式，以及 Redis、Database、Etcd、Consul、Nacos、Apollo、Vault、Zookeeper 等远程配置中心
  - icon: ⚡
    title: 高性能
    details: 基于源代码生成器的零反射绑定，内置缓存机制，经过基准测试验证的卓越性能
  - icon: 🔄
    title: 动态重载
    details: 支持配置热更新，无需重启应用即可生效，支持变更通知和回调
  - icon: 🛡️
    title: 类型安全
    details: 强类型配置绑定，编译时检查，IDE 智能提示支持
  - icon: 🎯
    title: 依赖注入
    details: 与 Microsoft.Extensions.DependencyInjection 无缝集成，支持 IOptions 模式
  - icon: 📦
    title: 模块化设计
    details: 按需引用配置源包，最小化依赖，灵活组合
---

## 快速体验

```bash
# 安装核心包
dotnet add package Apq.Cfg

# 安装需要的配置源（以 YAML 为例）
dotnet add package Apq.Cfg.Yaml
```

```csharp
// 创建配置
var cfg = new CfgBuilder()
    .AddYamlFile("config.yaml")
    .Build();

// 读取配置
var connectionString = cfg.Get<string>("Database:ConnectionString");
var maxRetries = cfg.Get<int>("App:MaxRetries", 3);
```

## 支持的配置源

<div class="source-grid">

| 本地配置源 | 远程配置源 |
|-----------|-----------|
| ✅ JSON | ✅ Redis |
| ✅ YAML | ✅ Database |
| ✅ TOML | ✅ Etcd |
| ✅ XML | ✅ Consul |
| ✅ INI | ✅ Nacos |
| ✅ ENV | ✅ Apollo |
| | ✅ Vault |
| | ✅ Zookeeper |

</div>

## 为什么选择 Apq.Cfg？

- **统一 API** - 无论使用哪种配置源，API 保持一致
- **零配置启动** - 合理的默认值，开箱即用
- **生产就绪** - 经过充分测试，支持 .NET 6/8/9
- **活跃维护** - 持续更新，快速响应问题
