using Apq.Cfg.DependencyInjection;
using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 对象绑定性能测试
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class ObjectBinderBenchmarks
{
    private string _testDir = null!;
    private ICfgRoot _cfg = null!;
    private ICfgSection _simpleSection = null!;
    private ICfgSection _nestedSection = null!;
    private ICfgSection _arraySection = null!;
    private ICfgSection _dictionarySection = null!;
    private ICfgSection _complexSection = null!;

    [GlobalSetup]
    public void Setup()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBinderBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Simple": {
                    "Name": "TestApp",
                    "Port": 8080,
                    "Enabled": true,
                    "Timeout": 30.5,
                    "Id": "550e8400-e29b-41d4-a716-446655440000"
                },
                "Nested": {
                    "Name": "ParentApp",
                    "Database": {
                        "Host": "localhost",
                        "Port": 5432,
                        "Name": "testdb"
                    },
                    "Cache": {
                        "Host": "redis.local",
                        "Port": 6379
                    }
                },
                "Array": {
                    "Tags": {
                        "0": "tag1",
                        "1": "tag2",
                        "2": "tag3",
                        "3": "tag4",
                        "4": "tag5"
                    },
                    "Ports": {
                        "0": 80,
                        "1": 443,
                        "2": 8080,
                        "3": 8443
                    }
                },
                "Dictionary": {
                    "Settings": {
                        "Key1": "Value1",
                        "Key2": "Value2",
                        "Key3": "Value3",
                        "Key4": "Value4",
                        "Key5": "Value5"
                    }
                },
                "Complex": {
                    "Name": "ComplexApp",
                    "Endpoints": {
                        "0": {
                            "Host": "api1.local",
                            "Port": 8001
                        },
                        "1": {
                            "Host": "api2.local",
                            "Port": 8002
                        },
                        "2": {
                            "Host": "api3.local",
                            "Port": 8003
                        }
                    },
                    "Metadata": {
                        "version": "1.0.0",
                        "author": "test",
                        "description": "A complex configuration"
                    }
                }
            }
            """);

        _cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        _simpleSection = _cfg.GetSection("Simple");
        _nestedSection = _cfg.GetSection("Nested");
        _arraySection = _cfg.GetSection("Array");
        _dictionarySection = _cfg.GetSection("Dictionary");
        _complexSection = _cfg.GetSection("Complex");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        (_cfg as IDisposable)?.Dispose();
        if (Directory.Exists(_testDir))
        {
            try { Directory.Delete(_testDir, true); }
            catch { }
        }
    }

    // ========== 简单类型绑定 ==========

    [Benchmark(Description = "Bind_SimpleTypes")]
    public SimpleOptions Bind_SimpleTypes()
    {
        var options = new SimpleOptions();
        ObjectBinder.BindSection(_simpleSection, options);
        return options;
    }

    [Benchmark(Description = "Bind_SimpleTypes_100")]
    public SimpleOptions Bind_SimpleTypes_100()
    {
        SimpleOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new SimpleOptions();
            ObjectBinder.BindSection(_simpleSection, options);
        }
        return options;
    }

    // ========== 嵌套对象绑定 ==========

    [Benchmark(Description = "Bind_NestedObject")]
    public NestedOptions Bind_NestedObject()
    {
        var options = new NestedOptions();
        ObjectBinder.BindSection(_nestedSection, options);
        return options;
    }

    [Benchmark(Description = "Bind_NestedObject_100")]
    public NestedOptions Bind_NestedObject_100()
    {
        NestedOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new NestedOptions();
            ObjectBinder.BindSection(_nestedSection, options);
        }
        return options;
    }

    // ========== 数组/列表绑定 ==========

    [Benchmark(Description = "Bind_Array")]
    public ArrayOptions Bind_Array()
    {
        var options = new ArrayOptions();
        ObjectBinder.BindSection(_arraySection, options);
        return options;
    }

    [Benchmark(Description = "Bind_Array_100")]
    public ArrayOptions Bind_Array_100()
    {
        ArrayOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ArrayOptions();
            ObjectBinder.BindSection(_arraySection, options);
        }
        return options;
    }

    // ========== 字典绑定 ==========

    [Benchmark(Description = "Bind_Dictionary")]
    public DictionaryOptions Bind_Dictionary()
    {
        var options = new DictionaryOptions();
        ObjectBinder.BindSection(_dictionarySection, options);
        return options;
    }

    [Benchmark(Description = "Bind_Dictionary_100")]
    public DictionaryOptions Bind_Dictionary_100()
    {
        DictionaryOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new DictionaryOptions();
            ObjectBinder.BindSection(_dictionarySection, options);
        }
        return options;
    }

    // ========== 复杂对象绑定 ==========

    [Benchmark(Description = "Bind_ComplexObject")]
    public ComplexOptions Bind_ComplexObject()
    {
        var options = new ComplexOptions();
        ObjectBinder.BindSection(_complexSection, options);
        return options;
    }

    [Benchmark(Description = "Bind_ComplexObject_100")]
    public ComplexOptions Bind_ComplexObject_100()
    {
        ComplexOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ComplexOptions();
            ObjectBinder.BindSection(_complexSection, options);
        }
        return options;
    }

    // ========== 测试用选项类 ==========

    public class SimpleOptions
    {
        public string? Name { get; set; }
        public int Port { get; set; }
        public bool Enabled { get; set; }
        public double Timeout { get; set; }
        public Guid Id { get; set; }
    }

    public class NestedOptions
    {
        public string? Name { get; set; }
        public DatabaseOptions? Database { get; set; }
        public CacheOptions? Cache { get; set; }
    }

    public class DatabaseOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Name { get; set; }
    }

    public class CacheOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    public class ArrayOptions
    {
        public string[]? Tags { get; set; }
        public List<int>? Ports { get; set; }
    }

    public class DictionaryOptions
    {
        public Dictionary<string, string>? Settings { get; set; }
    }

    public class ComplexOptions
    {
        public string? Name { get; set; }
        public List<EndpointOptions>? Endpoints { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class EndpointOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}
