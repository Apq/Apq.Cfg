# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

统一配置管理系统，支持多种配置格式和多层级配置合并。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、Redis、数据库
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
- **依赖注入集成**：提供 `AddApqCfg` 和 `ConfigureApqCfg<T>` 扩展方法
- **线程安全**：支持多线程并发读写
- **Microsoft.Extensions.Configuration 兼容**：可无缝转换为标准配置接口
- **Rx 支持**：通过 `ConfigChanges` 订阅配置变更事件

## 支持的框架

.NET 6.0 / 7.0 / 8.0 / 9.0

## 快速开始

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true)
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

### 动态配置重载

支持配置文件变更时自动更新，无需重启应用：

```csharp
using Apq.Cfg;
using Apq.Cfg.Changes;
using Microsoft.Extensions.Primitives;

// 构建配置（启用 reloadOnChange）
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false, reloadOnChange: true)
    .AddJson("appsettings.local.json", level: 1, writeable: true, reloadOnChange: true)
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
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true));

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

| 框架 | 测试数量 | 状态 |
|------|----------|------|
| .NET 6.0 | 199 | ✅ 全部通过 |
| .NET 8.0 | 199 | ✅ 全部通过 |
| .NET 9.0 | 199 | ✅ 全部通过 |

> 详细测试覆盖情况见 [tests/README.md](tests/README.md)

## 性能测试结果

**测试环境**：

- 系统：Windows 11
- SDK：.NET SDK 9.0.308
- 测试运行时：.NET 6.0.36、.NET 8.0.22、.NET 9.0.11
- 测试日期：2025-12-25

---

### 1. 读写基准测试 (ReadWriteBenchmarks)

#### 基本操作性能

| 操作 | .NET 9.0 | .NET 8.0 | .NET 6.0 | 说明 |
|------|----------|----------|----------|------|
| Get (字符串) | **16 ns** | 16 ns | 26 ns | 零内存分配 |
| Exists | **16 ns** | 16 ns | 26 ns | 零内存分配 |
| Set | **19 ns** | 19 ns | 26 ns | 零内存分配 |
| GetInt | **70 ns** | 75 ns | 164 ns | 类型转换 |

**结论**：

- 所有格式性能接近，差异在误差范围内
- **.NET 8/9 比 .NET 6 快约 38%**（基本操作）
- **.NET 9 类型转换比 .NET 6 快约 57%**
- 内存分配：.NET 9.0 (64B) < .NET 8.0 (88B) < .NET 6.0 (128B)

---

### 2. 大文件基准测试 (LargeFileBenchmarks)

#### 1000 条配置项加载性能

| 格式 | .NET 9.0 | .NET 8.0 | .NET 6.0 | 内存 |
|------|----------|----------|----------|------|
| **Ini** | **197 μs** | 198 μs | 251 μs | 318 KB |
| **Json** | **252 μs** | 257 μs | 267 μs | 380 KB |
| Xml | 633 μs | 527 μs | 571 μs | 1,160 KB |
| Yaml | 849 μs | 974 μs | 1,217 μs | 1,275 KB |
| Toml | 906 μs | 948 μs | 1,200 μs | 2,364 KB |

**结论**：

- **Ini 和 Json 是大文件场景的最佳选择**
- Ini 比 Json 快约 22%，内存少 16%
- Toml 内存占用最高（约为 Json 的 6 倍）
- .NET 9 在 Yaml/Toml 解析上比 .NET 6 快约 25-30%

---

### 3. 并发基准测试 (ConcurrencyBenchmarks)

#### 4 线程并发（Json 配置源）

| 操作 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| 读相同键 | **2.9 μs** | 2.9 μs | 5.7 μs |
| 读不同键 | **3.2 μs** | 3.3 μs | 6.2 μs |
| Exists | **4.6 μs** | 4.5 μs | 12.1 μs |
| 混合读写 | **11.7 μs** | 16.2 μs | 22.2 μs |
| 写不同键 | **14.8 μs** | 18.9 μs | 25.2 μs |
| 写相同键 | **38.0 μs** | 42.0 μs | 43.8 μs |

**结论**：

- 并发读性能优秀，.NET 8/9 比 .NET 6 快约 50%
- 混合读写场景 .NET 9 比 .NET 6 快约 47%
- 线程安全，支持高并发访问

---

### 4. 缓存效果测试 (CacheBenchmarks)

| 场景 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| 热路径读取 (1000次) | **13.7 μs** | 12.9 μs | 21.1 μs |
| 首次访问 | **15 ns** | 15 ns | 23 ns |
| 后续访问 (预热后) | **1.4 μs** | 1.5 μs | 2.4 μs |
| 缓存未命中 (1000次) | **13.6 μs** | 12.7 μs | 22.2 μs |

**结论**：

- 热路径读取性能稳定，.NET 8/9 比 .NET 6 快约 35%
- 首次访问仅需 15 ns，几乎无开销
- 缓存命中与未命中性能差异小，设计合理

---

### 5. 类型转换测试 (TypeConversionBenchmarks)

| 类型 | .NET 9.0 | .NET 8.0 | .NET 6.0 | 内存 (.NET 9) |
|------|----------|----------|----------|---------------|
| String | **18 ns** | 19 ns | 25 ns | 0 B |
| Int | **88 ns** | 74 ns | 143 ns | 64 B |
| Bool | **66 ns** | 94 ns | 107 ns | 64 B |
| Double | **121 ns** | 104 ns | 140 ns | 64 B |
| Guid | **80 ns** | 115 ns | 161 ns | 72 B |
| DateTime | **135 ns** | 203 ns | 209 ns | 64 B |
| Enum | **93 ns** | 157 ns | 240 ns | 64 B |

**结论**：

- .NET 9 在复杂类型转换上优势明显
- Enum 转换 .NET 9 比 .NET 6 快约 61%
- DateTime 转换 .NET 9 比 .NET 6 快约 35%
- 内存分配持续优化：.NET 9 (64B) < .NET 8 (88B) < .NET 6 (128B)

---

### 性能总结

#### 性能排名（综合）

1. **Json** - 综合最优，平衡性好
2. **Ini** - 大文件场景最快，内存最省
3. **Xml** - 中等性能
4. **Yaml** - 较慢但可接受
5. **Toml** - 最慢，内存占用最高

#### 运行时建议

- **推荐 .NET 8.0 或 .NET 9.0**，性能比 .NET 6.0 提升 35-60%
- 内存分配优化显著，GC 压力更小
- .NET 9 在类型转换和并发场景优势明显

#### 使用建议

| 场景 | 推荐格式 | 说明 |
|------|----------|------|
| 高频读写 | Json / Ini | 单次操作 16-20 ns |
| 大配置文件 | Ini / Json | Ini 内存最省 |
| 人类可读性优先 | Yaml / Toml | 性能可接受 |
| 与现有系统集成 | Xml | 兼容性好 |

> 性能测试运行方法见 [benchmarks/README.md](benchmarks/README.md)

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
