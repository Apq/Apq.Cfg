using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 并发访问性能基准测试
/// 测试多线程同时读写配置的性能
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ConcurrencyBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;
    private string _jsonPath = null!;

    [Params(1, 4, 8)]
    public int ThreadCount { get; set; }

    public ConcurrencyBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        _jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(_jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432,
                    "Name": "testdb"
                },
                "App": {
                    "Name": "BenchmarkApp",
                    "Version": "1.0.0"
                },
                "Settings": {
                    "Key1": "Value1",
                    "Key2": "Value2",
                    "Key3": "Value3",
                    "Key4": "Value4",
                    "Key5": "Value5",
                    "Key6": "Value6",
                    "Key7": "Value7",
                    "Key8": "Value8"
                }
            }
            """);

        _cfg = new CfgBuilder()
            .AddJson(_jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
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

    /// <summary>
    /// 多线程并发读取同一个键
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("ConcurrentRead")]
    public void ConcurrentRead_SameKey()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Get("Database:Host");
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发读取不同的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentRead")]
    public void ConcurrentRead_DifferentKeys()
    {
        var keys = new[] { "Settings:Key1", "Settings:Key2", "Settings:Key3", "Settings:Key4",
                          "Settings:Key5", "Settings:Key6", "Settings:Key7", "Settings:Key8" };
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var keyIndex = i % keys.Length;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Get(keys[keyIndex]);
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发写入不同的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentWrite")]
    public void ConcurrentWrite_DifferentKeys()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var threadId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _cfg.Set($"Temp:Thread{threadId}:Key{j}", $"Value{j}");
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程混合读写操作
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentMixed")]
    public void ConcurrentMixed_ReadWrite()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var threadId = i;
            var isReader = i % 2 == 0;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    if (isReader)
                    {
                        _ = _cfg.Get("Database:Host");
                        _ = _cfg.Exists("App:Name");
                    }
                    else
                    {
                        _cfg.Set($"Temp:Thread{threadId}:Key{j}", $"Value{j}");
                    }
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发检查键存在
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentExists")]
    public void ConcurrentExists()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Exists("Database:Host");
                    _ = _cfg.Exists("NonExistent:Key");
                }
            });
        }
        Task.WaitAll(tasks);
    }
}
