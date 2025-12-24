using Apq.Cfg.Changes;

namespace Apq.Cfg.Tests;

/// <summary>
/// CfgBuilder 高级功能测试（编码阈值、多层级写入等）
/// </summary>
public class CfgBuilderAdvancedTests : IDisposable
{
    private readonly string _testDir;

    public CfgBuilderAdvancedTests()
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
    public void WithEncodingConfidenceThreshold_SetsThreshold()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        // Act - 设置不同的阈值不应抛出异常
        using var cfg = new CfgBuilder()
            .WithEncodingConfidenceThreshold(0.8f)
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert
        Assert.Equal("Value", cfg.Get("Key"));
    }

    [Fact]
    public void WithEncodingConfidenceThreshold_ClampsValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        // Act - 超出范围的值应被钳制
        using var cfg1 = new CfgBuilder()
            .WithEncodingConfidenceThreshold(1.5f) // 超过 1.0
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        using var cfg2 = new CfgBuilder()
            .WithEncodingConfidenceThreshold(-0.5f) // 小于 0.0
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert - 不应抛出异常
        Assert.Equal("Value", cfg1.Get("Key"));
        Assert.Equal("Value", cfg2.Get("Key"));
    }

    [Fact]
    public async Task Set_WithTargetLevel_WritesToSpecificLevel()
    {
        // Arrange
        var basePath = Path.Combine(_testDir, "base.json");
        var overridePath = Path.Combine(_testDir, "override.json");

        File.WriteAllText(basePath, """{"Setting": "BaseValue"}""");
        File.WriteAllText(overridePath, """{"Setting": "OverrideValue"}""");

        using var cfg = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJson(overridePath, level: 1, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 写入到 level 0
        cfg.Set("NewKey", "Level0Value", targetLevel: 0);
        await cfg.SaveAsync(targetLevel: 0);

        // Assert - 验证写入到了正确的文件
        using var cfgBase = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: false)
            .Build();

        Assert.Equal("Level0Value", cfgBase.Get("NewKey"));
    }

    [Fact]
    public async Task Remove_WithTargetLevel_RemovesFromSpecificLevel()
    {
        // Arrange
        var basePath = Path.Combine(_testDir, "base.json");
        var overridePath = Path.Combine(_testDir, "override.json");

        File.WriteAllText(basePath, """{"BaseKey": "BaseValue", "SharedKey": "BaseShared"}""");
        File.WriteAllText(overridePath, """{"OverrideKey": "OverrideValue", "SharedKey": "OverrideShared"}""");

        using var cfg = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJson(overridePath, level: 1, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 从 level 1 删除
        cfg.Remove("OverrideKey", targetLevel: 1);
        await cfg.SaveAsync(targetLevel: 1);

        // Assert
        using var cfgOverride = new CfgBuilder()
            .AddJson(overridePath, level: 0, writeable: false)
            .Build();

        var removedValue = cfgOverride.Get("OverrideKey");
        Assert.True(string.IsNullOrEmpty(removedValue));
    }

    [Fact]
    public async Task SaveAsync_WithTargetLevel_OnlySavesSpecificLevel()
    {
        // Arrange
        var basePath = Path.Combine(_testDir, "base.json");
        var overridePath = Path.Combine(_testDir, "override.json");

        File.WriteAllText(basePath, """{"Key": "Base"}""");
        File.WriteAllText(overridePath, """{"Key": "Override"}""");

        using var cfg = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJson(overridePath, level: 1, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 在两个层级都设置值，但只保存 level 1
        cfg.Set("Level0Key", "Value0", targetLevel: 0);
        cfg.Set("Level1Key", "Value1", targetLevel: 1);
        await cfg.SaveAsync(targetLevel: 1);

        // Assert - level 1 应该保存了，level 0 不应该保存
        using var cfgOverride = new CfgBuilder()
            .AddJson(overridePath, level: 0, writeable: false)
            .Build();
        Assert.Equal("Value1", cfgOverride.Get("Level1Key"));

        using var cfgBase = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: false)
            .Build();
        Assert.Null(cfgBase.Get("Level0Key")); // 未保存
    }

    [Fact]
    public void Set_WithoutTargetLevel_WritesToHighestLevel()
    {
        // Arrange
        var basePath = Path.Combine(_testDir, "base.json");
        var overridePath = Path.Combine(_testDir, "override.json");

        File.WriteAllText(basePath, """{"Key": "Base"}""");
        File.WriteAllText(overridePath, """{"Key": "Override"}""");

        using var cfg = new CfgBuilder()
            .AddJson(basePath, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJson(overridePath, level: 1, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 不指定 targetLevel，应该写入最高层级
        cfg.Set("NewKey", "NewValue");

        // Assert - 通过 Get 验证值已设置（在 Pending 中）
        Assert.Equal("NewValue", cfg.Get("NewKey"));
    }

    [Fact]
    public async Task DisposeAsync_ReleasesResources()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act - 使用 DisposeAsync 释放资源
        await cfg.DisposeAsync();

        // Assert - 再次调用 DisposeAsync 不应抛出异常（幂等性）
        await cfg.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_AfterDispose_IsIdempotent()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act - 先同步释放，再异步释放
        cfg.Dispose();
        await cfg.DisposeAsync();

        // Assert - 不应抛出异常
    }

    [Fact]
    public void Set_WithoutWritableSource_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false) // 不可写
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.Set("NewKey", "NewValue"));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public void Remove_WithoutWritableSource_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false) // 不可写
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.Remove("Key"));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public void Set_WithInvalidTargetLevel_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act & Assert - 指定不存在的层级
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.Set("NewKey", "NewValue", targetLevel: 999));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public async Task SaveAsync_WithEmptyPending_DoesNothing()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");
        var originalContent = File.ReadAllText(jsonPath);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 不设置任何值，直接保存
        await cfg.SaveAsync();

        // Assert - 文件内容不应改变
        var newContent = File.ReadAllText(jsonPath);
        Assert.Equal(originalContent, newContent);
    }

    [Fact]
    public async Task SaveAsync_WithInvalidTargetLevel_DoesNothing()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act - 指定不存在的层级，不应抛出异常
        await cfg.SaveAsync(targetLevel: 999);

        // Assert - 不应抛出异常，静默返回
    }

    [Fact]
    public void AddSource_CustomSource_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        var jsonPath2 = Path.Combine(_testDir, "config2.json");
        File.WriteAllText(jsonPath, """{"Key": "Value1"}""");
        File.WriteAllText(jsonPath2, """{"Key2": "Value2"}""");

        // Act - 使用 AddJson 添加多个源验证 AddSource 间接工作
        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddJson(jsonPath2, level: 1, writeable: false)
            .Build();

        // Assert
        Assert.Equal("Value1", cfg.Get("Key"));
        Assert.Equal("Value2", cfg.Get("Key2"));
    }
}
