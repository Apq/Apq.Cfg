# Apq.Cfg.Toml

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

TOML 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg
- Tomlyn 0.19.0

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Toml;

var cfg = new CfgBuilder()
    .AddToml("config.toml", level: 0, writeable: true)
    .Build();

// 使用配置节访问
var dbSection = cfg.GetSection("Database");
var connStr = dbSection.Get("ConnectionString");

// 枚举子键
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection.Get(key)}");
}
```

## 方法签名

```csharp
public static CfgBuilder AddToml(
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
| `path` | TOML 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `isPrimaryWriter` | 是否为默认写入目标 |

## TOML 格式示例

```toml
AppName = "MyApp"

[Database]
ConnectionString = "Server=localhost;Database=mydb"
Timeout = 30

[Logging]
Level = "Information"
```

配置键映射：
- `AppName` -> `"AppName"`
- `[Database].ConnectionString` -> `"Database:ConnectionString"`
- `[Logging].Level` -> `"Logging:Level"`

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
