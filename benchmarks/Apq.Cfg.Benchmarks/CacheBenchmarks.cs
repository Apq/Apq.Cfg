using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 缓存效果性能基准测试
/// 测试热路径重复读取、缓存命中/未命中等场景
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class CacheBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;

    // 预定义的键，避免在测试中动态生成
    private readonly string[] _existingKeys = new string[100];
    private readonly string[] _nonExistingKeys = new string[100];

    public CacheBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        // 预生成键
        for (int i = 0; i < 100; i++)
        {
            _existingKeys[i] = $"Data:Key{i}";
            _nonExistingKeys[i] = $"NonExistent:Key{i}";
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        var jsonPath = Path.Combine(_testDir, "config.json");

        // 生成包含 100 个键的配置
        var content = "{\n  \"Data\": {\n";
        for (int i = 0; i < 100; i++)
        {
            content += $"    \"Key{i}\": \"Value{i}\"";
            if (i < 99) content += ",";
            content += "\n";
        }
        content += "  },\n  \"HotKey\": \"HotValue\"\n}";

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

    #region 热路径重复读取（缓存命中）

    /// <summary>
    /// 同一键连续读取 1000 次（测试缓存效果）
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("HotPath")]
    public void HotPath_SameKey_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get("HotKey");
        }
    }

    /// <summary>
    /// 同一键连续读取 10000 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("HotPath")]
    public void HotPath_SameKey_10000()
    {
        for (int i = 0; i < 10000; i++)
        {
            _ = _cfg.Get("HotKey");
        }
    }

    /// <summary>
    /// 两个键交替读取（模拟常见的双键热路径）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("HotPath")]
    public void HotPath_TwoKeys_Alternating()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get("HotKey");
            _ = _cfg.Get("Data:Key0");
        }
    }

    /// <summary>
    /// 少量键循环读取（模拟热点配置）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("HotPath")]
    public void HotPath_FewKeys_Loop()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get("Data:Key0");
            _ = _cfg.Get("Data:Key1");
            _ = _cfg.Get("Data:Key2");
            _ = _cfg.Get("Data:Key3");
            _ = _cfg.Get("Data:Key4");
        }
    }

    #endregion

    #region 缓存未命中（不存在的键）

    /// <summary>
    /// 读取不存在的键 1000 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("CacheMiss")]
    public void CacheMiss_NonExistentKey_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get("NonExistent:Key");
        }
    }

    /// <summary>
    /// 读取不同的不存在键（每次不同）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("CacheMiss")]
    public void CacheMiss_DifferentNonExistentKeys()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get(_nonExistingKeys[i]);
        }
    }

    /// <summary>
    /// 混合存在和不存在的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("CacheMiss")]
    public void CacheMiss_MixedExistence()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get(_existingKeys[i]);
            _ = _cfg.Get(_nonExistingKeys[i]);
        }
    }

    #endregion

    #region 冷路径（不同键访问）

    /// <summary>
    /// 顺序访问 100 个不同的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ColdPath")]
    public void ColdPath_Sequential_100Keys()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get(_existingKeys[i]);
        }
    }

    /// <summary>
    /// 随机访问模式（模拟真实场景）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ColdPath")]
    public void ColdPath_Random_Pattern()
    {
        // 使用固定的"随机"模式，确保可重复性
        int[] pattern = { 42, 7, 91, 23, 56, 3, 78, 15, 67, 34,
                          89, 12, 45, 0, 99, 28, 61, 8, 73, 50 };
        for (int round = 0; round < 50; round++)
        {
            foreach (var idx in pattern)
            {
                _ = _cfg.Get(_existingKeys[idx]);
            }
        }
    }

    #endregion

    #region Exists 缓存效果

    /// <summary>
    /// 同一键 Exists 检查 1000 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ExistsCache")]
    public void Exists_SameKey_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Exists("HotKey");
        }
    }

    /// <summary>
    /// 不存在键 Exists 检查 1000 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ExistsCache")]
    public void Exists_NonExistentKey_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Exists("NonExistent:Key");
        }
    }

    /// <summary>
    /// 混合 Exists 检查
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ExistsCache")]
    public void Exists_Mixed_1000()
    {
        for (int i = 0; i < 500; i++)
        {
            _ = _cfg.Exists("HotKey");
            _ = _cfg.Exists("NonExistent:Key");
        }
    }

    #endregion

    #region 写入后读取（缓存失效场景）

    /// <summary>
    /// 写入后立即读取同一键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("WriteInvalidation")]
    public void WriteAndRead_SameKey()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.SetValue("Temp:Key", $"Value{i}");
            _ = _cfg.Get("Temp:Key");
        }
    }

    /// <summary>
    /// 写入后读取不同键（测试写入是否影响其他键的缓存）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("WriteInvalidation")]
    public void WriteAndRead_DifferentKeys()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.SetValue($"Temp:Key{i}", $"Value{i}");
            _ = _cfg.Get("HotKey");
        }
    }

    /// <summary>
    /// 批量写入后批量读取
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("WriteInvalidation")]
    public void BatchWrite_ThenBatchRead()
    {
        // 批量写入
        for (int i = 0; i < 100; i++)
        {
            _cfg.SetValue($"Batch:Key{i}", $"Value{i}");
        }

        // 批量读取
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get($"Batch:Key{i}");
        }
    }

    #endregion

    #region 首次访问 vs 后续访问

    /// <summary>
    /// 首次访问新键（冷启动）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("FirstAccess")]
    public string? FirstAccess_NewKey()
    {
        return _cfg.Get("Data:Key50");
    }

    /// <summary>
    /// 后续访问（预热后）- 通过多次调用模拟
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("FirstAccess")]
    public void SubsequentAccess_Warmed()
    {
        // 预热
        _ = _cfg.Get("Data:Key50");

        // 后续访问
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg.Get("Data:Key50");
        }
    }

    #endregion
}
