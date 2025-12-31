# Apq.Cfg 性能基准测试

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

本目录包含 Apq.Cfg 配置库的性能基准测试，使用 [BenchmarkDotNet](https://benchmarkdotnet.org/) 框架。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 项目结构

```
benchmarks/
└── Apq.Cfg.Benchmarks/                   # 多目标框架基准测试项目
    ├── Apq.Cfg.Benchmarks.csproj         # 支持 net8.0;net10.0
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
    ├── MicrosoftConfigBenchmarks.cs      # Microsoft Configuration 转换测试
    │
    ├── # 新增功能测试
    ├── EncodingBenchmarks.cs             # 编码检测性能测试
    ├── ObjectBinderBenchmarks.cs         # 对象绑定性能测试（反射）
    ├── SourceGeneratorBenchmarks.cs      # 源生成器绑定性能测试（零反射）
    ├── DependencyInjectionBenchmarks.cs  # 依赖注入集成性能测试
    ├── CryptoBenchmarks.cs               # 加密脱敏性能测试
    │
    ├── # 远程配置中心测试
    ├── ConsulBenchmarks.cs               # Consul 配置中心性能测试
    └── EtcdBenchmarks.cs                 # Etcd 配置中心性能测试
```

## 运行基准测试

> **说明**：以下所有命令均在**项目根目录**执行。

### 基本运行

```bash
# 运行所有基准测试（Release 模式必须）
# 使用 -f 参数指定目标框架，分别测试 .NET 8 和 .NET 10
# 结果自动保存到带时间戳和框架名称的子目录，支持并行运行

# 测试 .NET 8
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net8.0 -- --filter *

# 测试 .NET 10
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *
```

### 并行运行（PowerShell）

```powershell
# 同时运行 .NET 8 和 .NET 10 测试，结果保存到不同目录
# 通过绑定 CPU 核心（ProcessorAffinity）减少竞争，需要至少 16 核 CPU
# 0x00FF = 核心 0-7，0xFF00 = 核心 8-15
# 注意：即使绑定核心，L3 缓存和内存带宽仍共享，追求精确结果请顺序执行

$p1 = Start-Process powershell -ArgumentList "-Command", "dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net8.0 -- --filter *" -PassThru; $p2 = Start-Process powershell -ArgumentList "-Command", "dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *" -PassThru; Start-Sleep -Milliseconds 500; $p1.ProcessorAffinity = 0x00FF; $p2.ProcessorAffinity = 0xFF00
```

### 运行特定测试

```bash
# 运行特定测试类
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *ReadWriteBenchmarks*
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *CacheBenchmarks*

# 运行特定测试方法
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *Json_Get*

# 组合多个过滤器
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *Json* --filter *Ini*
```

> **注意**：`--` 是必须的，它将后面的参数传递给 BenchmarkDotNet 而不是 dotnet 命令。

### 其他选项

```bash
# 列出所有可用测试（不实际运行）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --list flat
```

> **说明**：导出格式（Markdown、HTML、CSV）已在 `BenchmarkConfig` 中配置，无需手动指定。

## 基准测试类说明

### 1. ReadWriteBenchmarks - 读写性能测试

测试不同配置源（JSON/Env/INI/XML/YAML/TOML）的基本操作性能：

| 测试方法 | 说明 |
|----------|------|
| Json/Env/Ini/Xml/Yaml/Toml_Get | 读取字符串值 |
| Json/Env/Ini/Xml/Yaml/Toml_GetInt | 读取并转换类型（如 int） |
| Json/Env/Ini/Xml/Yaml/Toml_Exists | 检查键是否存在 |
| Json/Env/Ini/Xml/Yaml/Toml_Set | 写入配置值 |

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

### 13. EncodingBenchmarks - 编码检测性能测试

测试编码检测和映射的性能：

| 测试类别 | 测试方法 |
|----------|----------|
| BOM 检测 | Detect_UTF8_WithBOM, Detect_UTF16LE_WithBOM |
| 无 BOM 检测 | Detect_UTF8_NoBOM, Detect_GB2312 |
| 缓存效果 | Detect_Cached_1000, Detect_Uncached_100 |
| 大文件 | Detect_LargeFile |
| 编码映射 | Mapping_ExactPath_Lookup, Mapping_Wildcard_Lookup |
| 混合场景 | Detect_MixedEncodings_10 |

### 14. ObjectBinderBenchmarks - 对象绑定性能测试（反射）

测试 ObjectBinder（基于反射）绑定不同类型对象的性能：

| 测试类别 | 测试方法 |
|----------|----------|
| 简单类型 | Bind_SimpleTypes, Bind_SimpleTypes_100 |
| 嵌套对象 | Bind_NestedObject, Bind_NestedObject_100 |
| 数组/列表 | Bind_Array, Bind_Array_100 |
| 字典 | Bind_Dictionary, Bind_Dictionary_100 |
| 复杂对象 | Bind_ComplexObject, Bind_ComplexObject_100 |

### 15. SourceGeneratorBenchmarks - 源生成器绑定性能测试（零反射）

对比源生成器（编译时生成代码，零反射）与 ObjectBinder（运行时反射）的性能差异：

| 测试类别 | 测试方法 |
|----------|----------|
| 简单类型 | SourceGen_SimpleTypes vs Reflection_SimpleTypes |
| 简单类型批量 | SourceGen_SimpleTypes_100 vs Reflection_SimpleTypes_100 |
| 嵌套对象 | SourceGen_NestedObject vs Reflection_NestedObject |
| 嵌套对象批量 | SourceGen_NestedObject_100 vs Reflection_NestedObject_100 |
| 数组/列表 | SourceGen_Array vs Reflection_Array |
| 数组/列表批量 | SourceGen_Array_100 vs Reflection_Array_100 |
| 字典 | SourceGen_Dictionary vs Reflection_Dictionary |
| 字典批量 | SourceGen_Dictionary_100 vs Reflection_Dictionary_100 |
| 复杂对象 | SourceGen_ComplexObject vs Reflection_ComplexObject |
| 复杂对象批量 | SourceGen_ComplexObject_100 vs Reflection_ComplexObject_100 |
| BindTo | SourceGen_BindTo vs Reflection_BindTo |

> **预期结果**：源生成器绑定应比反射绑定快 2-5 倍，且内存分配更少。

### 16. DependencyInjectionBenchmarks - 依赖注入集成性能测试

测试 DI 集成的注册和解析性能：

| 测试类别 | 测试方法 |
|----------|----------|
| 注册 | AddApqCfg_Register, AddApqCfg_WithOptions_Register |
| ConfigureApqCfg | ConfigureApqCfg_Single, ConfigureApqCfg_Multiple |
| IOptions | Resolve_IOptions, Resolve_IOptions_100 |
| IOptionsMonitor | Resolve_IOptionsMonitor, Resolve_IOptionsMonitor_100 |
| IOptionsSnapshot | Resolve_IOptionsSnapshot, Resolve_IOptionsSnapshot_100 |
| ICfgRoot | Resolve_ICfgRoot, Resolve_ICfgRoot_ThenGet |
| 复杂对象 | Resolve_ComplexOptions, Resolve_MultipleOptions |

### 17. CryptoBenchmarks - 加密脱敏性能测试

测试各种加密算法、脱敏操作、缓存效果等场景：

| 测试类别 | 测试方法 |
|----------|----------|
| 加密算法对比 | AesGcm/AesCbc/ChaCha20/Sm4/TripleDes_Encrypt_Short |
| 解密算法对比 | AesGcm/ChaCha20_Decrypt_Short |
| 文本长度对比 | AesGcm_Encrypt_Short/Medium/Long, ChaCha20_Encrypt_Long |
| 批量加密 | AesGcm/ChaCha20_Encrypt_Batch100 |
| Transformer 性能 | Transformer_ShouldTransform_SensitiveKey/NonSensitiveKey/EncryptedValue |
| Transformer 缓存 | Transformer_ShouldTransform_Batch1000, Transformer_TransformOnRead |
| Masker 性能 | Masker_ShouldMask_SensitiveKey/NonSensitiveKey, Masker_Mask |
| Masker 缓存 | Masker_ShouldMask_Batch1000 |
| CfgRoot 集成 | CfgRoot_GetEncrypted_FirstAccess/Cached1000, CfgRoot_GetPlain/GetMasked |
| 脱敏快照 | CfgRoot_GetMaskedSnapshot |
| 模式匹配 | PatternMatch_Simple/Various/NoMatch_1000 |

运行加密脱敏基准测试：

```bash
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *Crypto*
```

### 18. ConsulBenchmarks - Consul 配置中心性能测试（需要 Consul 服务）

测试 Consul 配置源的读写性能：

| 测试类别 | 测试方法 |
|----------|----------|
| 基本读写 | Consul_Get, Consul_Set, Consul_Exists, Consul_Remove |
| 批量操作 | Consul_Get_Multiple, Consul_Set_Multiple |
| 热重载 | Consul_Watch_ChangeDetection |

> **注意**：需要运行 Consul 服务才能执行此测试。

### 18. EtcdBenchmarks - Etcd 配置中心性能测试（需要 Etcd 服务）

测试 Etcd 配置源的读写性能：

| 测试类别 | 测试方法 |
|----------|----------|
| 基本读写 | Etcd_Get, Etcd_Set, Etcd_Exists, Etcd_Remove |
| 批量操作 | Etcd_Get_Multiple, Etcd_Set_Multiple |
| 热重载 | Etcd_Watch_ChangeDetection |

> **注意**：需要运行 Etcd 服务才能执行此测试。

## 测试配置说明

本项目使用自定义 `BenchmarkConfig` 配置，通过 `-f` 参数指定目标框架进行测试。

- **迭代次数**：5 次预热 + 10 次实际测试
- **预计耗时**：单个框架约 **10 分钟**完成
- **测试覆盖**：约 250 个测试方法
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

| 测试类 | Get | Set | Exists | Remove | Save | Load | 并发 | 类型转换 | GetSection | GetMany | SetMany | ToMsConfig | ConfigChanges | 编码检测 | 对象绑定 | 源生成器 | DI | 加密 | 脱敏 |
|--------|-----|-----|--------|--------|------|------|------|----------|------------|---------|---------|------------|---------------|----------|----------|----------|-----|------|------|
| ReadWriteBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - |
| LargeFileBenchmarks | ✅ | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - |
| ConcurrencyBenchmarks | ✅ | ✅ | ✅ | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - | - |
| SaveBenchmarks | - | ✅ | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| RemoveBenchmarks | - | - | - | ✅ | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| MultiSourceBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - |
| KeyPathBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - |
| TypeConversionBenchmarks | ✅ | - | - | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - |
| CacheBenchmarks | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| GetSectionBenchmarks | ✅ | - | - | - | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - |
| BatchOperationBenchmarks | ✅ | ✅ | - | - | - | - | - | ✅ | - | ✅ | ✅ | - | - | - | - | - | - | - | - |
| MicrosoftConfigBenchmarks | ✅ | ✅ | - | - | - | - | - | - | - | - | - | ✅ | ✅ | - | - | - | - | - | - |
| EncodingBenchmarks | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | - | - | - | - | - |
| ObjectBinderBenchmarks | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | - | - | - | - |
| SourceGeneratorBenchmarks | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | ✅ | - | - | - |
| DependencyInjectionBenchmarks | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | - | - |
| **CryptoBenchmarks** | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | ✅ |
| ConsulBenchmarks | ✅ | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - | ✅ | - | - | - | - | - | - |
| EtcdBenchmarks | ✅ | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - | ✅ | - | - | - | - | - | - |

## 注意事项

1. **必须使用 Release 模式** - Debug 模式结果不准确
2. **必须指定框架** - 多目标项目需要 `-f net10.0` 等参数
3. **关闭其他程序** - 减少系统干扰
4. **多次运行** - BenchmarkDotNet 会自动预热和多次迭代
5. **结果对比** - 使用 `--runtimes` 参数可在一次运行中对比多个版本
6. **参数组合** - 部分测试有多个参数，会产生大量组合，可使用 `--filter` 缩小范围

> 详细性能测试结果见 [BENCHMARK_RESULTS.md](BENCHMARK_RESULTS.md)
