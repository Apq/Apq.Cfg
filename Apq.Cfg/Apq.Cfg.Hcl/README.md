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

var appName = cfg["app_name"];
var host = cfg["database:host"];
var port = cfg.GetValue<int>("database:port");
```

## 键路径映射

HOCON 嵌套结构使用冒号分隔的键路径：

| HOCON 路径 | 配置键 |
|------------|--------|
| `app_name` | `app_name` |
| `database.host` | `database:host` |
| `database.connection.timeout` | `database:connection:timeout` |

## HOCON/HCL 格式示例

```hocon
app_name = "MyApp"

database {
    host = "localhost"
    port = 5432
    connection_string = "Server=localhost;Database=mydb"
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
