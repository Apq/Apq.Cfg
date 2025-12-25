using Apq.Cfg.DependencyInjection;
using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 源生成器绑定性能测试
/// 对比源生成器（零反射）与 ObjectBinder（反射）的性能差异
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SourceGeneratorBenchmarks
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
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgSgBench_{Guid.NewGuid():N}");
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
            .AddJson(jsonPath, level: 0, writeable: false)
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

    // ========== 简单类型绑定对比 ==========

    [Benchmark(Description = "SourceGen_SimpleTypes")]
    [BenchmarkCategory("Simple")]
    public SgSimpleOptions SourceGen_SimpleTypes()
    {
        return SgSimpleOptions.BindFrom(_simpleSection);
    }

    [Benchmark(Description = "Reflection_SimpleTypes")]
    [BenchmarkCategory("Simple")]
    public ReflectionSimpleOptions Reflection_SimpleTypes()
    {
        var options = new ReflectionSimpleOptions();
        ObjectBinder.BindSection(_simpleSection, options);
        return options;
    }

    [Benchmark(Description = "SourceGen_SimpleTypes_100")]
    [BenchmarkCategory("Simple")]
    public SgSimpleOptions SourceGen_SimpleTypes_100()
    {
        SgSimpleOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = SgSimpleOptions.BindFrom(_simpleSection);
        }
        return options;
    }

    [Benchmark(Description = "Reflection_SimpleTypes_100")]
    [BenchmarkCategory("Simple")]
    public ReflectionSimpleOptions Reflection_SimpleTypes_100()
    {
        ReflectionSimpleOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ReflectionSimpleOptions();
            ObjectBinder.BindSection(_simpleSection, options);
        }
        return options;
    }

    // ========== 嵌套对象绑定对比 ==========

    [Benchmark(Description = "SourceGen_NestedObject")]
    [BenchmarkCategory("Nested")]
    public SgNestedOptions SourceGen_NestedObject()
    {
        return SgNestedOptions.BindFrom(_nestedSection);
    }

    [Benchmark(Description = "Reflection_NestedObject")]
    [BenchmarkCategory("Nested")]
    public ReflectionNestedOptions Reflection_NestedObject()
    {
        var options = new ReflectionNestedOptions();
        ObjectBinder.BindSection(_nestedSection, options);
        return options;
    }

    [Benchmark(Description = "SourceGen_NestedObject_100")]
    [BenchmarkCategory("Nested")]
    public SgNestedOptions SourceGen_NestedObject_100()
    {
        SgNestedOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = SgNestedOptions.BindFrom(_nestedSection);
        }
        return options;
    }

    [Benchmark(Description = "Reflection_NestedObject_100")]
    [BenchmarkCategory("Nested")]
    public ReflectionNestedOptions Reflection_NestedObject_100()
    {
        ReflectionNestedOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ReflectionNestedOptions();
            ObjectBinder.BindSection(_nestedSection, options);
        }
        return options;
    }

    // ========== 数组/列表绑定对比 ==========

    [Benchmark(Description = "SourceGen_Array")]
    [BenchmarkCategory("Array")]
    public SgArrayOptions SourceGen_Array()
    {
        return SgArrayOptions.BindFrom(_arraySection);
    }

    [Benchmark(Description = "Reflection_Array")]
    [BenchmarkCategory("Array")]
    public ReflectionArrayOptions Reflection_Array()
    {
        var options = new ReflectionArrayOptions();
        ObjectBinder.BindSection(_arraySection, options);
        return options;
    }

    [Benchmark(Description = "SourceGen_Array_100")]
    [BenchmarkCategory("Array")]
    public SgArrayOptions SourceGen_Array_100()
    {
        SgArrayOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = SgArrayOptions.BindFrom(_arraySection);
        }
        return options;
    }

    [Benchmark(Description = "Reflection_Array_100")]
    [BenchmarkCategory("Array")]
    public ReflectionArrayOptions Reflection_Array_100()
    {
        ReflectionArrayOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ReflectionArrayOptions();
            ObjectBinder.BindSection(_arraySection, options);
        }
        return options;
    }

    // ========== 字典绑定对比 ==========

    [Benchmark(Description = "SourceGen_Dictionary")]
    [BenchmarkCategory("Dictionary")]
    public SgDictionaryOptions SourceGen_Dictionary()
    {
        return SgDictionaryOptions.BindFrom(_dictionarySection);
    }

    [Benchmark(Description = "Reflection_Dictionary")]
    [BenchmarkCategory("Dictionary")]
    public ReflectionDictionaryOptions Reflection_Dictionary()
    {
        var options = new ReflectionDictionaryOptions();
        ObjectBinder.BindSection(_dictionarySection, options);
        return options;
    }

    [Benchmark(Description = "SourceGen_Dictionary_100")]
    [BenchmarkCategory("Dictionary")]
    public SgDictionaryOptions SourceGen_Dictionary_100()
    {
        SgDictionaryOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = SgDictionaryOptions.BindFrom(_dictionarySection);
        }
        return options;
    }

    [Benchmark(Description = "Reflection_Dictionary_100")]
    [BenchmarkCategory("Dictionary")]
    public ReflectionDictionaryOptions Reflection_Dictionary_100()
    {
        ReflectionDictionaryOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ReflectionDictionaryOptions();
            ObjectBinder.BindSection(_dictionarySection, options);
        }
        return options;
    }

    // ========== 复杂对象绑定对比 ==========

    [Benchmark(Description = "SourceGen_ComplexObject")]
    [BenchmarkCategory("Complex")]
    public SgComplexOptions SourceGen_ComplexObject()
    {
        return SgComplexOptions.BindFrom(_complexSection);
    }

    [Benchmark(Description = "Reflection_ComplexObject")]
    [BenchmarkCategory("Complex")]
    public ReflectionComplexOptions Reflection_ComplexObject()
    {
        var options = new ReflectionComplexOptions();
        ObjectBinder.BindSection(_complexSection, options);
        return options;
    }

    [Benchmark(Description = "SourceGen_ComplexObject_100")]
    [BenchmarkCategory("Complex")]
    public SgComplexOptions SourceGen_ComplexObject_100()
    {
        SgComplexOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = SgComplexOptions.BindFrom(_complexSection);
        }
        return options;
    }

    [Benchmark(Description = "Reflection_ComplexObject_100")]
    [BenchmarkCategory("Complex")]
    public ReflectionComplexOptions Reflection_ComplexObject_100()
    {
        ReflectionComplexOptions options = null!;
        for (int i = 0; i < 100; i++)
        {
            options = new ReflectionComplexOptions();
            ObjectBinder.BindSection(_complexSection, options);
        }
        return options;
    }

    // ========== BindTo 对比 ==========

    [Benchmark(Description = "SourceGen_BindTo")]
    [BenchmarkCategory("BindTo")]
    public SgSimpleOptions SourceGen_BindTo()
    {
        var options = new SgSimpleOptions();
        SgSimpleOptions.BindTo(_simpleSection, options);
        return options;
    }

    [Benchmark(Description = "Reflection_BindTo")]
    [BenchmarkCategory("BindTo")]
    public ReflectionSimpleOptions Reflection_BindTo()
    {
        var options = new ReflectionSimpleOptions();
        ObjectBinder.BindSection(_simpleSection, options);
        return options;
    }
}

// ========== 源生成器配置类（使用 [CfgSection] 特性） ==========

[CfgSection]
public partial class SgSimpleOptions
{
    public string? Name { get; set; }
    public int Port { get; set; }
    public bool Enabled { get; set; }
    public double Timeout { get; set; }
    public Guid Id { get; set; }
}

[CfgSection]
public partial class SgNestedOptions
{
    public string? Name { get; set; }
    public SgDatabaseOptions? Database { get; set; }
    public SgCacheOptions? Cache { get; set; }
}

[CfgSection]
public partial class SgDatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}

[CfgSection]
public partial class SgCacheOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}

[CfgSection]
public partial class SgArrayOptions
{
    public string[]? Tags { get; set; }
    public List<int>? Ports { get; set; }
}

[CfgSection]
public partial class SgDictionaryOptions
{
    public Dictionary<string, string>? Settings { get; set; }
}

[CfgSection]
public partial class SgComplexOptions
{
    public string? Name { get; set; }
    public List<SgEndpointOptions>? Endpoints { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

[CfgSection]
public partial class SgEndpointOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}

// ========== 反射绑定配置类（用于对比） ==========

public class ReflectionSimpleOptions
{
    public string? Name { get; set; }
    public int Port { get; set; }
    public bool Enabled { get; set; }
    public double Timeout { get; set; }
    public Guid Id { get; set; }
}

public class ReflectionNestedOptions
{
    public string? Name { get; set; }
    public ReflectionDatabaseOptions? Database { get; set; }
    public ReflectionCacheOptions? Cache { get; set; }
}

public class ReflectionDatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}

public class ReflectionCacheOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}

public class ReflectionArrayOptions
{
    public string[]? Tags { get; set; }
    public List<int>? Ports { get; set; }
}

public class ReflectionDictionaryOptions
{
    public Dictionary<string, string>? Settings { get; set; }
}

public class ReflectionComplexOptions
{
    public string? Name { get; set; }
    public List<ReflectionEndpointOptions>? Endpoints { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class ReflectionEndpointOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}
