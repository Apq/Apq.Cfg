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

### 多版本对比结果示例

使用 `--runtimes` 参数后，结果会包含各版本的对比：

| Method | Runtime | Mean | Allocated |
|--------|---------|------|-----------|
| Json_Get | .NET 6 | 100ns | 64 B |
| Json_Get | .NET 8 | 80ns | 64 B |
| Json_Get | .NET 9 | 75ns | 64 B |

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
