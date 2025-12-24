using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 自定义基准测试配置
/// 目标：在 10 分钟内完成所有测试，同时保证结果稳定性
/// </summary>
public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        // 自定义 Job：5 次预热 + 10 次迭代，平衡速度与准确性
        var baseJob = Job.Default
            .WithWarmupCount(5)
            .WithIterationCount(10)
            .WithLaunchCount(1);

        // 添加三个运行时
        AddJob(baseJob.WithRuntime(CoreRuntime.Core60).WithId(".NET 6.0"));
        AddJob(baseJob.WithRuntime(CoreRuntime.Core80).WithId(".NET 8.0"));
        AddJob(baseJob.WithRuntime(CoreRuntime.Core90).WithId(".NET 9.0"));

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
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        var benchmarkProjectDir = Path.GetDirectoryName(typeof(BenchmarkConfig).Assembly.Location)!;
        // 从 bin/Release/net9.0 回退到项目目录
        var projectDir = Path.GetFullPath(Path.Combine(benchmarkProjectDir, "..", "..", ".."));
        var artifactsPath = Path.Combine(projectDir, "BenchmarkDotNet.Artifacts", timestamp);
        WithArtifactsPath(artifactsPath);
    }
}
