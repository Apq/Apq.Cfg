# Apq.Cfg

统一配置管理系统，支持多种配置格式和多层级配置合并。

## 项目结构

```text
Apq.Cfg/
├── Apq.Cfg/                 # 核心库（JSON + 环境变量）
├── Apq.Cfg.Ini/             # INI 文件扩展
├── Apq.Cfg.Xml/             # XML 文件扩展
├── Apq.Cfg.Yaml/            # YAML 文件扩展
├── Apq.Cfg.Toml/            # TOML 文件扩展
├── Apq.Cfg.Redis/           # Redis 扩展
├── Apq.Cfg.Database/        # 数据库扩展
├── Samples/                 # 示例项目
│   └── Apq.Cfg.Samples/
├── tests/                   # 单元测试
│   ├── Apq.Cfg.Tests.Shared/    # 共享测试代码
│   ├── Apq.Cfg.Tests.Net6/      # .NET 6 测试项目
│   ├── Apq.Cfg.Tests.Net8/      # .NET 8 测试项目
│   └── Apq.Cfg.Tests.Net9/      # .NET 9 测试项目
├── benchmarks/              # 性能测试
│   └── Apq.Cfg.Benchmarks/      # 性能测试项目（多目标框架）
├── buildTools/              # 构建工具脚本
├── versions/                # 版本文件目录
└── nupkgs/                  # NuGet 包输出目录
```

## 特性

- 多格式支持（JSON、INI、XML、YAML、TOML、Redis、数据库）
- 智能编码检测与统一 UTF-8 写入
- 多层级配置合并
- 可写配置与热重载
- 线程安全（支持多线程并发读写）
- Microsoft.Extensions.Configuration 兼容

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

// 修改配置
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();
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

## 测试覆盖情况

### 测试统计（共 54 个测试）

| 测试类 | 测试数量 | 说明 |
|--------|----------|------|
| JsonCfgTests | 8 | JSON 配置源测试 |
| EnvVarsCfgTests | 4 | 环境变量配置源测试 |
| IniCfgTests | 5 | INI 文件配置源测试 |
| XmlCfgTests | 5 | XML 文件配置源测试 |
| YamlCfgTests | 6 | YAML 文件配置源测试 |
| TomlCfgTests | 6 | TOML 文件配置源测试 |
| RedisCfgTests | 5 | Redis 配置源测试 |
| DatabaseCfgTests | 5 | 数据库配置源测试 |
| CfgRootExtensionsTests | 4 | 扩展方法测试 |
| CfgBuilderAdvancedTests | 6 | 高级功能测试 |

### 公开 API 覆盖矩阵

| API | Json | Env | Ini | Xml | Yaml | Toml | Redis | DB |
|-----|:----:|:---:|:---:|:---:|:----:|:----:|:-----:|:--:|
| **ICfgRoot** |
| `Get(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Get<T>(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | - | - |
| `Exists(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Set(key, value)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Set(key, value, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `Remove(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Remove(key, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `SaveAsync()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `SaveAsync(targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration()` | ✅ | - | - | - | - | - | - | - |
| **CfgBuilder** |
| `AddJson()` | ✅ | - | - | - | - | - | - | - |
| `AddEnvironmentVariables()` | - | ✅ | - | - | - | - | - | - |
| `WithEncodingConfidenceThreshold()` | ✅ | - | - | - | - | - | - | - |
| `Build()` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CfgRootExtensions** |
| `TryGet<T>()` | ✅ | - | - | - | - | - | - | - |
| `GetRequired<T>()` | ✅ | - | - | - | - | - | - | - |
| **扩展包** |
| `AddIni()` | - | - | ✅ | - | - | - | - | - |
| `AddXml()` | - | - | - | ✅ | - | - | - | - |
| `AddYaml()` | - | - | - | - | ✅ | - | - | - |
| `AddToml()` | - | - | - | - | - | ✅ | - | - |
| `AddRedis()` | - | - | - | - | - | - | ✅ | - |
| `AddDatabase()` | - | - | - | - | - | - | - | ✅ |
| **多层级覆盖** |
| 高层级覆盖低层级 | ✅ | ✅ | - | - | - | - | ✅ | ✅ |

> 说明：`-` 表示该配置源不支持此功能（如环境变量只读）或该功能只需测试一次

### 测试覆盖率

**100%** - 所有公开 API 均已覆盖测试

### 多框架测试通过情况

| 框架 | 测试数量 | 状态 | 测试日期 |
|------|----------|------|----------|
| .NET 6.0 | 54 | ✅ 全部通过 | 2025-12-23 |
| .NET 8.0 | 54 | ✅ 全部通过 | 2025-12-23 |
| .NET 9.0 | 54 | ✅ 全部通过 | 2025-12-23 |

## 性能测试结果

**测试环境**：

- 系统：Windows 11
- SDK：.NET SDK 9.0.308
- 测试运行时：.NET 6.0、.NET 8.0、.NET 9.0

---

### 1. 读写基准测试 (ReadWriteBenchmarks)

#### 获取整数值 (GetInt) - 最快操作

| 格式 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| Json | **80 ns** | 90 ns | 142 ns |
| Xml | 83 ns | 88 ns | 143 ns |
| Toml | 83 ns | 88 ns | 144 ns |
| Yaml | 84 ns | 87 ns | 143 ns |
| Ini | 84 ns | 89 ns | 140 ns |

**结论**：

- 所有格式性能接近，差异在误差范围内
- **.NET 9.0 比 .NET 6.0 快约 40-45%**
- 内存分配：.NET 9.0 (64B) < .NET 8.0 (88B) < .NET 6.0 (128B)

#### 其他操作耗时排序

| 操作 | 耗时范围 |
|------|----------|
| GetInt | 80-143 ns |
| Exists | 288-330 ns |
| Set | 303-356 ns |
| Get (字符串) | 324-380 ns |

---

### 2. 大文件基准测试 (LargeFileBenchmarks)

#### 100 条配置项

| 格式 | 最快运行时 | 耗时 | 内存 |
|------|-----------|------|------|
| **Json** | .NET 8.0 | **259 μs** | 61 KB |
| **Ini** | .NET 8.0 | **260 μs** | 53 KB |
| Xml | .NET 8.0 | 334 μs | 154 KB |
| Yaml | .NET 9.0 | 412 μs | 156 KB |
| Toml | .NET 9.0 | 514 μs | 258 KB |

#### 1000 条配置项

| 格式 | 最快运行时 | 耗时 | 内存 |
|------|-----------|------|------|
| **Ini** | .NET 6.0 | **589 μs** | 327 KB |
| **Json** | .NET 9.0 | **667 μs** | 381 KB |
| Xml | .NET 9.0 | 1,082 μs | 1,161 KB |
| Yaml | .NET 9.0 | 1,524 μs | 1,277 KB |
| Toml | .NET 9.0 | 1,502 μs | 2,365 KB |

#### 5000 条配置项

| 格式 | 最快运行时 | 耗时 | 内存 |
|------|-----------|------|------|
| **Ini** | .NET 6.0 | **2,460 μs** | 1,524 KB |
| **Json** | .NET 6.0 | **2,707 μs** | 1,800 KB |
| Xml | .NET 9.0 | 4,666 μs | 5,730 KB |
| Yaml | .NET 9.0 | 6,802 μs | 6,226 KB |
| Toml | .NET 9.0 | 8,326 μs | 11,644 KB |

**结论**：

- **Ini 和 Json 是大文件场景的最佳选择**
- Toml 内存占用最高（约为 Json 的 6 倍）
- 数据量越大，格式间差异越明显

---

### 3. 并发基准测试 (ConcurrencyBenchmarks)

#### 单线程 (ThreadCount=1)

| 操作 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| 读不同键 | **37 μs** | 47 μs | 54 μs |
| 读相同键 | **38 μs** | 46 μs | 52 μs |
| 写不同键 | **39 μs** | 42 μs | 46 μs |
| 混合读写 | **66 μs** | 68 μs | 76 μs |
| Exists | 72 μs | 64 μs | 78 μs |

#### 8 线程并发

| 操作 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| 读相同键 | 455 μs | **399 μs** | 446 μs |
| 写不同键 | **439 μs** | 452 μs | 422 μs |
| 混合读写 | **645 μs** | 779 μs | 896 μs |
| Exists | 924 μs | 1,030 μs | 885 μs |

**结论**：

- 并发读写性能良好，线程数增加时耗时近似线性增长
- .NET 9.0 在混合读写场景优势明显（比 .NET 6.0 快 28%）
- 内存分配随 .NET 版本升级持续优化

---

### 性能总结

#### 性能排名（综合）

1. **Json** - 综合最优，平衡性好
2. **Ini** - 大文件场景最快，内存最省
3. **Xml** - 中等性能
4. **Yaml** - 较慢但可接受
5. **Toml** - 最慢，内存占用最高

#### 运行时建议

- **推荐 .NET 8.0 或 .NET 9.0**，性能比 .NET 6.0 提升 30-45%
- 内存分配优化显著，GC 压力更小

#### 使用建议

| 场景 | 推荐格式 |
|------|----------|
| 高频读写 | Json / Ini |
| 大配置文件 | Ini / Json |
| 人类可读性优先 | Yaml / Toml |
| 与现有系统集成 | Xml |

> 性能测试运行方法见 [benchmarks/README.md](benchmarks/README.md)

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com
