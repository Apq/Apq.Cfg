# Apq.Cfg.Xml

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)
[![国内文档](https://img.shields.io/badge/国内文档-CloudBase-green)](https://apq-9g6w58ii54088d8b-1251405840.tcloudbaseapp.com/)

XML 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：
- 国际访问：https://apq-cfg.vercel.app/
- 国内访问：https://apq-9g6w58ii54088d8b-1251405840.tcloudbaseapp.com/

## 依赖

- Apq.Cfg
- Microsoft.Extensions.Configuration.Xml（版本随目标框架：net6.0→6.0.0, net7.0→7.0.0, net8.0→8.0.0, net9.0→9.0.0）

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Xml;

var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0, writeable: true)
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
public static CfgBuilder AddXml(
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
| `path` | XML 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `isPrimaryWriter` | 是否为默认写入目标 |

## XML 格式示例

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <AppName>MyApp</AppName>
    <Database>
        <ConnectionString>Server=localhost;Database=mydb</ConnectionString>
        <Timeout>30</Timeout>
    </Database>
    <Logging>
        <Level>Information</Level>
    </Logging>
</configuration>
```

配置键映射：
- `<AppName>` -> `"AppName"`
- `<Database><ConnectionString>` -> `"Database:ConnectionString"`
- `<Logging><Level>` -> `"Logging:Level"`

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
