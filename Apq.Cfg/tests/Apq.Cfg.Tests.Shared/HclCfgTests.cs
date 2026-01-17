using Apq.Cfg.Hcl;

namespace Apq.Cfg.Tests;

/// <summary>
/// HOCON 配置源测试
/// </summary>
public class HclCfgTests : IDisposable
{
    private readonly string _testDir;

    public HclCfgTests()
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
    public void Get_HoconValue_ReturnsCorrectValue()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            app_name = "TestApp"

            database {
                host = "localhost"
                port = 5432
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Apq.Cfg uses colon notation for nested paths
        Assert.Equal("TestApp", cfg["app_name"]);
        Assert.Equal("localhost", cfg["database:host"]);
        Assert.Equal("5432", cfg["database:port"]);
    }

    [Fact]
    public void Get_TypedValue_ReturnsCorrectType()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            settings {
                max_retries = 5
                enabled = true
                timeout = 30.5
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.GetValue<int>("settings:max_retries"));
        Assert.True(cfg.GetValue<bool>("settings:enabled"));
        Assert.Equal(30.5, cfg.GetValue<double>("settings:timeout"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            app {
                original = "Value"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("app:new_key", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2["app:new_key"]);
        Assert.Equal("Value", cfg2["app:original"]);
    }

    [Fact]
    public void Get_NestedHocon_ReturnsCorrectValue()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            level1 {
              level2 {
                level3 {
                  value = "DeepValue"
                }
              }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("DeepValue", cfg["level1:level2:level3:value"]);
    }

    [Fact]
    public void Exists_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            section {
                key = "Value"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Apq.Cfg uses colon notation for nested paths
        Assert.True(cfg.Exists("section:key"));
        Assert.False(cfg.Exists("nonexistent"));
    }

    [Fact]
    public async Task Remove_AndSave_RemovesKey()
    {
        // Arrange
        var hclPath = Path.Combine(_testDir, "config.hcl");
        File.WriteAllText(hclPath, """
            app {
                to_remove = "Value"
                to_keep = "Value2"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Remove("app:to_remove");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddHclFile(hclPath, level: 0, writeable: false)
            .Build();

        var removedValue = cfg2["app:to_remove"];
        Assert.True(string.IsNullOrEmpty(removedValue));
        Assert.Equal("Value2", cfg2["app:to_keep"]);
    }
}
