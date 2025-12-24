using BenchmarkDotNet.Attributes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 持久化性能基准测试
/// 测试 SaveAsync 在不同数据量下的性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class SaveBenchmarks : IDisposable
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

    [Params(50)]
    public int ChangeCount { get; set; }

    public SaveBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        // 创建 JSON 配置文件
        _jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(_jsonPath, """
            {
                "App": {
                    "Name": "BenchmarkApp"
                }
            }
            """);
        _jsonCfg = new CfgBuilder()
            .AddJson(_jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 INI 配置文件
        _iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(_iniPath, """
            [App]
            Name=BenchmarkApp
            """);
        _iniCfg = new CfgBuilder()
            .AddIni(_iniPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 XML 配置文件
        _xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(_xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <App>
                    <Name>BenchmarkApp</Name>
                </App>
            </configuration>
            """);
        _xmlCfg = new CfgBuilder()
            .AddXml(_xmlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 YAML 配置文件
        _yamlPath = Path.Combine(_testDir, "config.yaml");
        File.WriteAllText(_yamlPath, """
            App:
              Name: BenchmarkApp
            """);
        _yamlCfg = new CfgBuilder()
            .AddYaml(_yamlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 TOML 配置文件
        _tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(_tomlPath, """
            [App]
            Name = "BenchmarkApp"
            """);
        _tomlCfg = new CfgBuilder()
            .AddToml(_tomlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // 每次迭代前重置配置文件，确保测试一致性
        File.WriteAllText(_jsonPath, """
            {
                "App": {
                    "Name": "BenchmarkApp"
                }
            }
            """);
        File.WriteAllText(_iniPath, """
            [App]
            Name=BenchmarkApp
            """);
        File.WriteAllText(_xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <App>
                    <Name>BenchmarkApp</Name>
                </App>
            </configuration>
            """);
        File.WriteAllText(_yamlPath, """
            App:
              Name: BenchmarkApp
            """);
        File.WriteAllText(_tomlPath, """
            [App]
            Name = "BenchmarkApp"
            """);
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

    #region SaveAsync 性能测试

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Save")]
    public async Task Json_Save()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _jsonCfg.Set($"Data:Key{i}", $"Value{i}");
        }
        await _jsonCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("Save")]
    public async Task Ini_Save()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _iniCfg.Set($"Data:Key{i}", $"Value{i}");
        }
        await _iniCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("Save")]
    public async Task Xml_Save()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _xmlCfg.Set($"Data:Key{i}", $"Value{i}");
        }
        await _xmlCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("Save")]
    public async Task Yaml_Save()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _yamlCfg.Set($"Data:Key{i}", $"Value{i}");
        }
        await _yamlCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("Save")]
    public async Task Toml_Save()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _tomlCfg.Set($"Data:Key{i}", $"Value{i}");
        }
        await _tomlCfg.SaveAsync();
    }

    #endregion

    #region 大值保存测试

    [Benchmark]
    [BenchmarkCategory("SaveLargeValue")]
    public async Task Json_SaveLargeValue()
    {
        var largeValue = new string('X', 10000);
        for (int i = 0; i < ChangeCount / 10; i++)
        {
            _jsonCfg.Set($"LargeData:Key{i}", largeValue);
        }
        await _jsonCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("SaveLargeValue")]
    public async Task Ini_SaveLargeValue()
    {
        var largeValue = new string('X', 10000);
        for (int i = 0; i < ChangeCount / 10; i++)
        {
            _iniCfg.Set($"LargeData:Key{i}", largeValue);
        }
        await _iniCfg.SaveAsync();
    }

    #endregion

    #region 频繁保存测试

    [Benchmark]
    [BenchmarkCategory("FrequentSave")]
    public async Task Json_FrequentSave()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _jsonCfg.Set($"Frequent:Key{i}", $"Value{i}");
            if (i % 10 == 0)
            {
                await _jsonCfg.SaveAsync();
            }
        }
        await _jsonCfg.SaveAsync();
    }

    [Benchmark]
    [BenchmarkCategory("FrequentSave")]
    public async Task Ini_FrequentSave()
    {
        for (int i = 0; i < ChangeCount; i++)
        {
            _iniCfg.Set($"Frequent:Key{i}", $"Value{i}");
            if (i % 10 == 0)
            {
                await _iniCfg.SaveAsync();
            }
        }
        await _iniCfg.SaveAsync();
    }

    #endregion
}
