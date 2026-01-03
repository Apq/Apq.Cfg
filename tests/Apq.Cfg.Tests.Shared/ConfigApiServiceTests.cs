namespace Apq.Cfg.Tests;

using Apq.Cfg.WebApi;
using Apq.Cfg.WebApi.Models;
using Apq.Cfg.WebApi.Services;
using Microsoft.Extensions.Options;

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

    private IOptions<WebApiOptions> CreateOptions(Action<WebApiOptions>? configure = null)
    {
        var options = new WebApiOptions();
        configure?.Invoke(options);
        return Options.Create(options);
    }

    private ICfgRoot CreateCfgRoot(string json, bool writeable = false)
    {
        var jsonPath = Path.Combine(_testDir, $"config_{Guid.NewGuid():N}.json");
        File.WriteAllText(jsonPath, json);
        return new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: writeable, isPrimaryWriter: writeable)
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
        var service = new ConfigApiService(cfg, CreateOptions());

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
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedValue("AppName");

        // Assert
        Assert.Equal("AppName", result.Key);
        Assert.Equal("TestApp", result.Value);
        Assert.True(result.Exists);
        Assert.False(result.IsMasked);
    }

    [Fact]
    public void GetMergedValue_NonExistentKey_ReturnsNotExists()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"AppName": "TestApp"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedValue("NonExistent");

        // Assert
        Assert.Equal("NonExistent", result.Key);
        Assert.Null(result.Value);
        Assert.False(result.Exists);
    }

    [Fact]
    public void GetMergedValue_SensitiveKey_IsMasked()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Database:Password": "secret123"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedValue("Database:Password");

        // Assert
        Assert.Equal("***", result.Value);
        Assert.True(result.Exists);
        Assert.True(result.IsMasked);
    }

    [Fact]
    public void GetMergedValue_SensitiveKey_NotMaskedWhenDisabled()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Database:Password": "secret123"}""");
        var service = new ConfigApiService(cfg, CreateOptions(o => o.MaskSensitiveValues = false));

        // Act
        var result = service.GetMergedValue("Database:Password");

        // Assert
        Assert.Equal("secret123", result.Value);
        Assert.False(result.IsMasked);
    }

    // ========== GetMergedSection 测试 ==========

    [Fact]
    public void GetMergedSection_ReturnsSection()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""
            {
                "Database": {
                    "Host": "localhost",
                    "Port": "5432"
                },
                "Other": "value"
            }
            """);
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedSection("Database");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("localhost", result["Database:Host"]);
        Assert.Equal("5432", result["Database:Port"]);
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
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedTree();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Children);
        Assert.Equal("App", result.Children[0].Key);
        Assert.Single(result.Children[0].Children);
        Assert.Equal("Name", result.Children[0].Children[0].Key);
        Assert.Equal("TestApp", result.Children[0].Children[0].Value);
        Assert.True(result.Children[0].Children[0].HasValue);
    }

    // ========== GetSources 测试 ==========

    [Fact]
    public void GetSources_ReturnsSourceList()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetSources();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, s => Assert.NotNull(s.Name));
    }

    // ========== SetValue 测试 ==========

    [Fact]
    public void SetValue_WithWriteableSource_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Original": "Value"}""", writeable: true);
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.SetValue("NewKey", "NewValue");

        // Assert
        Assert.True(result);
        Assert.Equal("NewValue", cfg["NewKey"]);
    }

    // ========== BatchUpdate 测试 ==========

    [Fact]
    public void BatchUpdate_UpdatesMultipleValues()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key1": "Value1"}""", writeable: true);
        var service = new ConfigApiService(cfg, CreateOptions());
        var request = new BatchUpdateRequest
        {
            Values = new Dictionary<string, string?>
            {
                ["Key2"] = "Value2",
                ["Key3"] = "Value3"
            }
        };

        // Act
        var result = service.BatchUpdate(request);

        // Assert
        Assert.True(result);
        Assert.Equal("Value2", cfg["Key2"]);
        Assert.Equal("Value3", cfg["Key3"]);
    }

    // ========== DeleteKey 测试 ==========

    [Fact]
    public void DeleteKey_RemovesKey()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"ToDelete": "Value", "ToKeep": "Value2"}""", writeable: true);
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.DeleteKey("ToDelete");

        // Assert
        Assert.True(result);
    }

    // ========== Reload 测试 ==========

    [Fact]
    public void Reload_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Key": "Value"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

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
        using var cfg = CreateCfgRoot("""
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);
        var service = new ConfigApiService(cfg, CreateOptions());

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
        using var cfg = CreateCfgRoot("""{"App:Name": "TestApp"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.Export("env");

        // Assert
        Assert.Contains("APP__NAME=TestApp", result);
    }

    [Fact]
    public void Export_KeyValue_ReturnsKeyValueFormat()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"App:Name": "TestApp"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.Export("kv");

        // Assert
        Assert.Contains("App:Name=TestApp", result);
    }

    // ========== 敏感值脱敏测试 ==========

    [Theory]
    [InlineData("Password", true)]
    [InlineData("DbPassword", true)]
    [InlineData("Secret", true)]
    [InlineData("ApiSecret", true)]
    [InlineData("ApiKey", true)]
    [InlineData("AccessToken", true)]
    [InlineData("ConnectionString", true)]
    [InlineData("DbConnectionString", true)]
    [InlineData("AppName", false)]
    [InlineData("Version", false)]
    [InlineData("Host", false)]
    public void SensitiveKeyDetection_WorksCorrectly(string key, bool shouldBeMasked)
    {
        // Arrange
        using var cfg = CreateCfgRoot($"""{"{"}"{key}": "value"{"}"}""");
        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedValue(key);

        // Assert
        Assert.Equal(shouldBeMasked, result.IsMasked);
        if (shouldBeMasked)
        {
            Assert.Equal("***", result.Value);
        }
        else
        {
            Assert.Equal("value", result.Value);
        }
    }

    // ========== 自定义敏感键模式测试 ==========

    [Fact]
    public void CustomSensitivePatterns_AreRespected()
    {
        // Arrange
        using var cfg = CreateCfgRoot("""{"Credential": "secret", "Password": "pass"}""");
        var service = new ConfigApiService(cfg, CreateOptions(o =>
        {
            o.SensitiveKeyPatterns = ["*Credential*"]; // 只匹配 Credential
        }));

        // Act
        var credentialResult = service.GetMergedValue("Credential");
        var passwordResult = service.GetMergedValue("Password");

        // Assert
        Assert.True(credentialResult.IsMasked);
        Assert.Equal("***", credentialResult.Value);
        Assert.False(passwordResult.IsMasked); // Password 不再被脱敏
        Assert.Equal("pass", passwordResult.Value);
    }

    // ========== 多层级配置测试 ==========

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
            .AddJson(basePath, level: 0, writeable: false)
            .AddJson(overridePath, level: 1, writeable: false)
            .Build();

        var service = new ConfigApiService(cfg, CreateOptions());

        // Act
        var result = service.GetMergedConfig();

        // Assert
        Assert.Equal("OverrideValue1", result["Setting1"]); // 被覆盖
        Assert.Equal("BaseValue2", result["Setting2"]); // 保持原值
    }
}
