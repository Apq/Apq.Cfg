# YAML 配置源

YAML 是一种人类友好的数据序列化格式，适合复杂配置。

## 安装

```bash
dotnet add package Apq.Cfg.Yaml
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Yaml` (0)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 0
.AddYamlFile("config.yaml")

// 指定自定义层级
.AddYamlFile("config.yaml", level: 5)
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Yaml;

var cfg = new CfgBuilder()
    .AddYamlFile("config.yaml")  // 使用默认层级 0
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddYamlFile("config.yaml", reloadOnChange: true)
    .AddYamlFile("config.local.yaml", level: 1, optional: true, reloadOnChange: true)
    .Build();
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
| `writeable` | 是否可写（默认 `false`） |
| `optional` | 文件不存在时是否忽略（默认 `true`） |
| `reloadOnChange` | 文件变更时是否自动重载（默认 `true`） |
| `isPrimaryWriter` | 是否为默认写入目标（默认 `false`） |

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddYamlFile("config.yaml", writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg["App:Name"] = "NewName";
await cfg.SaveAsync();
```

## YAML 文件格式

### 基本结构

```yaml
App:
  Name: MyApplication
  Version: 1.0.0

Database:
  Host: localhost
  Port: 5432
  Database: mydb
  Username: admin
  Password: secret

Logging:
  Level: Information
  EnableConsole: true
```

### 数组配置

```yaml
Servers:
  - server1.example.com
  - server2.example.com
  - server3.example.com

Endpoints:
  - Name: api
    Url: https://api.example.com
    Timeout: 30
  - Name: auth
    Url: https://auth.example.com
    Timeout: 10
```

### 多行字符串

```yaml
Description: |
  这是一个多行描述。
  可以包含多个段落。
  
  保留换行符。

Query: >
  SELECT * FROM users
  WHERE active = true
  ORDER BY created_at DESC
```

### 锚点和别名

```yaml
defaults: &defaults
  Timeout: 30
  RetryCount: 3

ApiService:
  <<: *defaults
  Url: https://api.example.com

AuthService:
  <<: *defaults
  Url: https://auth.example.com
  Timeout: 10  # 覆盖默认值
```

## 键路径映射

YAML 结构映射为冒号分隔的键路径：

| YAML 路径 | 配置键 |
|-----------|--------|
| `App.Name` | `App:Name` |
| `Database.Host` | `Database:Host` |
| `Servers[0]` | `Servers:0` |
| `Endpoints[0].Name` | `Endpoints:0:Name` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.UTF8
};

var cfg = new CfgBuilder()
    .AddYamlFile("config.yaml", encoding: options)
    .Build();
```

### 从流加载

```csharp
using var stream = File.OpenRead("config.yaml");
var cfg = new CfgBuilder()
    .AddSource(new YamlStreamCfgSource(stream))
    .Build();
```

## 与 JSON 混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml", level: 1, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## YAML vs JSON

| 特性 | YAML | JSON |
|------|------|------|
| 可读性 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| 注释支持 | ✅ | ❌ |
| 多行字符串 | ✅ | ❌ |
| 锚点/别名 | ✅ | ❌ |
| 解析速度 | 较慢 | 较快 |
| 工具支持 | 良好 | 优秀 |

## 下一步

- [XML 配置源](/config-sources/xml) - XML 格式支持
- [TOML 配置源](/config-sources/toml) - TOML 格式支持
