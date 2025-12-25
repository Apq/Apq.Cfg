using BenchmarkDotNet.Attributes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// GetSection 性能基准测试
/// 测试不同配置源的 GetSection/GetChildKeys 操作性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class GetSectionBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _jsonCfg = null!;
    private ICfgRoot _iniCfg = null!;
    private ICfgRoot _xmlCfg = null!;
    private ICfgRoot _yamlCfg = null!;
    private ICfgRoot _tomlCfg = null!;

    public GetSectionBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        // 创建 JSON 配置文件（包含多层嵌套）
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Connection": {
                        "Host": "localhost",
                        "Port": 5432,
                        "Name": "testdb"
                    },
                    "Pool": {
                        "MinSize": 5,
                        "MaxSize": 100
                    }
                },
                "App": {
                    "Name": "BenchmarkApp",
                    "Version": "1.0.0",
                    "Settings": {
                        "MaxRetries": 3,
                        "Timeout": 30,
                        "Enabled": true
                    }
                },
                "Logging": {
                    "Level": "Info",
                    "Output": "Console"
                }
            }
            """);
        _jsonCfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // 创建 INI 配置文件
        var iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(iniPath, """
            [Database:Connection]
            Host=localhost
            Port=5432
            Name=testdb

            [Database:Pool]
            MinSize=5
            MaxSize=100

            [App]
            Name=BenchmarkApp
            Version=1.0.0

            [App:Settings]
            MaxRetries=3
            Timeout=30
            Enabled=true

            [Logging]
            Level=Info
            Output=Console
            """);
        _iniCfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: false)
            .Build();

        // 创建 XML 配置文件
        var xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <Database>
                    <Connection>
                        <Host>localhost</Host>
                        <Port>5432</Port>
                        <Name>testdb</Name>
                    </Connection>
                    <Pool>
                        <MinSize>5</MinSize>
                        <MaxSize>100</MaxSize>
                    </Pool>
                </Database>
                <App>
                    <Name>BenchmarkApp</Name>
                    <Version>1.0.0</Version>
                    <Settings>
                        <MaxRetries>3</MaxRetries>
                        <Timeout>30</Timeout>
                        <Enabled>true</Enabled>
                    </Settings>
                </App>
                <Logging>
                    <Level>Info</Level>
                    <Output>Console</Output>
                </Logging>
            </configuration>
            """);
        _xmlCfg = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: false)
            .Build();

        // 创建 YAML 配置文件
        var yamlPath = Path.Combine(_testDir, "config.yaml");
        File.WriteAllText(yamlPath, """
            Database:
              Connection:
                Host: localhost
                Port: 5432
                Name: testdb
              Pool:
                MinSize: 5
                MaxSize: 100
            App:
              Name: BenchmarkApp
              Version: 1.0.0
              Settings:
                MaxRetries: 3
                Timeout: 30
                Enabled: true
            Logging:
              Level: Info
              Output: Console
            """);
        _yamlCfg = new CfgBuilder()
            .AddYaml(yamlPath, level: 0, writeable: false)
            .Build();

        // 创建 TOML 配置文件
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Database.Connection]
            Host = "localhost"
            Port = 5432
            Name = "testdb"

            [Database.Pool]
            MinSize = 5
            MaxSize = 100

            [App]
            Name = "BenchmarkApp"
            Version = "1.0.0"

            [App.Settings]
            MaxRetries = 3
            Timeout = 30
            Enabled = true

            [Logging]
            Level = "Info"
            Output = "Console"
            """);
        _tomlCfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
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

    #region GetSection 性能测试

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("GetSection")]
    public ICfgSection Json_GetSection() => _jsonCfg.GetSection("Database");

    [Benchmark]
    [BenchmarkCategory("GetSection")]
    public ICfgSection Ini_GetSection() => _iniCfg.GetSection("Database");

    [Benchmark]
    [BenchmarkCategory("GetSection")]
    public ICfgSection Xml_GetSection() => _xmlCfg.GetSection("Database");

    [Benchmark]
    [BenchmarkCategory("GetSection")]
    public ICfgSection Yaml_GetSection() => _yamlCfg.GetSection("Database");

    [Benchmark]
    [BenchmarkCategory("GetSection")]
    public ICfgSection Toml_GetSection() => _tomlCfg.GetSection("Database");

    #endregion

    #region GetSection 嵌套访问性能测试

    [Benchmark]
    [BenchmarkCategory("GetSectionNested")]
    public ICfgSection Json_GetSection_Nested() => _jsonCfg.GetSection("Database").GetSection("Connection");

    [Benchmark]
    [BenchmarkCategory("GetSectionNested")]
    public ICfgSection Ini_GetSection_Nested() => _iniCfg.GetSection("Database").GetSection("Connection");

    [Benchmark]
    [BenchmarkCategory("GetSectionNested")]
    public ICfgSection Xml_GetSection_Nested() => _xmlCfg.GetSection("Database").GetSection("Connection");

    [Benchmark]
    [BenchmarkCategory("GetSectionNested")]
    public ICfgSection Yaml_GetSection_Nested() => _yamlCfg.GetSection("Database").GetSection("Connection");

    [Benchmark]
    [BenchmarkCategory("GetSectionNested")]
    public ICfgSection Toml_GetSection_Nested() => _tomlCfg.GetSection("Database").GetSection("Connection");

    #endregion

    #region GetSection + Get 组合性能测试

    [Benchmark]
    [BenchmarkCategory("GetSectionThenGet")]
    public string? Json_GetSection_ThenGet() => _jsonCfg.GetSection("Database:Connection").Get("Host");

    [Benchmark]
    [BenchmarkCategory("GetSectionThenGet")]
    public string? Ini_GetSection_ThenGet() => _iniCfg.GetSection("Database:Connection").Get("Host");

    [Benchmark]
    [BenchmarkCategory("GetSectionThenGet")]
    public string? Xml_GetSection_ThenGet() => _xmlCfg.GetSection("Database:Connection").Get("Host");

    [Benchmark]
    [BenchmarkCategory("GetSectionThenGet")]
    public string? Yaml_GetSection_ThenGet() => _yamlCfg.GetSection("Database:Connection").Get("Host");

    [Benchmark]
    [BenchmarkCategory("GetSectionThenGet")]
    public string? Toml_GetSection_ThenGet() => _tomlCfg.GetSection("Database:Connection").Get("Host");

    #endregion

    #region GetChildKeys 性能测试

    [Benchmark]
    [BenchmarkCategory("GetChildKeys")]
    public IEnumerable<string> Json_GetChildKeys() => _jsonCfg.GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("GetChildKeys")]
    public IEnumerable<string> Ini_GetChildKeys() => _iniCfg.GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("GetChildKeys")]
    public IEnumerable<string> Xml_GetChildKeys() => _xmlCfg.GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("GetChildKeys")]
    public IEnumerable<string> Yaml_GetChildKeys() => _yamlCfg.GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("GetChildKeys")]
    public IEnumerable<string> Toml_GetChildKeys() => _tomlCfg.GetChildKeys();

    #endregion

    #region Section GetChildKeys 性能测试

    [Benchmark]
    [BenchmarkCategory("SectionGetChildKeys")]
    public IEnumerable<string> Json_Section_GetChildKeys() => _jsonCfg.GetSection("Database").GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("SectionGetChildKeys")]
    public IEnumerable<string> Ini_Section_GetChildKeys() => _iniCfg.GetSection("Database").GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("SectionGetChildKeys")]
    public IEnumerable<string> Xml_Section_GetChildKeys() => _xmlCfg.GetSection("Database").GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("SectionGetChildKeys")]
    public IEnumerable<string> Yaml_Section_GetChildKeys() => _yamlCfg.GetSection("Database").GetChildKeys();

    [Benchmark]
    [BenchmarkCategory("SectionGetChildKeys")]
    public IEnumerable<string> Toml_Section_GetChildKeys() => _tomlCfg.GetSection("Database").GetChildKeys();

    #endregion

    #region 直接 Get vs GetSection+Get 对比

    [Benchmark]
    [BenchmarkCategory("DirectVsSection")]
    public string? Json_DirectGet() => _jsonCfg.Get("Database:Connection:Host");

    [Benchmark]
    [BenchmarkCategory("DirectVsSection")]
    public string? Json_SectionGet() => _jsonCfg.GetSection("Database:Connection").Get("Host");

    #endregion
}
