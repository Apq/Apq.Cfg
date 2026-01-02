using Apq.Cfg.Zookeeper;
using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// Zookeeper 配置中心性能测试
/// 注意：需要运行 Zookeeper 服务才能执行此测试
/// </summary>
[MemoryDiagnoser]
[Config(typeof(BenchmarkConfig))]
public class ZookeeperBenchmarks
{
    private const string ConnectionString = "localhost:2181";
    private const string RootPath = "/benchmark/config";

    private ICfgRoot? _cfg;
    private bool _isZookeeperAvailable;

    [GlobalSetup]
    public void Setup()
    {
        try
        {
            _cfg = new CfgBuilder()
                .AddZookeeper(options =>
                {
                    options.ConnectionString = ConnectionString;
                    options.RootPath = RootPath;
                    options.EnableHotReload = false;
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0, isPrimaryWriter: true)
                .Build();

            // 初始化测试数据
            for (int i = 0; i < 100; i++)
            {
                _cfg.Set($"Key{i}", $"Value{i}");
            }
            _cfg.SaveAsync().GetAwaiter().GetResult();

            _isZookeeperAvailable = true;
        }
        catch
        {
            _isZookeeperAvailable = false;
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_cfg != null && _isZookeeperAvailable)
        {
            // 清理测试数据
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    _cfg.Remove($"Key{i}");
                }
                _cfg.SaveAsync().GetAwaiter().GetResult();
            }
            catch { }

            _cfg.Dispose();
        }
    }

    [Benchmark]
    public string? Zookeeper_Get()
    {
        if (!_isZookeeperAvailable) return null;
        return _cfg!.Get("Key0");
    }

    [Benchmark]
    public void Zookeeper_Set()
    {
        if (!_isZookeeperAvailable) return;
        _cfg!.Set("BenchmarkKey", "BenchmarkValue");
    }

    [Benchmark]
    public bool Zookeeper_Exists()
    {
        if (!_isZookeeperAvailable) return false;
        return _cfg!.Exists("Key0");
    }

    [Benchmark]
    public void Zookeeper_Get_Multiple()
    {
        if (!_isZookeeperAvailable) return;
        for (int i = 0; i < 10; i++)
        {
            _ = _cfg!.Get($"Key{i}");
        }
    }

    [Benchmark]
    public void Zookeeper_Set_Multiple()
    {
        if (!_isZookeeperAvailable) return;
        for (int i = 0; i < 10; i++)
        {
            _cfg!.Set($"BenchmarkKey{i}", $"Value{i}");
        }
    }

    [Benchmark]
    public int Zookeeper_Get_Int()
    {
        if (!_isZookeeperAvailable) return 0;
        _cfg!.Set("IntKey", "42");
        return _cfg.GetValue<int>("IntKey");
    }
}
