namespace Apq.Cfg.Tests;

/// <summary>
/// JSON 配置源测试
/// </summary>
public class JsonCfgTests : IDisposable
{
    private readonly string _testDir;

    public JsonCfgTests()
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

    [Fact]
    public void Get_SimpleValue_ReturnsCorrectValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "AppName": "TestApp",
                "Version": "1.0.0"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("TestApp", cfg["AppName"]);
        Assert.Equal("1.0.0", cfg["Version"]);
    }

    [Fact]
    public void Get_NestedValue_ReturnsCorrectValue()
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

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("localhost", cfg["Database:Host"]);
        Assert.Equal("5432", cfg["Database:Port"]);
    }

    [Fact]
    public void Get_TypedValue_ReturnsCorrectType()
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

        // Act & Assert
        Assert.Equal(3, cfg.GetValue<int>("Settings:MaxRetries"));
        Assert.True(cfg.GetValue<bool>("Settings:Enabled"));
        Assert.Equal(30.5, cfg.GetValue<double>("Settings:Timeout"));
    }

    [Fact]
    public void Exists_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.Exists("Key"));
        Assert.False(cfg.Exists("NonExistent"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Original": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert - 重新读取验证
        using var cfg2 = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2["NewKey"]);
        Assert.Equal("Value", cfg2["Original"]);
    }

    [Fact]
    public async Task Remove_AndSave_RemovesKey()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"ToRemove": "Value", "ToKeep": "Value2"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Remove("ToRemove");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // 验证删除后值为 null 或空字符串
        var removedValue = cfg2["ToRemove"];
        Assert.True(string.IsNullOrEmpty(removedValue), $"Expected null or empty, but got: '{removedValue}'");
        Assert.Equal("Value2", cfg2["ToKeep"]);
    }

    [Fact]
    public void MultiLevel_HigherLevelOverrides()
    {
        // Arrange
        var basePath = Path.Combine(_testDir, "base.json");
        var overridePath = Path.Combine(_testDir, "override.json");

        File.WriteAllText(basePath, """
            {
                "Setting1": "BaseValue1",
                "Setting2": "BaseValue2"
            }
            """);

        File.WriteAllText(overridePath, """
            {
                "Setting1": "OverrideValue1"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(basePath, level: 0, writeable: false)
            .AddJsonFile(overridePath, level: 1, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("OverrideValue1", cfg["Setting1"]); // 被覆盖
        Assert.Equal("BaseValue2", cfg["Setting2"]); // 保持原值
    }

    [Fact]
    public void ToMicrosoftConfiguration_ReturnsValidConfiguration()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var msConfig = cfg.ToMicrosoftConfiguration();

        // Assert
        Assert.NotNull(msConfig);
        Assert.Equal("TestApp", msConfig["App:Name"]);
    }

    [Fact]
    public void Get_LongValue_ReturnsCorrectType()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"BigNumber": 9223372036854775807}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(9223372036854775807L, cfg.GetValue<long>("BigNumber"));
    }

    [Fact]
    public void Get_DecimalValue_ReturnsCorrectType()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Price": 123.456}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(123.456m, cfg.GetValue<decimal>("Price"));
    }

    public enum TestLogLevel { Debug, Info, Warning, Error }

    [Fact]
    public void Get_EnumValue_ReturnsCorrectType()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"LogLevel": "Warning"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(TestLogLevel.Warning, cfg.GetValue<TestLogLevel>("LogLevel"));
    }

    [Fact]
    public void Get_EnumValue_CaseInsensitive()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"LogLevel": "warning"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(TestLogLevel.Warning, cfg.GetValue<TestLogLevel>("LogLevel"));
    }

    [Fact]
    public void Get_InvalidValue_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"NotANumber": "abc"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - 无效值返回默认值（与 Microsoft.Extensions.Configuration 行为一致）
        Assert.Equal(default(int), cfg.GetValue<int>("NotANumber"));
    }

    [Fact]
    public void Get_NonExistentKey_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Null(cfg["NonExistent"]);
        Assert.Equal(default, cfg.GetValue<int>("NonExistent"));
        Assert.Null(cfg.GetValue<string>("NonExistent"));
    }

    [Fact]
    public void Get_NullableInt_ReturnsCorrectValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"NullableInt": 42}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(42, cfg.GetValue<int?>("NullableInt"));
        Assert.Null(cfg.GetValue<int?>("NonExistent"));
    }
}
