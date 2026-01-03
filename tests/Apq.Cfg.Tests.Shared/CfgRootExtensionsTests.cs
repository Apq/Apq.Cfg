namespace Apq.Cfg.Tests;

using Apq.Cfg.Crypto;

/// <summary>
/// CfgRootExtensions 扩展方法测试
/// </summary>
public class CfgRootExtensionsTests : IDisposable
{
    private readonly string _testDir;

    public CfgRootExtensionsTests()
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
    public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"IntValue": 42, "StringValue": "Hello"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.TryGetValue<int>("IntValue", out var intValue));
        Assert.Equal(42, intValue);

        Assert.True(cfg.TryGetValue<string>("StringValue", out var stringValue));
        Assert.Equal("Hello", stringValue);
    }

    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalseAndDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.False(cfg.TryGetValue<int>("NonExistent", out var intValue));
        Assert.Equal(default, intValue);

        Assert.False(cfg.TryGetValue<string>("NonExistent", out var stringValue));
        Assert.Null(stringValue);
    }

    [Fact]
    public void GetRequired_ExistingKey_ReturnsValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"RequiredInt": 100, "RequiredString": "Required"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(100, cfg.GetRequired<int>("RequiredInt"));
        Assert.Equal("Required", cfg.GetRequired<string>("RequiredString"));
    }

    [Fact]
    public void GetRequired_NonExistingKey_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.GetRequired<string>("NonExistent"));
        Assert.Contains("NonExistent", ex.Message);
    }

    #region GetMasked 测试

    [Fact]
    public void GetMasked_WithoutMasker_ReturnsOriginalValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Password": "MySecretPassword123"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act - 没有配置脱敏器时，返回原始值
        var maskedValue = cfg.GetMasked("Password");

        // Assert
        Assert.Equal("MySecretPassword123", maskedValue);
    }

    [Fact]
    public void GetMasked_WithMasker_ReturnsMaskedValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Database:Password": "MySecretPassword123"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // Act
        var maskedValue = cfg.GetMasked("Database:Password");

        // Assert - 敏感键应该被脱敏
        Assert.NotEqual("MySecretPassword123", maskedValue);
        Assert.Contains("***", maskedValue);
    }

    [Fact]
    public void GetMasked_NonSensitiveKey_ReturnsOriginalValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"AppName": "TestApp"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // Act
        var maskedValue = cfg.GetMasked("AppName");

        // Assert - 非敏感键返回原始值
        Assert.Equal("TestApp", maskedValue);
    }

    [Fact]
    public void GetMasked_NonExistingKey_ReturnsNullPlaceholder()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var maskedValue = cfg.GetMasked("NonExistent");

        // Assert
        Assert.Equal("[null]", maskedValue);
    }

    #endregion

    #region GetMaskedSnapshot 测试

    [Fact]
    public void GetMaskedSnapshot_WithoutMasker_ReturnsAllOriginalValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"AppName": "TestApp", "Version": "1.0.0"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var snapshot = cfg.GetMaskedSnapshot();

        // Assert
        Assert.Equal(2, snapshot.Count);
        Assert.Equal("TestApp", snapshot["AppName"]);
        Assert.Equal("1.0.0", snapshot["Version"]);
    }

    [Fact]
    public void GetMaskedSnapshot_WithMasker_ReturnsMaskedSensitiveValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "AppName": "TestApp",
                "Database": {
                    "Password": "MySecretPassword123"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // Act
        var snapshot = cfg.GetMaskedSnapshot();

        // Assert - 扁平化的键，敏感键被脱敏
        Assert.True(snapshot.ContainsKey("AppName"));
        Assert.Equal("TestApp", snapshot["AppName"]);
        Assert.True(snapshot.ContainsKey("Database:Password"));
        // 敏感键应该被脱敏，不应该包含原始密码
        Assert.DoesNotContain("MySecretPassword123", snapshot["Database:Password"]);
    }

    [Fact]
    public void GetMaskedSnapshot_EmptyConfig_ReturnsEmptyDictionary()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var snapshot = cfg.GetMaskedSnapshot();

        // Assert
        Assert.Empty(snapshot);
    }

    #endregion
}
