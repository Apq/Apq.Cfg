# 简介

Apq.Cfg 是一个高性能的 .NET 配置组件库，旨在提供统一、灵活、高效的配置管理解决方案。

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
    .AddJsonFile("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));

// 使用 IOptions 模式
services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### 🔧 易于扩展

清晰的接口设计，轻松实现自定义配置源：

```csharp
public class MyCustomSource : ICfgSource
{
    public int Level { get; }
    public bool IsWriteable { get; }

    public Task<IDictionary<string, string?>> LoadAsync(CancellationToken cancellationToken)
    {
        // 实现自定义加载逻辑
    }
}
```

### 🔐 加密脱敏

内置配置加密和脱敏功能，保护敏感信息：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv()  // 自动解密 {ENC} 前缀的值
    .AddSensitiveMasking()          // 日志输出时自动脱敏
    .Build();

// 读取时自动解密
var password = cfg["Database:Password"];

// 日志输出时脱敏
logger.LogInfo("密码: {0}", cfg.GetMasked("Database:Password"));
// 输出: 密码: myS***ord
```

### 📝 配置模板

支持变量引用，实现配置的动态组合和复用（`Microsoft.Extensions.Configuration` 不支持此功能）：

```csharp
// config.json: { "App:Name": "MyApp", "App:LogPath": "${App:Name}/logs" }
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .Build();

// 解析变量引用
var logPath = cfg.GetResolved("App:LogPath");
// 返回: "MyApp/logs"

// 引用环境变量和系统属性
var home = cfg.GetResolved("Paths:Home");     // ${ENV:USERPROFILE}
var machine = cfg.GetResolved("Paths:Machine"); // ${SYS:MachineName}
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
    .AddJsonFile("config.json", level: 0, writeable: false)
    .Build();

// 读取配置
var value = cfg["Section:Key"];
var typedValue = cfg.GetValue<int>("Section:IntKey");
```

## 兼容性

| 框架 | 版本 |
|------|------|
| .NET | 8.0, 10.0 (LTS) |

## 下一步

- [安装指南](/guide/installation) - 详细的安装说明
- [快速开始](/guide/quick-start) - 5 分钟上手教程
- [迁移指南](/guide/migration) - 从 Microsoft.Extensions.Configuration 迁移
- [配置源](/config-sources/) - 了解所有支持的配置源
- [配置模板](/guide/template) - 变量引用与动态配置
- [加密脱敏](/guide/encryption-masking) - 保护敏感配置
- [API 参考](/api/) - 完整的 API 文档
