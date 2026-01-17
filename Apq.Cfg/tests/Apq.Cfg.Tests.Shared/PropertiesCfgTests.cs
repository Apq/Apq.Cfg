using Apq.Cfg.Properties;

namespace Apq.Cfg.Tests;

/// <summary>
/// Properties 配置源测试
/// </summary>
public class PropertiesCfgTests : IDisposable
{
    private readonly string _testDir;

    public PropertiesCfgTests()
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
    public void Get_PropertiesValue_ReturnsCorrectValue()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            Database.Host=localhost
            Database.Port=5432
            App.Name=TestApp
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Apq.Cfg uses colon notation for nested paths
        Assert.Equal("localhost", cfg["Database:Host"]);
        Assert.Equal("5432", cfg["Database:Port"]);
        Assert.Equal("TestApp", cfg["App:Name"]);
    }

    [Fact]
    public void Get_TypedValue_ReturnsCorrectType()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            Settings.MaxRetries=5
            Settings.Enabled=true
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.GetValue<int>("Settings:MaxRetries"));
        Assert.True(cfg.GetValue<bool>("Settings:Enabled"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            App.Original=Value
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("App:NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2["App:NewKey"]);
        Assert.Equal("Value", cfg2["App:Original"]);
    }

    [Fact]
    public void Get_RootLevelKey_ReturnsValue()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            RootKey=RootValue
            App.Name=TestApp
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("RootValue", cfg["RootKey"]);
    }

    [Fact]
    public void Exists_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            Section.Key=Value
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Apq.Cfg uses colon notation for nested paths
        Assert.True(cfg.Exists("Section:Key"));
        Assert.False(cfg.Exists("NonExistent"));
    }

    [Fact]
    public async Task Remove_AndSave_RemovesKey()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            App.ToRemove=Value
            App.ToKeep=Value2
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Remove("App:ToRemove");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        var removedValue = cfg2["App:ToRemove"];
        Assert.True(string.IsNullOrEmpty(removedValue));
        Assert.Equal("Value2", cfg2["App:ToKeep"]);
    }

    [Fact]
    public void Get_WithColonSeparator_ReturnsValue()
    {
        // Arrange
        var propsPath = Path.Combine(_testDir, "config.properties");
        File.WriteAllText(propsPath, """
            Database:Connection:Timeout=30
            """, System.Text.Encoding.UTF8);

        using var cfg = new CfgBuilder()
            .AddPropertiesFile(propsPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Apq.Cfg uses colon notation for nested paths
        Assert.Equal("30", cfg["Database:Connection:Timeout"]);
    }
}
