# INI 配置源

INI 是一种简单的配置格式，适合简单的键值对配置。

## 安装

```bash
dotnet add package Apq.Cfg.Ini
```

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Ini;

var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: false)
    .Build();
```

### 可选文件和重载

```csharp
var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: false, reloadOnChange: true)
    .AddIni("config.local.ini", level: 1, writeable: false, optional: true, reloadOnChange: true)
    .Build();
```

### 可写配置

```csharp
var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: true, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg.Set("Database:Host", "newhost");
await cfg.SaveAsync();
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

## INI 文件格式

### 基本结构

```ini
; 这是注释
# 这也是注释

; 根级别配置
AppName=MyApp
Version=1.0.0

[Database]
Host=localhost
Port=5432
Name=mydb
Username=admin
Password=secret

[Logging]
Level=Information
EnableConsole=true
```

### 多行值

```ini
[Query]
; 使用反斜杠续行
SQL=SELECT * FROM users \
    WHERE active = true \
    ORDER BY created_at DESC
```

## 键路径映射

INI 结构映射为冒号分隔的键路径：

| INI 路径 | 配置键 |
|----------|--------|
| `AppName` | `AppName` |
| `[Database] Host` | `Database:Host` |
| `[Database] Port` | `Database:Port` |
| `[Logging] Level` | `Logging:Level` |

## 高级选项

### 指定编码

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.Specified,
    ReadEncoding = Encoding.GetEncoding("GBK")
};

var cfg = new CfgBuilder()
    .AddIni("config.ini", level: 0, writeable: false, encoding: options)
    .Build();
```

### 处理遗留 GBK 编码文件

```csharp
var cfg = new CfgBuilder()
    .AddReadEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GBK"))
    .AddWriteEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GBK"))
    .AddIni("legacy.ini", level: 0, writeable: true)
    .Build();
```

## 与其他格式混合使用

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddIni("config.ini", level: 1, writeable: false, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

## INI 格式特点

| 特性 | 说明 |
|------|------|
| 简单性 | ⭐⭐⭐⭐⭐ 最简单的配置格式 |
| 注释支持 | ✅ 支持 `;` 和 `#` 注释 |
| 嵌套支持 | ❌ 仅支持一级节（Section） |
| 数组支持 | ❌ 不原生支持 |
| 类型支持 | ❌ 所有值都是字符串 |

## 适用场景

- 简单的应用配置
- 遗留系统配置
- Windows 应用程序配置
- 需要人工编辑的配置文件

## 下一步

- [TOML 配置源](/config-sources/toml) - TOML 格式支持
- [环境变量](/config-sources/env) - 环境变量配置
