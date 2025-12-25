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
    ├── BatchOperationBenchmarks.cs       # 批量操作测试（GetMany/SetMany）
    │
    ├── # 高级场景测试
    ├── MultiSourceBenchmarks.cs          # 多源合并测试
    ├── KeyPathBenchmarks.cs              # 键路径深度测试
    ├── TypeConversionBenchmarks.cs       # 类型转换测试（含扩展方法）
    ├── CacheBenchmarks.cs                # 缓存效果测试
    ├── GetSectionBenchmarks.cs           # 配置节访问测试
    └── MicrosoftConfigBenchmarks.cs      # Microsoft Configuration 转换测试
```

## 运行基准测试

> **说明**：以下所有命令均在**项目根目录**执行。

### 基本运行

```bash
# 运行所有基准测试（Release 模式必须）
# 使用 .NET 9 作为宿主运行，自动测试 .NET 6/8/9 三个版本
# 结果自动保存到带时间戳的子目录
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *
```

### 运行特定测试

```bash
# 运行特定测试类
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *ReadWriteBenchmarks*
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *CacheBenchmarks*

# 运行特定测试方法
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Json_Get*

# 组合多个过滤器
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Json* --filter *Ini*
```

> **注意**：`--` 是必须的，它将后面的参数传递给 BenchmarkDotNet 而不是 dotnet 命令。

### 其他选项

```bash
# 列出所有可用测试（不实际运行）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --list flat
```

> **说明**：导出格式（Markdown、HTML、CSV）已在 `BenchmarkConfig` 中配置，无需手动指定。

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

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Load | 纯加载性能 |
| Json/Ini/Xml/Yaml/Toml_LoadAndRead | 加载后读取特定键 |

### 3. ConcurrencyBenchmarks - 并发访问测试

测试多线程环境下的性能，支持多种配置源和线程数量参数化：

| 参数 | 说明 |
|------|------|
| ThreadCount: 1/4/8 | 线程数量 |
| SourceType: Json/Ini | 配置源类型（选取最快和最常用的两种） |

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
| ChangeCount: 10/100 | 变更数量 |

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Save | 批量写入后保存 |
| Json/Ini_SaveLargeValue | 大值保存测试 |
| Json/Ini_FrequentSave | 频繁保存测试 |

### 5. RemoveBenchmarks - 删除操作测试

测试 Remove 操作在不同场景下的性能：

| 参数 | 说明 |
|------|------|
| KeyCount: 10/50 | 键数量 |

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
| SourceCount: 1/3/5 | 配置源数量 |

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
| PathDepth: 1/5/10 | 路径深度 |

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
| Extensions | TryGet_Success/Failure, GetRequired_Success, GetOrDefault_ExistingKey/NonExistingKey |
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

### 10. GetSectionBenchmarks - 配置节访问测试

测试 GetSection 和 GetChildKeys 操作的性能：

| 测试类别 | 测试方法 |
|----------|----------|
| GetSection | Json/Ini/Xml/Yaml/Toml_GetSection（基本配置节获取） |
| GetSectionNested | Json/Ini/Xml/Yaml/Toml_GetSection_Nested（嵌套配置节获取） |
| GetSectionThenGet | Json/Ini/Xml/Yaml/Toml_GetSection_ThenGet（配置节 + 读取组合） |
| GetChildKeys | Json/Ini/Xml/Yaml/Toml_GetChildKeys（根级子键枚举） |
| SectionGetChildKeys | Json/Ini/Xml/Yaml/Toml_Section_GetChildKeys（配置节子键枚举） |
| DirectVsSection | Json_DirectGet vs Json_SectionGet（直接访问 vs 配置节访问对比） |

### 11. BatchOperationBenchmarks - 批量操作测试

测试 GetMany/SetMany 批量操作与单次循环操作的性能对比：

| 测试类别 | 测试方法 |
|----------|----------|
| GetMany | GetMany_10Keys/50Keys/100Keys vs Get_Loop_10Keys/50Keys/100Keys |
| GetManyTyped | GetMany_Typed_10Keys vs Get_Typed_Loop_10Keys |
| SetMany | SetMany_10Keys/50Keys/100Keys vs Set_Loop_10Keys/50Keys/100Keys |
| Mixed | BatchRead_ThenBatchWrite vs LoopRead_ThenLoopWrite |

### 12. MicrosoftConfigBenchmarks - Microsoft Configuration 转换测试

测试 ToMicrosoftConfiguration 和 ConfigChanges 的性能：

| 测试类别 | 测试方法 |
|----------|----------|
| ToMsConfig | ToMicrosoftConfiguration_Static/Dynamic/Multiple |
| ReadComparison | Read_ViaApqCfg vs Read_ViaMsConfig, BatchRead_ViaApqCfg vs BatchRead_ViaMsConfig |
| ConfigChanges | Subscribe_ConfigChanges, Subscribe_AndTriggerChange, MultipleSubscribers |

## 测试配置说明

本项目使用自定义 `BenchmarkConfig` 配置，自动对比 .NET 6/8/9 三个版本的性能。

- **迭代次数**：5 次预热 + 10 次实际测试
- **预计耗时**：全部测试约 **15 分钟**完成
- **测试覆盖**：约 150 个测试方法 × 3 个运行时 = 450 个测试点
- **导出格式**：自动生成 Markdown、HTML、CSV 三种格式报告

## 测试结果

运行完成后，结果保存在带时间戳的子目录中，便于追踪历史性能变化：

```
benchmarks/Apq.Cfg.Benchmarks/
└── BenchmarkDotNet.Artifacts/
    ├── 2024-12-24_143052/              # 按时间戳保留
    │   └── results/
    │       ├── *-report.csv            # CSV 格式数据
    │       ├── *-report.html           # HTML 可视化报告
    │       └── *-report-github.md      # GitHub Markdown 格式
    ├── 2024-12-25_091530/              # 另一次测试
    │   └── results/
    └── ...
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

| 测试类 | Get | Set | Exists | Remove | Save | Load | 并发 | 类型转换 | GetSection | GetMany | SetMany | ToMsConfig | ConfigChanges |
|--------|-----|-----|--------|--------|------|------|------|----------|------------|---------|---------|------------|---------------|
| ReadWriteBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - |
| LargeFileBenchmarks | ✅ | - | - | - | - | ✅ | - | - | - | - | - | - | - |
| ConcurrencyBenchmarks | ✅ | ✅ | ✅ | - | - | - | ✅ | - | - | - | - | - | - |
| SaveBenchmarks | - | ✅ | - | - | ✅ | - | - | - | - | - | - | - | - |
| RemoveBenchmarks | - | - | - | ✅ | ✅ | - | - | - | - | - | - | - | - |
| MultiSourceBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - |
| KeyPathBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - |
| TypeConversionBenchmarks | ✅ | - | - | - | - | - | - | ✅ | - | - | - | - | - |
| CacheBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - | - | - |
| GetSectionBenchmarks | ✅ | - | - | - | - | - | - | - | ✅ | - | - | - | - |
| BatchOperationBenchmarks | ✅ | ✅ | - | - | - | - | - | ✅ | - | ✅ | ✅ | - | - |
| MicrosoftConfigBenchmarks | ✅ | ✅ | - | - | - | - | - | - | - | - | - | ✅ | ✅ |

## 注意事项

1. **必须使用 Release 模式** - Debug 模式结果不准确
2. **必须指定框架** - 多目标项目需要 `-f net9.0` 等参数
3. **关闭其他程序** - 减少系统干扰
4. **多次运行** - BenchmarkDotNet 会自动预热和多次迭代
5. **结果对比** - 使用 `--runtimes` 参数可在一次运行中对比多个版本
6. **参数组合** - 部分测试有多个参数，会产生大量组合，可使用 `--filter` 缩小范围

> 详细性能测试结果见 [BENCHMARK_RESULTS.md](BENCHMARK_RESULTS.md)
