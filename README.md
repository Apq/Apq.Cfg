# Apq.Cfg

统一配置管理系统，支持多种配置格式和多层级配置合并。

## 项目结构

```text
Apq.Cfg/
├── Apq.Cfg/                 # 核心库（JSON + 环境变量）
├── Apq.Cfg.Ini/             # INI 文件扩展
├── Apq.Cfg.Xml/             # XML 文件扩展
├── Apq.Cfg.Yaml/            # YAML 文件扩展
├── Apq.Cfg.Toml/            # TOML 文件扩展
├── Apq.Cfg.Redis/           # Redis 扩展
├── Apq.Cfg.Database/        # 数据库扩展
├── Apq.Cfg.Tests.Net6/      # .NET 6 测试项目
├── Apq.Cfg.Tests.Net8/      # .NET 8 测试项目
└── Apq.Cfg.Tests.Net9/      # .NET 9 测试项目
```

## 特性

- 多格式支持（JSON、INI、XML、YAML、TOML、Redis、数据库）
- 智能编码检测与统一 UTF-8 写入
- 多层级配置合并
- 可写配置与热重载
- Microsoft.Extensions.Configuration 兼容

## 支持的框架

.NET 6.0 / 7.0 / 8.0 / 9.0

## 快速开始

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
var value = cfg.Get("Database:ConnectionString");

// 修改配置
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();
```

## 构建与测试

```bash
dotnet build
dotnet test
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com
