using BenchmarkDotNet.Attributes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 删除操作性能基准测试
/// 测试 Remove 操作在不同场景下的性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class RemoveBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _jsonCfg = null!;
    private ICfgRoot _iniCfg = null!;
    private ICfgRoot _xmlCfg = null!;
    private ICfgRoot _yamlCfg = null!;
    private ICfgRoot _tomlCfg = null!;

    private string _jsonPath = null!;
    private string _iniPath = null!;
    private string _xmlPath = null!;
    private string _yamlPath = null!;
    private string _tomlPath = null!;

    [Params(10, 50)]
    public int KeyCount { get; set; }

    public RemoveBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        _jsonPath = Path.Combine(_testDir, "config.json");
        _iniPath = Path.Combine(_testDir, "config.ini");
        _xmlPath = Path.Combine(_testDir, "config.xml");
        _yamlPath = Path.Combine(_testDir, "config.yaml");
        _tomlPath = Path.Combine(_testDir, "config.toml");

        ResetConfigFiles();
        CreateCfgInstances();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // 每次迭代前重置配置
        _jsonCfg?.Dispose();
        _iniCfg?.Dispose();
        _xmlCfg?.Dispose();
        _yamlCfg?.Dispose();
        _tomlCfg?.Dispose();

        ResetConfigFiles();
        CreateCfgInstances();
    }

    private void ResetConfigFiles()
    {
        // 生成包含多个键的 JSON 配置
        var jsonContent = "{\n";
        for (int i = 0; i < KeyCount; i++)
        {
            jsonContent += $"  \"Key{i}\": \"Value{i}\"";
            if (i < KeyCount - 1) jsonContent += ",";
            jsonContent += "\n";
        }
        jsonContent += "}";
        File.WriteAllText(_jsonPath, jsonContent);

        // 生成 INI 配置
        var iniContent = "[Data]\n";
        for (int i = 0; i < KeyCount; i++)
        {
            iniContent += $"Key{i}=Value{i}\n";
        }
        File.WriteAllText(_iniPath, iniContent);

        // 生成 XML 配置
        var xmlContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<configuration>\n  <Data>\n";
        for (int i = 0; i < KeyCount; i++)
        {
            xmlContent += $"    <Key{i}>Value{i}</Key{i}>\n";
        }
        xmlContent += "  </Data>\n</configuration>";
        File.WriteAllText(_xmlPath, xmlContent);

        // 生成 YAML 配置
        var yamlContent = "Data:\n";
        for (int i = 0; i < KeyCount; i++)
        {
            yamlContent += $"  Key{i}: Value{i}\n";
        }
        File.WriteAllText(_yamlPath, yamlContent);

        // 生成 TOML 配置
        var tomlContent = "[Data]\n";
        for (int i = 0; i < KeyCount; i++)
        {
            tomlContent += $"Key{i} = \"Value{i}\"\n";
        }
        File.WriteAllText(_tomlPath, tomlContent);
    }

    private void CreateCfgInstances()
    {
        _jsonCfg = new CfgBuilder()
            .AddJson(_jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        _iniCfg = new CfgBuilder()
            .AddIni(_iniPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        _xmlCfg = new CfgBuilder()
            .AddXml(_xmlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        _yamlCfg = new CfgBuilder()
            .AddYaml(_yamlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        _tomlCfg = new CfgBuilder()
            .AddToml(_tomlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        _jsonCfg?.Dispose();
        _iniCfg?.Dispose();
        _xmlCfg?.Dispose();
        _yamlCfg?.Dispose();
        _tomlCfg?.Dispose();

        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region 单键删除性能测试

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("RemoveSingle")]
    public void Json_RemoveSingle()
    {
        _jsonCfg.Remove("Key0");
    }

    [Benchmark]
    [BenchmarkCategory("RemoveSingle")]
    public void Ini_RemoveSingle()
    {
        _iniCfg.Remove("Data:Key0");
    }

    [Benchmark]
    [BenchmarkCategory("RemoveSingle")]
    public void Xml_RemoveSingle()
    {
        _xmlCfg.Remove("Data:Key0");
    }

    [Benchmark]
    [BenchmarkCategory("RemoveSingle")]
    public void Yaml_RemoveSingle()
    {
        _yamlCfg.Remove("Data:Key0");
    }

    [Benchmark]
    [BenchmarkCategory("RemoveSingle")]
    public void Toml_RemoveSingle()
    {
        _tomlCfg.Remove("Data:Key0");
    }

    #endregion

    #region 批量删除性能测试

    [Benchmark]
    [BenchmarkCategory("RemoveBatch")]
    public void Json_RemoveBatch()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _jsonCfg.Remove($"Key{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("RemoveBatch")]
    public void Ini_RemoveBatch()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _iniCfg.Remove($"Data:Key{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("RemoveBatch")]
    public void Xml_RemoveBatch()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _xmlCfg.Remove($"Data:Key{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("RemoveBatch")]
    public void Yaml_RemoveBatch()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _yamlCfg.Remove($"Data:Key{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("RemoveBatch")]
    public void Toml_RemoveBatch()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _tomlCfg.Remove($"Data:Key{i}");
        }
    }

    #endregion

    #region 删除不存在的键

    [Benchmark]
    [BenchmarkCategory("RemoveNonExistent")]
    public void Json_RemoveNonExistent()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _jsonCfg.Remove($"NonExistent:Key{i}");
        }
    }

    [Benchmark]
    [BenchmarkCategory("RemoveNonExistent")]
    public void Ini_RemoveNonExistent()
    {
        for (int i = 0; i < KeyCount; i++)
        {
            _iniCfg.Remove($"NonExistent:Key{i}");
        }
    }

    #endregion

    #region 删除后保存

    [Benchmark]
    [BenchmarkCategory("RemoveAndSave")]
    public async Task Json_RemoveAndSave()
    {
        for (int i = 0; i < KeyCount / 2; i++)
        {
            _jsonCfg.Remove($"Key{i}");
        }
        await _jsonCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("RemoveAndSave")]
    public async Task Ini_RemoveAndSave()
    {
        for (int i = 0; i < KeyCount / 2; i++)
        {
            _iniCfg.Remove($"Data:Key{i}");
        }
        await _iniCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("RemoveAndSave")]
    public async Task Xml_RemoveAndSave()
    {
        for (int i = 0; i < KeyCount / 2; i++)
        {
            _xmlCfg.Remove($"Data:Key{i}");
        }
        await _xmlCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("RemoveAndSave")]
    public async Task Yaml_RemoveAndSave()
    {
        for (int i = 0; i < KeyCount / 2; i++)
        {
            _yamlCfg.Remove($"Data:Key{i}");
        }
        await _yamlCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("RemoveAndSave")]
    public async Task Toml_RemoveAndSave()
    {
        for (int i = 0; i < KeyCount / 2; i++)
        {
            _tomlCfg.Remove($"Data:Key{i}");
        }
        await _tomlCfg.SaveAsync();
    }

    #endregion
}
