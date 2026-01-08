using Apq.Cfg;

namespace Apq.Cfg.Tests;

/// <summary>
/// Source Generator 生成的配置类测试
/// </summary>
public class SourceGeneratorTests : IDisposable
{
    private readonly string _testDir;

    public SourceGeneratorTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgSgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void BindFrom_SimpleProperties_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "simple.json");
        File.WriteAllText(jsonPath, @"{
            ""Database"": {
                ""ConnectionString"": ""Server=localhost;Database=test"",
                ""Timeout"": 30,
                ""MaxRetries"": 3,
                ""EnableLogging"": true
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Database");
        var config = TestDatabaseConfig.BindFrom(section);

        // Assert
        Assert.Equal("Server=localhost;Database=test", config.ConnectionString);
        Assert.Equal(30, config.Timeout);
        Assert.Equal(3, config.MaxRetries);
        Assert.True(config.EnableLogging);
    }

    [Fact]
    public void BindFrom_NestedObject_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "nested.json");
        File.WriteAllText(jsonPath, @"{
            ""App"": {
                ""Name"": ""TestApp"",
                ""Version"": ""1.0.0"",
                ""Database"": {
                    ""ConnectionString"": ""Server=db"",
                    ""Timeout"": 60,
                    ""MaxRetries"": 5,
                    ""EnableLogging"": false
                }
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("App");
        var config = TestAppConfig.BindFrom(section);

        // Assert
        Assert.Equal("TestApp", config.Name);
        Assert.Equal("1.0.0", config.Version);
        Assert.NotNull(config.Database);
        Assert.Equal("Server=db", config.Database.ConnectionString);
        Assert.Equal(60, config.Database.Timeout);
    }

    [Fact]
    public void BindFrom_ArrayProperty_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "array.json");
        File.WriteAllText(jsonPath, @"{
            ""Server"": {
                ""Name"": ""WebServer"",
                ""Ports"": [80, 443, 8080],
                ""AllowedHosts"": [""localhost"", ""127.0.0.1"", ""example.com""]
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Server");
        var config = TestServerConfig.BindFrom(section);

        // Assert
        Assert.Equal("WebServer", config.Name);
        Assert.NotNull(config.Ports);
        Assert.Equal(3, config.Ports.Length);
        Assert.Equal(80, config.Ports[0]);
        Assert.Equal(443, config.Ports[1]);
        Assert.Equal(8080, config.Ports[2]);
        Assert.NotNull(config.AllowedHosts);
        Assert.Equal(3, config.AllowedHosts.Count);
        Assert.Contains("localhost", config.AllowedHosts);
    }

    [Fact]
    public void BindFrom_DictionaryProperty_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "dict.json");
        File.WriteAllText(jsonPath, @"{
            ""Features"": {
                ""Name"": ""FeatureFlags"",
                ""Flags"": {
                    ""DarkMode"": true,
                    ""BetaFeatures"": false,
                    ""Analytics"": true
                }
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Features");
        var config = TestFeaturesConfig.BindFrom(section);

        // Assert
        Assert.Equal("FeatureFlags", config.Name);
        Assert.NotNull(config.Flags);
        Assert.Equal(3, config.Flags.Count);
        Assert.True(config.Flags["DarkMode"]);
        Assert.False(config.Flags["BetaFeatures"]);
        Assert.True(config.Flags["Analytics"]);
    }

    [Fact]
    public void BindFrom_NullableProperties_ShouldHandleNulls()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "nullable.json");
        File.WriteAllText(jsonPath, @"{
            ""Optional"": {
                ""RequiredValue"": ""test"",
                ""OptionalInt"": 42
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Optional");
        var config = TestOptionalConfig.BindFrom(section);

        // Assert
        Assert.Equal("test", config.RequiredValue);
        Assert.Equal(42, config.OptionalInt);
        Assert.Null(config.OptionalString);
    }

    [Fact]
    public void BindTo_ExistingObject_ShouldUpdateProperties()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "update.json");
        File.WriteAllText(jsonPath, @"{
            ""Database"": {
                ""ConnectionString"": ""NewConnection"",
                ""Timeout"": 120
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        var existingConfig = new TestDatabaseConfig
        {
            ConnectionString = "OldConnection",
            Timeout = 30,
            MaxRetries = 5,
            EnableLogging = true
        };

        // Act
        var section = cfg.GetSection("Database");
        TestDatabaseConfig.BindTo(section, existingConfig);

        // Assert
        Assert.Equal("NewConnection", existingConfig.ConnectionString);
        Assert.Equal(120, existingConfig.Timeout);
        // 未在配置中指定的属性保持原值
        Assert.Equal(5, existingConfig.MaxRetries);
        Assert.True(existingConfig.EnableLogging);
    }

    [Fact]
    public void BindFrom_EmptySection_ShouldReturnDefaultValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "empty.json");
        File.WriteAllText(jsonPath, @"{}");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("NonExistent");
        var config = TestDatabaseConfig.BindFrom(section);

        // Assert
        Assert.Null(config.ConnectionString);
        Assert.Equal(0, config.Timeout);
        Assert.Equal(0, config.MaxRetries);
        Assert.False(config.EnableLogging);
    }

    [Fact]
    public void GeneratedExtension_ShouldWork()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "ext.json");
        File.WriteAllText(jsonPath, @"{
            ""TestDatabase"": {
                ""ConnectionString"": ""ExtTest"",
                ""Timeout"": 99
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act - 使用生成的扩展方法
        var config = cfg.GetTestDatabaseConfig();

        // Assert
        Assert.Equal("ExtTest", config.ConnectionString);
        Assert.Equal(99, config.Timeout);
    }

    [Fact]
    public void BindFrom_ReadOnlyList_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "readonly_list.json");
        File.WriteAllText(jsonPath, @"{
            ""ReadOnly"": {
                ""Name"": ""TestReadOnly"",
                ""Tags"": [""tag1"", ""tag2"", ""tag3""]
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("ReadOnly");
        var config = TestReadOnlyConfig.BindFrom(section);

        // Assert
        Assert.Equal("TestReadOnly", config.Name);
        Assert.NotNull(config.Tags);
        Assert.Equal(3, config.Tags.Count);
        Assert.Equal("tag1", config.Tags[0]);
        Assert.Equal("tag2", config.Tags[1]);
        Assert.Equal("tag3", config.Tags[2]);
    }

    [Fact]
    public void BindFrom_ReadOnlyDictionary_ShouldBindCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "readonly_dict.json");
        File.WriteAllText(jsonPath, @"{
            ""ReadOnly"": {
                ""Name"": ""TestScores"",
                ""Scores"": {
                    ""Math"": 95,
                    ""English"": 88,
                    ""Science"": 92
                }
            }
        }");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("ReadOnly");
        var config = TestReadOnlyConfig.BindFrom(section);

        // Assert
        Assert.Equal("TestScores", config.Name);
        Assert.NotNull(config.Scores);
        Assert.Equal(3, config.Scores.Count);
        Assert.Equal(95, config.Scores["Math"]);
        Assert.Equal(88, config.Scores["English"]);
        Assert.Equal(92, config.Scores["Science"]);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }
        catch { }
    }
}

// 测试用配置类 - 使用 [CfgSection] 特性标记

[CfgSection("TestDatabase")]
public partial class TestDatabaseConfig
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; }
    public int MaxRetries { get; set; }
    public bool EnableLogging { get; set; }
}

[CfgSection("App")]
public partial class TestAppConfig
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public TestDatabaseConfig? Database { get; set; }
}

[CfgSection("Server")]
public partial class TestServerConfig
{
    public string? Name { get; set; }
    public int[]? Ports { get; set; }
    public List<string>? AllowedHosts { get; set; }
}

[CfgSection("Features")]
public partial class TestFeaturesConfig
{
    public string? Name { get; set; }
    public Dictionary<string, bool>? Flags { get; set; }
}

[CfgSection("Optional")]
public partial class TestOptionalConfig
{
    public string? RequiredValue { get; set; }
    public int? OptionalInt { get; set; }
    public string? OptionalString { get; set; }
}

[CfgSection("ReadOnly")]
public partial class TestReadOnlyConfig
{
    public string? Name { get; set; }
    public IReadOnlyList<string>? Tags { get; set; }
    public IReadOnlyDictionary<string, int>? Scores { get; set; }
}
