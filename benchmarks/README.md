# Apq.Cfg 性能基准测试

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

本目录包含 Apq.Cfg 配置库的性能基准测试，使用 [BenchmarkDotNet](https://benchmarkdotnet.org/) 框架。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 项目结构

```
benchmarks/
└── Apq.Cfg.Benchmarks/                   # 多目标框架基准测试项目
    ├── Apq.Cfg.Benchmarks.csproj         # 支持 net6.0;net8.0;net9.0
    ├── Program.cs                        # 入口程序
    │
    ├── # 基础性能测试
    ├── ReadWriteBenchmarks.cs            # 读写性能测试
    ├── LargeFileBenchmarks.cs            # 大文件加载测试
    ├── ConcurrencyBenchmarks.cs          # 并发访问测试
    │
    ├── # 操作性能测试
    ├── SaveBenchmarks.cs                 # 持久化性能测试
    ├── RemoveBenchmarks.cs               # 删除操作测试
    │
    ├── # 高级场景测试
    ├── MultiSourceBenchmarks.cs          # 多源合并测试
    ├── KeyPathBenchmarks.cs              # 键路径深度测试
    ├── TypeConversionBenchmarks.cs       # 类型转换测试
    └── CacheBenchmarks.cs                # 缓存效果测试
```

## 基准测试类说明

### 1. ReadWriteBenchmarks - 读写性能测试

测试不同配置源（JSON/INI/XML/YAML/TOML）的基本操作性能：

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Get | 读取字符串值 |
| Json/Ini/Xml/Yaml/Toml_GetInt | 读取并转换类型（如 int） |
| Json/Ini/Xml/Yaml/Toml_Exists | 检查键是否存在 |
| Json/Ini/Xml/Yaml/Toml_Set | 写入配置值 |

### 2. LargeFileBenchmarks - 大文件加载测试

测试加载大量配置项时的性能，参数化测试项数量：

| 参数 | 说明 |
|------|------|
| 100 项 | 小型配置 |
| 1000 项 | 中型配置 |
| 5000 项 | 大型配置 |
| 10000 项 | 超大型配置 |
| 50000 项 | 极限测试 |

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Load | 纯加载性能 |
| Json/Ini/Xml/Yaml/Toml_LoadAndRead | 加载后读取特定键 |

### 3. ConcurrencyBenchmarks - 并发访问测试

测试多线程环境下的性能，支持多种配置源和线程数量参数化：

| 参数 | 说明 |
|------|------|
| ThreadCount: 1/4/8/16 | 线程数量 |
| SourceType: Json/Ini/Xml/Yaml/Toml | 配置源类型 |

| 测试方法 | 说明 |
|----------|------|
| ConcurrentRead_SameKey | 多线程读取同一键 |
| ConcurrentRead_DifferentKeys | 多线程读取不同键 |
| ConcurrentWrite_DifferentKeys | 多线程写入不同键 |
| ConcurrentWrite_SameKey | 多线程写入同一键（竞争场景） |
| ConcurrentMixed_ReadWrite | 混合读写操作 |
| ConcurrentExists | 并发检查键存在 |

### 4. SaveBenchmarks - 持久化性能测试

测试 SaveAsync 在不同数据量下的性能：

| 参数 | 说明 |
|------|------|
| ChangeCount: 10/50/100/500 | 变更数量 |

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Save | 批量写入后保存 |
| Json/Ini_SaveLargeValue | 大值保存测试 |
| Json/Ini_FrequentSave | 频繁保存测试 |

### 5. RemoveBenchmarks - 删除操作测试

测试 Remove 操作在不同场景下的性能：

| 参数 | 说明 |
|------|------|
| KeyCount: 10/50/100 | 键数量 |

| 测试方法 | 说明 |
|----------|------|
| *_RemoveSingle | 单键删除 |
| *_RemoveBatch | 批量删除 |
| *_RemoveNonExistent | 删除不存在的键 |
| *_RemoveAndSave | 删除后保存 |

### 6. MultiSourceBenchmarks - 多源合并测试

测试多个配置源叠加时的查询性能：

| 参数 | 说明 |
|------|------|
| SourceCount: 1/2/3/5 | 配置源数量 |

| 测试方法 | 说明 |
|----------|------|
| Read_HighPriorityKey | 读取高优先级源的键 |
| Read_LowPriorityKey | 读取低优先级源的键 |
| Read_SharedKey | 读取被覆盖的共享键 |
| Read_MultipleKeys | 批量读取多个键 |
| Exists_* | 存在性检查 |
| Write_* | 写入测试 |
| Get_Int* | 类型转换测试 |

### 7. KeyPathBenchmarks - 键路径深度测试

测试不同深度的键路径解析性能：

| 参数 | 说明 |
|------|------|
| PathDepth: 1/3/5/10/20 | 路径深度 |

| 测试方法 | 说明 |
|----------|------|
| Get_DeepKey | 读取深层嵌套的键 |
| Get_DeepKey_Multiple | 批量读取深层键 |
| Get_DeepKey_Int/Bool | 深层键类型转换 |
| Exists_DeepKey | 深层键存在性检查 |
| Set_DeepKey | 写入深层键 |
| Get_ShallowKey | 浅层键对比基准 |

### 8. TypeConversionBenchmarks - 类型转换测试

测试 Get<T> 不同类型转换的性能开销：

| 测试类别 | 测试方法 |
|----------|----------|
| BasicTypes | Get_String, Get_Int, Get_Long, Get_Double, Get_Decimal, Get_Bool |
| ComplexTypes | Get_Guid, Get_DateTime, Get_Enum, Get_NullableInt |
| Batch | Get_*_Multiple（批量转换） |
| SpecialValues | Get_LongString, Get_Unicode, Get_SpecialChars, Get_EmptyString |
| Extensions | TryGet_Success/Failure, GetRequired_Success |
| Mixed | Get_MixedTypes（混合类型读取） |

### 9. CacheBenchmarks - 缓存效果测试

测试热路径重复读取、缓存命中/未命中等场景：

| 测试类别 | 测试方法 |
|----------|----------|
| HotPath | HotPath_SameKey_1000/10000, HotPath_TwoKeys_Alternating, HotPath_FewKeys_Loop |
| CacheMiss | CacheMiss_NonExistentKey_1000, CacheMiss_DifferentNonExistentKeys, CacheMiss_MixedExistence |
| ColdPath | ColdPath_Sequential_100Keys, ColdPath_Random_Pattern |
| ExistsCache | Exists_SameKey_1000, Exists_NonExistentKey_1000, Exists_Mixed_1000 |
| WriteInvalidation | WriteAndRead_SameKey, WriteAndRead_DifferentKeys, BatchWrite_ThenBatchRead |
| FirstAccess | FirstAccess_NewKey, SubsequentAccess_Warmed |

## 运行基准测试

> **说明**：以下所有命令均在**项目根目录**执行，测试结果保存在测试项目目录下。

### 基本运行

```bash
# 运行所有基准测试（Release 模式必须）
# 重要：多目标框架项目必须使用 -f 指定框架

# 使用 .NET 9 运行
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 使用 .NET 8 运行
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net8.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 使用 .NET 6 运行
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net6.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
```

### 多版本对比测试

BenchmarkDotNet 支持在一次运行中对比多个 .NET 版本的性能：

```bash
# 同时测试 .NET 6、8、9 的性能对比
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --runtimes net6.0 net8.0 net9.0 --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
```

### 运行特定测试

```bash
# 运行特定测试类
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *ReadWriteBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *LargeFileBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *ConcurrencyBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *SaveBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *RemoveBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *MultiSourceBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *KeyPathBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *TypeConversionBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *CacheBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 运行特定测试方法
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Json_Get* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *HotPath* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 组合多个过滤器
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Json* --filter *Ini* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
```

> **注意**：`--` 是必须的，它将后面的参数传递给 BenchmarkDotNet 而不是 dotnet 命令。

### 常用参数

```bash
# 快速测试（减少迭代次数，用于验证功能是否正常）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --job short --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 列出所有可用测试（不实际运行）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --list flat

# 导出为不同格式
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --exporters markdown --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --exporters html --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --exporters csv --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
```

### 高级选项

```bash
# 内存诊断（默认已启用）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --memory --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

# 显示详细信息
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter * --info --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
```

## 测试结果

运行完成后，结果默认保存在 `BenchmarkDotNet.Artifacts/results/` 目录：

```
benchmarks/Apq.Cfg.Benchmarks/
└── BenchmarkDotNet.Artifacts/
    └── results/
        ├── *-report.csv          # CSV 格式数据
        ├── *-report.html         # HTML 可视化报告
        └── *-report-github.md    # GitHub Markdown 格式
```

### 结果解读

| 列名 | 说明 |
|------|------|
| Mean | 平均执行时间 |
| Error | 误差范围 |
| StdDev | 标准差 |
| Median | 中位数 |
| Rank | 性能排名 |
| Gen0/Gen1/Gen2 | GC 代数统计 |
| Allocated | 内存分配量 |

## 测试覆盖矩阵

| 测试类 | Get | Set | Exists | Remove | Save | Load | 并发 | 类型转换 |
|--------|-----|-----|--------|--------|------|------|------|----------|
| ReadWriteBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ |
| LargeFileBenchmarks | ✅ | - | - | - | - | ✅ | - | - |
| ConcurrencyBenchmarks | ✅ | ✅ | ✅ | - | - | - | ✅ | - |
| SaveBenchmarks | - | ✅ | - | - | ✅ | - | - | - |
| RemoveBenchmarks | - | - | - | ✅ | ✅ | - | - | - |
| MultiSourceBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ |
| KeyPathBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ |
| TypeConversionBenchmarks | ✅ | - | - | - | - | - | - | ✅ |
| CacheBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | - |

## 注意事项

1. **必须使用 Release 模式** - Debug 模式结果不准确
2. **必须指定框架** - 多目标项目需要 `-f net9.0` 等参数
3. **关闭其他程序** - 减少系统干扰
4. **多次运行** - BenchmarkDotNet 会自动预热和多次迭代
5. **结果对比** - 使用 `--runtimes` 参数可在一次运行中对比多个版本
6. **参数组合** - 部分测试有多个参数，会产生大量组合，可使用 `--filter` 缩小范围
