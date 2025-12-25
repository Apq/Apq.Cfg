# Apq.Cfg 配置系统与同类开源组件对比

## 一、项目概述

**Apq.Cfg** 是一个统一配置管理系统，采用模块化设计，核心库 + 扩展包的架构。

### 项目结构

| 项目 | 说明 |
|------|------|
| **Apq.Cfg** | 核心库（JSON + 环境变量） |
| **Apq.Cfg.Ini** | INI 文件扩展 |
| **Apq.Cfg.Xml** | XML 文件扩展 |
| **Apq.Cfg.Yaml** | YAML 文件扩展 |
| **Apq.Cfg.Toml** | TOML 文件扩展 |

### 核心 API

#### ICfgRoot 接口

| 方法 | 说明 |
|------|------|
| `Get(key)` / `Get<T>(key)` | 读取配置值（支持泛型类型转换） |
| `Exists(key)` | 检查配置键是否存在 |
| `Set(key, value, targetLevel?)` | 设置配置值 |
| `Remove(key, targetLevel?)` | 删除配置键 |
| `SaveAsync(targetLevel?)` | 持久化配置到文件 |
| `GetSection(key)` | 获取配置节 |
| `GetChildKeys()` | 获取所有顶级配置键 |
| `GetMany(keys)` / `GetMany<T>(keys)` | 批量获取配置值 |
| `SetMany(values, targetLevel?)` | 批量设置配置值 |
| `ToMicrosoftConfiguration()` | 转换为 IConfigurationRoot |
| `ConfigChanges` | Rx 配置变更订阅 |

#### CfgBuilder 构建器

| 方法 | 说明 |
|------|------|
| `AddJson(path, level, writeable, ...)` | 添加 JSON 配置源 |
| `AddEnvironmentVariables(level, prefix?)` | 添加环境变量配置源 |
| `AddSource(source)` | 添加自定义配置源 |
| `WithEncodingConfidenceThreshold(threshold)` | 设置编码检测置信度阈值 |
| `WithEncodingDetectionLogging(handler)` | 启用编码检测日志 |
| `AddReadEncodingMapping(...)` | 添加读取编码映射（完整路径/通配符/正则） |
| `AddWriteEncodingMapping(...)` | 添加写入编码映射（完整路径/通配符/正则） |
| `Build()` | 构建 ICfgRoot 实例 |

### 核心特性

1. **多层级配置合并** - 支持 level 参数控制优先级，高层级覆盖低层级
2. **可写配置** - 支持运行时修改并持久化到指定配置源
3. **动态重载** - 文件变更自动检测、防抖、增量更新
4. **智能编码检测** - 使用 UTF.Unknown 库自动检测文件编码
5. **Rx 支持** - 通过 `IObservable<ConfigChangeEvent>` 订阅变更
6. **线程安全** - 使用 `ConcurrentDictionary` 和 `Interlocked` 操作
7. **Microsoft.Extensions.Configuration 兼容** - 无缝转换为标准配置接口
8. **依赖注入集成** - 提供 `AddApqCfg` 和 `ConfigureApqCfg<T>` 扩展方法

---

## 二、与同类开源组件对比

### 对比组件

| 组件 | 定位 | 特点 |
|------|------|------|
| **Microsoft.Extensions.Configuration** | .NET 官方标准配置框架 | 只读、扩展性好、生态完善 |
| **NetEscapades.Configuration.Yaml** | YAML 扩展 | 为 MS Configuration 添加 YAML 支持 |
| **Tomlyn** | TOML 解析库 | 高性能 TOML 解析 |
| **Apollo** | 携程分布式配置中心 | 功能完整、管理界面、灰度发布 |
| **Nacos** | 阿里分布式配置中心 | 服务发现 + 配置管理 |
| **Consul** | HashiCorp 服务网格 | KV 存储、服务发现 |

### 功能对比矩阵

| 功能 | Apq.Cfg | MS.Extensions | Apollo | Nacos | Consul |
|------|:-------:|:-------------:|:------:|:-----:|:------:|
| **格式支持** |
| JSON | ✅ | ✅ | ✅ | ✅ | ✅ |
| YAML | ✅ | ❌(需扩展) | ✅ | ✅ | ❌ |
| INI | ✅ | ✅ | ❌ | ❌ | ❌ |
| XML | ✅ | ✅ | ❌ | ❌ | ❌ |
| TOML | ✅ | ❌ | ❌ | ❌ | ❌ |
| 环境变量 | ✅ | ✅ | ✅ | ✅ | ✅ |
| **核心功能** |
| 可写配置 | ✅ | ❌ | ✅ | ✅ | ✅ |
| 多层级合并 | ✅ | ✅ | ✅ | ✅ | ❌ |
| 指定层级写入 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 动态重载 | ✅ | ✅ | ✅ | ✅ | ✅ |
| 防抖处理 | ✅ | ❌ | ✅ | ✅ | ❌ |
| 增量更新 | ✅ | ❌ | ✅ | ✅ | ❌ |
| **高级功能** |
| Rx 订阅 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 编码检测 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 编码映射 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 配置节 (GetSection) | ✅ | ✅ | ✅ | ✅ | ❌ |
| 批量操作 | ✅ | ❌ | ✅ | ✅ | ✅ |
| 类型转换 | ✅ | ✅ | ✅ | ✅ | ❌ |
| DI 集成 | ✅ | ✅ | ✅ | ✅ | ✅ |
| **分布式特性** |
| 分布式一致性 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 管理界面 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 灰度发布 | ❌ | ❌ | ✅ | ✅ | ❌ |
| 权限控制 | ❌ | ❌ | ✅ | ✅ | ✅ |
| 版本管理 | ❌ | ❌ | ✅ | ✅ | ❌ |

### 详细对比分析

#### 1. vs Microsoft.Extensions.Configuration

| 维度 | Apq.Cfg | MS.Extensions.Configuration |
|------|---------|----------------------------|
| **可写性** | ✅ 支持写入并持久化 | ❌ 只读 |
| **层级控制** | ✅ 可指定 targetLevel 写入 | ❌ 无法控制写入目标 |
| **变更通知** | ✅ Rx + IChangeToken | ⚠️ 仅 IChangeToken |
| **编码处理** | ✅ 自动检测 + 映射 | ❌ 需手动处理 |
| **批量操作** | ✅ GetMany/SetMany | ❌ 需循环调用 |
| **格式支持** | ✅ 内置 5 种格式 | ⚠️ 需安装多个扩展包 |
| **学习成本** | ⚠️ 新 API | ✅ 官方标准 |
| **生态** | ⚠️ 新项目 | ✅ 完善 |

**评价**：Apq.Cfg 在功能上是 MS.Extensions.Configuration 的超集，特别是可写配置和编码处理是其独特优势。但 MS Configuration 作为官方标准，生态更完善，文档更丰富。

#### 2. vs Apollo/Nacos

| 维度 | Apq.Cfg | Apollo/Nacos |
|------|---------|--------------|
| **部署复杂度** | ✅ 零依赖，直接使用 | ❌ 需部署服务端 |
| **分布式支持** | ❌ 本地配置 | ✅ 分布式配置中心 |
| **管理界面** | ❌ 无 | ✅ Web 管理界面 |
| **灰度发布** | ❌ 不支持 | ✅ 支持 |
| **权限控制** | ❌ 无 | ✅ 完善 |
| **性能** | ✅ 纳秒级读取 | ⚠️ 网络延迟 |
| **离线可用** | ✅ 完全离线 | ⚠️ 需缓存 |

**评价**：Apq.Cfg 和 Apollo/Nacos 定位不同。Apq.Cfg 是轻量级本地配置库，适合单机应用或不需要分布式配置的场景。Apollo/Nacos 是企业级分布式配置中心，适合微服务架构。

---

## 三、技术亮点分析

### 1. 多层级合并机制 ⭐⭐⭐⭐⭐

这是 **Apq.Cfg 的核心差异化特性**：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "MYAPP_")
    .Build();

// 读取时自动从最高优先级获取
var value = cfg.Get("Database:Host");

// 写入时可指定目标层级
cfg.Set("App:LastRun", DateTime.Now.ToString(), targetLevel: 1);
await cfg.SaveAsync(targetLevel: 1);
```

**优势**：
- 比 MS Configuration 的简单覆盖更灵活
- 支持指定层级写入，避免污染高优先级配置源
- 环境变量可以覆盖文件配置，但不会被写入

### 2. 动态重载实现 ⭐⭐⭐⭐⭐

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", reloadOnChange: true)
    .Build();

var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 100,           // 防抖时间
    EnableDynamicReload = true,  // 启用动态重载
    Strategy = ReloadStrategy.Eager,  // 立即重载
    RollbackOnError = true,      // 错误时回滚
    HistorySize = 5              // 保留历史记录
});

// Rx 订阅
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

**特点**：
- **防抖处理**：使用 Timer 合并快速连续变更
- **增量更新**：只重新加载变化的配置源
- **层级感知**：只有最终合并值变化才触发通知
- **错误回滚**：解析失败时自动回滚到之前的配置

### 3. 智能编码处理 ⭐⭐⭐⭐⭐

```csharp
var cfg = new CfgBuilder()
    // 设置编码检测置信度阈值
    .WithEncodingConfidenceThreshold(0.7f)
    // 编码检测日志
    .WithEncodingDetectionLogging(result =>
    {
        Console.WriteLine($"编码: {result.Encoding.EncodingName}");
        Console.WriteLine($"置信度: {result.Confidence:P0}");
        Console.WriteLine($"方法: {result.Method}");
    })
    // 完整路径映射
    .AddReadEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))
    // 通配符映射
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    // 正则表达式映射
    .AddReadEncodingMappingRegex(@"config.*\.json$", Encoding.UTF8)
    .AddJson("config.json")
    .Build();
```

**特点**：
- BOM 优先检测（UTF-8、UTF-16 LE/BE、UTF-32 LE/BE）
- UTF.Unknown 库辅助检测，支持 GBK、GB2312 等
- 检测结果自动缓存，文件修改后自动失效
- 支持完整路径、通配符、正则表达式三种映射方式

### 4. 性能优化 ⭐⭐⭐⭐⭐

基于 BenchmarkDotNet 的性能测试结果（.NET 9.0）：

| 操作 | 性能 | 说明 |
|------|------|------|
| 单次读取 | **17 ns** | 零内存分配 |
| 单次写入 | **20 ns** | 零内存分配 |
| 类型转换 (Int) | **60 ns** | 特化处理 |
| GetSection | **19 ns** | 轻量级封装 |
| 1000 项加载 (INI) | **186 μs** | 最快格式 |
| 1000 项加载 (JSON) | **251 μs** | 次快格式 |
| 4 线程并发读 | **6.5 μs** | 线程安全 |
| ToMicrosoftConfiguration | **66 ns** | 快速转换 |

**优化手段**：
- 缓存排序后的层级数组 `_levelsDescending`
- `ThreadStatic` 复用集合避免 GC
- 类型转换特化处理避免反射
- `Interlocked` 原子操作保证线程安全

### 5. 依赖注入集成 ⭐⭐⭐⭐

```csharp
var services = new ServiceCollection();

// 注册配置服务
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", writeable: true, isPrimaryWriter: true));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");
services.ConfigureApqCfg<LoggingOptions>("Logging");

// 使用
var provider = services.BuildServiceProvider();
var cfgRoot = provider.GetRequiredService<ICfgRoot>();
var msConfig = provider.GetRequiredService<IConfigurationRoot>();
var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
```

---

## 四、测试覆盖

### 单元测试

**最后运行时间**: 2025-12-25

| 框架 | 通过 | 失败 | 跳过 | 总计 | 状态 |
|------|------|------|------|------|------|
| .NET 6.0 | 253 | 0 | 0 | 253 | ✅ 通过 |
| .NET 8.0 | 253 | 0 | 0 | 253 | ✅ 通过 |
| .NET 9.0 | 253 | 0 | 0 | 253 | ✅ 通过 |

### 性能测试

12 个基准测试类，覆盖：
- 读写基准测试
- 大文件基准测试
- 并发基准测试
- 缓存效果测试
- 类型转换测试
- 批量操作测试
- 配置节测试
- 保存测试
- 删除测试
- 多源合并测试
- 键路径深度测试
- Microsoft Configuration 转换测试

---

## 五、综合评价

### 评分

| 维度 | 评分 | 说明 |
|------|:----:|------|
| **架构设计** | ⭐⭐⭐⭐⭐ | 模块化清晰，扩展性好，Builder 模式流畅 |
| **功能完整性** | ⭐⭐⭐⭐⭐ | 格式支持全面，特性丰富，API 设计合理 |
| **性能** | ⭐⭐⭐⭐⭐ | 纳秒级读取，零内存分配，优化到位 |
| **代码质量** | ⭐⭐⭐⭐⭐ | 线程安全，内存优化，异常处理完善 |
| **测试覆盖** | ⭐⭐⭐⭐⭐ | 253 测试，多框架验证，性能基准完整 |
| **文档** | ⭐⭐⭐⭐ | README 完整，示例丰富，API 文档清晰 |
| **生态/社区** | ⭐⭐ | 新项目，尚无社区 |

### 总体评分：⭐⭐⭐⭐½ (4.5/5)

### 适用场景

✅ **推荐使用**：
- 需要多格式配置支持的 .NET 应用
- 需要可写配置的场景（如用户偏好设置、运行时配置修改）
- 需要多层级配置合并（开发/测试/生产环境覆盖）
- 对性能有要求的高频配置读取场景
- 需要处理多种编码的配置文件（如遗留系统迁移）
- 需要 Rx 响应式配置变更订阅

❌ **不推荐使用**：
- 需要分布式配置中心功能（推荐 Apollo/Nacos）
- 需要配置审计、权限控制
- 需要配置版本管理和回滚
- 需要 Web 管理界面

### 与竞品定位对比

```
轻量级 ◄─────────────────────────────────────────────► 重量级

MS.Extensions.Configuration (只读，标准)
        │
        ▼
    Apq.Cfg (可写，功能丰富，本地配置)
        │
        ▼
    Apollo / Nacos / Consul (分布式配置中心)
```

**Apq.Cfg 填补了 MS Configuration 和分布式配置中心之间的空白**：
- 比官方库更丰富的功能（可写、Rx、编码检测、批量操作）
- 保持轻量级和高性能（纳秒级读取）
- 无需部署额外服务，开箱即用

---

## 六、迁移指南

### 从 MS.Extensions.Configuration 迁移

```csharp
// 之前 (MS Configuration)
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var value = config["Database:ConnectionString"];

// 之后 (Apq.Cfg)
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson($"config.{env}.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2)
    .Build();

var value = cfg.Get("Database:ConnectionString");

// 如果需要兼容现有代码，可以转换为 IConfigurationRoot
var msConfig = cfg.ToMicrosoftConfiguration();
```

### 主要差异

| 功能 | MS Configuration | Apq.Cfg |
|------|------------------|---------|
| 配置文件名 | `appsettings.json` | `config.json`（建议） |
| 添加配置源 | `AddJsonFile(path)` | `AddJson(path, level, writeable)` |
| 读取配置 | `config["key"]` | `cfg.Get("key")` |
| 类型转换 | `config.GetValue<T>("key")` | `cfg.Get<T>("key")` |
| 配置节 | `config.GetSection("key")` | `cfg.GetSection("key")` |
| 写入配置 | ❌ 不支持 | `cfg.Set("key", "value")` |
| 保存配置 | ❌ 不支持 | `await cfg.SaveAsync()` |

---

*分析日期：2025-12-25*
*基于 Apq.Cfg v1.0.2*
