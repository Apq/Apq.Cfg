using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// Microsoft Configuration 转换性能基准测试
/// 测试 ToMicrosoftConfiguration 和 ConfigChanges 的性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class MicrosoftConfigBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;
    private IConfigurationRoot _msConfig = null!;

    public MicrosoftConfigBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        var jsonPath = Path.Combine(_testDir, "config.json");

        // 生成包含多层嵌套的配置
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432,
                    "Name": "testdb",
                    "Connection": {
                        "Timeout": 30,
                        "MaxRetries": 3
                    }
                },
                "App": {
                    "Name": "BenchmarkApp",
                    "Version": "1.0.0",
                    "Settings": {
                        "MaxRetries": 3,
                        "Enabled": true
                    }
                },
                "Logging": {
                    "Level": "Info",
                    "Output": "Console"
                }
            }
            """);

        _cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 预先创建一个 Microsoft Configuration 用于读取测试
        _msConfig = _cfg.ToMicrosoftConfiguration();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        _cfg?.Dispose();

        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region ToMicrosoftConfiguration 性能测试

    /// <summary>
    /// 转换为 Microsoft Configuration（静态快照）
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("ToMsConfig")]
    public IConfigurationRoot ToMicrosoftConfiguration_Static()
    {
        return _cfg.ToMicrosoftConfiguration();
    }

    /// <summary>
    /// 转换为支持动态重载的 Microsoft Configuration
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ToMsConfig")]
    public IConfigurationRoot ToMicrosoftConfiguration_Dynamic()
    {
        return _cfg.ToMicrosoftConfiguration(new Apq.Cfg.Changes.DynamicReloadOptions());
    }

    /// <summary>
    /// 多次转换（测试缓存效果）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ToMsConfig")]
    public void ToMicrosoftConfiguration_Multiple()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.ToMicrosoftConfiguration();
        }
    }

    #endregion

    #region Microsoft Configuration 读取性能对比

    /// <summary>
    /// 通过 ICfgRoot 读取
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ReadComparison")]
    public string? Read_ViaApqCfg()
    {
        return _cfg["Database:Host"];
    }

    /// <summary>
    /// 通过 IConfigurationRoot 读取
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ReadComparison")]
    public string? Read_ViaMsConfig()
    {
        return _msConfig["Database:Host"];
    }

    /// <summary>
    /// 批量读取对比 - ICfgRoot
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ReadComparison")]
    public void BatchRead_ViaApqCfg()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg["Database:Host"];
            _ = _cfg["Database:Port"];
            _ = _cfg["App:Name"];
        }
    }

    /// <summary>
    /// 批量读取对比 - IConfigurationRoot
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ReadComparison")]
    public void BatchRead_ViaMsConfig()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _msConfig["Database:Host"];
            _ = _msConfig["Database:Port"];
            _ = _msConfig["App:Name"];
        }
    }

    #endregion

    #region ConfigChanges 订阅性能测试

    /// <summary>
    /// 订阅配置变更
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConfigChanges")]
    public void Subscribe_ConfigChanges()
    {
        using var subscription = _cfg.ConfigChanges.Subscribe(_ => { });
    }

    /// <summary>
    /// 订阅并触发变更
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConfigChanges")]
    public void Subscribe_AndTriggerChange()
    {
        int changeCount = 0;
        using var subscription = _cfg.ConfigChanges.Subscribe(_ => changeCount++);

        // 触发变更
        _cfg.SetValue("Temp:Key", "Value");
    }

    /// <summary>
    /// 多订阅者场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConfigChanges")]
    public void MultipleSubscribers()
    {
        var subscriptions = new List<IDisposable>();
        try
        {
            // 创建 10 个订阅者
            for (int i = 0; i < 10; i++)
            {
                subscriptions.Add(_cfg.ConfigChanges.Subscribe(_ => { }));
            }

            // 触发变更
            _cfg.SetValue("Temp:MultiKey", "Value");
        }
        finally
        {
            foreach (var sub in subscriptions)
            {
                sub.Dispose();
            }
        }
    }

    #endregion
}
