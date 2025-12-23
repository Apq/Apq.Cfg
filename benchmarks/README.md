# Apq.Cfg 性能基准测试

本目录包含 Apq.Cfg 配置库的性能基准测试，使用 [BenchmarkDotNet](https://benchmarkdotnet.org/) 框架。

## 项目结构

```
benchmarks/
└── Apq.Cfg.Benchmarks/               # 多目标框架基准测试项目
    ├── Apq.Cfg.Benchmarks.csproj     # 支持 net6.0;net8.0;net9.0
    ├── ReadWriteBenchmarks.cs        # 读写性能测试
    ├── LargeFileBenchmarks.cs        # 大文件加载测试
    ├── ConcurrencyBenchmarks.cs      # 并发访问测试
    └── Program.cs                    # 入口程序
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

| 测试方法 | 说明 |
|----------|------|
| Json/Ini/Xml/Yaml/Toml_Load | 纯加载性能 |
| Json/Ini/Xml/Yaml/Toml_LoadAndRead | 加载后读取特定键 |

### 3. ConcurrencyBenchmarks - 并发访问测试

测试多线程环境下的性能，参数化线程数量：

| 参数 | 说明 |
|------|------|
| 1 线程 | 基准单线程 |
| 4 线程 | 中等并发 |
| 8 线程 | 高并发 |

| 测试方法 | 说明 |
|----------|------|
| ConcurrentRead_SameKey | 多线程读取同一键 |
| ConcurrentRead_DifferentKeys | 多线程读取不同键 |
| ConcurrentWrite_DifferentKeys | 多线程写入不同键 |
| ConcurrentMixed_ReadWrite | 混合读写操作 |
| ConcurrentExists | 并发检查键存在 |

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

# 运行特定测试方法
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Json_Get* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0 -- --filter *Ini_Load* --artifacts benchmarks/Apq.Cfg.Benchmarks/BenchmarkDotNet.Artifacts

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

### 最新测试结果 (2024-12)

**测试环境**：
- 系统：Windows 11
- SDK：.NET SDK 9.0.308
- 测试运行时：.NET 6.0、.NET 8.0、.NET 9.0

---

#### 1. 读写基准测试 (ReadWriteBenchmarks)

##### 获取整数值 (GetInt) - 最快操作

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

##### 其他操作耗时排序

| 操作 | 耗时范围 |
|------|----------|
| GetInt | 80-143 ns |
| Exists | 288-330 ns |
| Set | 303-356 ns |
| Get (字符串) | 324-380 ns |

---

#### 2. 大文件基准测试 (LargeFileBenchmarks)

##### 100 条配置项

| 格式 | 最快运行时 | 耗时 | 内存 |
|------|-----------|------|------|
| **Json** | .NET 8.0 | **259 μs** | 61 KB |
| **Ini** | .NET 8.0 | **260 μs** | 53 KB |
| Xml | .NET 8.0 | 334 μs | 154 KB |
| Yaml | .NET 9.0 | 412 μs | 156 KB |
| Toml | .NET 9.0 | 514 μs | 258 KB |

##### 1000 条配置项

| 格式 | 最快运行时 | 耗时 | 内存 |
|------|-----------|------|------|
| **Ini** | .NET 6.0 | **589 μs** | 327 KB |
| **Json** | .NET 9.0 | **667 μs** | 381 KB |
| Xml | .NET 9.0 | 1,082 μs | 1,161 KB |
| Yaml | .NET 9.0 | 1,524 μs | 1,277 KB |
| Toml | .NET 9.0 | 1,502 μs | 2,365 KB |

##### 5000 条配置项

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

#### 3. 并发基准测试 (ConcurrencyBenchmarks)

##### 单线程 (ThreadCount=1)

| 操作 | .NET 9.0 | .NET 8.0 | .NET 6.0 |
|------|----------|----------|----------|
| 读不同键 | **37 μs** | 47 μs | 54 μs |
| 读相同键 | **38 μs** | 46 μs | 52 μs |
| 写不同键 | **39 μs** | 42 μs | 46 μs |
| 混合读写 | **66 μs** | 68 μs | 76 μs |
| Exists | 72 μs | 64 μs | 78 μs |

##### 8 线程并发

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

#### 总结

##### 性能排名（综合）

1. **Json** - 综合最优，平衡性好
2. **Ini** - 大文件场景最快，内存最省
3. **Xml** - 中等性能
4. **Yaml** - 较慢但可接受
5. **Toml** - 最慢，内存占用最高

##### 运行时建议

- **推荐 .NET 8.0 或 .NET 9.0**，性能比 .NET 6.0 提升 30-45%
- 内存分配优化显著，GC 压力更小

##### 使用建议

| 场景 | 推荐格式 |
|------|----------|
| 高频读写 | Json / Ini |
| 大配置文件 | Ini / Json |
| 人类可读性优先 | Yaml / Toml |
| 与现有系统集成 | Xml |

---

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

## 注意事项

1. **必须使用 Release 模式** - Debug 模式结果不准确
2. **必须指定框架** - 多目标项目需要 `-f net9.0` 等参数
3. **关闭其他程序** - 减少系统干扰
4. **多次运行** - BenchmarkDotNet 会自动预热和多次迭代
5. **结果对比** - 使用 `--runtimes` 参数可在一次运行中对比多个版本
