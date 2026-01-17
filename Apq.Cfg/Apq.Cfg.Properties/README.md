# Apq.Cfg.Properties

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Java Properties 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Properties` (0)。

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Properties;

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties", level: 0, writeable: true)
    .Build();

var appName = cfg["AppName"];
var db = cfg.GetSection("Database");
var connStr = db["ConnectionString"];
```

## Properties 格式示例

```properties
# 根级别配置
AppName=MyApp

[Database]
ConnectionString=Server=localhost;Database=mydb
Timeout=30

[Logging]
Level=Information
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
