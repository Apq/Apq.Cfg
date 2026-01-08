namespace Apq.Cfg.Tests;

/// <summary>
/// ICfgSection 配置节测试
/// </summary>
public class CfgSectionTests : IDisposable
{
    private readonly string _testDir;

    public CfgSectionTests()
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

    #region GetSection 基本功能

    [Fact]
    public void GetSection_SimpleSection_ReturnsCorrectValues()
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

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var dbSection = cfg.GetSection("Database");

        // Assert
        Assert.Equal("Database", dbSection.Path);
        Assert.Equal("localhost", dbSection["Host"]);
        Assert.Equal("5432", dbSection["Port"]);
        Assert.Equal("testdb", dbSection["Name"]);
    }

    [Fact]
    public void GetSection_TypedGet_ReturnsCorrectType()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Settings": {
                    "MaxRetries": 3,
                    "Enabled": true,
                    "Timeout": 30.5
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Settings");

        // Assert
        Assert.Equal(3, section.GetValue<int>("MaxRetries"));
        Assert.True(section.GetValue<bool>("Enabled"));
        Assert.Equal(30.5, section.GetValue<double>("Timeout"));
    }

    [Fact]
    public void GetSection_NestedSection_ReturnsCorrectValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Connection": {
                        "Host": "localhost",
                        "Port": 5432
                    },
                    "Pool": {
                        "MinSize": 5,
                        "MaxSize": 100
                    }
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var dbSection = cfg.GetSection("Database");
        var connSection = dbSection.GetSection("Connection");
        var poolSection = dbSection.GetSection("Pool");

        // Assert
        Assert.Equal("Database:Connection", connSection.Path);
        Assert.Equal("localhost", connSection["Host"]);
        Assert.Equal(5432, connSection.GetValue<int>("Port"));

        Assert.Equal("Database:Pool", poolSection.Path);
        Assert.Equal(5, poolSection.GetValue<int>("MinSize"));
        Assert.Equal(100, poolSection.GetValue<int>("MaxSize"));
    }

    [Fact]
    public void GetSection_Exists_ReturnsCorrectResult()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "TestApp",
                    "Version": "1.0.0"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("App");

        // Assert
        Assert.True(section.Exists("Name"));
        Assert.True(section.Exists("Version"));
        Assert.False(section.Exists("NonExistent"));
    }

    #endregion

    #region GetSection 写入功能

    [Fact]
    public async Task GetSection_Set_UpdatesValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "OldName"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        var section = cfg.GetSection("App");
        section.SetValue("Name", "NewName");
        await cfg.SaveAsync();

        // Assert
        Assert.Equal("NewName", section["Name"]);
        Assert.Equal("NewName", cfg["App:Name"]);
    }

    [Fact]
    public async Task GetSection_Remove_RemovesKey()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "TestApp",
                    "ToRemove": "RemoveMe"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        var section = cfg.GetSection("App");
        section.Remove("ToRemove");
        await cfg.SaveAsync();

        // Assert
        Assert.True(section.Exists("Name"));
        Assert.False(section.Exists("ToRemove"));
    }

    #endregion

    #region GetChildKeys 功能

    [Fact]
    public void GetChildKeys_ReturnsAllTopLevelKeys()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": { "Name": "Test" },
                "Database": { "Host": "localhost" },
                "Logging": { "Level": "Info" }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var keys = cfg.GetChildKeys().ToList();

        // Assert
        Assert.Contains("App", keys);
        Assert.Contains("Database", keys);
        Assert.Contains("Logging", keys);
        Assert.Equal(3, keys.Count);
    }

    [Fact]
    public void GetSection_GetChildKeys_ReturnsSectionKeys()
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

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Database");
        var keys = section.GetChildKeys().ToList();

        // Assert
        Assert.Contains("Host", keys);
        Assert.Contains("Port", keys);
        Assert.Contains("Name", keys);
        Assert.Equal(3, keys.Count);
    }

    #endregion

    #region GetSection 边界情况

    [Fact]
    public void GetSection_NonExistentSection_ReturnsEmptySection()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("NonExistent");

        // Assert
        Assert.Equal("NonExistent", section.Path);
        Assert.Null(section["AnyKey"]);
        Assert.False(section.Exists("AnyKey"));
    }

    [Fact]
    public void GetSection_EmptyPath_WorksCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"TopLevel": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act - 获取空路径的 section，然后访问顶级键
        var section = cfg.GetSection("");

        // Assert
        Assert.Equal("", section.Path);
        Assert.Equal("Value", section["TopLevel"]);
    }

    [Fact]
    public void GetSection_DeepNesting_WorksCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Level1": {
                    "Level2": {
                        "Level3": {
                            "Level4": {
                                "DeepValue": "Found"
                            }
                        }
                    }
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var section = cfg.GetSection("Level1")
            .GetSection("Level2")
            .GetSection("Level3")
            .GetSection("Level4");

        // Assert
        Assert.Equal("Level1:Level2:Level3:Level4", section.Path);
        Assert.Equal("Found", section["DeepValue"]);
    }

    #endregion

    #region GetValue 带默认值扩展方法

    [Fact]
    public void GetValue_WithDefault_ExistingKey_ReturnsValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"IntValue": 42}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(42, cfg.GetValue("IntValue", 0));
    }

    [Fact]
    public void GetValue_WithDefault_NonExistingKey_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(100, cfg.GetValue("NonExistent", 100));
        Assert.Equal("DefaultString", cfg.GetValue("NonExistent", "DefaultString"));
    }

    #endregion
}
