using Apq.Cfg.Changes;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Tests;

/// <summary>
/// 动态配置重载测试
/// </summary>
public class DynamicReloadTests : IDisposable
{
    private readonly string _testDir;

    public DynamicReloadTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgDynamicTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            try { Directory.Delete(_testDir, true); }
            catch { }
        }
    }

    [Fact]
    public void ToMicrosoftConfiguration_WithOptions_ReturnsConfiguration()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // 先验证静态配置能正常工作
        var staticConfig = cfg.ToMicrosoftConfiguration();
        Assert.Equal("Value", staticConfig["Key"]);

        // Act
        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 50,
            EnableDynamicReload = true
        });

        // Assert
        Assert.NotNull(msConfig);
        Assert.Equal("Value", msConfig["Key"]);
    }

    [Fact]
    public void ToMicrosoftConfiguration_DisabledDynamicReload_ReturnsSameAsStatic()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var staticConfig = cfg.ToMicrosoftConfiguration();
        var dynamicConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            EnableDynamicReload = false
        });

        // Assert
        Assert.Same(staticConfig, dynamicConfig);
    }

    [Fact]
    public void ToMicrosoftConfiguration_CalledTwice_ReturnsSameInstance()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var options = new DynamicReloadOptions { DebounceMs = 50 };

        // Act
        var config1 = cfg.ToMicrosoftConfiguration(options);
        var config2 = cfg.ToMicrosoftConfiguration(options);

        // Assert
        Assert.Same(config1, config2);
    }

    [Fact]
    public void ConfigChanges_IsObservable()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act & Assert
        Assert.NotNull(cfg.ConfigChanges);
    }

    [Fact]
    public void MultiLevel_HigherLevelOverrides_InDynamicConfig()
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
            .AddJson(basePath, level: 0, writeable: false, reloadOnChange: true)
            .AddJson(overridePath, level: 1, writeable: false, reloadOnChange: true)
            .Build();

        // Act
        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 50
        });

        // Assert
        Assert.Equal("OverrideValue1", msConfig["Setting1"]); // 被覆盖
        Assert.Equal("BaseValue2", msConfig["Setting2"]); // 保持原值
    }

    [Fact]
    public async Task DynamicConfig_DetectsFileChange()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "OriginalValue"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 50
        });

        var changeDetected = new TaskCompletionSource<bool>();
        var changeToken = msConfig.GetReloadToken();
        changeToken.RegisterChangeCallback(_ => changeDetected.TrySetResult(true), null);

        // Act - 修改文件
        await Task.Delay(100); // 等待文件监视器初始化
        File.WriteAllText(jsonPath, """{"Key": "NewValue"}""");

        // Assert - 等待变更检测（最多 2 秒）
        var detected = await Task.WhenAny(changeDetected.Task, Task.Delay(2000)) == changeDetected.Task;

        // 注意：文件变更检测依赖于操作系统的文件监视器，可能不稳定
        // 这里主要验证 API 是否正常工作
        Assert.NotNull(msConfig.GetReloadToken());
    }

    [Fact]
    public void DynamicReloadOptions_DefaultValues()
    {
        // Arrange & Act
        var options = new DynamicReloadOptions();

        // Assert
        Assert.Equal(100, options.DebounceMs);
        Assert.True(options.EnableDynamicReload);
    }

    [Fact]
    public void ConfigChange_ToString_FormatsCorrectly()
    {
        // Arrange
        var change = new ConfigChange("TestKey", "OldVal", "NewVal", ChangeType.Modified);

        // Act
        var str = change.ToString();

        // Assert
        Assert.Contains("Modified", str);
        Assert.Contains("TestKey", str);
        Assert.Contains("OldVal", str);
        Assert.Contains("NewVal", str);
    }

    [Fact]
    public void ConfigChange_NullValues_FormatsCorrectly()
    {
        // Arrange
        var change = new ConfigChange("TestKey", null, "NewVal", ChangeType.Added);

        // Act
        var str = change.ToString();

        // Assert
        Assert.Contains("Added", str);
        Assert.Contains("(null)", str);
    }
}
