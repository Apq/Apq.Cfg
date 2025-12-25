# Apq.Cfg 改进方向分析

本文档针对架构设计、动态重载、性能优化、编码检测四个方面，分析可能的改进方向。

---

## 1. 架构设计 ⭐⭐⭐⭐ → ⭐⭐⭐⭐⭐

| 改进点 | 说明 |
|--------|------|
| **依赖注入支持** | 添加 `IServiceCollection` 扩展方法，如 `services.AddApqCfg(builder => ...)` |
| **配置源优先级动态调整** | 支持运行时调整 level，而非仅构建时固定 |
| **配置分组/命名空间** | 支持 `cfg.GetSection("Database")` 返回子配置对象 |
| **配置验证** | 添加 `IConfigValidator` 接口，支持启动时验证配置完整性 |
| **插件化加载** | 支持通过反射或 MEF 动态加载扩展包，无需显式引用 |
| **ConvertValue 代码重复** | `MergedCfgRoot` 和 `CfgRootExtensions` 中有重复的类型转换逻辑，应抽取为共享工具类 |

```csharp
// 示例：依赖注入支持
services.AddApqCfg(cfg => cfg
    .AddJson("appsettings.json", level: 0)
    .AddEnvironmentVariables(level: 1)
    .WithValidation<AppSettings>());
```

---

## 2. 动态重载实现 ⭐⭐⭐⭐ → ⭐⭐⭐⭐⭐ ✅ 已完成

| 改进点 | 说明 | 状态 |
|--------|------|------|
| **可配置的重载策略** | 支持 Eager（立即）/ Lazy（访问时）/ Manual（手动）三种模式 | ✅ 已实现 |
| **变更过滤器** | 支持只监听特定 key 前缀的变更，减少不必要的通知 | ✅ 已实现 |
| **变更批次 ID** | 为每次变更事件添加唯一 ID，便于追踪和日志 | ✅ 已实现 |
| **重载失败回滚** | 配置源加载失败时保留旧值，而非清空 | ✅ 已实现 |
| **异步事件处理** | `OnMergedChangesAsync` 支持异步订阅者 | ✅ 已实现 |
| **变更历史** | 可选的变更历史记录，支持查询最近 N 次变更 | ✅ 已实现 |

### 使用示例

```csharp
// 配置动态重载选项
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 100,                           // 防抖时间
    Strategy = ReloadStrategy.Eager,            // 重载策略：Eager/Lazy/Manual
    KeyPrefixFilters = new[] { "Database:" },   // 只监听 Database: 前缀的变更
    RollbackOnError = true,                     // 重载失败时回滚
    HistorySize = 10                            // 保留最近 10 次变更历史
});

// 订阅变更事件（带批次 ID）
cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine($"[{e.BatchId}] {e.Timestamp}: {e.Changes.Count} 个配置变更");
});

// 订阅重载错误事件
cfg.ReloadErrors.Subscribe(e =>
{
    Console.WriteLine($"重载失败: {e.Exception.Message}, 已回滚: {e.RolledBack}");
});

// 手动重载（Manual 策略）
cfg.Reload();

// 获取变更历史
var history = cfg.GetChangeHistory();
```

---

## 3. 性能优化 ⭐⭐⭐⭐ → ⭐⭐⭐⭐⭐

| 改进点 | 说明 |
|--------|------|
| **值缓存** | 对 `Get<T>` 的类型转换结果进行缓存，避免重复解析 |
| **Span/Memory 优化** | 键路径解析使用 `ReadOnlySpan<char>` 避免字符串分配 |
| **对象池** | 使用 `ArrayPool<T>` 替代 `ThreadStatic`，更好的内存复用 |
| **批量操作 API** | 添加 `GetMany(keys)` / `SetMany(dict)` 减少锁竞争 |
| **冷热数据分离** | 高频访问的 key 使用更快的数据结构（如 FrozenDictionary .NET 8+） |
| **Source Generator** | 使用源生成器在编译时生成强类型配置类，零反射 |

```csharp
// 示例：批量操作
var values = cfg.GetMany(new[] { "App:Name", "App:Version", "App:Debug" });

// 示例：Source Generator 生成的强类型配置
[CfgSection("Database")]
public partial class DatabaseConfig
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}
```

---

## 4. 编码检测 ⭐⭐⭐⭐ → ⭐⭐⭐⭐⭐

| 改进点 | 说明 |
|--------|------|
| **编码检测缓存** | 缓存已检测文件的编码结果，避免每次读取都检测 |
| **BOM 优先检测** | 先检查 BOM 标记（更快更准确），再用 UTF.Unknown |
| **编码转换策略** | 支持配置：保持原编码 / 统一转 UTF-8 / 按文件类型决定 |
| **编码检测日志** | 可选的日志输出，记录检测结果和置信度 |
| **自定义编码映射** | 支持用户指定特定文件的编码，覆盖自动检测 |
| **写入编码可配置** | 当前固定 UTF-8 无 BOM，应支持配置（如 .ps1 需要 UTF-8 BOM） |

```csharp
// 示例：编码策略配置
.AddJson("config.json", encoding: new EncodingOptions
{
    ReadStrategy = EncodingStrategy.AutoDetect,
    WriteEncoding = Encoding.UTF8,
    FallbackEncoding = Encoding.GetEncoding("GB2312")
})
```

---

## 优先级建议

| 优先级 | 改进项 | 理由 |
|:------:|--------|------|
| **P0** | 依赖注入支持 | .NET 生态标配，提升易用性 |
| **P0** | ConvertValue 代码去重 | 技术债务，维护成本 |
| **P1** | 批量操作 API | 性能提升明显 |
| **P1** | 重载失败回滚 | 稳定性保障 |
| **P1** | 写入编码可配置 | 实际需求（如 PowerShell） |
| **P2** | 值缓存 | 高频场景性能提升 |
| **P2** | 变更过滤器 | 减少不必要的处理 |
| **P3** | Source Generator | 高级特性，开发成本高 |

---

## 实现路线图

### 阶段一：基础完善

- [ ] 抽取 `ConvertValue` 为共享工具类
- [ ] 添加依赖注入扩展 `Apq.Cfg.DependencyInjection`
- [ ] 写入编码可配置

### 阶段二：稳定性增强

- [x] 重载失败回滚机制 ✅ 2025-12-25
- [x] 变更批次 ID 和日志 ✅ 2025-12-25
- [ ] 编码检测缓存

### 阶段三：性能提升

- [ ] 批量操作 API
- [ ] 值缓存机制
- [ ] FrozenDictionary 支持（.NET 8+）

### 阶段四：高级特性

- [ ] Source Generator 强类型配置
- [ ] 配置验证框架
- [x] 变更历史记录 ✅ 2025-12-25

### 已完成的动态重载改进（2025-12-25）

- [x] 可配置的重载策略（Eager/Lazy/Manual）
- [x] 变更过滤器（KeyPrefixFilters）
- [x] 变更批次 ID（BatchId）
- [x] 重载失败回滚（RollbackOnError）
- [x] 异步事件处理（OnMergedChangesAsync）
- [x] 变更历史记录（HistorySize）

---

*分析日期：2025-12-25*
