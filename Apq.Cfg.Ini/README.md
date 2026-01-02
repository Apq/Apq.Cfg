# Apq.Cfg.Ini

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

INI 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg
- Microsoft.Extensions.Configuration.Ini（版本随目标框架：net8.0→8.0.0, net10.0→10.0.1）

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Ini` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddIni("config.ini")

// 指定自定义层级
.AddIni("config.ini", level: 50)
```

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Ini;

var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: true)
    .Build();

// 使用索引器访问
var appName = cfg["AppName"];

// 使用配置节
var db = cfg.GetSection("Database");
var connStr = db["ConnectionString"];
var timeout = db.Get<int>("Timeout");
```

## 方法签名

```csharp
public static CfgBuilder AddIni(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false)
```

## 参数说明

| 参数 | 说明 |
|------|------|
| `path` | INI 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `isPrimaryWriter` | 是否为默认写入目标 |

## INI 格式示例

```ini
; 根级别配置
AppName=MyApp

[Database]
ConnectionString=Server=localhost;Database=mydb
Timeout=30

[Logging]
Level=Information
```

配置键映射：
- `AppName` -> `"AppName"`
- `Database:ConnectionString` -> `"Database:ConnectionString"`
- `Logging:Level` -> `"Logging:Level"`

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
