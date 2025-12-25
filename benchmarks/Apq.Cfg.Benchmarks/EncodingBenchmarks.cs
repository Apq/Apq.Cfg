using System.Text;
using Apq.Cfg.EncodingSupport;
using Apq.Cfg.Sources.File;
using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 编码检测性能测试
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class EncodingBenchmarks
{
    private string _testDir = null!;
    private string _utf8NoBomFile = null!;
    private string _utf8BomFile = null!;
    private string _utf16LeFile = null!;
    private string _gb2312File = null!;
    private string _largeUtf8File = null!;

    [GlobalSetup]
    public void Setup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgEncodingBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        // UTF-8 无 BOM
        _utf8NoBomFile = Path.Combine(_testDir, "utf8_nobom.json");
        File.WriteAllText(_utf8NoBomFile, """{"key": "value", "中文": "测试"}""", new UTF8Encoding(false));

        // UTF-8 有 BOM
        _utf8BomFile = Path.Combine(_testDir, "utf8_bom.json");
        File.WriteAllText(_utf8BomFile, """{"key": "value", "中文": "测试"}""", new UTF8Encoding(true));

        // UTF-16 LE
        _utf16LeFile = Path.Combine(_testDir, "utf16le.json");
        File.WriteAllText(_utf16LeFile, """{"key": "value", "中文": "测试"}""", Encoding.Unicode);

        // GB2312
        _gb2312File = Path.Combine(_testDir, "gb2312.json");
        File.WriteAllText(_gb2312File, """{"key": "value", "中文": "测试"}""", Encoding.GetEncoding("GB2312"));

        // 大文件 UTF-8
        _largeUtf8File = Path.Combine(_testDir, "large_utf8.json");
        var sb = new StringBuilder();
        sb.Append("{");
        for (int i = 0; i < 1000; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append($"\"key{i}\": \"value{i}中文测试\"");
        }
        sb.Append("}");
        File.WriteAllText(_largeUtf8File, sb.ToString(), new UTF8Encoding(true));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testDir))
        {
            try { Directory.Delete(_testDir, true); }
            catch { }
        }
    }

    // ========== BOM 检测测试 ==========

    [Benchmark(Description = "Detect_UTF8_WithBOM")]
    public EncodingDetectionResult Detect_UTF8_WithBOM()
    {
        return FileCfgSourceBase.EncodingDetector.Detect(_utf8BomFile);
    }

    [Benchmark(Description = "Detect_UTF16LE_WithBOM")]
    public EncodingDetectionResult Detect_UTF16LE_WithBOM()
    {
        return FileCfgSourceBase.EncodingDetector.Detect(_utf16LeFile);
    }

    // ========== 无 BOM 检测测试 ==========

    [Benchmark(Description = "Detect_UTF8_NoBOM")]
    public EncodingDetectionResult Detect_UTF8_NoBOM()
    {
        // 清除缓存以测试实际检测性能
        FileCfgSourceBase.EncodingDetector.ClearCache();
        return FileCfgSourceBase.EncodingDetector.Detect(_utf8NoBomFile);
    }

    [Benchmark(Description = "Detect_GB2312")]
    public EncodingDetectionResult Detect_GB2312()
    {
        FileCfgSourceBase.EncodingDetector.ClearCache();
        return FileCfgSourceBase.EncodingDetector.Detect(_gb2312File);
    }

    // ========== 缓存效果测试 ==========

    [Benchmark(Description = "Detect_Cached_1000")]
    public EncodingDetectionResult Detect_Cached_1000()
    {
        EncodingDetectionResult result = default!;
        for (int i = 0; i < 1000; i++)
        {
            result = FileCfgSourceBase.EncodingDetector.Detect(_utf8BomFile);
        }
        return result;
    }

    [Benchmark(Description = "Detect_Uncached_100")]
    public EncodingDetectionResult Detect_Uncached_100()
    {
        EncodingDetectionResult result = default!;
        for (int i = 0; i < 100; i++)
        {
            FileCfgSourceBase.EncodingDetector.ClearCache();
            result = FileCfgSourceBase.EncodingDetector.Detect(_utf8NoBomFile);
        }
        return result;
    }

    // ========== 大文件检测测试 ==========

    [Benchmark(Description = "Detect_LargeFile")]
    public EncodingDetectionResult Detect_LargeFile()
    {
        FileCfgSourceBase.EncodingDetector.ClearCache();
        return FileCfgSourceBase.EncodingDetector.Detect(_largeUtf8File);
    }

    // ========== 编码映射测试 ==========

    [Benchmark(Description = "Mapping_ExactPath_Lookup")]
    public Encoding? Mapping_ExactPath_Lookup()
    {
        return FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(_utf8BomFile);
    }

    [Benchmark(Description = "Mapping_Wildcard_Lookup")]
    public Encoding? Mapping_Wildcard_Lookup()
    {
        // 假设已配置 *.json 的通配符映射
        return FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(
            Path.Combine(_testDir, "test.json"));
    }

    // ========== 混合场景测试 ==========

    [Benchmark(Description = "Detect_MixedEncodings_10")]
    public void Detect_MixedEncodings_10()
    {
        FileCfgSourceBase.EncodingDetector.ClearCache();
        FileCfgSourceBase.EncodingDetector.Detect(_utf8NoBomFile);
        FileCfgSourceBase.EncodingDetector.Detect(_utf8BomFile);
        FileCfgSourceBase.EncodingDetector.Detect(_utf16LeFile);
        FileCfgSourceBase.EncodingDetector.Detect(_gb2312File);
        FileCfgSourceBase.EncodingDetector.Detect(_utf8NoBomFile);
        FileCfgSourceBase.EncodingDetector.Detect(_utf8BomFile);
        FileCfgSourceBase.EncodingDetector.Detect(_utf16LeFile);
        FileCfgSourceBase.EncodingDetector.Detect(_gb2312File);
        FileCfgSourceBase.EncodingDetector.Detect(_utf8NoBomFile);
        FileCfgSourceBase.EncodingDetector.Detect(_utf8BomFile);
    }
}
