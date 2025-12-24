using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 大文件加载性能基准测试
/// 测试不同配置源加载大量配置项的性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class LargeFileBenchmarks : IDisposable
{
    private readonly string _testDir;

    [Params(100, 1000, 5000)]
    public int ItemCount { get; set; }

    private string _jsonPath = null!;
    private string _iniPath = null!;
    private string _xmlPath = null!;
    private string _yamlPath = null!;
    private string _tomlPath = null!;

    public LargeFileBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        _jsonPath = Path.Combine(_testDir, $"large_{ItemCount}.json");
        _iniPath = Path.Combine(_testDir, $"large_{ItemCount}.ini");
        _xmlPath = Path.Combine(_testDir, $"large_{ItemCount}.xml");
        _yamlPath = Path.Combine(_testDir, $"large_{ItemCount}.yaml");
        _tomlPath = Path.Combine(_testDir, $"large_{ItemCount}.toml");

        GenerateJsonFile(_jsonPath, ItemCount);
        GenerateIniFile(_iniPath, ItemCount);
        GenerateXmlFile(_xmlPath, ItemCount);
        GenerateYamlFile(_yamlPath, ItemCount);
        GenerateTomlFile(_tomlPath, ItemCount);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region 文件生成方法

    private static void GenerateJsonFile(string path, int count)
    {
        var data = new Dictionary<string, object>();
        for (int i = 0; i < count; i++)
        {
            var section = $"Section{i / 10}";
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, object>();
            }
            ((Dictionary<string, object>)data[section])[$"Key{i}"] = $"Value{i}";
        }
        File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void GenerateIniFile(string path, int count)
    {
        var sb = new StringBuilder();
        var currentSection = -1;
        for (int i = 0; i < count; i++)
        {
            var section = i / 10;
            if (section != currentSection)
            {
                currentSection = section;
                sb.AppendLine($"[Section{section}]");
            }
            sb.AppendLine($"Key{i}=Value{i}");
        }
        File.WriteAllText(path, sb.ToString());
    }

    private static void GenerateXmlFile(string path, int count)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<configuration>");
        var currentSection = -1;
        for (int i = 0; i < count; i++)
        {
            var section = i / 10;
            if (section != currentSection)
            {
                if (currentSection >= 0)
                {
                    sb.AppendLine($"  </Section{currentSection}>");
                }
                currentSection = section;
                sb.AppendLine($"  <Section{section}>");
            }
            sb.AppendLine($"    <Key{i}>Value{i}</Key{i}>");
        }
        if (currentSection >= 0)
        {
            sb.AppendLine($"  </Section{currentSection}>");
        }
        sb.AppendLine("</configuration>");
        File.WriteAllText(path, sb.ToString());
    }

    private static void GenerateYamlFile(string path, int count)
    {
        var sb = new StringBuilder();
        var currentSection = -1;
        for (int i = 0; i < count; i++)
        {
            var section = i / 10;
            if (section != currentSection)
            {
                currentSection = section;
                sb.AppendLine($"Section{section}:");
            }
            sb.AppendLine($"  Key{i}: Value{i}");
        }
        File.WriteAllText(path, sb.ToString());
    }

    private static void GenerateTomlFile(string path, int count)
    {
        var sb = new StringBuilder();
        var currentSection = -1;
        for (int i = 0; i < count; i++)
        {
            var section = i / 10;
            if (section != currentSection)
            {
                currentSection = section;
                sb.AppendLine($"[Section{section}]");
            }
            sb.AppendLine($"Key{i} = \"Value{i}\"");
        }
        File.WriteAllText(path, sb.ToString());
    }

    #endregion

    #region 加载性能测试

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Load")]
    public void Json_Load()
    {
        using var cfg = new CfgBuilder()
            .AddJson(_jsonPath, level: 0, writeable: false)
            .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Load")]
    public void Ini_Load()
    {
        using var cfg = new CfgBuilder()
            .AddIni(_iniPath, level: 0, writeable: false)
            .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Load")]
    public void Xml_Load()
    {
        using var cfg = new CfgBuilder()
            .AddXml(_xmlPath, level: 0, writeable: false)
            .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Load")]
    public void Yaml_Load()
    {
        using var cfg = new CfgBuilder()
            .AddYaml(_yamlPath, level: 0, writeable: false)
            .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Load")]
    public void Toml_Load()
    {
        using var cfg = new CfgBuilder()
            .AddToml(_tomlPath, level: 0, writeable: false)
            .Build();
    }

    #endregion

    #region 加载后读取性能测试

    [Benchmark]
    [BenchmarkCategory("LoadAndRead")]
    public string? Json_LoadAndRead()
    {
        using var cfg = new CfgBuilder()
            .AddJson(_jsonPath, level: 0, writeable: false)
            .Build();
        return cfg.Get($"Section{ItemCount / 20}:Key{ItemCount / 2}");
    }

    [Benchmark]
    [BenchmarkCategory("LoadAndRead")]
    public string? Ini_LoadAndRead()
    {
        using var cfg = new CfgBuilder()
            .AddIni(_iniPath, level: 0, writeable: false)
            .Build();
        return cfg.Get($"Section{ItemCount / 20}:Key{ItemCount / 2}");
    }

    [Benchmark]
    [BenchmarkCategory("LoadAndRead")]
    public string? Xml_LoadAndRead()
    {
        using var cfg = new CfgBuilder()
            .AddXml(_xmlPath, level: 0, writeable: false)
            .Build();
        return cfg.Get($"Section{ItemCount / 20}:Key{ItemCount / 2}");
    }

    [Benchmark]
    [BenchmarkCategory("LoadAndRead")]
    public string? Yaml_LoadAndRead()
    {
        using var cfg = new CfgBuilder()
            .AddYaml(_yamlPath, level: 0, writeable: false)
            .Build();
        return cfg.Get($"Section{ItemCount / 20}:Key{ItemCount / 2}");
    }

    [Benchmark]
    [BenchmarkCategory("LoadAndRead")]
    public string? Toml_LoadAndRead()
    {
        using var cfg = new CfgBuilder()
            .AddToml(_tomlPath, level: 0, writeable: false)
            .Build();
        return cfg.Get($"Section{ItemCount / 20}:Key{ItemCount / 2}");
    }

    #endregion
}
