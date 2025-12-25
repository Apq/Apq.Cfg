# Apq.Cfg.Yaml

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

YAML 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 依赖

- Apq.Cfg
- YamlDotNet 16.3.0

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;

var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: true)
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
public static CfgBuilder AddYaml(
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
| `path` | YAML 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `isPrimaryWriter` | 是否为默认写入目标 |

## YAML 格式示例

```yaml
AppName: MyApp

Database:
  ConnectionString: Server=localhost;Database=mydb
  Timeout: 30

Logging:
  Level: Information
```

配置键映射：
- `AppName` -> `"AppName"`
- `Database.ConnectionString` -> `"Database:ConnectionString"`
- `Logging.Level` -> `"Logging:Level"`

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
