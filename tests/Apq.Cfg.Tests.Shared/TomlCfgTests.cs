using Apq.Cfg.Toml;

namespace Apq.Cfg.Tests;

/// <summary>
/// TOML 配置源测试
/// </summary>
public class TomlCfgTests : IDisposable
{
    private readonly string _testDir;

    public TomlCfgTests()
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
    public void Get_TomlValue_ReturnsCorrectValue()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Database]
            Host = "localhost"
            Port = 5432

            [App]
            Name = "TestApp"
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("localhost", cfg.Get("Database:Host"));
        Assert.Equal("5432", cfg.Get("Database:Port"));
        Assert.Equal("TestApp", cfg.Get("App:Name"));
    }

    [Fact]
    public void Get_TypedValue_ReturnsCorrectType()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Settings]
            MaxRetries = 5
            Enabled = true
            Timeout = 30.5
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.GetValue<int>("Settings:MaxRetries"));
        Assert.True(cfg.GetValue<bool>("Settings:Enabled"));
        Assert.Equal(30.5, cfg.GetValue<double>("Settings:Timeout"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [App]
            Original = "Value"
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Set("App:NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2.Get("App:NewKey"));
        Assert.Equal("Value", cfg2.Get("App:Original"));
    }

    [Fact]
    public void Get_NestedToml_ReturnsCorrectValue()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Level1.Level2.Level3]
            Value = "DeepValue"
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("DeepValue", cfg.Get("Level1:Level2:Level3:Value"));
    }

    [Fact]
    public void Exists_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [Section]
            Key = "Value"
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.Exists("Section:Key"));
        Assert.False(cfg.Exists("NonExistent"));
    }

    [Fact]
    public async Task Remove_AndSave_RemovesKey()
    {
        // Arrange
        var tomlPath = Path.Combine(_testDir, "config.toml");
        File.WriteAllText(tomlPath, """
            [App]
            ToRemove = "Value"
            ToKeep = "Value2"
            """);

        using var cfg = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Remove("App:ToRemove");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddToml(tomlPath, level: 0, writeable: false)
            .Build();

        var removedValue = cfg2.Get("App:ToRemove");
        Assert.True(string.IsNullOrEmpty(removedValue));
        Assert.Equal("Value2", cfg2.Get("App:ToKeep"));
    }
}
