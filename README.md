# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

.NET 统一配置组件库，支持多种配置格式和多层级配置合并。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 目录

- [特性](#特性)
- [5分钟上手](#5分钟上手)
- [故障排除](#故障排除)
- [支持的框架](#支持的框架)
- [NuGet 包](#nuget-包)
- [批量操作](#批量操作)
- [动态配置重载](#动态配置重载)
- [配置加密脱敏](#配置加密脱敏)
- [.env 文件支持](#env-文件支持)
- [编码处理](#编码处理)
- [依赖注入集成](#依赖注入集成)
- [配置验证](#配置验证)
- [配置快照导出](#配置快照导出)
- [配置模板与变量替换](#配置模板与变量替换)
- [远程配置中心](#远程配置中心)
- [源生成器（Native AOT 支持）](#源生成器native-aot-支持)
- [构建与测试](#构建与测试)
- [性能亮点](#性能亮点)
- [项目结构](#项目结构)

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、Env、Redis、数据库
- **远程配置中心**：支持 Consul、Etcd、Nacos、Apollo 等配置中心，支持热重载
- **配置加密脱敏**：
  - 敏感配置值（密码、API密钥等）自动加密存储、读取时解密
  - 日志输出时自动脱敏，保护敏感信息
  - 支持 AES-GCM、AES-CBC、ChaCha20、RSA、SM4 等多种加密算法
- **智能编码处理**：
  - 读取时自动检测（BOM 优先，UTF.Unknown 库辅助，支持缓存）
  - 写入时统一 UTF-8 无 BOM
  - 支持完整路径、通配符、正则表达式三种编码映射方式
- **多层级配置合并**：重复的Key处理: 高层级覆盖低层级
- **可写配置**：支持配置修改并持久化到指定配置源
- **热重载**：文件配置源支持变更自动重载
- **动态配置重载**：支持文件变更自动检测、防抖、增量更新
- **配置节**：支持按路径获取配置子节（`GetSection`），简化嵌套配置访问
- **批量操作**：`GetMany`、`SetMany` 减少锁竞争，提升并发性能
  - 支持高性能回调方式（零堆分配）
- **依赖注入集成**：提供 `AddApqCfg` 和 `ConfigureApqCfg<T>` 扩展方法
- **线程安全**：支持多线程并发读写
- **Microsoft.Extensions.Configuration 兼容**：可无缝转换为标准配置接口
- **Rx 支持**：通过 `ConfigChanges` 订阅配置变更事件

## 5分钟上手

以下是最常见的使用场景，帮助您快速上手 Apq.Cfg：

### 基本用法

```csharp
using Apq.Cfg;

// 创建配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .Build();

// 读取配置（使用索引器）
var host = cfg["Database:Host"];
var port = cfg.Get<int>("Database:Port");

// 使用配置节简化嵌套访问
var db = cfg.GetSection("Database");
var name = db["Name"];
var timeout = db.Get<int>("Timeout");

// 修改配置
cfg["App:LastRun"] = DateTime.Now.ToString();
await cfg.SaveAsync();
```

> 更多功能：[动态配置重载](#动态配置重载) | [依赖注入集成](#依赖注入集成) | [远程配置中心](#远程配置中心) | [源生成器](#源生成器native-aot-支持)

## 故障排除

| 问题        | 原因                 | 解决方案                                                       |
| --------- | ------------------ | ---------------------------------------------------------- |
| 配置返回 null | 路径错误/文件不存在         | 使用 `Path.Combine(AppContext.BaseDirectory, "config.json")` |
| 类型转换失败    | 值格式不正确             | 使用 `cfg.Get<int?>("Key") ?? 默认值` 或 `TryGet<T>()`           |
| 远程配置连接失败  | 网络/认证问题            | 添加 `OnConnectFailed` 回调，使用 try-catch 降级到本地配置               |
| 热重载不生效    | 未启用 reloadOnChange | 添加 `reloadOnChange: true` 参数                               |
| 编码乱码      | 非 UTF-8 文件         | 使用 `AddReadEncodingMapping()` 指定编码                         |
| 性能问题      | 未使用高性能 API         | 使用 `GetMany(keys, callback)` 回调方式或源生成器                     |

> 更多详情请查看 [单元测试覆盖分析报告](docs/单元测试覆盖分析报告.md) 或提交 Issue 到 Gitee 仓库。

## 支持的框架

.NET 8.0 / 10.0 (LTS)

### 依赖版本策略

为了最大化兼容性，`Microsoft.Extensions.*` 包根据目标框架使用匹配的版本：

| 目标框架 | Microsoft.Extensions.* 版本 |
|---------|---------------------------|
| net8.0  | 8.0.0                     |
| net10.0 | 10.0.1                    |

这确保下游项目不会被强制升级依赖，避免版本冲突。

## 配置源默认层级

配置源按 `level` 参数排序，数值越大优先级越高。相同键的配置值，高层级会覆盖低层级。

| 层级范围 | 用途 | 配置源 | 默认值 |
|----------|------|--------|--------|
| 0-99 | 基础配置文件 | Json, Ini, Xml, Yaml, Toml | 0 |
| 100-199 | 远程存储 | Redis, Database | 100 |
| 200-299 | 配置中心 | Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| 300-399 | 密钥管理 | Vault | 300 |
| 400+ | 环境相关 | .env, EnvironmentVariables | 400 |

> 层级间隔 100，方便用户在中间插入自定义层级。例如：`config.json` (0) → `config.local.json` (50) → Redis (100)

## NuGet 包

| 包名                                                                                | 说明                   |
| --------------------------------------------------------------------------------- | -------------------- |
| [Apq.Cfg](https://www.nuget.org/packages/Apq.Cfg)                                 | 核心库，包含 JSON 支持       |
| [Apq.Cfg.Ini](https://www.nuget.org/packages/Apq.Cfg.Ini)                         | INI 格式支持             |
| [Apq.Cfg.Xml](https://www.nuget.org/packages/Apq.Cfg.Xml)                         | XML 格式支持             |
| [Apq.Cfg.Yaml](https://www.nuget.org/packages/Apq.Cfg.Yaml)                       | YAML 格式支持            |
| [Apq.Cfg.Toml](https://www.nuget.org/packages/Apq.Cfg.Toml)                       | TOML 格式支持            |
| [Apq.Cfg.Env](https://www.nuget.org/packages/Apq.Cfg.Env)                         | .env 文件格式支持          |
| [Apq.Cfg.Redis](https://www.nuget.org/packages/Apq.Cfg.Redis)                     | Redis 配置源            |
| [Apq.Cfg.Database](https://www.nuget.org/packages/Apq.Cfg.Database)               | 数据库配置源               |
| [Apq.Cfg.Consul](https://www.nuget.org/packages/Apq.Cfg.Consul)                   | Consul 配置中心          |
| [Apq.Cfg.Etcd](https://www.nuget.org/packages/Apq.Cfg.Etcd)                       | Etcd 配置中心            |
| [Apq.Cfg.Nacos](https://www.nuget.org/packages/Apq.Cfg.Nacos)                     | Nacos 配置中心           |
| [Apq.Cfg.Apollo](https://www.nuget.org/packages/Apq.Cfg.Apollo)                   | Apollo 配置中心          |
| [Apq.Cfg.Zookeeper](https://www.nuget.org/packages/Apq.Cfg.Zookeeper)             | Zookeeper 配置中心       |
| [Apq.Cfg.Vault](https://www.nuget.org/packages/Apq.Cfg.Vault)                     | HashiCorp Vault 密钥管理 |
| [Apq.Cfg.Crypto](https://www.nuget.org/packages/Apq.Cfg.Crypto)                   | 配置加密脱敏（含多种算法）        |
| [Apq.Cfg.Crypto.DataProtection](https://www.nuget.org/packages/Apq.Cfg.Crypto.DataProtection) | ASP.NET Core Data Protection 集成 |
| [Apq.Cfg.Crypto.Tool](https://www.nuget.org/packages/Apq.Cfg.Crypto.Tool)         | 加密命令行工具 (dotnet tool) |
| [Apq.Cfg.SourceGenerator](https://www.nuget.org/packages/Apq.Cfg.SourceGenerator) | 源生成器，支持 Native AOT   |

## 批量操作

支持两种批量获取方式：

```csharp
// 方式1：返回字典（简单易用）
var values = cfg.GetMany(new[] { "Key1", "Key2", "Key3" });
foreach (var kv in values)
{
    Console.WriteLine($"{kv.Key}: {kv.Value}");
}

// 方式2：回调方式（高性能，零堆分配）
cfg.GetMany(new[] { "Key1", "Key2", "Key3" }, (key, value) =>
{
    Console.WriteLine($"{key}: {value}");
});

// 带类型转换的批量获取
cfg.GetMany<int>(new[] { "Port1", "Port2" }, (key, value) =>
{
    Console.WriteLine($"{key}: {value}");
});

// 批量设置
cfg.SetMany(new Dictionary<string, string?>
{
    ["Key1"] = "Value1",
    ["Key2"] = "Value2"
});
await cfg.SaveAsync();
```

### 动态配置重载

支持配置文件变更时自动更新，无需重启应用：

```csharp
using Apq.Cfg;
using Apq.Cfg.Changes;
using Microsoft.Extensions.Primitives;

// 构建配置（启用 reloadOnChange）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false, reloadOnChange: true)
    .AddJson("config.local.json", level: 1, writeable: true, reloadOnChange: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 获取支持动态重载的 Microsoft Configuration
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 100,           // 防抖时间窗口（毫秒）
    EnableDynamicReload = true  // 启用动态重载
});

// 方式1：使用 IChangeToken 监听变更
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));

// 方式2：使用 Rx 订阅配置变更事件
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

#### 动态重载特性

- **防抖处理**：批量文件保存时，多次快速变化合并为一次处理
- **增量更新**：只重新加载发生变化的配置源，而非全部重载
- **层级覆盖感知**：只有当最终合并值真正发生变化时才触发通知
- **多源支持**：支持多个配置源同时存在的场景

### 配置加密脱敏

支持敏感配置值的加密存储和日志脱敏，保护密码、API密钥等敏感信息：

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;

// 使用 AES-GCM 加密（推荐）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .AddAesGcmEncryption("base64key...")  // 或使用 AddAesGcmEncryptionFromEnv()
    .AddSensitiveMasking()  // 添加脱敏支持
    .Build();

// 读取时自动解密
var password = cfg.Get("Database:Password");

// 写入时自动加密（匹配敏感键模式的值）
cfg.Set("Database:Password", "newPassword");
await cfg.SaveAsync();
// 文件中保存的是: "Database:Password": "{ENC}base64ciphertext..."

// 日志输出时使用脱敏值
var maskedPassword = cfg.GetMasked("Database:Password");
// 输出: new***ord

// 获取所有配置的脱敏快照（用于调试）
var snapshot = cfg.GetMaskedSnapshot();
```

配置文件中的加密值使用 `{ENC}` 前缀标记：

```json
{
    "Database": {
        "Host": "localhost",
        "Password": "{ENC}base64ciphertext..."
    },
    "Api": {
        "Key": "{ENC}base64ciphertext..."
    }
}
```

#### 命令行工具

使用 `Apq.Cfg.Crypto.Tool` 命令行工具批量加密配置文件：

```bash
# 安装工具
dotnet tool install -g Apq.Cfg.Crypto.Tool

# 生成密钥
apqenc generate-key

# 加密单个值
apqenc encrypt --key "base64key..." --value "mySecret"

# 批量加密配置文件
apqenc encrypt-file --key "base64key..." --file config.json

# 预览将要加密的键
apqenc encrypt-file --key "base64key..." --file config.json --dry-run
```

#### 支持的加密算法

`Apq.Cfg.Crypto` 包内置多种加密算法：

| 算法 | 扩展方法 | 安全级别 | 适用场景 |
|------|----------|----------|----------|
| AES-GCM | `AddAesGcmEncryption` | ⭐⭐⭐⭐⭐ | 推荐首选，认证加密 |
| AES-CBC | `AddAesCbcEncryption` | ⭐⭐⭐⭐ | 兼容性好，需配合 HMAC |
| ChaCha20-Poly1305 | `AddChaCha20Encryption` | ⭐⭐⭐⭐⭐ | 高性能，移动端友好 |
| RSA | `AddRsaEncryption` | ⭐⭐⭐⭐ | 非对称加密，密钥分发 |
| SM4 | `AddSm4Encryption` | ⭐⭐⭐⭐ | 国密算法，合规要求 |
| Triple DES | `AddTripleDesEncryption` | ⭐⭐⭐ | 遗留系统兼容 |

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;

// ChaCha20-Poly1305（高性能）
var cfg1 = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .AddChaCha20Encryption("base64key...")
    .Build();

// SM4 国密算法（合规要求）
var cfg2 = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .AddSm4Encryption("base64key...")
    .Build();

// RSA 非对称加密（密钥分发场景）
var cfg3 = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, isPrimaryWriter: true)
    .AddRsaEncryption("/path/to/private.pem")
    .Build();
```

> 详细文档见 [Apq.Cfg.Crypto/README.md](Apq.Cfg.Crypto/README.md) 和 [配置加密脱敏设计方案](docs/配置加密脱敏设计方案.md)

### .env 文件支持

支持 .env 文件格式，常用于开发环境配置：

```csharp
using Apq.Cfg;
using Apq.Cfg.Env;

var cfg = new CfgBuilder()
    .AddEnv(".env", level: 0)
    .AddEnv(".env.local", level: 1, writeable: true, isPrimaryWriter: true)
    .Build();

// 读取配置（DATABASE__HOST 自动转换为 DATABASE:HOST）
var dbHost = cfg["DATABASE:HOST"];
var dbPort = cfg.Get<int>("DATABASE:PORT");
```

.env 文件示例：

```env
# 应用配置
APP_NAME=MyApp
APP_DEBUG=true

# 数据库配置（使用 __ 表示嵌套）
DATABASE__HOST=localhost
DATABASE__PORT=5432

# 支持引号包裹的值
MESSAGE="Hello, World!"
MULTILINE="Line1\nLine2"

# 支持 export 前缀
export API_KEY=secret123
```

### 编码处理

所有文件配置源（JSON、INI、XML、YAML、TOML、Env）均支持智能编码处理：

- **读取时自动检测**：
  - BOM 优先检测（UTF-8、UTF-16 LE/BE、UTF-32 LE/BE）
  - UTF.Unknown 库辅助检测，支持 GBK、GB2312 等常见编码
  - 检测结果自动缓存，文件修改后自动失效
- **写入时统一 UTF-8**：默认使用 UTF-8 无 BOM，PowerShell 脚本（*.ps1、*.psm1、*.psd1）默认使用 UTF-8 BOM
- **编码映射**：支持完整路径、通配符、正则表达式三种匹配方式

```csharp
var cfg = new CfgBuilder()
    // 为特定文件指定读取编码
    .AddReadEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))
    // 为 PowerShell 脚本指定写入编码（UTF-8 BOM）
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    // 设置编码检测置信度阈值（默认 0.6）
    .WithEncodingConfidenceThreshold(0.7f)
    // 启用编码检测日志
    .WithEncodingDetectionLogging(result => Console.WriteLine($"检测到编码: {result}"))
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 依赖注入集成

支持与 Microsoft.Extensions.DependencyInjection 无缝集成：

```csharp
using Apq.Cfg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();

// 注册 Apq.Cfg 配置
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");

var provider = services.BuildServiceProvider();
var cfgRoot = provider.GetRequiredService<ICfgRoot>();
var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}
```

### 配置验证

支持在构建时或运行时验证配置值的有效性：

```csharp
using Apq.Cfg;
using Apq.Cfg.Validation;

// 构建并验证配置
var (cfg, result) = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)
        .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
        .OneOf("App:Environment", "Development", "Staging", "Production"))
    .BuildAndValidate(throwOnError: false);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"配置错误: {error}");
    }
}

// 运行时验证
cfg.ValidateAndThrow(v => v.Required("App:Name"));

// 依赖注入集成
services.AddApqCfgWithValidation(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)));
```

#### 内置验证规则

| 规则 | 说明 | 示例 |
|------|------|------|
| `Required` | 必填验证 | `.Required("Database:ConnectionString")` |
| `Range` | 范围验证（支持 int/double/decimal） | `.Range("Server:Port", 1, 65535)` |
| `Regex` | 正则表达式验证 | `.Regex("App:Email", @"^[\w-\.]+@...")` |
| `OneOf` | 枚举值验证 | `.OneOf("App:Env", "Dev", "Prod")` |
| `Length` | 长度验证 | `.Length("App:Name", 3, 50)` |
| `DependsOn` | 依赖验证 | `.DependsOn("Db:Password", "Db:Username")` |
| `Custom` | 自定义验证 | `.Custom("Key", v => v?.StartsWith("X"), "错误消息")` |

> 详细文档见 [配置验证指南](docs/site/guide/validation.md)

### 配置快照导出

支持将当前配置状态导出为多种格式，便于调试、备份和迁移：

```csharp
using Apq.Cfg;
using Apq.Cfg.Snapshot;

// 导出为 JSON（默认）
var json = cfg.ExportSnapshot();

// 导出为环境变量格式
var env = cfg.ExportSnapshotAsEnv(prefix: "MYAPP_");

// 导出为字典
var dict = cfg.ExportSnapshotAsDictionary();

// 使用选项自定义导出
var filtered = cfg.ExportSnapshot(new ExportOptions
{
    Format = ExportFormat.Json,
    IncludeMetadata = true,
    MaskSensitiveValues = true,
    ExcludeKeys = new[] { "Secrets:*", "Database:Password" }
});

// 导出到文件
await cfg.ExportSnapshotToFileAsync("config-snapshot.json");
```

#### 导出格式

| 格式 | 说明 | 示例输出 |
|------|------|----------|
| `Json` | 嵌套 JSON 结构 | `{"App":{"Name":"MyApp"}}` |
| `KeyValue` | 扁平键值对 | `App:Name=MyApp` |
| `Env` | 环境变量格式 | `APP__NAME=MyApp` |

### 配置模板与变量替换

支持在配置值中使用变量引用，实现配置的动态组合和复用。

> **注意**：这是 Apq.Cfg 的差异化功能，`Microsoft.Extensions.Configuration` 不支持变量替换。

```csharp
using Apq.Cfg;

// config.json
// {
//     "App": {
//         "Name": "MyApp",
//         "LogPath": "${App:Name}/logs",
//         "DataPath": "${App:Name}/data"
//     },
//     "Paths": {
//         "Home": "${ENV:USERPROFILE}",
//         "Machine": "${SYS:MachineName}"
//     }
// }

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// 使用 GetResolved 获取解析后的值
var logPath = cfg.GetResolved("App:LogPath");
// 返回: "MyApp/logs"

// 引用环境变量
var homePath = cfg.GetResolved("Paths:Home");
// 返回: "C:\Users\username"

// 引用系统属性
var machine = cfg.GetResolved("Paths:Machine");
// 返回: "SERVER01"

// 解析任意模板字符串
var message = cfg.ResolveVariables("Application ${App:Name} running on ${SYS:MachineName}");
// 返回: "Application MyApp running on SERVER01"

// 类型转换
var port = cfg.GetResolved<int>("Settings:Port");
```

#### 变量语法

| 语法 | 说明 | 示例 |
|------|------|------|
| `${Key}` | 引用其他配置键 | `${App:Name}` |
| `${ENV:Name}` | 引用环境变量 | `${ENV:USERPROFILE}` |
| `${SYS:Property}` | 引用系统属性 | `${SYS:MachineName}` |

#### 支持的系统属性

| 属性名 | 说明 |
|--------|------|
| `MachineName` | 计算机名 |
| `UserName` | 当前用户名 |
| `ProcessId` | 当前进程 ID |
| `Now` | 当前时间（ISO 8601 格式） |
| `Today` | 当前日期（yyyy-MM-dd） |
| `ProcessorCount` | 处理器数量 |

> 详细文档见 [配置模板指南](docs/site/guide/template.md)

### 远程配置中心

支持 Consul、Etcd、Nacos、Apollo、Zookeeper 等远程配置中心，支持热重载：

```csharp
using Apq.Cfg;
using Apq.Cfg.Consul;

// 使用 Consul 配置中心
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddConsul(options => {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
        options.EnableHotReload = true;
    }, level: 10)
    .Build();

// 读取配置
var dbHost = cfg["Database:Host"];

// 订阅配置变更
cfg.ConfigChanges.Subscribe(e => {
    foreach (var (key, change) in e.Changes)
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
});
```

#### 配置中心对比

| 配置中心          | 写入  | 热重载 | 适用场景        | 特点              |
| ------------- |:---:|:---:| ----------- | --------------- |
| **Consul**    | ✅   | ✅   | 微服务、服务发现    | KV 存储 + 服务发现一体化 |
| **Etcd**      | ✅   | ✅   | K8s 生态、强一致性 | 高可用、强一致性        |
| **Nacos**     | ✅   | ✅   | 阿里云、国内微服务   | 功能丰富、中文文档好      |
| **Apollo**    | ❌   | ✅   | 大型企业、灰度发布   | 权限管理、多环境支持      |
| **Zookeeper** | ✅   | ✅   | 分布式协调、传统项目  | 成熟稳定、广泛使用       |
| **Vault**     | ✅   | ✅   | 密钥管理、安全敏感   | 审计完善、多种认证       |

> 详细的配置源选择建议请参阅 [配置源选择指南](docs/配置源选择指南.md)

### 源生成器（Native AOT 支持）

使用 `Apq.Cfg.SourceGenerator` 包可以在编译时生成零反射的配置绑定代码：

```csharp
using Apq.Cfg;

[CfgSection("AppSettings")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public int Port { get; set; }
}

// 使用源生成器绑定配置（零反射）
var cfg = new CfgBuilder().AddJson("config.json").Build();
var appConfig = AppConfig.BindFrom(cfg.GetSection("AppSettings"));
```

## 构建与测试

```bash
# 构建
dotnet build

# 运行单元测试
dotnet test

# 运行性能测试（需要管理员权限以获得准确结果）
cd benchmarks/Apq.Cfg.Benchmarks
dotnet run -c Release
```

### 单元测试通过情况

**最后运行时间**: 2026-01-02

| 框架       | 通过  | 失败  | 跳过  | 总计  | 状态   |
| -------- | --- | --- | --- | --- | ---- |
| .NET 8.0 | 456 | 0   | 41  | 497 | ✅ 通过 |
| .NET 10.0 | 456 | 0   | 41  | 497 | ✅ 通过 |

#### 跳过测试说明

共 41 个测试被跳过，原因是需要外部服务支持：

| 配置源       | 跳过数量 | 原因                                                |
| --------- | ---- | ------------------------------------------------- |
| Redis     | 0    | ✅ 已配置                                             |
| Database  | 0    | ✅ 已配置                                             |
| Zookeeper | 6    | 需要 Zookeeper 服务（配置 `TestConnections:Zookeeper`）   |
| Apollo    | 6    | 需要 Apollo 配置中心（配置 `TestConnections:Apollo`）       |
| Consul    | 6    | 需要 Consul 服务（配置 `TestConnections:Consul`）         |
| Etcd      | 6    | 需要 Etcd 服务（配置 `TestConnections:Etcd`）             |
| Nacos     | 9    | 需要 Nacos 配置中心（配置 `TestConnections:Nacos`）         |
| Vault     | 8    | 需要 HashiCorp Vault 服务（配置 `TestConnections:Vault`） |

> 这些测试使用 `[SkippableFact]` 特性，在未配置相应服务时自动跳过。配置服务连接信息后可完整运行，配置文件位于 `tests/appsettings.json`（三个测试项目共用）。

> 详细测试覆盖情况见 [tests/README.md](tests/README.md)

> 详细性能测试结果见 [benchmarks/性能测试对比分析_2025-12-25_223016_vs_2025-12-26_035103.md](benchmarks/性能测试对比分析_2025-12-25_223016_vs_2025-12-26_035103.md)

## 性能亮点

| 场景        | 性能指标            | 说明                                            |
| --------- | --------------- | --------------------------------------------- |
| **基本读写**  | 17-22 ns        | Get/Set 操作纳秒级响应                               |
| **类型转换**  | 67-136 ns       | 支持所有标准类型                                      |
| **批量操作**  | 零堆分配            | `GetMany(keys, callback)` 回调版本比返回字典版本快 43-50% |
| **并发读取**  | 14-19 μs (16线程) | 高并发场景性能提升 19%                                 |
| **缓存命中**  | 1.5-1.7 μs      | 缓存性能提升 12%                                    |
| **配置节**   | 18-29 ns        | GetSection 操作性能提升 10-15%                      |
| **源生成器**  | 2.1-2.7 μs      | 比反射绑定快约 100 倍                                 |
| **DI 解析** | 6-12 ns         | Scoped 解析性能极佳                                 |
| **编码检测**  | 30-117 μs       | UTF-8 加载性能提升 23%                              |
| **热重载**   | 防抖 + 增量         | 只重载变化的配置源                                     |

**运行时建议**：推荐 .NET 8.0 或 .NET 10.0。

## 项目结构

```
Apq.Cfg/                      # 核心库（JSON、环境变量、DI 集成）
Apq.Cfg.Ini/                 # INI 格式支持
Apq.Cfg.Xml/                 # XML 格式支持
Apq.Cfg.Yaml/                # YAML 格式支持
Apq.Cfg.Toml/                # TOML 格式支持
Apq.Cfg.Env/                 # .env 文件格式支持
Apq.Cfg.Redis/               # Redis 配置源
Apq.Cfg.Database/            # 数据库配置源
Apq.Cfg.Consul/              # Consul 配置中心
Apq.Cfg.Etcd/                # Etcd 配置中心
Apq.Cfg.Nacos/               # Nacos 配置中心
Apq.Cfg.Apollo/              # Apollo 配置中心
Apq.Cfg.Zookeeper/           # Zookeeper 配置中心
Apq.Cfg.Vault/               # HashiCorp Vault 密钥管理
Apq.Cfg.Crypto/              # 配置加密脱敏（含多种算法）
Apq.Cfg.Crypto.DataProtection/ # ASP.NET Core Data Protection 集成
Apq.Cfg.Crypto.Tool/         # 加密命令行工具
Apq.Cfg.SourceGenerator/     # 源生成器（Native AOT 支持）
tests/                       # 单元测试
benchmarks/                  # 性能基准测试
docs/                        # 技术文档
Samples/                     # 示例项目
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
