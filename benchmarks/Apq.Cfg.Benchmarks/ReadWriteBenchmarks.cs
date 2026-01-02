using BenchmarkDotNet.Attributes;
using Apq.Cfg.Env;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 读写性能基准测试
/// 测试不同配置源的 Get/Set/Exists 操作性能
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class ReadWriteBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _jsonCfg = null!;
    private ICfgRoot _envCfg = null!;
    private ICfgRoot _iniCfg = null!;
    private ICfgRoot _xmlCfg = null!;
    private ICfgRoot _yamlCfg = null!;
    private ICfgRoot _tomlCfg = null!;

    public ReadWriteBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        // 创建 JSON 配置文件
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432,
                    "Name": "testdb"
                },
                "App": {
                    "Name": "BenchmarkApp",
                    "Version": "1.0.0",
                    "MaxRetries": 3,
                    "Enabled": true
                }
            }
            """);
        _jsonCfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 .env 配置文件
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            DATABASE_HOST=localhost
            DATABASE_PORT=5432
            DATABASE_NAME=testdb
            APP_NAME=BenchmarkApp
            APP_VERSION=1.0.0
            APP_MAXRETRIES=3
            APP_ENABLED=true
            """);
        _envCfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 INI 配置文件
        var iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(iniPath, """
            [Database]
            Host=localhost
            Port=5432
            Name=testdb

            [App]
            Name=BenchmarkApp
            Version=1.0.0
            MaxRetries=3
            Enabled=true
            """);
        _iniCfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 XML 配置文件
        var xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <Database>
                    <Host>localhost</Host>
                    <Port>5432</Port>
                    <Name>testdb</Name>
                </Database>
                <App>
                    <Name>BenchmarkApp</Name>
                    <Version>1.0.0</Version>
                    <MaxRetries>3</MaxRetries>
                    <Enabled>true</Enabled>
                </App>
            </configuration>
            """);
        _xmlCfg = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 YAML 配置文件
        var yamlPath = Path.Combine(_testDir, "config.yaml");
        File.WriteAllText(yamlPath, """
            Database:
              Host: localhost
              Port: 5432
              Name: testdb
            App:
              Name: BenchmarkApp
              Version: 1.0.0
              MaxRetries: 3
              Enabled: true
            """);
        _yamlCfg = new CfgBuilder()
            .AddYaml(yamlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 创建 TOML 配置文件
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Database]
            Host = "localhost"
            Port = 5432
            Name = "testdb"

            [App]
            Name = "BenchmarkApp"
            Version = "1.0.0"
            MaxRetries = 3
            Enabled = true
            """);
        _tomlCfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: true, isPrimaryWriter: true)
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
        _envCfg?.Dispose();
        _iniCfg?.Dispose();
        _xmlCfg?.Dispose();
        _yamlCfg?.Dispose();
        _tomlCfg?.Dispose();

        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region Get 性能测试

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Get")]
    public string? Json_Get() => _jsonCfg.Get("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Get")]
    public string? Env_Get() => _envCfg.Get("DATABASE_HOST");

    [Benchmark]
    [BenchmarkCategory("Get")]
    public string? Ini_Get() => _iniCfg.Get("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Get")]
    public string? Xml_Get() => _xmlCfg.Get("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Get")]
    public string? Yaml_Get() => _yamlCfg.Get("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Get")]
    public string? Toml_Get() => _tomlCfg.Get("Database:Host");

    #endregion

    #region Get<T> 类型转换性能测试

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Json_GetInt() => _jsonCfg.GetValue<int>("Database:Port");

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Env_GetInt() => _envCfg.GetValue<int>("DATABASE_PORT");

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Ini_GetInt() => _iniCfg.GetValue<int>("Database:Port");

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Xml_GetInt() => _xmlCfg.GetValue<int>("Database:Port");

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Yaml_GetInt() => _yamlCfg.GetValue<int>("Database:Port");

    [Benchmark]
    [BenchmarkCategory("GetTyped")]
    public int Toml_GetInt() => _tomlCfg.GetValue<int>("Database:Port");

    #endregion

    #region Exists 性能测试

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Json_Exists() => _jsonCfg.Exists("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Env_Exists() => _envCfg.Exists("DATABASE_HOST");

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Ini_Exists() => _iniCfg.Exists("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Xml_Exists() => _xmlCfg.Exists("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Yaml_Exists() => _yamlCfg.Exists("Database:Host");

    [Benchmark]
    [BenchmarkCategory("Exists")]
    public bool Toml_Exists() => _tomlCfg.Exists("Database:Host");

    #endregion

    #region Set 性能测试

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Json_Set() => _jsonCfg.Set("App:TempKey", "TempValue");

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Env_Set() => _envCfg.Set("APP_TEMPKEY", "TempValue");

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Ini_Set() => _iniCfg.Set("App:TempKey", "TempValue");

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Xml_Set() => _xmlCfg.Set("App:TempKey", "TempValue");

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Yaml_Set() => _yamlCfg.Set("App:TempKey", "TempValue");

    [Benchmark]
    [BenchmarkCategory("Set")]
    public void Toml_Set() => _tomlCfg.Set("App:TempKey", "TempValue");

    #endregion
}
