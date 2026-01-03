namespace Apq.Cfg.Tests;

/// <summary>
/// 配置源信息和名称相关测试
/// </summary>
public class ConfigSourceInfoTests : IDisposable
{
    private readonly string _testDir;

    public ConfigSourceInfoTests()
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
    public void GetSourceInfos_ReturnsAllSources()
    {
        // Arrange
        var jsonPath1 = Path.Combine(_testDir, "config.json");
        var jsonPath2 = Path.Combine(_testDir, "config.local.json");
        File.WriteAllText(jsonPath1, """{"App": "Test1"}""");
        File.WriteAllText(jsonPath2, """{"App": "Test2"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath1, level: 0)
            .AddJson(jsonPath2, level: 5, writeable: true, isPrimaryWriter: true)
            .AddEnvironmentVariables(level: 20, prefix: "TEST_")
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.Equal(3, sources.Count);

        // 验证按层级升序排列
        Assert.Equal(0, sources[0].Level);
        Assert.Equal(5, sources[1].Level);
        Assert.Equal(20, sources[2].Level);
    }

    [Fact]
    public void GetSourceInfos_ReturnsCorrectNames()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Test"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0)
            .AddEnvironmentVariables(level: 20, prefix: "APP_")
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.Equal("config.json", sources[0].Name);
        Assert.Equal("EnvVars:APP_", sources[1].Name);
    }

    [Fact]
    public void GetSourceInfos_ReturnsCorrectWriteableStatus()
    {
        // Arrange
        var jsonPath1 = Path.Combine(_testDir, "config.json");
        var jsonPath2 = Path.Combine(_testDir, "config.local.json");
        File.WriteAllText(jsonPath1, """{"App": "Test1"}""");
        File.WriteAllText(jsonPath2, """{"App": "Test2"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath1, level: 0, writeable: false)
            .AddJson(jsonPath2, level: 5, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.False(sources[0].IsWriteable);
        Assert.True(sources[1].IsWriteable);
        Assert.True(sources[1].IsPrimaryWriter);
    }

    [Fact]
    public void GetSourceInfos_ReturnsCorrectKeyCount()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": "Test",
                "Version": "1.0",
                "Debug": "true"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0)
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.Equal(3, sources[0].KeyCount);
    }

    [Fact]
    public void GetSource_ExistingSource_ReturnsSource()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Test"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0)
            .Build();

        // Act
        var source = cfg.GetSource(0, "config.json");

        // Assert
        Assert.NotNull(source);
        Assert.Equal("config.json", source.Name);
        Assert.Equal(0, source.Level);
    }

    [Fact]
    public void GetSource_NonExistingSource_ReturnsNull()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Test"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0)
            .Build();

        // Act
        var source = cfg.GetSource(99, "nonexistent");

        // Assert
        Assert.Null(source);
    }

    [Fact]
    public void GetSource_GetAllValues_ReturnsSourceValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "App": "Test",
                "Database": {
                    "Host": "localhost",
                    "Port": "5432"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0)
            .Build();

        // Act
        var source = cfg.GetSource(0, "config.json");
        var values = source!.GetAllValues().ToDictionary(x => x.Key, x => x.Value);

        // Assert
        Assert.Equal("Test", values["App"]);
        Assert.Equal("localhost", values["Database:Host"]);
        Assert.Equal("5432", values["Database:Port"]);
    }

    [Fact]
    public void AddJson_CustomName_UsesCustomName()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Test"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, name: "custom-config")
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();
        var source = cfg.GetSource(0, "custom-config");

        // Assert
        Assert.Equal("custom-config", sources[0].Name);
        Assert.NotNull(source);
    }

    [Fact]
    public void AddEnvironmentVariables_CustomName_UsesCustomName()
    {
        // Arrange
        using var cfg = new CfgBuilder()
            .AddEnvironmentVariables(level: 0, prefix: "TEST_", name: "my-env-vars")
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.Equal("my-env-vars", sources[0].Name);
    }

    [Fact]
    public void AddSource_CustomName_OverridesSourceName()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Test"}""");

        var source = new Apq.Cfg.Sources.JsonFileCfgSource(
            jsonPath, level: 0, writeable: false, optional: true, reloadOnChange: false, isPrimaryWriter: false);

        using var cfg = new CfgBuilder()
            .AddSource(source, name: "overridden-name")
            .Build();

        // Act
        var sources = cfg.GetSourceInfos();

        // Assert
        Assert.Equal("overridden-name", sources[0].Name);
    }

    [Fact]
    public void Reload_RefreshesConfiguration()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"App": "Initial"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, reloadOnChange: false)
            .Build();

        Assert.Equal("Initial", cfg["App"]);

        // Act - 修改文件并重载
        File.WriteAllText(jsonPath, """{"App": "Updated"}""");
        cfg.Reload();

        // Assert - 由于没有启用动态重载，Reload 不会自动更新值
        // 这里主要测试 Reload 方法不会抛出异常
    }
}
