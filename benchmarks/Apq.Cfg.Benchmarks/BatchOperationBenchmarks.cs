using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 批量操作性能基准测试
/// 测试 GetMany/SetMany 与单次操作的性能对比
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class BatchOperationBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;

    // 预定义的键
    private readonly string[] _keys10 = new string[10];
    private readonly string[] _keys50 = new string[50];
    private readonly string[] _keys100 = new string[100];

    public BatchOperationBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        // 预生成键
        for (int i = 0; i < 100; i++)
        {
            var key = $"Data:Key{i}";
            if (i < 10) _keys10[i] = key;
            if (i < 50) _keys50[i] = key;
            _keys100[i] = key;
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        var jsonPath = Path.Combine(_testDir, "config.json");

        // 生成包含 100 个键的配置（使用数字字符串以支持类型转换测试）
        var content = "{\n  \"Data\": {\n";
        for (int i = 0; i < 100; i++)
        {
            content += $"    \"Key{i}\": \"{i}\"";
            if (i < 99) content += ",";
            content += "\n";
        }
        content += "  }\n}";

        File.WriteAllText(jsonPath, content);

        _cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();
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

    #region GetMany vs 单次 Get 对比

    /// <summary>
    /// 使用 GetMany 批量获取 10 个键
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("GetMany")]
    public IReadOnlyDictionary<string, string?> GetMany_10Keys()
    {
        return _cfg.GetMany(_keys10);
    }

    /// <summary>
    /// 使用单次 Get 循环获取 10 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetMany")]
    public void Get_Loop_10Keys()
    {
        foreach (var key in _keys10)
        {
            _ = _cfg.Get(key);
        }
    }

    /// <summary>
    /// 使用 GetMany 批量获取 50 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetMany")]
    public IReadOnlyDictionary<string, string?> GetMany_50Keys()
    {
        return _cfg.GetMany(_keys50);
    }

    /// <summary>
    /// 使用单次 Get 循环获取 50 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetMany")]
    public void Get_Loop_50Keys()
    {
        foreach (var key in _keys50)
        {
            _ = _cfg.Get(key);
        }
    }

    /// <summary>
    /// 使用 GetMany 批量获取 100 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetMany")]
    public IReadOnlyDictionary<string, string?> GetMany_100Keys()
    {
        return _cfg.GetMany(_keys100);
    }

    /// <summary>
    /// 使用单次 Get 循环获取 100 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetMany")]
    public void Get_Loop_100Keys()
    {
        foreach (var key in _keys100)
        {
            _ = _cfg.Get(key);
        }
    }

    #endregion

    #region GetMany<T> 类型转换批量获取

    /// <summary>
    /// 使用 GetMany<int> 批量获取并转换类型
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetManyTyped")]
    public void GetMany_Typed_10Keys()
    {
        _ = _cfg.GetMany<int>(_keys10);
    }

    /// <summary>
    /// 使用单次 Get<int> 循环获取并转换类型
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("GetManyTyped")]
    public void Get_Typed_Loop_10Keys()
    {
        foreach (var key in _keys10)
        {
            _ = _cfg.Get<int>(key);
        }
    }

    #endregion

    #region SetMany vs 单次 Set 对比

    /// <summary>
    /// 使用 SetMany 批量设置 10 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void SetMany_10Keys()
    {
        var values = new Dictionary<string, string?>();
        for (int i = 0; i < 10; i++)
        {
            values[$"Batch:Key{i}"] = $"Value{i}";
        }
        _cfg.SetMany(values);
    }

    /// <summary>
    /// 使用单次 Set 循环设置 10 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void Set_Loop_10Keys()
    {
        for (int i = 0; i < 10; i++)
        {
            _cfg.Set($"Loop:Key{i}", $"Value{i}");
        }
    }

    /// <summary>
    /// 使用 SetMany 批量设置 50 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void SetMany_50Keys()
    {
        var values = new Dictionary<string, string?>();
        for (int i = 0; i < 50; i++)
        {
            values[$"Batch50:Key{i}"] = $"Value{i}";
        }
        _cfg.SetMany(values);
    }

    /// <summary>
    /// 使用单次 Set 循环设置 50 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void Set_Loop_50Keys()
    {
        for (int i = 0; i < 50; i++)
        {
            _cfg.Set($"Loop50:Key{i}", $"Value{i}");
        }
    }

    /// <summary>
    /// 使用 SetMany 批量设置 100 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void SetMany_100Keys()
    {
        var values = new Dictionary<string, string?>();
        for (int i = 0; i < 100; i++)
        {
            values[$"Batch100:Key{i}"] = $"Value{i}";
        }
        _cfg.SetMany(values);
    }

    /// <summary>
    /// 使用单次 Set 循环设置 100 个键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SetMany")]
    public void Set_Loop_100Keys()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.Set($"Loop100:Key{i}", $"Value{i}");
        }
    }

    #endregion

    #region 混合读写场景

    /// <summary>
    /// 批量读取后批量写入
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void BatchRead_ThenBatchWrite()
    {
        // 批量读取
        var values = _cfg.GetMany(_keys10);

        // 批量写入（修改后的值）
        var newValues = new Dictionary<string, string?>();
        foreach (var kv in values)
        {
            newValues[kv.Key + "_copy"] = kv.Value + "_modified";
        }
        _cfg.SetMany(newValues);
    }

    /// <summary>
    /// 单次读取后单次写入（循环）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void LoopRead_ThenLoopWrite()
    {
        // 循环读取
        var values = new Dictionary<string, string?>();
        foreach (var key in _keys10)
        {
            values[key] = _cfg.Get(key);
        }

        // 循环写入
        foreach (var kv in values)
        {
            _cfg.Set(kv.Key + "_copy2", kv.Value + "_modified");
        }
    }

    #endregion
}
