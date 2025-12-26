# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

统一配置管理系统，支持多种配置格式和多层级配置合并。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、Redis、数据库
- **远程配置中心**：支持 Consul、Etcd、Nacos、Apollo 等配置中心，支持热重载
- **智能编码处理**：
  - 读取时自动检测（BOM 优先，UTF.Unknown 库辅助，支持缓存）
  - 写入时统一 UTF-8 无 BOM（PowerShell 脚本自动使用 UTF-8 BOM）
  - 支持完整路径、通配符、正则表达式三种编码映射方式
- **多层级配置合并**：高层级覆盖低层级
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

## 支持的框架

.NET 6.0 / 7.0 / 8.0 / 9.0

## NuGet 包

| 包名 | 说明 |
|------|------|
| [Apq.Cfg](https://www.nuget.org/packages/Apq.Cfg) | 核心库，包含 JSON 支持 |
| [Apq.Cfg.Ini](https://www.nuget.org/packages/Apq.Cfg.Ini) | INI 格式支持 |
| [Apq.Cfg.Xml](https://www.nuget.org/packages/Apq.Cfg.Xml) | XML 格式支持 |
| [Apq.Cfg.Yaml](https://www.nuget.org/packages/Apq.Cfg.Yaml) | YAML 格式支持 |
| [Apq.Cfg.Toml](https://www.nuget.org/packages/Apq.Cfg.Toml) | TOML 格式支持 |
| [Apq.Cfg.Redis](https://www.nuget.org/packages/Apq.Cfg.Redis) | Redis 配置源 |
| [Apq.Cfg.Database](https://www.nuget.org/packages/Apq.Cfg.Database) | 数据库配置源 |
| [Apq.Cfg.Consul](https://www.nuget.org/packages/Apq.Cfg.Consul) | Consul 配置中心 |
| [Apq.Cfg.Etcd](https://www.nuget.org/packages/Apq.Cfg.Etcd) | Etcd 配置中心 |
| [Apq.Cfg.Nacos](https://www.nuget.org/packages/Apq.Cfg.Nacos) | Nacos 配置中心 |
| [Apq.Cfg.Apollo](https://www.nuget.org/packages/Apq.Cfg.Apollo) | Apollo 配置中心 |
| [Apq.Cfg.Zookeeper](https://www.nuget.org/packages/Apq.Cfg.Zookeeper) | Zookeeper 配置中心 |
| [Apq.Cfg.Vault](https://www.nuget.org/packages/Apq.Cfg.Vault) | HashiCorp Vault 密钥管理 |
| [Apq.Cfg.SourceGenerator](https://www.nuget.org/packages/Apq.Cfg.SourceGenerator) | 源生成器，支持 Native AOT |

## 快速开始

```csharp
using Apq.Cfg;

var cfg = CfgBuilder.Create()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
var value = cfg.Get("Database:ConnectionString");

// 使用配置节简化嵌套访问
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host");
var port = dbSection.Get<int>("Port");

// 枚举配置节的子键
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection.Get(key)}");
}

// 修改配置
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();
```

### 批量操作

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
var cfg = CfgBuilder.Create()
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

### 编码处理

所有文件配置源（JSON、INI、XML、YAML、TOML）均支持智能编码处理：

- **读取时自动检测**：
  - BOM 优先检测（UTF-8、UTF-16 LE/BE、UTF-32 LE/BE）
  - UTF.Unknown 库辅助检测，支持 GBK、GB2312 等常见编码
  - 检测结果自动缓存，文件修改后自动失效
- **写入时统一 UTF-8**：默认使用 UTF-8 无 BOM，PowerShell 脚本（*.ps1、*.psm1、*.psd1）自动使用 UTF-8 BOM
- **编码映射**：支持完整路径、通配符、正则表达式三种匹配方式

```csharp
var cfg = CfgBuilder.Create()
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
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");
services.ConfigureApqCfg<LoggingOptions>("Logging");

var provider = services.BuildServiceProvider();

// 通过 DI 获取配置
var cfgRoot = provider.GetRequiredService<ICfgRoot>();
var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}
```

### 远程配置中心

支持 Consul、Etcd、Nacos、Apollo、Zookeeper 等远程配置中心，支持热重载：

```csharp
using Apq.Cfg;
using Apq.Cfg.Consul;
using Apq.Cfg.Etcd;
using Apq.Cfg.Nacos;
using Apq.Cfg.Apollo;
using Apq.Cfg.Vault;

// 使用 Consul 配置中心
var cfg = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddConsul(options => {
        options.Address = "http://localhost:8500";
        options.KeyPrefix = "app/config/";
        options.EnableHotReload = true;  // 启用热重载
    }, level: 10)
    .Build();

// 使用 Etcd 配置中心
var cfg2 = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddEtcd(options => {
        options.Endpoints = new[] { "http://localhost:2379" };
        options.KeyPrefix = "/app/config/";
        options.EnableHotReload = true;  // 启用热重载
    }, level: 10)
    .Build();

// 使用 Nacos 配置中心
var cfg3 = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddNacos(options => {
        options.ServerAddresses = "localhost:8848";
        options.Namespace = "public";
        options.DataId = "app-config";
        options.Group = "DEFAULT_GROUP";
        options.Username = "nacos";      // 可选
        options.Password = "nacos";      // 可选
        options.DataFormat = NacosDataFormat.Json;  // 支持 Json/Yaml/Properties
    }, level: 10)
    .Build();

// 使用 Apollo 配置中心
var cfg4 = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddApollo(options => {
        options.AppId = "my-app";
        options.MetaServer = "http://localhost:8080";
        options.Cluster = "default";
        options.Namespaces = new[] { "application", "common" };
        options.Secret = "your-secret";  // 可选，用于访问控制
        options.EnableHotReload = true;  // 启用热重载
    }, level: 10)
    .Build();

// 使用 Zookeeper 配置中心
var cfg5 = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddZookeeper(options => {
        options.ConnectionString = "localhost:2181";
        options.RootPath = "/app/config";
        options.EnableHotReload = true;  // 启用热重载
    }, level: 10)
    .Build();

// Zookeeper 简化用法
var cfg5_simple = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddZookeeper("localhost:2181", "/app/config", level: 10)
    .Build();

// Zookeeper JSON 模式
var cfg5_json = CfgBuilder.Create()
    .AddZookeeperJson("localhost:2181", "/app/config.json", level: 10)
    .Build();

// 使用 HashiCorp Vault 密钥管理（KV V2）
var cfg6 = CfgBuilder.Create()
    .AddJson("config.json", level: 0)
    .AddVaultV2(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "kv",
        path: "myapp/config",
        level: 10,
        enableHotReload: true
    )
    .Build();

// 使用 Vault UserPass 认证
var cfg6_userpass = CfgBuilder.Create()
    .AddVaultUserPass(
        address: "http://localhost:8200",
        username: "myapp",
        password: "secure-password",
        enginePath: "kv",
        path: "myapp/production",
        kvVersion: 2,
        level: 10
    )
    .Build();

// 使用 Vault AppRole 认证
var cfg6_approle = CfgBuilder.Create()
    .AddVaultAppRole(
        address: "http://localhost:8200",
        roleId: "role-id-value",
        roleSecret: "role-secret-value",
        enginePath: "kv",
        path: "myapp/staging",
        kvVersion: 2,
        level: 10
    )
    .Build();

// Vault KV V1 引擎支持
var cfg6_v1 = CfgBuilder.Create()
    .AddVaultV1(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "secret",
        path: "myapp",
        level: 10
    )
    .Build();

// 订阅配置变更
cfg.ConfigChanges.Subscribe(change => {
    Console.WriteLine($"配置变更: {change.Key} = {change.NewValue}");
});
```

#### 配置中心对比

| 配置中心 | 写入支持 | 数据格式 | 热重载 | 特点 |
|---------|---------|---------|--------|------|
| **Consul** | ✅ | KV/JSON/YAML | ✅ Blocking Query | KV 存储，支持前缀监听 |
| **Etcd** | ✅ | KV/JSON | ✅ Watch API | 强一致性，支持前缀监听 |
| **Nacos** | ✅ | JSON/YAML/Properties | ❌ | 支持命名空间、分组、多 DataId |
| **Apollo** | ❌ | Properties | ✅ 长轮询 + 通知 | 支持多环境、集群、命名空间 |
| **Zookeeper** | ✅ | KV/JSON | ✅ Watch API | 节点监听，支持会话管理 |
| **Vault** | ✅ | KV (V1/V2) | ✅ 轮询 | 密钥管理，支持 Token/AppRole 认证 |

### 源生成器（Native AOT 支持）

使用 `Apq.Cfg.SourceGenerator` 包可以在编译时生成零反射的配置绑定代码，完全支持 Native AOT：

```bash
dotnet add package Apq.Cfg.SourceGenerator
```

定义配置类时使用 `[CfgSection]` 特性标记，类必须是 `partial` 的：

```csharp
using Apq.Cfg;

[CfgSection("AppSettings")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public int Port { get; set; }
    public DatabaseConfig? Database { get; set; }
}

[CfgSection]
public partial class DatabaseConfig
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
}
```

源生成器会自动生成 `BindFrom` 和 `BindTo` 静态方法：

```csharp
// 构建配置
var cfgRoot = CfgBuilder.Create()
    .AddJson("config.json")
    .AddIni("config.ini")
    .Build();

// 使用源生成器绑定配置（零反射）
var appConfig = AppConfig.BindFrom(cfgRoot.GetSection("AppSettings"));

Console.WriteLine($"App: {appConfig.Name}");
Console.WriteLine($"Port: {appConfig.Port}");
Console.WriteLine($"Database: {appConfig.Database?.ConnectionString}");
```

源生成器支持的类型：
- **简单类型**：`string`、`int`、`long`、`bool`、`double`、`decimal`、`DateTime`、`Guid`、枚举等
- **集合类型**：`T[]`、`List<T>`、`HashSet<T>`、`Dictionary<TKey, TValue>`
- **复杂类型**：嵌套的配置类（需要同样标记 `[CfgSection]`）

> 详细文档见 [Apq.Cfg.SourceGenerator/README.md](Apq.Cfg.SourceGenerator/README.md)

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

**最后运行时间**: 2025-12-26

| 框架 | 通过 | 失败 | 跳过 | 总计 | 状态 |
|------|------|------|------|------|------|
| .NET 6.0 | 290 | 0 | 0 | 290 | ✅ 通过 |
| .NET 8.0 | 290 | 0 | 0 | 290 | ✅ 通过 |
| .NET 9.0 | 290 | 0 | 0 | 290 | ✅ 通过 |

> 详细测试覆盖情况见 [tests/README.md](tests/README.md)

> 详细性能测试结果见 [benchmarks/性能测试对比分析_2025-12-25_223016_vs_2025-12-26_035103.md](benchmarks/性能测试对比分析_2025-12-25_223016_vs_2025-12-26_035103.md)

## 性能亮点

| 场景 | 性能指标 | 说明 |
|------|----------|------|
| **基本读写** | 17-22 ns | Get/Set 操作纳秒级响应 |
| **类型转换** | 67-136 ns | 支持所有标准类型 |
| **批量操作** | 零堆分配 | `GetMany(keys, callback)` 回调版本比返回字典版本快 43-50% |
| **并发读取** | 14-19 μs (16线程) | 高并发场景性能提升 19% |
| **缓存命中** | 1.5-1.7 μs | 缓存性能提升 12% |
| **配置节** | 18-29 ns | GetSection 操作性能提升 10-15% |
| **源生成器** | 2.1-2.7 μs | 比反射绑定快约 100 倍 |
| **DI 解析** | 6-12 ns | Scoped 解析性能极佳 |
| **编码检测** | 30-117 μs | UTF-8 加载性能提升 23% |
| **热重载** | 防抖 + 增量 | 只重载变化的配置源 |

**运行时建议**：推荐 .NET 8.0 或 .NET 9.0，性能比 .NET 6.0 提升 35-55%。

## 项目结构

```
Apq.Cfg/                      # 核心库（JSON、环境变量、DI 集成）
Apq.Cfg.Ini/                 # INI 格式支持
Apq.Cfg.Xml/                 # XML 格式支持
Apq.Cfg.Yaml/                # YAML 格式支持
Apq.Cfg.Toml/                # TOML 格式支持
Apq.Cfg.Redis/               # Redis 配置源
Apq.Cfg.Database/            # 数据库配置源
Apq.Cfg.Consul/              # Consul 配置中心
Apq.Cfg.Etcd/                # Etcd 配置中心
Apq.Cfg.Nacos/               # Nacos 配置中心
Apq.Cfg.Apollo/              # Apollo 配置中心
Apq.Cfg.Zookeeper/           # Zookeeper 配置中心
Apq.Cfg.Vault/               # HashiCorp Vault 密钥管理
Apq.Cfg.SourceGenerator/     # 源生成器（Native AOT 支持）
tests/                       # 单元测试（290 个测试用例）
benchmarks/                  # 性能基准测试（18 个测试类）
docs/                        # 技术文档
Samples/                     # 示例项目
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
