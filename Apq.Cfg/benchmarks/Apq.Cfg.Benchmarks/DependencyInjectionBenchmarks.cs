using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 依赖注入集成性能测试
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class DependencyInjectionBenchmarks
{
    private string _testDir = null!;
    private string _jsonPath = null!;
    private IServiceProvider _provider = null!;

    [GlobalSetup]
    public void Setup()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgDIBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        _jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(_jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432,
                    "Name": "testdb"
                },
                "Logging": {
                    "Level": "Warning",
                    "Format": "json"
                },
                "App": {
                    "Name": "BenchmarkApp",
                    "Version": "1.0.0",
                    "Debug": false,
                    "Endpoints": {
                        "0": {
                            "Host": "api1.local",
                            "Port": 8001
                        },
                        "1": {
                            "Host": "api2.local",
                            "Port": 8002
                        }
                    }
                }
            }
            """);

        // 预构建一个 ServiceProvider 用于解析测试
        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(_jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("Database");
        services.ConfigureApqCfg<LoggingOptions>("Logging");
        services.ConfigureApqCfg<AppOptions>("App");
        _provider = services.BuildServiceProvider();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        (_provider as IDisposable)?.Dispose();
        if (Directory.Exists(_testDir))
        {
            try { Directory.Delete(_testDir, true); }
            catch { }
        }
    }

    // ========== AddApqCfg 注册性能 ==========

    [Benchmark(Description = "AddApqCfg_Register")]
    public IServiceProvider AddApqCfg_Register()
    {
        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(_jsonPath, level: 0, writeable: false));
        var provider = services.BuildServiceProvider();
        (provider as IDisposable)?.Dispose();
        return provider;
    }

    [Benchmark(Description = "AddApqCfg_WithOptions_Register")]
    public IServiceProvider AddApqCfg_WithOptions_Register()
    {
        var services = new ServiceCollection();
        services.AddApqCfg<DatabaseOptions>(
            cfg => cfg.AddJsonFile(_jsonPath, level: 0, writeable: false),
            sectionKey: "Database");
        var provider = services.BuildServiceProvider();
        (provider as IDisposable)?.Dispose();
        return provider;
    }

    // ========== ConfigureApqCfg 注册性能 ==========

    [Benchmark(Description = "ConfigureApqCfg_Single")]
    public IServiceProvider ConfigureApqCfg_Single()
    {
        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(_jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("Database");
        var provider = services.BuildServiceProvider();
        (provider as IDisposable)?.Dispose();
        return provider;
    }

    [Benchmark(Description = "ConfigureApqCfg_Multiple")]
    public IServiceProvider ConfigureApqCfg_Multiple()
    {
        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(_jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("Database");
        services.ConfigureApqCfg<LoggingOptions>("Logging");
        services.ConfigureApqCfg<AppOptions>("App");
        var provider = services.BuildServiceProvider();
        (provider as IDisposable)?.Dispose();
        return provider;
    }

    // ========== IOptions<T> 解析性能 ==========

    [Benchmark(Description = "Resolve_IOptions")]
    public DatabaseOptions Resolve_IOptions()
    {
        return _provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    }

    [Benchmark(Description = "Resolve_IOptions_100")]
    public DatabaseOptions Resolve_IOptions_100()
    {
        DatabaseOptions result = null!;
        for (int i = 0; i < 100; i++)
        {
            result = _provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        }
        return result;
    }

    // ========== IOptionsMonitor<T> 解析性能 ==========

    [Benchmark(Description = "Resolve_IOptionsMonitor")]
    public DatabaseOptions Resolve_IOptionsMonitor()
    {
        return _provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>().CurrentValue;
    }

    [Benchmark(Description = "Resolve_IOptionsMonitor_100")]
    public DatabaseOptions Resolve_IOptionsMonitor_100()
    {
        DatabaseOptions result = null!;
        var monitor = _provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
        for (int i = 0; i < 100; i++)
        {
            result = monitor.CurrentValue;
        }
        return result;
    }

    // ========== IOptionsSnapshot<T> 解析性能 ==========

    [Benchmark(Description = "Resolve_IOptionsSnapshot")]
    public DatabaseOptions Resolve_IOptionsSnapshot()
    {
        using var scope = _provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>().Value;
    }

    [Benchmark(Description = "Resolve_IOptionsSnapshot_100")]
    public DatabaseOptions Resolve_IOptionsSnapshot_100()
    {
        DatabaseOptions result = null!;
        for (int i = 0; i < 100; i++)
        {
            using var scope = _provider.CreateScope();
            result = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>().Value;
        }
        return result;
    }

    // ========== ICfgRoot 解析性能 ==========

    [Benchmark(Description = "Resolve_ICfgRoot")]
    public ICfgRoot Resolve_ICfgRoot()
    {
        return _provider.GetRequiredService<ICfgRoot>();
    }

    [Benchmark(Description = "Resolve_ICfgRoot_ThenGet")]
    public string? Resolve_ICfgRoot_ThenGet()
    {
        var cfg = _provider.GetRequiredService<ICfgRoot>();
        return cfg["Database:Host"];
    }

    // ========== 复杂对象解析性能 ==========

    [Benchmark(Description = "Resolve_ComplexOptions")]
    public AppOptions Resolve_ComplexOptions()
    {
        return _provider.GetRequiredService<IOptions<AppOptions>>().Value;
    }

    // ========== 多选项解析性能 ==========

    [Benchmark(Description = "Resolve_MultipleOptions")]
    public (DatabaseOptions, LoggingOptions, AppOptions) Resolve_MultipleOptions()
    {
        var db = _provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        var log = _provider.GetRequiredService<IOptions<LoggingOptions>>().Value;
        var app = _provider.GetRequiredService<IOptions<AppOptions>>().Value;
        return (db, log, app);
    }

    // ========== 测试用选项类 ==========

    public class DatabaseOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Name { get; set; }
    }

    public class LoggingOptions
    {
        public string? Level { get; set; }
        public string? Format { get; set; }
    }

    public class AppOptions
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public bool Debug { get; set; }
        public List<EndpointOptions>? Endpoints { get; set; }
    }

    public class EndpointOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}
