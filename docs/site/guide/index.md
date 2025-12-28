# 简介

Apq.Cfg 是一个高性能的 .NET 配置管理库，旨在提供统一、灵活、高效的配置管理解决方案。

## 为什么选择 Apq.Cfg？

### 🚀 高性能

- **值缓存机制**：避免重复解析，提升读取性能
- **快速集合**：优化的内部数据结构
- **延迟加载**：按需加载配置，减少启动时间

### 📦 多配置源支持

支持多种配置格式和配置中心：

- **本地配置**：JSON、YAML、XML、INI、TOML、环境变量
- **远程配置**：Consul、Redis、Apollo、Vault、Etcd、Zookeeper、Nacos

### 🔄 动态重载

- 文件变更自动检测
- 远程配置实时同步
- 可配置的重载策略
- 变更事件通知

### 💉 依赖注入集成

完美集成 Microsoft.Extensions.DependencyInjection：

```csharp
services.AddApqCfg(cfg => cfg
    .AddJsonFile("config.json")
    .AddConsul("http://localhost:8500", "myapp/config"));

// 使用 IOptions 模式
services.Configure<DatabaseOptions>(cfg.GetSection("Database"));
```

### 🔧 易于扩展

清晰的接口设计，轻松实现自定义配置源：

```csharp
public class MyCustomSource : ICfgSource
{
    public Task<IDictionary<string, string>> LoadAsync()
    {
        // 实现自定义加载逻辑
    }
}
```

## 快速开始

### 安装

```bash
dotnet add package Apq.Cfg
```

### 基本用法

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .Build();

// 读取配置
var value = cfg["Section:Key"];
var typedValue = cfg.GetValue<int>("Section:IntKey");
```

## 兼容性

| 框架 | 版本 |
|------|------|
| .NET | 6.0, 7.0, 8.0, 9.0 |
| .NET Standard | 2.0, 2.1 |

## 下一步

- [安装指南](/guide/installation) - 详细的安装说明
- [快速开始](/guide/quick-start) - 5 分钟上手教程
- [配置源](/config-sources/) - 了解所有支持的配置源
- [API 参考](/api/) - 完整的 API 文档
