# Apq.Cfg.Yaml

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

YAML 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg
- YamlDotNet 16.3.0

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Yaml` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddYaml("config.yaml")

// 指定自定义层级
.AddYaml("config.yaml", level: 50)
```

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;

var cfg = new CfgBuilder()
    .AddYaml("config.yaml", level: 0, writeable: true)
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
