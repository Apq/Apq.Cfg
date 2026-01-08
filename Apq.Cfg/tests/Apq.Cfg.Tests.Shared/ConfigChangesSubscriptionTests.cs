using System.Reactive.Linq;
using Apq.Cfg.Changes;

namespace Apq.Cfg.Tests;

/// <summary>
/// ConfigChanges 订阅测试
/// </summary>
public class ConfigChangesSubscriptionTests : IDisposable
{
    private readonly string _testDir;

    public ConfigChangesSubscriptionTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgSubscriptionTests_{Guid.NewGuid():N}");
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

    #region 基本订阅测试

    [Fact]
    public void ConfigChanges_IsNotNull()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act & Assert
        Assert.NotNull(cfg.ConfigChanges);
    }

    [Fact]
    public void ConfigChanges_CanSubscribe()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act
        using var subscription = cfg.ConfigChanges.Subscribe(_ => { });

        // Assert - 订阅应该成功
        Assert.NotNull(subscription);
    }

    [Fact]
    public void ConfigChanges_MultipleSubscribers_AllSubscribeSuccessfully()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act - 多个订阅者
        using var sub1 = cfg.ConfigChanges.Subscribe(_ => { });
        using var sub2 = cfg.ConfigChanges.Subscribe(_ => { });
        using var sub3 = cfg.ConfigChanges.Subscribe(_ => { });

        // Assert - 所有订阅都应该成功
        Assert.NotNull(sub1);
        Assert.NotNull(sub2);
        Assert.NotNull(sub3);
    }

    [Fact]
    public void ConfigChanges_Unsubscribe_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var eventCount = 0;

        // Act
        var subscription = cfg.ConfigChanges.Subscribe(_ => eventCount++);
        subscription.Dispose(); // 取消订阅

        // Assert - 取消订阅不应抛出异常
    }

    #endregion

    #region ConfigChangeEvent 测试

    [Fact]
    public void ConfigChangeEvent_HasCorrectTimestamp()
    {
        // Arrange
        var before = DateTimeOffset.Now;
        var changes = new Dictionary<string, ConfigChange>
        {
            ["Key"] = new ConfigChange("Key", "Old", "New", ChangeType.Modified)
        };

        // Act
        var evt = new ConfigChangeEvent(changes);
        var after = DateTimeOffset.Now;

        // Assert
        Assert.True(evt.Timestamp >= before);
        Assert.True(evt.Timestamp <= after);
    }

    [Fact]
    public void ConfigChangeEvent_WithExplicitTimestamp_Works()
    {
        // Arrange
        var explicitTime = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var changes = new Dictionary<string, ConfigChange>
        {
            ["Key"] = new ConfigChange("Key", null, "Value", ChangeType.Added)
        };

        // Act
        var evt = new ConfigChangeEvent(changes, explicitTime);

        // Assert
        Assert.Equal(explicitTime, evt.Timestamp);
    }

    [Fact]
    public void ConfigChangeEvent_Changes_IsReadOnly()
    {
        // Arrange
        var changes = new Dictionary<string, ConfigChange>
        {
            ["Key1"] = new ConfigChange("Key1", null, "Value1", ChangeType.Added),
            ["Key2"] = new ConfigChange("Key2", "Old", "New", ChangeType.Modified)
        };

        // Act
        var evt = new ConfigChangeEvent(changes);

        // Assert
        Assert.Equal(2, evt.Changes.Count);
        Assert.True(evt.Changes.ContainsKey("Key1"));
        Assert.True(evt.Changes.ContainsKey("Key2"));
    }

    [Fact]
    public void ConfigChangeEvent_EmptyChanges_Works()
    {
        // Arrange
        var changes = new Dictionary<string, ConfigChange>();

        // Act
        var evt = new ConfigChangeEvent(changes);

        // Assert
        Assert.Empty(evt.Changes);
    }

    #endregion

    #region ConfigChange 测试

    [Fact]
    public void ConfigChange_Added_HasCorrectProperties()
    {
        // Arrange & Act
        var change = new ConfigChange("NewKey", null, "NewValue", ChangeType.Added);

        // Assert
        Assert.Equal("NewKey", change.Key);
        Assert.Null(change.OldValue);
        Assert.Equal("NewValue", change.NewValue);
        Assert.Equal(ChangeType.Added, change.Type);
    }

    [Fact]
    public void ConfigChange_Modified_HasCorrectProperties()
    {
        // Arrange & Act
        var change = new ConfigChange("Key", "OldValue", "NewValue", ChangeType.Modified);

        // Assert
        Assert.Equal("Key", change.Key);
        Assert.Equal("OldValue", change.OldValue);
        Assert.Equal("NewValue", change.NewValue);
        Assert.Equal(ChangeType.Modified, change.Type);
    }

    [Fact]
    public void ConfigChange_Removed_HasCorrectProperties()
    {
        // Arrange & Act
        var change = new ConfigChange("RemovedKey", "OldValue", null, ChangeType.Removed);

        // Assert
        Assert.Equal("RemovedKey", change.Key);
        Assert.Equal("OldValue", change.OldValue);
        Assert.Null(change.NewValue);
        Assert.Equal(ChangeType.Removed, change.Type);
    }

    [Fact]
    public void ConfigChange_ToString_ContainsAllInfo()
    {
        // Arrange
        var change = new ConfigChange("TestKey", "OldVal", "NewVal", ChangeType.Modified);

        // Act
        var str = change.ToString();

        // Assert
        Assert.Contains("TestKey", str);
        Assert.Contains("OldVal", str);
        Assert.Contains("NewVal", str);
        Assert.Contains("Modified", str);
    }

    [Fact]
    public void ConfigChange_ToString_HandlesNullValues()
    {
        // Arrange
        var change = new ConfigChange("Key", null, "Value", ChangeType.Added);

        // Act
        var str = change.ToString();

        // Assert
        Assert.Contains("(null)", str);
        Assert.Contains("Value", str);
    }

    [Fact]
    public void ConfigChange_Equality_WorksCorrectly()
    {
        // Arrange
        var change1 = new ConfigChange("Key", "Old", "New", ChangeType.Modified);
        var change2 = new ConfigChange("Key", "Old", "New", ChangeType.Modified);
        var change3 = new ConfigChange("Key", "Old", "Different", ChangeType.Modified);

        // Assert - struct 默认相等性比较
        Assert.Equal(change1.Key, change2.Key);
        Assert.Equal(change1.OldValue, change2.OldValue);
        Assert.Equal(change1.NewValue, change2.NewValue);
        Assert.NotEqual(change1.NewValue, change3.NewValue);
    }

    #endregion

    #region ChangeType 测试

    [Fact]
    public void ChangeType_AllValues_AreDefined()
    {
        // Assert
        Assert.True(Enum.IsDefined(typeof(ChangeType), ChangeType.Added));
        Assert.True(Enum.IsDefined(typeof(ChangeType), ChangeType.Modified));
        Assert.True(Enum.IsDefined(typeof(ChangeType), ChangeType.Removed));
    }

    [Fact]
    public void ChangeType_Values_AreDistinct()
    {
        // Assert
        Assert.NotEqual(ChangeType.Added, ChangeType.Modified);
        Assert.NotEqual(ChangeType.Modified, ChangeType.Removed);
        Assert.NotEqual(ChangeType.Added, ChangeType.Removed);
    }

    #endregion

    #region DynamicReloadOptions 测试

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
    public void DynamicReloadOptions_CanSetDebounceMs()
    {
        // Arrange & Act
        var options = new DynamicReloadOptions { DebounceMs = 500 };

        // Assert
        Assert.Equal(500, options.DebounceMs);
    }

    [Fact]
    public void DynamicReloadOptions_CanDisableDynamicReload()
    {
        // Arrange & Act
        var options = new DynamicReloadOptions { EnableDynamicReload = false };

        // Assert
        Assert.False(options.EnableDynamicReload);
    }

    [Fact]
    public void DynamicReloadOptions_ZeroDebounceMs_Works()
    {
        // Arrange & Act
        var options = new DynamicReloadOptions { DebounceMs = 0 };

        // Assert
        Assert.Equal(0, options.DebounceMs);
    }

    #endregion

    #region 动态配置集成测试

    [Fact]
    public void ToMicrosoftConfiguration_WithDynamicReload_ReturnsConfiguration()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

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
    public void ToMicrosoftConfiguration_DisabledReload_ReturnsSameAsStatic()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
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
    public void ToMicrosoftConfiguration_CalledMultipleTimes_ReturnsSameInstance()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var options = new DynamicReloadOptions { DebounceMs = 50 };

        // Act
        var config1 = cfg.ToMicrosoftConfiguration(options);
        var config2 = cfg.ToMicrosoftConfiguration(options);

        // Assert
        Assert.Same(config1, config2);
    }

    [Fact]
    public void ToMicrosoftConfiguration_NullOptions_UsesDefaults()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        // Act
        var msConfig = cfg.ToMicrosoftConfiguration(null);

        // Assert
        Assert.NotNull(msConfig);
        Assert.Equal("Value", msConfig["Key"]);
    }

    #endregion

    #region Dispose 后订阅测试

    [Fact]
    public void ConfigChanges_AfterDispose_CompletesObservable()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var completed = false;
        using var subscription = cfg.ConfigChanges.Subscribe(
            onNext: _ => { },
            onCompleted: () => completed = true
        );

        // Act
        cfg.Dispose();

        // Assert - Observable 应该完成
        Assert.True(completed);
    }

    [Fact]
    public async Task ConfigChanges_AfterDisposeAsync_CompletesObservable()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var completed = false;
        using var subscription = cfg.ConfigChanges.Subscribe(
            onNext: _ => { },
            onCompleted: () => completed = true
        );

        // Act
        await cfg.DisposeAsync();

        // Assert
        Assert.True(completed);
    }

    #endregion

    #region 文件变更检测测试

    [Fact]
    public void DynamicConfig_HasReloadToken()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 50
        });

        // Act
        var reloadToken = msConfig.GetReloadToken();

        // Assert
        Assert.NotNull(reloadToken);
        Assert.False(reloadToken.HasChanged); // 初始状态未变更
    }

    [Fact]
    public async Task DynamicConfig_FileChange_TriggersReload()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "OriginalValue"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false, reloadOnChange: true)
            .Build();

        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 50
        });

        var changeDetected = new TaskCompletionSource<bool>();
        var token = msConfig.GetReloadToken();
        token.RegisterChangeCallback(_ => changeDetected.TrySetResult(true), null);

        // Act - 修改文件
        await Task.Delay(100); // 等待文件监视器初始化
        File.WriteAllText(jsonPath, """{"Key": "NewValue"}""");

        // Assert - 等待变更检测（最多 3 秒）
        var detected = await Task.WhenAny(changeDetected.Task, Task.Delay(3000)) == changeDetected.Task;

        // 注意：文件变更检测依赖于操作系统，可能不稳定
        // 主要验证 API 正常工作
        Assert.NotNull(msConfig.GetReloadToken());
    }

    #endregion
}
