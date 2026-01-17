# Apq.Cfg.Hcl

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

HOCON (Human-Optimized Config Object Notation) 文件配置源扩展包，兼容 HCL 格式。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg
- Hocon.Extensions.Configuration (2.0.4)

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Hcl` (0)。

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Hcl;

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl", level: 0, writeable: true)
    .Build();

var appName = cfg["AppName"];
var db = cfg.GetSection("Database");
var connStr = db["ConnectionString"];
```

## HOCON/HCL 格式示例

```hocon
app_name = "MyApp"

database {
    connection_string = "Server=localhost;Database=mydb"
    timeout = 30
}

[logging]
level = "Information"
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
