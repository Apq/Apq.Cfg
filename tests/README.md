# Apq.Cfg 测试

本目录包含 Apq.Cfg 的单元测试项目。

## 项目结构

```text
tests/
├── Apq.Cfg.Tests.Shared/    # 共享测试代码
├── Apq.Cfg.Tests.Net6/      # .NET 6 测试项目
├── Apq.Cfg.Tests.Net8/      # .NET 8 测试项目
└── Apq.Cfg.Tests.Net9/      # .NET 9 测试项目
```

## 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定框架的测试
dotnet test tests/Apq.Cfg.Tests.Net9/

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~JsonCfgTests"
```

## 测试统计（共 253 个测试）

| 测试类 | 测试数量 | 说明 |
|--------|----------|------|
| JsonCfgTests | 15 | JSON 配置源测试 |
| EnvVarsCfgTests | 4 | 环境变量配置源测试 |
| IniCfgTests | 5 | INI 文件配置源测试 |
| XmlCfgTests | 5 | XML 文件配置源测试 |
| YamlCfgTests | 6 | YAML 文件配置源测试 |
| TomlCfgTests | 6 | TOML 文件配置源测试 |
| RedisCfgTests | 5 | Redis 配置源测试 |
| DatabaseCfgTests | 5 | 数据库配置源测试 |
| CfgRootExtensionsTests | 4 | 扩展方法测试（TryGet/GetRequired）|
| CfgBuilderAdvancedTests | 14 | 高级功能测试 |
| DynamicReloadTests | 22 | 动态配置重载测试 |
| EncodingDetectionTests | 14 | 编码检测测试 |
| ConcurrencyTests | 9 | 并发安全测试 |
| BoundaryConditionTests | 25 | 边界条件测试 |
| ExceptionHandlingTests | 18 | 异常处理测试 |
| ConfigChangesSubscriptionTests | 28 | 配置变更订阅测试 |
| CfgSectionTests | 13 | 配置节（GetSection/GetChildKeys/GetOrDefault）测试 |
| ServiceCollectionExtensionsTests | 11 | 依赖注入扩展测试 |
| EncodingTests | 22 | 编码映射测试 |
| PerformanceOptimizationTests | 22 | 性能优化测试（GetMany/SetMany/缓存）|

## 公开 API 覆盖矩阵

| API | Json | Env | Ini | Xml | Yaml | Toml | Redis | DB |
|-----|:----:|:---:|:---:|:---:|:----:|:----:|:-----:|:--:|
| **ICfgRoot** |
| `Get(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Get<T>(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | - | - |
| `Exists(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `GetMany(keys)` | ✅ | - | - | - | - | - | - | - |
| `GetMany<T>(keys)` | ✅ | - | - | - | - | - | - | - |
| `Set(key, value)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `SetMany(values)` | ✅ | - | - | - | - | - | - | - |
| `Set(key, value, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `Remove(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Remove(key, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `SaveAsync()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `SaveAsync(targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration()` | ✅ | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration(options)` | ✅ | - | - | - | - | - | - | - |
| `ConfigChanges` | ✅ | - | - | - | - | - | - | - |
| `GetSection(path)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | - | - |
| `GetChildKeys()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | - | - |
| `Dispose/DisposeAsync` | ✅ | - | - | - | - | - | - | - |
| **CfgBuilder** |
| `AddJson()` | ✅ | - | - | - | - | - | - | - |
| `AddEnvironmentVariables()` | - | ✅ | - | - | - | - | - | - |
| `AddSource()` | ✅ | - | - | - | - | - | - | - |
| `WithEncodingConfidenceThreshold()` | ✅ | - | - | - | - | - | - | - |
| `AddReadEncodingMapping()` | ✅ | - | - | - | - | - | - | - |
| `AddWriteEncodingMapping()` | ✅ | - | - | - | - | - | - | - |
| `ConfigureEncodingMapping()` | ✅ | - | - | - | - | - | - | - |
| `WithEncodingDetectionLogging()` | ✅ | - | - | - | - | - | - | - |
| `Build()` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CfgRootExtensions** |
| `TryGet<T>()` | ✅ | - | - | - | - | - | - | - |
| `GetRequired<T>()` | ✅ | - | - | - | - | - | - | - |
| `GetOrDefault<T>()` | ✅ | - | - | - | - | - | - | - |
| **FileCfgSourceBase** |
| `EncodingDetector` | ✅ | - | - | - | - | - | - | - |
| `EncodingConfidenceThreshold` | ✅ | - | - | - | - | - | - | - |
| **扩展包** |
| `AddIni()` | - | - | ✅ | - | - | - | - | - |
| `AddXml()` | - | - | - | ✅ | - | - | - | - |
| `AddYaml()` | - | - | - | - | ✅ | - | - | - |
| `AddToml()` | - | - | - | - | - | ✅ | - | - |
| `AddRedis()` | - | - | - | - | - | - | ✅ | - |
| `AddDatabase()` | - | - | - | - | - | - | - | ✅ |
| **依赖注入扩展** |
| `AddApqCfg()` | ✅ | - | - | - | - | - | - | - |
| `ConfigureApqCfg<T>()` | ✅ | - | - | - | - | - | - | - |
| **多层级覆盖** |
| 高层级覆盖低层级 | ✅ | ✅ | - | - | - | - | ✅ | ✅ |

> 说明：`-` 表示该配置源不支持此功能（如环境变量只读）或该功能只需测试一次

## 测试场景覆盖

| 场景类别 | 测试文件 | 测试数量 |
|----------|----------|----------|
| 基本读写 | JsonCfgTests, 各格式测试 | 47 |
| 类型转换 | JsonCfgTests | 15 |
| 编码检测 | EncodingDetectionTests | 14 |
| 编码映射 | EncodingTests | 22 |
| 并发安全 | ConcurrencyTests | 9 |
| 边界条件 | BoundaryConditionTests | 25 |
| 异常处理 | ExceptionHandlingTests | 18 |
| 动态重载 | DynamicReloadTests | 22 |
| 变更订阅 | ConfigChangesSubscriptionTests | 28 |
| 配置节访问 | CfgSectionTests | 13 |
| 依赖注入 | ServiceCollectionExtensionsTests | 11 |
| 批量操作 | PerformanceOptimizationTests | 22 |

## 性能基准测试

性能基准测试位于 `benchmarks/Apq.Cfg.Benchmarks/` 目录，使用 BenchmarkDotNet 框架。

| 基准测试文件 | 说明 |
|--------------|------|
| ReadWriteBenchmarks | 不同配置源的 Get/Set/Exists 性能对比 |
| CacheBenchmarks | 缓存效果测试（热路径、缓存命中/未命中）|
| TypeConversionBenchmarks | 类型转换性能测试（含 TryGet/GetRequired/GetOrDefault）|
| ConcurrencyBenchmarks | 并发读写性能测试 |
| GetSectionBenchmarks | GetSection/GetChildKeys 性能测试 |
| SaveBenchmarks | SaveAsync 持久化性能测试 |
| RemoveBenchmarks | Remove 操作性能测试 |
| MultiSourceBenchmarks | 多配置源合并性能测试 |
| LargeFileBenchmarks | 大文件配置性能测试 |
| KeyPathBenchmarks | 键路径解析性能测试 |
| BatchOperationBenchmarks | GetMany/SetMany 批量操作性能测试 |
| MicrosoftConfigBenchmarks | ToMicrosoftConfiguration/ConfigChanges 性能测试 |

运行性能测试：

```bash
cd benchmarks/Apq.Cfg.Benchmarks
dotnet run -c Release
```

## 测试覆盖率

**100%** - 所有公开 API 均已覆盖测试
