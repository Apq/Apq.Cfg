namespace Apq.Cfg.Tests;

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
    public void TryGet_ExistingKey_ReturnsTrueAndValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"IntValue": 42, "StringValue": "Hello"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.TryGet<int>("IntValue", out var intValue));
        Assert.Equal(42, intValue);

        Assert.True(cfg.TryGet<string>("StringValue", out var stringValue));
        Assert.Equal("Hello", stringValue);
    }

    [Fact]
    public void TryGet_NonExistingKey_ReturnsFalseAndDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.False(cfg.TryGet<int>("NonExistent", out var intValue));
        Assert.Equal(default, intValue);

        Assert.False(cfg.TryGet<string>("NonExistent", out var stringValue));
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
}
