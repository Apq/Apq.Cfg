using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 键路径深度性能基准测试
/// 测试不同深度的键路径解析性能
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class KeyPathBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;

    [Params(1, 3, 5, 10, 20)]
    public int PathDepth { get; set; }

    private string _deepKey = null!;
    private string _nonExistentDeepKey = null!;

    public KeyPathBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        var jsonPath = Path.Combine(_testDir, "config.json");

        // 生成深层嵌套的 JSON 结构
        var content = GenerateNestedJson(PathDepth);
        File.WriteAllText(jsonPath, content);

        _cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 生成对应深度的键路径
        _deepKey = GenerateKeyPath(PathDepth);
        _nonExistentDeepKey = GenerateKeyPath(PathDepth) + ":NonExistent";
    }

    private static string GenerateNestedJson(int depth)
    {
        if (depth <= 0) return "{}";

        var indent = new string(' ', 2);
        var result = "{\n";

        void AddLevel(int currentDepth, int indentLevel)
        {
            var currentIndent = new string(' ', indentLevel * 2);
            var levelName = $"Level{currentDepth}";

            if (currentDepth == depth)
            {
                result += $"{currentIndent}\"{levelName}\": {{\n";
                result += $"{currentIndent}  \"Value\": \"DeepValue\",\n";
                result += $"{currentIndent}  \"Number\": 12345,\n";
                result += $"{currentIndent}  \"Boolean\": true\n";
                result += $"{currentIndent}}}";
            }
            else
            {
                result += $"{currentIndent}\"{levelName}\": {{\n";
                AddLevel(currentDepth + 1, indentLevel + 1);
                result += $"\n{currentIndent}}}";
            }
        }

        AddLevel(1, 1);
        result += "\n}";
        return result;
    }

    private static string GenerateKeyPath(int depth)
    {
        var parts = new string[depth + 1];
        for (int i = 0; i < depth; i++)
        {
            parts[i] = $"Level{i + 1}";
        }
        parts[depth] = "Value";
        return string.Join(":", parts);
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

    #region 深层键读取性能测试

    /// <summary>
    /// 读取深层嵌套的键
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("DeepRead")]
    public string? Get_DeepKey()
    {
        return _cfg.Get(_deepKey);
    }

    /// <summary>
    /// 批量读取深层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepRead")]
    public void Get_DeepKey_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get(_deepKey);
        }
    }

    /// <summary>
    /// 读取深层键并转换类型
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepRead")]
    public int Get_DeepKey_Int()
    {
        var numberKey = _deepKey.Replace(":Value", ":Number");
        return _cfg.Get<int>(numberKey);
    }

    /// <summary>
    /// 读取深层键并转换为布尔值
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepRead")]
    public bool Get_DeepKey_Bool()
    {
        var boolKey = _deepKey.Replace(":Value", ":Boolean");
        return _cfg.Get<bool>(boolKey);
    }

    #endregion

    #region 深层键存在性检查

    /// <summary>
    /// 检查深层键是否存在
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepExists")]
    public bool Exists_DeepKey()
    {
        return _cfg.Exists(_deepKey);
    }

    /// <summary>
    /// 检查不存在的深层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepExists")]
    public bool Exists_NonExistentDeepKey()
    {
        return _cfg.Exists(_nonExistentDeepKey);
    }

    /// <summary>
    /// 批量检查深层键存在性
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepExists")]
    public void Exists_DeepKey_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Exists(_deepKey);
            _ = _cfg.Exists(_nonExistentDeepKey);
        }
    }

    #endregion

    #region 深层键写入性能测试

    /// <summary>
    /// 写入深层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepWrite")]
    public void Set_DeepKey()
    {
        _cfg.Set(_deepKey, "NewDeepValue");
    }

    /// <summary>
    /// 批量写入深层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepWrite")]
    public void Set_DeepKey_Multiple()
    {
        for (int i = 0; i < 100; i++)
        {
            _cfg.Set(_deepKey, $"NewDeepValue{i}");
        }
    }

    /// <summary>
    /// 创建新的深层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("DeepWrite")]
    public void Set_NewDeepKey()
    {
        for (int i = 0; i < 100; i++)
        {
            var newKey = _deepKey.Replace(":Value", $":NewKey{i}");
            _cfg.Set(newKey, $"NewValue{i}");
        }
    }

    #endregion

    #region 浅层 vs 深层对比

    /// <summary>
    /// 读取浅层键（1层）作为对比基准
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Comparison")]
    public string? Get_ShallowKey()
    {
        return _cfg.Get("Level1:Value");
    }

    /// <summary>
    /// 批量读取浅层键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Comparison")]
    public void Get_ShallowKey_Multiple()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfg.Get("Level1:Value");
        }
    }

    #endregion
}
