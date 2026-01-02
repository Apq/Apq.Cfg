using Apq.Cfg.Internal;

namespace Apq.Cfg.Tests;

/// <summary>
/// 性能优化相关测试
/// </summary>
public class PerformanceOptimizationTests : IDisposable
{
    private readonly string _testDir;

    public PerformanceOptimizationTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgPerfTests_{Guid.NewGuid():N}");
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

    // ========== 批量操作 API 测试 ==========

    [Fact]
    public void GetMany_ReturnsAllRequestedKeys()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Key1": "Value1",
                "Key2": "Value2",
                "Key3": "Value3"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        // Act
        var result = cfg.GetMany(new[] { "Key1", "Key2", "Key3" });

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Value1", result["Key1"]);
        Assert.Equal("Value2", result["Key2"]);
        Assert.Equal("Value3", result["Key3"]);
    }

    [Fact]
    public void GetMany_ReturnsNullForMissingKeys()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key1": "Value1"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        // Act
        var result = cfg.GetMany(new[] { "Key1", "NonExistent" });

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Value1", result["Key1"]);
        Assert.Null(result["NonExistent"]);
    }

    [Fact]
    public void GetMany_Generic_ConvertsTypes()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Int1": "42",
                "Int2": "100",
                "Int3": "999"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        // Act
        var result = cfg.GetMany<int>(new[] { "Int1", "Int2", "Int3" });

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(42, result["Int1"]);
        Assert.Equal(100, result["Int2"]);
        Assert.Equal(999, result["Int3"]);
    }

    [Fact]
    public void GetMany_IncludesPendingChanges()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key1": "Original"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        cfg.SetValue("Key1", "Modified");
        cfg.SetValue("Key2", "NewValue");

        // Act
        var result = cfg.GetMany(new[] { "Key1", "Key2" });

        // Assert
        Assert.Equal("Modified", result["Key1"]);
        Assert.Equal("NewValue", result["Key2"]);
    }

    [Fact]
    public void SetMany_SetsMultipleValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        // Act
        cfg.SetManyValues(new Dictionary<string, string?>
        {
            ["Key1"] = "Value1",
            ["Key2"] = "Value2",
            ["Key3"] = "Value3"
        });

        // Assert
        Assert.Equal("Value1", cfg["Key1"]);
        Assert.Equal("Value2", cfg["Key2"]);
        Assert.Equal("Value3", cfg["Key3"]);
    }

    [Fact]
    public async Task SetMany_PersistsAfterSave()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, "{}");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        // Act
        cfg.SetManyValues(new Dictionary<string, string?>
        {
            ["Key1"] = "Value1",
            ["Key2"] = "Value2"
        });
        await cfg.SaveAsync();

        // Assert - 重新读取文件验证
        var content = File.ReadAllText(jsonPath);
        Assert.Contains("Key1", content);
        Assert.Contains("Value1", content);
        Assert.Contains("Key2", content);
        Assert.Contains("Value2", content);
    }

    // ========== GetMany 回调方式测试（高性能 API）==========

    [Fact]
    public void GetMany_Callback_InvokesForEachKey()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Key1": "Value1",
                "Key2": "Value2",
                "Key3": "Value3"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var results = new Dictionary<string, string?>();

        // Act
        cfg.GetMany(new[] { "Key1", "Key2", "Key3" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal("Value1", results["Key1"]);
        Assert.Equal("Value2", results["Key2"]);
        Assert.Equal("Value3", results["Key3"]);
    }

    [Fact]
    public void GetMany_Callback_ReturnsNullForMissingKeys()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key1": "Value1"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var results = new Dictionary<string, string?>();

        // Act
        cfg.GetMany(new[] { "Key1", "NonExistent" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("Value1", results["Key1"]);
        Assert.Null(results["NonExistent"]);
    }

    [Fact]
    public void GetMany_Callback_IncludesPendingChanges()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key1": "Original"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        cfg.SetValue("Key1", "Modified");
        cfg.SetValue("Key2", "NewValue");

        var results = new Dictionary<string, string?>();

        // Act
        cfg.GetMany(new[] { "Key1", "Key2" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal("Modified", results["Key1"]);
        Assert.Equal("NewValue", results["Key2"]);
    }

    [Fact]
    public void GetMany_Callback_PreservesKeyOrder()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "A": "1",
                "B": "2",
                "C": "3"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var orderedKeys = new List<string>();

        // Act
        cfg.GetMany(new[] { "C", "A", "B" }, (key, value) =>
        {
            orderedKeys.Add(key);
        });

        // Assert - 回调顺序应与输入顺序一致
        Assert.Equal(new[] { "C", "A", "B" }, orderedKeys);
    }

    [Fact]
    public void GetMany_Generic_Callback_ConvertsTypes()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Int1": "42",
                "Int2": "100",
                "Int3": "999"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var results = new Dictionary<string, int?>();

        // Act
        cfg.GetMany<int>(new[] { "Int1", "Int2", "Int3" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal(42, results["Int1"]);
        Assert.Equal(100, results["Int2"]);
        Assert.Equal(999, results["Int3"]);
    }

    [Fact]
    public void GetMany_Generic_Callback_ReturnsDefaultForMissingKeys()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Int1": "42"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var results = new Dictionary<string, int?>();

        // Act
        cfg.GetMany<int>(new[] { "Int1", "NonExistent" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(42, results["Int1"]);
        Assert.Equal(default(int), results["NonExistent"]);
    }

    [Fact]
    public void GetMany_Generic_Callback_IncludesPendingChanges()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Int1": "10"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        cfg.SetValue("Int1", "20");
        cfg.SetValue("Int2", "30");

        var results = new Dictionary<string, int?>();

        // Act
        cfg.GetMany<int>(new[] { "Int1", "Int2" }, (key, value) =>
        {
            results[key] = value;
        });

        // Assert
        Assert.Equal(20, results["Int1"]);
        Assert.Equal(30, results["Int2"]);
    }

    [Fact]
    public void GetMany_Callback_WithEmptyKeys_DoesNotInvokeCallback()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key1": "Value1"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true)
            .Build();

        var callCount = 0;

        // Act
        cfg.GetMany(Array.Empty<string>(), (key, value) =>
        {
            callCount++;
        });

        // Assert
        Assert.Equal(0, callCount);
    }

    // ========== KeyPathParser 测试 ==========

    [Fact]
    public void KeyPathParser_GetFirstSegment_ReturnsFirstPart()
    {
        // Act & Assert
        Assert.Equal("Database", KeyPathParser.GetFirstSegment("Database:ConnectionString").ToString());
        Assert.Equal("SingleKey", KeyPathParser.GetFirstSegment("SingleKey").ToString());
        Assert.Equal("A", KeyPathParser.GetFirstSegment("A:B:C").ToString());
    }

    [Fact]
    public void KeyPathParser_GetRemainder_ReturnsRestAfterFirstSegment()
    {
        // Act & Assert
        Assert.Equal("ConnectionString", KeyPathParser.GetRemainder("Database:ConnectionString").ToString());
        Assert.True(KeyPathParser.GetRemainder("SingleKey").IsEmpty);
        Assert.Equal("B:C", KeyPathParser.GetRemainder("A:B:C").ToString());
    }

    [Fact]
    public void KeyPathParser_GetLastSegment_ReturnsLastPart()
    {
        // Act & Assert
        Assert.Equal("ConnectionString", KeyPathParser.GetLastSegment("Database:ConnectionString").ToString());
        Assert.Equal("SingleKey", KeyPathParser.GetLastSegment("SingleKey").ToString());
        Assert.Equal("C", KeyPathParser.GetLastSegment("A:B:C").ToString());
    }

    [Fact]
    public void KeyPathParser_GetParentPath_ReturnsPathWithoutLastSegment()
    {
        // Act & Assert
        Assert.Equal("Database", KeyPathParser.GetParentPath("Database:ConnectionString").ToString());
        Assert.True(KeyPathParser.GetParentPath("SingleKey").IsEmpty);
        Assert.Equal("A:B", KeyPathParser.GetParentPath("A:B:C").ToString());
    }

    [Fact]
    public void KeyPathParser_GetDepth_ReturnsCorrectDepth()
    {
        // Act & Assert
        Assert.Equal(0, KeyPathParser.GetDepth(""));
        Assert.Equal(1, KeyPathParser.GetDepth("SingleKey"));
        Assert.Equal(2, KeyPathParser.GetDepth("Database:ConnectionString"));
        Assert.Equal(3, KeyPathParser.GetDepth("A:B:C"));
    }

    [Fact]
    public void KeyPathParser_StartsWithSegment_MatchesCorrectly()
    {
        // Act & Assert
        Assert.True(KeyPathParser.StartsWithSegment("Database:ConnectionString", "Database"));
        Assert.True(KeyPathParser.StartsWithSegment("Database", "Database"));
        Assert.False(KeyPathParser.StartsWithSegment("DatabaseBackup:Path", "Database"));
        Assert.False(KeyPathParser.StartsWithSegment("Other:Key", "Database"));
    }

    [Fact]
    public void KeyPathParser_Combine_JoinsPathsCorrectly()
    {
        // Act & Assert
        Assert.Equal("Database:ConnectionString", KeyPathParser.Combine("Database", "ConnectionString"));
        Assert.Equal("ConnectionString", KeyPathParser.Combine("", "ConnectionString"));
        Assert.Equal("Database", KeyPathParser.Combine("Database", ""));
        Assert.Equal("A:B:C", KeyPathParser.Combine("A:B", "C"));
    }

    [Fact]
    public void KeyPathParser_EnumerateSegments_IteratesAllSegments()
    {
        // Arrange
        var segments = new List<string>();

        // Act
        foreach (var segment in KeyPathParser.EnumerateSegments("A:B:C:D"))
        {
            segments.Add(segment.ToString());
        }

        // Assert
        Assert.Equal(4, segments.Count);
        Assert.Equal("A", segments[0]);
        Assert.Equal("B", segments[1]);
        Assert.Equal("C", segments[2]);
        Assert.Equal("D", segments[3]);
    }

    // ========== ValueCache 测试 ==========

    [Fact]
    public void ValueCache_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var cache = new ValueCache();

        // Act
        cache.SetValue("Key1", 42);
        cache.SetValue("Key2", "StringValue");

        // Assert
        Assert.True(cache.TryGetValue<int>("Key1", out var intValue));
        Assert.Equal(42, intValue);

        Assert.True(cache.TryGetValue<string>("Key2", out var stringValue));
        Assert.Equal("StringValue", stringValue);
    }

    [Fact]
    public void ValueCache_TryGetValue_ReturnsFalseForMissingKey()
    {
        // Arrange
        var cache = new ValueCache();

        // Act & Assert
        Assert.False(cache.TryGetValue<int>("NonExistent", out _));
    }

    [Fact]
    public void ValueCache_DifferentTypes_CachedSeparately()
    {
        // Arrange
        var cache = new ValueCache();

        // Act
        cache.SetValue<int>("Key", 42);
        cache.SetValue<string>("Key", "42");

        // Assert
        Assert.True(cache.TryGetValue<int>("Key", out var intValue));
        Assert.Equal(42, intValue);

        Assert.True(cache.TryGetValue<string>("Key", out var stringValue));
        Assert.Equal("42", stringValue);
    }

    [Fact]
    public void ValueCache_Invalidate_RemovesKey()
    {
        // Arrange
        var cache = new ValueCache();
        cache.SetValue("Key1", 42);
        cache.SetValue("Key2", 100);

        // Act
        cache.Invalidate("Key1");

        // Assert
        Assert.False(cache.TryGetValue<int>("Key1", out _));
        Assert.True(cache.TryGetValue<int>("Key2", out _));
    }

    [Fact]
    public void ValueCache_Clear_RemovesAllKeys()
    {
        // Arrange
        var cache = new ValueCache();
        cache.SetValue("Key1", 42);
        cache.SetValue("Key2", 100);

        // Act
        cache.Clear();

        // Assert
        Assert.False(cache.TryGetValue<int>("Key1", out _));
        Assert.False(cache.TryGetValue<int>("Key2", out _));
    }

    [Fact]
    public void ValueCache_Version_IncrementsOnInvalidate()
    {
        // Arrange
        var cache = new ValueCache();
        var initialVersion = cache.Version;

        // Act
        cache.SetValue("Key", 42);
        var afterSetVersion = cache.Version;

        cache.Invalidate("Key");
        var afterInvalidateVersion = cache.Version;

        // Assert
        Assert.Equal(initialVersion, afterSetVersion); // SetValue 不改变版本
        Assert.Equal(initialVersion + 1, afterInvalidateVersion); // Invalidate 增加版本
    }

    // ========== FastCollections 测试 ==========

    [Fact]
    public void FastReadOnlyDictionary_WorksCorrectly()
    {
        // Arrange
        var source = new Dictionary<string, int>
        {
            ["A"] = 1,
            ["B"] = 2,
            ["C"] = 3
        };

        // Act
        var fast = source.ToFastReadOnly();

        // Assert
        Assert.Equal(3, fast.Count);
        Assert.Equal(1, fast["A"]);
        Assert.Equal(2, fast["B"]);
        Assert.Equal(3, fast["C"]);
        Assert.True(fast.ContainsKey("A"));
        Assert.False(fast.ContainsKey("D"));
        Assert.True(fast.TryGetValue("B", out var value));
        Assert.Equal(2, value);
    }

    [Fact]
    public void FastReadOnlySet_WorksCorrectly()
    {
        // Arrange
        var source = new[] { "A", "B", "C" };

        // Act
        var fast = source.ToFastReadOnlySet();

        // Assert
        Assert.Equal(3, fast.Count);
        Assert.True(fast.Contains("A"));
        Assert.True(fast.Contains("B"));
        Assert.True(fast.Contains("C"));
        Assert.False(fast.Contains("D"));
    }
}
