using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 自定义基准测试配置
/// 使用当前运行时进行测试，通过 -f 参数切换目标框架
/// 运行方式：
///   dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net8.0 -- --filter *
///   dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net10.0 -- --filter *
/// </summary>
public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        // 自定义 Job：5 次预热 + 10 次迭代，平衡速度与准确性
        // 使用当前运行时（由 -f 参数决定）
        var baseJob = Job.Default
            .WithWarmupCount(5)
            .WithIterationCount(10)
            .WithLaunchCount(1);

        AddJob(baseJob);

        // 添加内存诊断
        AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);

        // 添加导出器
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);
        AddExporter(BenchmarkDotNet.Exporters.Csv.CsvExporter.Default);

        // 添加列
        AddColumn(StatisticColumn.Mean);
        AddColumn(StatisticColumn.Error);
        AddColumn(StatisticColumn.StdDev);

        // 添加控制台日志
        AddLogger(ConsoleLogger.Default);

        // 设置摘要样式
        WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend));

        // 设置输出目录：使用绝对路径，确保结果保存到 benchmarks 项目目录下
        // 包含框架名称作为子目录，支持并行运行不同框架的测试
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
        // 从 ".NET 8.0.22" 或 ".NET 10.0.1" 提取主版本号 "net8" 或 "net10"
        var desc = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription; // ".NET 8.0.22"
        var version = desc.Replace(".NET ", "").Split('.')[0]; // "8" 或 "10"
        var framework = $"net{version}";
        var benchmarkProjectDir = Path.GetDirectoryName(typeof(BenchmarkConfig).Assembly.Location)!;
        // 从 bin/Release/netX.0 回退到项目目录
        var projectDir = Path.GetFullPath(Path.Combine(benchmarkProjectDir, "..", "..", ".."));
        var artifactsPath = Path.Combine(projectDir, "BenchmarkDotNet.Artifacts", timestamp, framework);
        WithArtifactsPath(artifactsPath);
    }
}
