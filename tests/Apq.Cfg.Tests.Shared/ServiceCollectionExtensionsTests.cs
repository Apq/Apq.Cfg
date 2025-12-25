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
            .AddJson(jsonPath, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();

        // Assert
        var cfgRoot = provider.GetService<ICfgRoot>();
        Assert.NotNull(cfgRoot);
        Assert.Equal("TestApp", cfgRoot.Get("AppName"));
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
            .AddJson(jsonPath, level: 0, writeable: false));

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
            .AddJson(jsonPath, level: 0, writeable: false));

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
                .AddJson(jsonPath, level: 0, writeable: false)
                .Build();
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var cfgRoot = provider.GetService<ICfgRoot>();
        Assert.NotNull(cfgRoot);
        Assert.Equal("Success", cfgRoot.Get("FactoryTest"));
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
            .AddJson(jsonPath, level: 0, writeable: false));
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
            cfg => cfg.AddJson(jsonPath, level: 0, writeable: false),
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
            .AddJson(jsonPath, level: 0, writeable: false));
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
            .AddJson(jsonPath, level: 0, writeable: false));
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
            .AddJson(jsonPath, level: 0, writeable: false));
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
            .AddJson(jsonPath, level: 0, writeable: false));
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
        services.AddApqCfg(cfg => cfg.AddJson(jsonPath1, level: 0, writeable: false));
        services.AddApqCfg(cfg => cfg.AddJson(jsonPath2, level: 0, writeable: false));

        var provider = services.BuildServiceProvider();
        var cfgRoot = provider.GetRequiredService<ICfgRoot>();

        // Assert
        Assert.Equal("First", cfgRoot.Get("Source"));
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

    #endregion
}
