using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 类型转换性能基准测试
/// 测试 Get&lt;T&gt; 不同类型转换的性能开销
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class TypeConversionBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;

    public TypeConversionBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Types": {
                    "String": "HelloWorld",
                    "LongString": "This is a very long string value that contains many characters to test string handling performance in the configuration system",
                    "Int": "12345",
                    "Long": "9223372036854775807",
                    "Double": "3.14159265358979",
                    "Decimal": "12345.6789012345",
                    "Bool": "true",
                    "BoolFalse": "false",
                    "Guid": "550e8400-e29b-41d4-a716-446655440000",
                    "DateTime": "2024-12-24T12:00:00",
                    "Enum": "Warning",
                    "NullableInt": "42",
                    "EmptyString": "",
                    "Whitespace": "   ",
                    "Unicode": "你好世界🌍",
                    "SpecialChars": "Hello:World=Test\"Quote'Single"
                }
            }
            """);

        _cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
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

    #region 基础类型转换

    /// <summary>
    /// 获取字符串（无转换，作为基准）
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("BasicTypes")]
    public string? Get_String()
    {
        return _cfg["Types:String"];
    }

    /// <summary>
    /// 获取字符串（泛型方式）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public string? Get_String_Generic()
    {
        return _cfg.GetValue<string>("Types:String");
    }

    /// <summary>
    /// 获取整数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public int Get_Int()
    {
        return _cfg.GetValue<int>("Types:Int");
    }

    /// <summary>
    /// 获取长整数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public long Get_Long()
    {
        return _cfg.GetValue<long>("Types:Long");
    }

    /// <summary>
    /// 获取双精度浮点数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public double Get_Double()
    {
        return _cfg.GetValue<double>("Types:Double");
    }

    /// <summary>
    /// 获取十进制数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public decimal Get_Decimal()
    {
        return _cfg.GetValue<decimal>("Types:Decimal");
    }

    /// <summary>
    /// 获取布尔值
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BasicTypes")]
    public bool Get_Bool()
    {
        return _cfg.GetValue<bool>("Types:Bool");
    }

    #endregion

    #region 复杂类型转换

    /// <summary>
    /// 获取 Guid
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ComplexTypes")]
    public Guid Get_Guid()
    {
        return _cfg.GetValue<Guid>("Types:Guid");
    }

    /// <summary>
    /// 获取 DateTime
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ComplexTypes")]
    public DateTime Get_DateTime()
    {
        return _cfg.GetValue<DateTime>("Types:DateTime");
    }

    /// <summary>
    /// 获取枚举值
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ComplexTypes")]
    public LogLevel Get_Enum()
    {
        return _cfg.GetValue<LogLevel>("Types:Enum");
    }

    /// <summary>
    /// 获取可空整数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ComplexTypes")]
    public int? Get_NullableInt()
    {
        return _cfg.GetValue<int?>("Types:NullableInt");
    }

    #endregion

    #region 批量类型转换

    /// <summary>
    /// 批量获取整数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Batch")]
    public void Get_Int_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.GetValue<int>("Types:Int");
        }
    }

    /// <summary>
    /// 批量获取布尔值
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Batch")]
    public void Get_Bool_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.GetValue<bool>("Types:Bool");
        }
    }

    /// <summary>
    /// 批量获取双精度浮点数
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Batch")]
    public void Get_Double_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.GetValue<double>("Types:Double");
        }
    }

    /// <summary>
    /// 批量获取字符串
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Batch")]
    public void Get_String_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg["Types:String"];
        }
    }

    #endregion

    #region 特殊值处理

    /// <summary>
    /// 获取长字符串
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SpecialValues")]
    public string? Get_LongString()
    {
        return _cfg["Types:LongString"];
    }

    /// <summary>
    /// 获取 Unicode 字符串
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SpecialValues")]
    public string? Get_Unicode()
    {
        return _cfg["Types:Unicode"];
    }

    /// <summary>
    /// 获取包含特殊字符的字符串
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SpecialValues")]
    public string? Get_SpecialChars()
    {
        return _cfg["Types:SpecialChars"];
    }

    /// <summary>
    /// 获取空字符串
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("SpecialValues")]
    public string? Get_EmptyString()
    {
        return _cfg["Types:EmptyString"];
    }

    #endregion

    #region TryGetValue 扩展方法

    /// <summary>
    /// TryGetValue 成功场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Extensions")]
    public bool TryGetValue_Success()
    {
        return _cfg.TryGetValue<int>("Types:Int", out _);
    }

    /// <summary>
    /// TryGetValue 失败场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Extensions")]
    public bool TryGetValue_Failure()
    {
        return _cfg.TryGetValue<int>("Types:NonExistent", out _);
    }

    /// <summary>
    /// GetRequired 成功场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Extensions")]
    public int GetRequired_Success()
    {
        return _cfg.GetRequired<int>("Types:Int");
    }

    /// <summary>
    /// GetOrDefault 存在键场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Extensions")]
    public int GetOrDefault_ExistingKey()
    {
        return _cfg.GetOrDefault("Types:Int", 0);
    }

    /// <summary>
    /// GetOrDefault 不存在键场景
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Extensions")]
    public int GetOrDefault_NonExistingKey()
    {
        return _cfg.GetOrDefault("Types:NonExistent", 100);
    }

    #endregion

    #region 混合类型操作

    /// <summary>
    /// 混合读取多种类型
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void Get_MixedTypes()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _cfg["Types:String"];
            _ = _cfg.GetValue<int>("Types:Int");
            _ = _cfg.GetValue<bool>("Types:Bool");
            _ = _cfg.GetValue<double>("Types:Double");
            _ = _cfg.GetValue<long>("Types:Long");
        }
    }

    #endregion
}

/// <summary>
/// 用于枚举转换测试的日志级别
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}
