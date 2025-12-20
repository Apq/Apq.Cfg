using Apq.Cfg.Ini;

namespace Apq.Cfg.Tests;

/// <summary>
/// INI 配置源测试
/// </summary>
public class IniCfgTests : IDisposable
{
    private readonly string _testDir;

    public IniCfgTests()
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
    public void Get_IniValue_ReturnsCorrectValue()
    {
        // Arrange
        var iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(iniPath, """
            [Database]
            Host=localhost
            Port=5432

            [App]
            Name=TestApp
            """);

        using var cfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: false)
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
        var iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(iniPath, """
            [Settings]
            MaxRetries=5
            Enabled=true
            """);

        using var cfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.Get<int>("Settings:MaxRetries"));
        Assert.True(cfg.Get<bool>("Settings:Enabled"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var iniPath = Path.Combine(_testDir, "config.ini");
        File.WriteAllText(iniPath, """
            [App]
            Original=Value
            """);

        using var cfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Set("App:NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2.Get("App:NewKey"));
        Assert.Equal("Value", cfg2.Get("App:Original"));
    }
}
