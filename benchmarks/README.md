# Apq.Cfg 性能基准测试

本目录包含 Apq.Cfg 配置库的性能基准测试，使用 [BenchmarkDotNet](https://benchmarkdotnet.org/) 框架。

## 项目结构

```
benchmarks/
├── Apq.Cfg.Benchmarks.Shared/     # 共享的基准测试类
│   ├── ReadWriteBenchmarks.cs     # 读写性能测试
│   ├── LargeFileBenchmarks.cs     # 大文件加载测试
│   └── ConcurrencyBenchmarks.cs   # 并发访问测试
├── Apq.Cfg.Benchmarks.Net6/       # .NET 6 测试项目
├── Apq.Cfg.Benchmarks.Net8/       # .NET 8 测试项目
└── Apq.Cfg.Benchmarks.Net9/       # .NET 9 测试项目
```

## 基准测试类说明

### 1. ReadWriteBenchmarks - 读写性能测试

测试不同配置源（JSON/INI/XML/YAML/TOML）的基本操作性能：

| 测试类别 | 说明 |
|----------|------|
| Get | 读取字符串值 |
| GetTyped | 读取并转换类型（如 int） |
| Exists | 检查键是否存在 |
| Set | 写入配置值 |

### 2. LargeFileBenchmarks - 大文件加载测试

测试加载大量配置项时的性能，参数化测试项数量：

- **100 项** - 小型配置
- **1000 项** - 中型配置
- **5000 项** - 大型配置

测试类别：
- **Load** - 纯加载性能
- **LoadAndRead** - 加载后读取特定键

### 3. ConcurrencyBenchmarks - 并发访问测试

测试多线程环境下的性能，参数化线程数量：

- **1 线程** - 基准单线程
- **4 线程** - 中等并发
- **8 线程** - 高并发

测试类别：
- **ConcurrentRead_SameKey** - 多线程读取同一键
- **ConcurrentRead_DifferentKeys** - 多线程读取不同键
- **ConcurrentWrite_DifferentKeys** - 多线程写入不同键
- **ConcurrentMixed_ReadWrite** - 混合读写操作
- **ConcurrentExists** - 并发检查键存在

## 运行基准测试

### 基本运行

```bash
# 运行所有基准测试（Release 模式必须）
# 重要：必须使用 -- --filter * 指定测试，否则会进入交互模式等待输入

# .NET 6
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net6 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net6/BenchmarkDotNet.Artifacts

# .NET 8
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net8 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net8/BenchmarkDotNet.Artifacts

# .NET 9
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
```

### 运行特定测试

```bash
# 运行特定测试类（以 .NET 9 为例，其他版本替换项目名即可）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *ReadWriteBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *LargeFileBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *ConcurrencyBenchmarks* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 运行特定测试方法
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *Json_Get* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *Ini_Load* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 运行特定类别
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *Get* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *Set* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 组合多个过滤器
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *Json* --filter *Ini* --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
```

> **注意**：`--` 是必须的，它将后面的参数传递给 BenchmarkDotNet 而不是 dotnet 命令。

### 常用参数

```bash
# 快速测试（减少迭代次数，用于验证功能是否正常）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter *ReadWriteBenchmarks* --job short --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 运行所有测试的快速版本
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --job short --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 列出所有可用测试（不实际运行）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --list flat

# 导出为不同格式
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --exporters markdown --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --exporters html --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --exporters csv --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
```

### 高级选项

```bash
# 指定运行时
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net6 -f net6.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net6/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net8 -f net8.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net8/BenchmarkDotNet.Artifacts
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -f net9.0 -- --filter * --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 内存诊断（默认已启用）
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --memory --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts

# 显示详细信息
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks.Net9 -- --filter * --info --artifacts benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts
```

## 并行运行多个测试（CPU 亲和性隔离）

在多核机器上（如 64G 内存、32 核），可以同时运行多个性能测试，使用 CPU 亲和性隔离避免相互干扰。

### 8 核分配方案（运行 2 个版本，需 16 核）

```powershell
# Net6，绑定到 CPU 0-7
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net6" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFF }

# Net8，绑定到 CPU 8-15
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net8" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFF00 }
```

### 8 核分配方案（运行 3 个版本，需 24 核）

```powershell
# Net6，绑定到 CPU 0-7
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net6" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFF }

# Net8，绑定到 CPU 8-15
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net8" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFF00 }

# Net9，绑定到 CPU 16-23
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net9" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFF0000 }
```

### 16 核分配方案（运行 2 个版本，需 32 核）

```powershell
# Net6，绑定到 CPU 0-15
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net6" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFFFF }

# Net8，绑定到 CPU 16-31
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net8" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFFFF0000 }
```

### 10 核分配方案（运行 3 个版本，需 30 核）

```powershell
# Net6，绑定到 CPU 0-9
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net6" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0x3FF }

# Net8，绑定到 CPU 10-19
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net8" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0xFFC00 }

# Net9，绑定到 CPU 20-29
Start-Process -FilePath "dotnet" -ArgumentList "run -c Release -- --filter *" -WorkingDirectory "D:\ApqGitee\Apq.Cfg\benchmarks\Apq.Cfg.Benchmarks.Net9" -PassThru | ForEach-Object { $_.ProcessorAffinity = 0x3FF00000 }
```

### 亲和性掩码参考

| 亲和性掩码 | 核心范围 |
|-----------|---------|
| `0xFF` | CPU 0-7 |
| `0xFF00` | CPU 8-15 |
| `0xFF0000` | CPU 16-23 |
| `0xFF000000` | CPU 24-31 |
| `0xFFFF` | CPU 0-15 |
| `0xFFFF0000` | CPU 16-31 |
| `0x3FF` | CPU 0-9 |
| `0xFFC00` | CPU 10-19 |
| `0x3FF00000` | CPU 20-29 |

## 测试结果

运行完成后，结果保存在各项目目录下的 `BenchmarkDotNet.Artifacts/results/` 目录：

```
benchmarks/Apq.Cfg.Benchmarks.Net6/BenchmarkDotNet.Artifacts/results/
benchmarks/Apq.Cfg.Benchmarks.Net8/BenchmarkDotNet.Artifacts/results/
benchmarks/Apq.Cfg.Benchmarks.Net9/BenchmarkDotNet.Artifacts/results/
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

## 注意事项

1. **必须使用 Release 模式** - Debug 模式结果不准确
2. **关闭其他程序** - 减少系统干扰
3. **多次运行** - BenchmarkDotNet 会自动预热和多次迭代
4. **结果对比** - 使用相同环境进行对比测试
