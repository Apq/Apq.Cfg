# XML 配置源

XML 是一种经典的配置格式，适合需要严格结构验证的场景。

## 安装

```bash
dotnet add package Apq.Cfg.Xml
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Xml;

var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0)
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0, reloadOnChange: true)
    .AddXml("config.local.xml", level: 1, optional: true, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
await cfg.SaveAsync();
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
| `writeable` | 是否可写（默认 `false`） |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |

## XML 文件格式

### 基本结构

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

### 数组配置

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <Servers>
        <Server>server1.example.com</Server>
        <Server>server2.example.com</Server>
        <Server>server3.example.com</Server>
    </Servers>
    <Endpoints>
        <Endpoint>
            <Name>api</Name>
            <Url>https://api.example.com</Url>
            <Timeout>30</Timeout>
        </Endpoint>
        <Endpoint>
            <Name>auth</Name>
            <Url>https://auth.example.com</Url>
            <Timeout>10</Timeout>
        </Endpoint>
    </Endpoints>
</configuration>
```

### 使用属性

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <Database Host="localhost" Port="5432" Name="mydb" />
    <Logging Level="Information" EnableConsole="true" />
</configuration>
```

## 键路径映射

XML 结构映射为冒号分隔的键路径：

| XML 路径 | 配置键 |
|----------|--------|
| `<AppName>` | `AppName` |
| `<Database><ConnectionString>` | `Database:ConnectionString` |
| `<Servers><Server>[0]` | `Servers:Server:0` |
| `<Endpoints><Endpoint>[0]<Name>` | `Endpoints:Endpoint:0:Name` |
| `<Database Host="...">` | `Database:Host` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddXml("config.xml", level: 0, encoding: options)
    .Build();
```

## 与其他格式混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddXml("config.xml", level: 1, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

## XML vs JSON vs YAML

| 特性 | XML | JSON | YAML |
|------|-----|------|------|
| 可读性 | ⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 注释支持 | ✅ | ❌ | ✅ |
| Schema 验证 | ✅ | ✅ | ❌ |
| 属性支持 | ✅ | ❌ | ❌ |
| 解析速度 | 较慢 | 较快 | 较慢 |
| 工具支持 | 优秀 | 优秀 | 良好 |

## 下一步

- [INI 配置源](/config-sources/ini) - INI 格式支持
- [TOML 配置源](/config-sources/toml) - TOML 格式支持
