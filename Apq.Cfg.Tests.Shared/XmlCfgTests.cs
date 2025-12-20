using Apq.Cfg.Xml;

namespace Apq.Cfg.Tests;

/// <summary>
/// XML 配置源测试
/// </summary>
public class XmlCfgTests : IDisposable
{
    private readonly string _testDir;

    public XmlCfgTests()
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
    public void Get_XmlValue_ReturnsCorrectValue()
    {
        // Arrange
        var xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <Database>
                    <Host>localhost</Host>
                    <Port>5432</Port>
                </Database>
                <App>
                    <Name>TestApp</Name>
                </App>
            </configuration>
            """);

        using var cfg = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: false)
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
        var xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <Settings>
                    <MaxRetries>5</MaxRetries>
                    <Enabled>true</Enabled>
                </Settings>
            </configuration>
            """);

        using var cfg = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.Get<int>("Settings:MaxRetries"));
        Assert.True(cfg.Get<bool>("Settings:Enabled"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var xmlPath = Path.Combine(_testDir, "config.xml");
        File.WriteAllText(xmlPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
                <App>
                    <Original>Value</Original>
                </App>
            </configuration>
            """);

        using var cfg = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Set("App:NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddXml(xmlPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2.Get("App:NewKey"));
        Assert.Equal("Value", cfg2.Get("App:Original"));
    }
}
