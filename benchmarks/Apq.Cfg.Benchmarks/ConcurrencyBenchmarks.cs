using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 并发访问性能基准测试
/// 测试多线程同时读写配置的性能
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ConcurrencyBenchmarks : IDisposable
{
    private readonly string _testDir;
    private ICfgRoot _cfg = null!;
    private string _configPath = null!;

    [Params(1, 4, 8, 16)]
    public int ThreadCount { get; set; }

    [Params("Json", "Ini", "Xml", "Yaml", "Toml")]
    public string SourceType { get; set; } = "Json";

    public ConcurrencyBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [GlobalSetup]
    public void Setup()
    {
        _configPath = Path.Combine(_testDir, $"config.{SourceType.ToLower()}");

        switch (SourceType)
        {
            case "Json":
                File.WriteAllText(_configPath, """
                    {
                        "Database": {
                            "Host": "localhost",
                            "Port": 5432,
                            "Name": "testdb"
                        },
                        "App": {
                            "Name": "BenchmarkApp",
                            "Version": "1.0.0"
                        },
                        "Settings": {
                            "Key1": "Value1",
                            "Key2": "Value2",
                            "Key3": "Value3",
                            "Key4": "Value4",
                            "Key5": "Value5",
                            "Key6": "Value6",
                            "Key7": "Value7",
                            "Key8": "Value8",
                            "Key9": "Value9",
                            "Key10": "Value10",
                            "Key11": "Value11",
                            "Key12": "Value12",
                            "Key13": "Value13",
                            "Key14": "Value14",
                            "Key15": "Value15",
                            "Key16": "Value16"
                        }
                    }
                    """);
                _cfg = new CfgBuilder()
                    .AddJson(_configPath, level: 0, writeable: true, isPrimaryWriter: true)
                    .Build();
                break;

            case "Ini":
                File.WriteAllText(_configPath, """
                    [Database]
                    Host=localhost
                    Port=5432
                    Name=testdb

                    [App]
                    Name=BenchmarkApp
                    Version=1.0.0

                    [Settings]
                    Key1=Value1
                    Key2=Value2
                    Key3=Value3
                    Key4=Value4
                    Key5=Value5
                    Key6=Value6
                    Key7=Value7
                    Key8=Value8
                    Key9=Value9
                    Key10=Value10
                    Key11=Value11
                    Key12=Value12
                    Key13=Value13
                    Key14=Value14
                    Key15=Value15
                    Key16=Value16
                    """);
                _cfg = new CfgBuilder()
                    .AddIni(_configPath, level: 0, writeable: true, isPrimaryWriter: true)
                    .Build();
                break;

            case "Xml":
                File.WriteAllText(_configPath, """
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
                        </App>
                        <Settings>
                            <Key1>Value1</Key1>
                            <Key2>Value2</Key2>
                            <Key3>Value3</Key3>
                            <Key4>Value4</Key4>
                            <Key5>Value5</Key5>
                            <Key6>Value6</Key6>
                            <Key7>Value7</Key7>
                            <Key8>Value8</Key8>
                            <Key9>Value9</Key9>
                            <Key10>Value10</Key10>
                            <Key11>Value11</Key11>
                            <Key12>Value12</Key12>
                            <Key13>Value13</Key13>
                            <Key14>Value14</Key14>
                            <Key15>Value15</Key15>
                            <Key16>Value16</Key16>
                        </Settings>
                    </configuration>
                    """);
                _cfg = new CfgBuilder()
                    .AddXml(_configPath, level: 0, writeable: true, isPrimaryWriter: true)
                    .Build();
                break;

            case "Yaml":
                File.WriteAllText(_configPath, """
                    Database:
                      Host: localhost
                      Port: 5432
                      Name: testdb
                    App:
                      Name: BenchmarkApp
                      Version: 1.0.0
                    Settings:
                      Key1: Value1
                      Key2: Value2
                      Key3: Value3
                      Key4: Value4
                      Key5: Value5
                      Key6: Value6
                      Key7: Value7
                      Key8: Value8
                      Key9: Value9
                      Key10: Value10
                      Key11: Value11
                      Key12: Value12
                      Key13: Value13
                      Key14: Value14
                      Key15: Value15
                      Key16: Value16
                    """);
                _cfg = new CfgBuilder()
                    .AddYaml(_configPath, level: 0, writeable: true, isPrimaryWriter: true)
                    .Build();
                break;

            case "Toml":
                File.WriteAllText(_configPath, """
                    [Database]
                    Host = "localhost"
                    Port = 5432
                    Name = "testdb"

                    [App]
                    Name = "BenchmarkApp"
                    Version = "1.0.0"

                    [Settings]
                    Key1 = "Value1"
                    Key2 = "Value2"
                    Key3 = "Value3"
                    Key4 = "Value4"
                    Key5 = "Value5"
                    Key6 = "Value6"
                    Key7 = "Value7"
                    Key8 = "Value8"
                    Key9 = "Value9"
                    Key10 = "Value10"
                    Key11 = "Value11"
                    Key12 = "Value12"
                    Key13 = "Value13"
                    Key14 = "Value14"
                    Key15 = "Value15"
                    Key16 = "Value16"
                    """);
                _cfg = new CfgBuilder()
                    .AddToml(_configPath, level: 0, writeable: true, isPrimaryWriter: true)
                    .Build();
                break;
        }
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

    /// <summary>
    /// 多线程并发读取同一个键
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("ConcurrentRead")]
    public void ConcurrentRead_SameKey()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Get("Database:Host");
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发读取不同的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentRead")]
    public void ConcurrentRead_DifferentKeys()
    {
        var keys = new[] { "Settings:Key1", "Settings:Key2", "Settings:Key3", "Settings:Key4",
                          "Settings:Key5", "Settings:Key6", "Settings:Key7", "Settings:Key8",
                          "Settings:Key9", "Settings:Key10", "Settings:Key11", "Settings:Key12",
                          "Settings:Key13", "Settings:Key14", "Settings:Key15", "Settings:Key16" };
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var keyIndex = i % keys.Length;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Get(keys[keyIndex]);
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发写入不同的键
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentWrite")]
    public void ConcurrentWrite_DifferentKeys()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var threadId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _cfg.Set($"Temp:Thread{threadId}:Key{j}", $"Value{j}");
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程混合读写操作
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentMixed")]
    public void ConcurrentMixed_ReadWrite()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var threadId = i;
            var isReader = i % 2 == 0;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    if (isReader)
                    {
                        _ = _cfg.Get("Database:Host");
                        _ = _cfg.Exists("App:Name");
                    }
                    else
                    {
                        _cfg.Set($"Temp:Thread{threadId}:Key{j}", $"Value{j}");
                    }
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发检查键存在
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentExists")]
    public void ConcurrentExists()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _ = _cfg.Exists("Database:Host");
                    _ = _cfg.Exists("NonExistent:Key");
                }
            });
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 多线程并发写入同一个键（竞争场景）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("ConcurrentWrite")]
    public void ConcurrentWrite_SameKey()
    {
        var tasks = new Task[ThreadCount];
        for (int i = 0; i < ThreadCount; i++)
        {
            var threadId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _cfg.Set("Temp:SharedKey", $"Thread{threadId}_Value{j}");
                }
            });
        }
        Task.WaitAll(tasks);
    }
}
