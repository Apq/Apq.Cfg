using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.Tests;

/// <summary>
/// 依赖注入扩展测试
/// </summary>
public class ServiceCollectionExtensionsTests : IDisposable
{
    private readonly string _testDir;

    public ServiceCollectionExtensionsTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region AddApqCfg 基本功能

    [Fact]
    public void AddApqCfg_RegistersICfgRoot()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"AppName": "TestApp"}""");

        var services = new ServiceCollection();

        // Act
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();

        // Assert
        var cfgRoot = provider.GetService<ICfgRoot>();
        Assert.NotNull(cfgRoot);
        Assert.Equal("TestApp", cfgRoot["AppName"]);
    }

    [Fact]
    public void AddApqCfg_RegistersIConfigurationRoot()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"AppName": "TestApp"}""");

        var services = new ServiceCollection();

        // Act
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();

        // Assert
        var msConfig = provider.GetService<IConfigurationRoot>();
        Assert.NotNull(msConfig);
        Assert.Equal("TestApp", msConfig["AppName"]);
    }

    [Fact]
    public void AddApqCfg_RegistersAsSingleton()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();

        // Act
        var cfg1 = provider.GetService<ICfgRoot>();
        var cfg2 = provider.GetService<ICfgRoot>();

        // Assert
        Assert.Same(cfg1, cfg2);
    }

    #endregion

    #region AddApqCfg 工厂方法

    [Fact]
    public void AddApqCfg_WithFactory_RegistersICfgRoot()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"FactoryTest": "Success"}""");

        var services = new ServiceCollection();

        // Act
        services.AddApqCfg(sp =>
        {
            return new CfgBuilder()
                .AddJsonFile(jsonPath, level: 0, writeable: false)
                .Build();
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var cfgRoot = provider.GetService<ICfgRoot>();
        Assert.NotNull(cfgRoot);
        Assert.Equal("Success", cfgRoot["FactoryTest"]);
    }

    #endregion

    #region ConfigureApqCfg 强类型配置

    [Fact]
    public void ConfigureApqCfg_BindsOptions()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432,
                    "Name": "testdb"
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("Database");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        // Assert
        Assert.Equal("localhost", options.Host);
        Assert.Equal(5432, options.Port);
        Assert.Equal("testdb", options.Name);
    }

    [Fact]
    public void AddApqCfg_WithOptions_BindsOptions()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "MyApp",
                    "Version": "2.0.0",
                    "Debug": true
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg<AppOptions>(
            cfg => cfg.AddJsonFile(jsonPath, level: 0, writeable: false),
            sectionKey: "App");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<AppOptions>>().Value;

        // Assert
        Assert.Equal("MyApp", options.Name);
        Assert.Equal("2.0.0", options.Version);
        Assert.True(options.Debug);
    }

    [Fact]
    public void ConfigureApqCfg_WithNestedSection_BindsCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Services": {
                    "Api": {
                        "Url": "https://api.example.com",
                        "Timeout": 30
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<ApiOptions>("Services:Api");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<ApiOptions>>().Value;

        // Assert
        Assert.Equal("https://api.example.com", options.Url);
        Assert.Equal(30, options.Timeout);
    }

    #endregion

    #region 类型转换测试

    [Fact]
    public void ConfigureApqCfg_ConvertsVariousTypes()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "TypeTest": {
                    "IntValue": 42,
                    "LongValue": 9223372036854775807,
                    "BoolValue": true,
                    "DoubleValue": 3.14159,
                    "DecimalValue": 123.456,
                    "GuidValue": "550e8400-e29b-41d4-a716-446655440000",
                    "DateTimeValue": "2024-01-15T10:30:00"
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<TypeTestOptions>("TypeTest");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<TypeTestOptions>>().Value;

        // Assert
        Assert.Equal(42, options.IntValue);
        Assert.Equal(9223372036854775807L, options.LongValue);
        Assert.True(options.BoolValue);
        Assert.Equal(3.14159, options.DoubleValue, 5);
        Assert.Equal(123.456m, options.DecimalValue);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), options.GuidValue);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), options.DateTimeValue);
    }

    [Fact]
    public void ConfigureApqCfg_ConvertsEnumType()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Logging": {
                    "Level": "Warning"
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<LoggingOptions>("Logging");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<LoggingOptions>>().Value;

        // Assert
        Assert.Equal(LogLevel.Warning, options.Level);
    }

    #endregion

    #region 边界情况

    [Fact]
    public void ConfigureApqCfg_NonExistentSection_ReturnsDefaultValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Other": "Value"}""");

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("NonExistent");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        // Assert
        Assert.Null(options.Host);
        Assert.Equal(0, options.Port);
        Assert.Null(options.Name);
    }

    [Fact]
    public void AddApqCfg_CalledTwice_UsesFirstRegistration()
    {
        // Arrange
        var jsonPath1 = Path.Combine(_testDir, "config1.json");
        var jsonPath2 = Path.Combine(_testDir, "config2.json");
        File.WriteAllText(jsonPath1, """{"Source": "First"}""");
        File.WriteAllText(jsonPath2, """{"Source": "Second"}""");

        var services = new ServiceCollection();

        // Act - 第二次调用应该被忽略（TryAddSingleton）
        services.AddApqCfg(cfg => cfg.AddJsonFile(jsonPath1, level: 0, writeable: false));
        services.AddApqCfg(cfg => cfg.AddJsonFile(jsonPath2, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();
        var cfgRoot = provider.GetRequiredService<ICfgRoot>();

        // Assert
        Assert.Equal("First", cfgRoot["Source"]);
    }

    #endregion

    #region IOptionsMonitor 测试

    [Fact]
    public void ConfigureApqCfg_RegistersIOptionsMonitor()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true, reloadOnChange: true));
        services.ConfigureApqCfg<DatabaseOptions>("Database");

        var provider = services.BuildServiceProvider();

        // Act
        var monitor = provider.GetService<IOptionsMonitor<DatabaseOptions>>();

        // Assert
        Assert.NotNull(monitor);
        Assert.Equal("localhost", monitor.CurrentValue.Host);
        Assert.Equal(5432, monitor.CurrentValue.Port);
    }

    [Fact]
    public async Task IOptionsMonitor_NotifiesOnChange()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true, reloadOnChange: true));
        services.ConfigureApqCfg<DatabaseOptions>("Database");

        var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
        var cfgRoot = provider.GetRequiredService<ICfgRoot>();

        var changeNotified = false;
        string? newHost = null;

        monitor.OnChange((options, _) =>
        {
            changeNotified = true;
            newHost = options.Host;
        });

        // Act - 直接修改文件触发变更（而不是通过 Set 方法）
        // 因为 ConfigChanges 是通过文件监视器触发的
        await Task.Delay(100); // 等待文件监视器初始化
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "newhost.local",
                    "Port": 5432
                }
            }
            """);

        // 等待变更通知（文件监视器 + 防抖）
        await Task.Delay(500);

        // Assert
        Assert.True(changeNotified);
        Assert.Equal("newhost.local", newHost);
        Assert.Equal("newhost.local", monitor.CurrentValue.Host);
    }

    #endregion

    #region IOptionsSnapshot 测试

    [Fact]
    public void ConfigureApqCfg_RegistersIOptionsSnapshot()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DatabaseOptions>("Database");

        var provider = services.BuildServiceProvider();

        // Act - IOptionsSnapshot 是 Scoped，需要创建 scope
        using var scope = provider.CreateScope();
        var snapshot = scope.ServiceProvider.GetService<IOptionsSnapshot<DatabaseOptions>>();

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal("localhost", snapshot.Value.Host);
        Assert.Equal(5432, snapshot.Value.Port);
    }

    #endregion

    #region 嵌套对象绑定测试

    [Fact]
    public void ConfigureApqCfg_BindsNestedObjects()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "TestApp",
                    "Database": {
                        "Host": "db.local",
                        "Port": 3306
                    },
                    "Cache": {
                        "Host": "redis.local",
                        "Port": 6379
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<AppWithNestedOptions>("App");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<AppWithNestedOptions>>().Value;

        // Assert
        Assert.Equal("TestApp", options.Name);
        Assert.NotNull(options.Database);
        Assert.Equal("db.local", options.Database.Host);
        Assert.Equal(3306, options.Database.Port);
        Assert.NotNull(options.Cache);
        Assert.Equal("redis.local", options.Cache.Host);
        Assert.Equal(6379, options.Cache.Port);
    }

    [Fact]
    public void ConfigureApqCfg_BindsDeeplyNestedObjects()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Root": {
                    "Level1": {
                        "Level2": {
                            "Value": "DeepValue"
                        }
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DeepNestedOptions>("Root");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<DeepNestedOptions>>().Value;

        // Assert
        Assert.NotNull(options.Level1);
        Assert.NotNull(options.Level1.Level2);
        Assert.Equal("DeepValue", options.Level1.Level2.Value);
    }

    #endregion

    #region 集合绑定测试

    [Fact]
    public void ConfigureApqCfg_BindsStringArray()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Config": {
                    "Tags": {
                        "0": "tag1",
                        "1": "tag2",
                        "2": "tag3"
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<ArrayOptions>("Config");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<ArrayOptions>>().Value;

        // Assert
        Assert.NotNull(options.Tags);
        Assert.Collection(options.Tags,
            item => Assert.Equal("tag1", item),
            item => Assert.Equal("tag2", item),
            item => Assert.Equal("tag3", item));
    }

    [Fact]
    public void ConfigureApqCfg_BindsIntList()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Config": {
                    "Ports": {
                        "0": 80,
                        "1": 443,
                        "2": 8080
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<ListOptions>("Config");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<ListOptions>>().Value;

        // Assert
        Assert.NotNull(options.Ports);
        Assert.Collection(options.Ports,
            item => Assert.Equal(80, item),
            item => Assert.Equal(443, item),
            item => Assert.Equal(8080, item));
    }

    [Fact]
    public void ConfigureApqCfg_BindsDictionary()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Config": {
                    "Settings": {
                        "Key1": "Value1",
                        "Key2": "Value2"
                    }
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<DictionaryOptions>("Config");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<DictionaryOptions>>().Value;

        // Assert
        Assert.NotNull(options.Settings);
        Assert.Collection(options.Settings.OrderBy(x => x.Key),
            item => { Assert.Equal("Key1", item.Key); Assert.Equal("Value1", item.Value); },
            item => { Assert.Equal("Key2", item.Key); Assert.Equal("Value2", item.Value); });
    }

    [Fact]
    public void ConfigureApqCfg_BindsComplexObjectList()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Config": {
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

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: false));
        services.ConfigureApqCfg<ComplexListOptions>("Config");

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<ComplexListOptions>>().Value;

        // Assert
        Assert.NotNull(options.Endpoints);
        Assert.Collection(options.Endpoints,
            item => { Assert.Equal("api1.local", item.Host); Assert.Equal(8001, item.Port); },
            item => { Assert.Equal("api2.local", item.Host); Assert.Equal(8002, item.Port); });
    }

    #endregion

    #region ConfigureApqCfg 带变更回调测试

    [Fact]
    public async Task ConfigureApqCfg_WithOnChange_InvokesCallback()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config-callback.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432
                }
            }
            """);

        var services = new ServiceCollection();
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true, reloadOnChange: true));

        var callbackInvoked = false;
        string? newHost = null;

        services.ConfigureApqCfg<DatabaseOptions>("Database", options =>
        {
            callbackInvoked = true;
            newHost = options.Host;
        });

        var provider = services.BuildServiceProvider();

        // 触发 IOptionsMonitor 的创建和回调注册
        var monitor = provider.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
        // 解析 IDisposable 服务以确保回调被注册
        var disposables = provider.GetServices<IDisposable>().ToList();

        // Act - 直接修改文件触发变更
        await Task.Delay(200); // 等待文件监视器初始化
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "callback.local",
                    "Port": 5432
                }
            }
            """);

        // 等待变更通知（文件监视器 + 防抖 + 处理）
        await Task.Delay(800);

        // Assert
        Assert.True(callbackInvoked);
        Assert.Equal("callback.local", newHost);
    }

    #endregion

    #region 测试用选项类

    public class DatabaseOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Name { get; set; }
    }

    public class AppOptions
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public bool Debug { get; set; }
    }

    public class ApiOptions
    {
        public string? Url { get; set; }
        public int Timeout { get; set; }
    }

    public class TypeTestOptions
    {
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public bool BoolValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }
        public Guid GuidValue { get; set; }
        public DateTime DateTimeValue { get; set; }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class LoggingOptions
    {
        public LogLevel Level { get; set; }
    }

    // 嵌套对象测试用类
    public class AppWithNestedOptions
    {
        public string? Name { get; set; }
        public DatabaseOptions? Database { get; set; }
        public CacheOptions? Cache { get; set; }
    }

    public class CacheOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    public class DeepNestedOptions
    {
        public Level1Options? Level1 { get; set; }
    }

    public class Level1Options
    {
        public Level2Options? Level2 { get; set; }
    }

    public class Level2Options
    {
        public string? Value { get; set; }
    }

    // 集合测试用类
    public class ArrayOptions
    {
        public string[]? Tags { get; set; }
    }

    public class ListOptions
    {
        public List<int>? Ports { get; set; }
    }

    public class DictionaryOptions
    {
        public Dictionary<string, string>? Settings { get; set; }
    }

    public class ComplexListOptions
    {
        public List<EndpointOptions>? Endpoints { get; set; }
    }

    public class EndpointOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    #endregion
}
