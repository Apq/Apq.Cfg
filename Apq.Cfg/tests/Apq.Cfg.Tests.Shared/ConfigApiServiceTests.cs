namespace Apq.Cfg.Tests;

using Apq.Cfg.WebApi.Models;
using Apq.Cfg.WebApi.Services;

/// <summary>
/// ConfigApiService 服务测试
/// </summary>
public class ConfigApiServiceTests : IDisposable
{
    private readonly string _testDir;

    public ConfigApiServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgWebApiTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    private ICfgRoot CreateCfgRoot(string json, bool writeable = false)
    {
        var jsonPath = Path.Combine(_testDir, $"config_{Guid.NewGuid():N}.json");
        File.WriteAllText(jsonPath, json);
        return new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: writeable, isPrimaryWriter: writeable)
            .Build();
    }

    // ========== GetMergedConfig 测试 ==========

    [Fact]
    public void GetMergedConfig_ReturnsAllValues()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""
            {
                "App": {
                    "Name": "TestApp",
                    "Version": "1.0.0"
                },
                "Database": {
                    "Host": "localhost"
                }
            }
            """);
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedConfig();

        // Assert
        Assert.Equal("TestApp", result["App:Name"]);
        Assert.Equal("1.0.0", result["App:Version"]);
        Assert.Equal("localhost", result["Database:Host"]);
    }

    // ========== GetMergedValue 测试 ==========

    [Fact]
    public void GetMergedValue_ExistingKey_ReturnsValue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"AppName": "TestApp"}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedValue("AppName");

        // Assert
        Assert.True(result.Exists);
        Assert.Equal("TestApp", result.Value);
    }

    [Fact]
    public void GetMergedValue_NonExistingKey_ReturnsNotExists()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"AppName": "TestApp"}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedValue("NonExisting");

        // Assert
        Assert.False(result.Exists);
        Assert.Null(result.Value);
    }

    [Fact]
    public void GetMergedValue_NestedKey_ReturnsValue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"App": {"Name": "TestApp"}}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedValue("App:Name");

        // Assert
        Assert.True(result.Exists);
        Assert.Equal("TestApp", result.Value);
    }

    // ========== GetMergedSection 测试 ==========

    [Fact]
    public void GetMergedSection_ReturnsOnlySectionValues()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""
            {
                "App": {
                    "Name": "TestApp",
                    "Version": "1.0.0"
                },
                "Database": {
                    "Host": "localhost"
                }
            }
            """);
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedSection("App");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("TestApp", result["App:Name"]);
        Assert.Equal("1.0.0", result["App:Version"]);
        Assert.False(result.ContainsKey("Database:Host"));
    }

    // ========== GetMergedTree 测试 ==========

    [Fact]
    public void GetMergedTree_ReturnsTreeStructure()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetMergedTree();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Children);
        Assert.Equal("App", result.Children[0].Key);
        Assert.Single(result.Children[0].Children);
        Assert.Equal("Name", result.Children[0].Children[0].Key);
        Assert.Equal("TestApp", result.Children[0].Children[0].Value);
    }

    // ========== GetSources 测试 ==========

    [Fact]
    public void GetSources_ReturnsSourceList()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetSources();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(0, result[0].Level);
    }

    // ========== GetSourceConfig 测试 ==========

    [Fact]
    public void GetSourceConfig_ValidSource_ReturnsConfig()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg);
        var sources = service.GetSources();
        var firstSource = sources[0];

        // Act
        var result = service.GetSourceConfig(firstSource.Level, firstSource.Name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Value", result["Key"]);
    }

    [Fact]
    public void GetSourceConfig_InvalidSource_ReturnsNull()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.GetSourceConfig(999, "NonExisting");

        // Assert
        Assert.Null(result);
    }

    // ========== SetValue 测试 ==========

    [Fact]
    public void SetValue_WriteableSource_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "OldValue"}""", writeable: true);
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.SetValue("Key", "NewValue");

        // Assert
        Assert.True(result);
        Assert.Equal("NewValue", cfg["Key"]);
    }

    // ========== BatchUpdate 测试 ==========

    [Fact]
    public void BatchUpdate_WriteableSource_UpdatesMultipleValues()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key1": "Value1", "Key2": "Value2"}""", writeable: true);
        var service = new ConfigApiService(cfg);
        var request = new BatchUpdateRequest
        {
            Values = new Dictionary<string, string?>
            {
                ["Key1"] = "NewValue1",
                ["Key2"] = "NewValue2"
            }
        };

        // Act
        var result = service.BatchUpdate(request);

        // Assert
        Assert.True(result);
        Assert.Equal("NewValue1", cfg["Key1"]);
        Assert.Equal("NewValue2", cfg["Key2"]);
    }

    // ========== DeleteKey 测试 ==========

    [Fact]
    public void DeleteKey_WriteableSource_RemovesKey()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value", "Other": "Keep"}""", writeable: true);
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.DeleteKey("Key");

        // Assert
        Assert.True(result);
        Assert.Null(cfg["Key"]);
        Assert.Equal("Keep", cfg["Other"]);
    }

    // ========== Reload 测试 ==========

    [Fact]
    public void Reload_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.Reload();

        // Assert
        Assert.True(result);
    }

    // ========== Export 测试 ==========

    [Fact]
    public void Export_Json_ReturnsJsonFormat()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"App": {"Name": "TestApp"}}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.Export("json");

        // Assert
        Assert.Contains("App", result);
        Assert.Contains("Name", result);
        Assert.Contains("TestApp", result);
    }

    [Fact]
    public void Export_Env_ReturnsEnvFormat()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"App": {"Name": "TestApp"}}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.Export("env");

        // Assert
        Assert.Contains("APP__NAME=TestApp", result);
    }

    [Fact]
    public void Export_KeyValue_ReturnsKeyValueFormat()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"App": {"Name": "TestApp"}}""");
        var service = new ConfigApiService(cfg);

        // Act
        var result = service.Export("keyvalue");

        // Assert
        Assert.Contains("App:Name=TestApp", result);
    }

    // ========== SaveAsync 测试 ==========

    [Fact]
    public async Task SaveAsync_WriteableSource_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""", writeable: true);
        var service = new ConfigApiService(cfg);

        // Act
        var result = await service.SaveAsync();

        // Assert
        Assert.True(result);
    }
}
