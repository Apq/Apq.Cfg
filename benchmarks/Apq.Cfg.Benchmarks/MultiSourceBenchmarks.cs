using BenchmarkDotNet.Attributes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 多源合并性能基准测试
/// 测试多个配置源叠加时的查询性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class MultiSourceBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;

    [Params(3)]
    public int SourceCount { get; set; }

    public MultiSourceBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        var builder = new CfgBuilder();

        // 根据 SourceCount 添加不同数量的配置源
        for (int i = 0; i < SourceCount; i++)
        {
            var jsonPath = Path.Combine(_testDir, $"config{i}.json");
            var content = $$"""
                {
                    "Source{{i}}": {
                        "Name": "Source{{i}}",
                        "Level": {{i}}
                    },
                    "Shared": {
                        "Key": "ValueFromSource{{i}}",
                        "Priority": {{i}}
                    },
                    "Data": {
                        "Key1": "Value1_Source{{i}}",
                        "Key2": "Value2_Source{{i}}",
                        "Key3": "Value3_Source{{i}}",
                        "Key4": "Value4_Source{{i}}",
                        "Key5": "Value5_Source{{i}}"
                    }
                }
                """;
            File.WriteAllText(jsonPath, content);
            builder.AddJson(jsonPath, level: i, writeable: i == 0, isPrimaryWriter: i == 0);
        }

        _cfg = builder.Build();
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

    #region 读取性能测试

    /// <summary>
    /// 读取最高优先级源的键
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Read")]
    public string? Read_HighPriorityKey()
    {
        return _cfg.Get("Source0:Name");
    }

    /// <summary>
    /// 读取最低优先级源的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Read")]
    public string? Read_LowPriorityKey()
    {
        var key = $"Source{SourceCount - 1}:Name";
        return _cfg.Get(key);
    }

    /// <summary>
    /// 读取被覆盖的共享键（测试优先级合并）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Read")]
    public string? Read_SharedKey()
    {
        return _cfg.Get("Shared:Key");
    }

    /// <summary>
    /// 批量读取多个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Read")]
    public void Read_MultipleKeys()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get("Data:Key1");
            _ = _cfg.Get("Data:Key2");
            _ = _cfg.Get("Data:Key3");
            _ = _cfg.Get("Data:Key4");
            _ = _cfg.Get("Data:Key5");
        }
    }

    #endregion

    #region Exists 性能测试

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Exists_HighPriorityKey()
    {
        return _cfg.Exists("Source0:Name");
    }

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Exists_LowPriorityKey()
    {
        var key = $"Source{SourceCount - 1}:Name";
        return _cfg.Exists(key);
    }

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Exists_NonExistentKey()
    {
        return _cfg.Exists("NonExistent:Key");
    }

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public void Exists_MultipleKeys()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Exists("Data:Key1");
            _ = _cfg.Exists("Data:Key2");
            _ = _cfg.Exists("NonExistent:Key");
        }
    }

    #endregion

    #region 写入性能测试

    [Benchmark]
    [BenchmarkCategory("Write")]
    public void Write_NewKey()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.Set($"NewData:Key{i}", $"Value{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("Write")]
    public void Write_OverrideKey()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.Set("Shared:Key", $"NewValue{i}");
        }
    }

    #endregion

    #region 类型转换性能测试

    [Benchmark]
    [BenchmarkCategory("TypeConversion")]
    public int Get_Int()
    {
        return _cfg.Get<int>("Shared:Priority");
    }

    [Benchmark]
    [BenchmarkCategory("TypeConversion")]
    public void Get_Int_Multiple()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get<int>("Shared:Priority");
        }
    }

    #endregion
}
