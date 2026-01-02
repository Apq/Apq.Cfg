namespace Apq.Cfg.Tests;

/// <summary>
/// 边界条件测试
/// </summary>
public class BoundaryConditionTests : IDisposable
{
    private readonly string _testDir;

    public BoundaryConditionTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgBoundaryTests_{Guid.NewGuid():N}");
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

    #region 空值和空字符串测试

    [Fact]
    public void Get_EmptyKey_ReturnsNull()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Null(cfg.Get(""));
    }

    [Fact]
    public void Exists_EmptyKey_ReturnsFalse()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.False(cfg.Exists(""));
    }

    [Fact]
    public void Set_EmptyStringValue_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("EmptyValue", "");

        // Assert
        Assert.Equal("", cfg.Get("EmptyValue"));
    }

    [Fact]
    public void Set_NullValue_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("NullValue", null);

        // Assert
        Assert.Null(cfg.Get("NullValue"));
    }

    #endregion

    #region 特殊字符键名测试

    [Fact]
    public void Get_KeyWithColon_ReturnsNestedValue()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Parent": {"Child": "NestedValue"}}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("NestedValue", cfg.Get("Parent:Child"));
    }

    [Fact]
    public void Get_KeyWithSpecialChars_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key-With-Dashes": "Value1", "Key_With_Underscores": "Value2", "Key.With.Dots": "Value3"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Value1", cfg.Get("Key-With-Dashes"));
        Assert.Equal("Value2", cfg.Get("Key_With_Underscores"));
        Assert.Equal("Value3", cfg.Get("Key.With.Dots"));
    }

    [Fact]
    public void Get_KeyWithUnicode_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"中文键": "中文值", "日本語キー": "日本語値", "한국어키": "한국어값"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("中文值", cfg.Get("中文键"));
        Assert.Equal("日本語値", cfg.Get("日本語キー"));
        Assert.Equal("한국어값", cfg.Get("한국어키"));
    }

    [Fact]
    public void Get_KeyWithNumbers_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"123": "NumericKey", "Key123": "MixedKey", "123Key": "NumericPrefix"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("NumericKey", cfg.Get("123"));
        Assert.Equal("MixedKey", cfg.Get("Key123"));
        Assert.Equal("NumericPrefix", cfg.Get("123Key"));
    }

    #endregion

    #region 空文件和大文件测试

    [Fact]
    public void Build_EmptyJsonFile_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "empty.json");
        File.WriteAllText(jsonPath, "{}");

        // Act
        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert
        Assert.Null(cfg.Get("AnyKey"));
        Assert.False(cfg.Exists("AnyKey"));
    }

    [Fact]
    public void Build_LargeJsonFile_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "large.json");
        var sb = new System.Text.StringBuilder();
        sb.Append("{");
        for (int i = 0; i < 10000; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append($"\"Key{i}\": \"Value{i}\"");
        }
        sb.Append("}");
        File.WriteAllText(jsonPath, sb.ToString());

        // Act
        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert
        Assert.Equal("Value0", cfg.Get("Key0"));
        Assert.Equal("Value9999", cfg.Get("Key9999"));
        Assert.Equal("Value5000", cfg.Get("Key5000"));
    }

    [Fact]
    public void Build_DeeplyNestedJson_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "nested.json");
        // 创建 10 层嵌套
        var json = """
            {
                "L1": {
                    "L2": {
                        "L3": {
                            "L4": {
                                "L5": {
                                    "L6": {
                                        "L7": {
                                            "L8": {
                                                "L9": {
                                                    "L10": "DeepValue"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            """;
        File.WriteAllText(jsonPath, json);

        // Act
        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert
        Assert.Equal("DeepValue", cfg.Get("L1:L2:L3:L4:L5:L6:L7:L8:L9:L10"));
    }

    #endregion

    #region 长键名和长值测试

    [Fact]
    public void Get_VeryLongKey_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        var longKey = new string('K', 1000);
        File.WriteAllText(jsonPath, $"{{\"{longKey}\": \"Value\"}}");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Value", cfg.Get(longKey));
    }

    [Fact]
    public void Get_VeryLongValue_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        var longValue = new string('V', 10000);
        File.WriteAllText(jsonPath, $"{{\"Key\": \"{longValue}\"}}");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(longValue, cfg.Get("Key"));
    }

    [Fact]
    public async Task Set_VeryLongValue_PersistsCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");
        var longValue = new string('V', 10000);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("LongKey", longValue);
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();
        Assert.Equal(longValue, cfg2.Get("LongKey"));
    }

    #endregion

    #region 数组和复杂结构测试

    [Fact]
    public void Get_ArrayElement_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Items": ["First", "Second", "Third"]}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("First", cfg.Get("Items:0"));
        Assert.Equal("Second", cfg.Get("Items:1"));
        Assert.Equal("Third", cfg.Get("Items:2"));
    }

    [Fact]
    public void Get_NestedArrayElement_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Users": [
                    {"Name": "Alice", "Age": 30},
                    {"Name": "Bob", "Age": 25}
                ]
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Alice", cfg.Get("Users:0:Name"));
        Assert.Equal("30", cfg.Get("Users:0:Age"));
        Assert.Equal("Bob", cfg.Get("Users:1:Name"));
        Assert.Equal("25", cfg.Get("Users:1:Age"));
    }

    #endregion

    #region 多层级边界测试

    [Fact]
    public void MultiLevel_SameKeyDifferentLevels_HigherWins()
    {
        // Arrange
        var level0Path = Path.Combine(_testDir, "level0.json");
        var level1Path = Path.Combine(_testDir, "level1.json");
        var level2Path = Path.Combine(_testDir, "level2.json");

        File.WriteAllText(level0Path, """{"Key": "Level0"}""");
        File.WriteAllText(level1Path, """{"Key": "Level1"}""");
        File.WriteAllText(level2Path, """{"Key": "Level2"}""");

        using var cfg = new CfgBuilder()
            .AddJson(level0Path, level: 0, writeable: false)
            .AddJson(level1Path, level: 1, writeable: false)
            .AddJson(level2Path, level: 2, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Level2", cfg.Get("Key"));
    }

    [Fact]
    public void MultiLevel_NegativeLevel_Works()
    {
        // Arrange
        var negativePath = Path.Combine(_testDir, "negative.json");
        var zeroPath = Path.Combine(_testDir, "zero.json");

        File.WriteAllText(negativePath, """{"Key": "Negative"}""");
        File.WriteAllText(zeroPath, """{"Key": "Zero"}""");

        using var cfg = new CfgBuilder()
            .AddJson(negativePath, level: -1, writeable: false)
            .AddJson(zeroPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - level 0 应该覆盖 level -1
        Assert.Equal("Zero", cfg.Get("Key"));
    }

    [Fact]
    public void MultiLevel_LargeGapBetweenLevels_Works()
    {
        // Arrange
        var level0Path = Path.Combine(_testDir, "level0.json");
        var level1000Path = Path.Combine(_testDir, "level1000.json");

        File.WriteAllText(level0Path, """{"Key": "Level0"}""");
        File.WriteAllText(level1000Path, """{"Key": "Level1000"}""");

        using var cfg = new CfgBuilder()
            .AddJson(level0Path, level: 0, writeable: false)
            .AddJson(level1000Path, level: 1000, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Level1000", cfg.Get("Key"));
    }

    #endregion

    #region 类型转换边界测试

    [Fact]
    public void Get_IntMaxValue_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, $"{{\"MaxInt\": {int.MaxValue}}}");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(int.MaxValue, cfg.GetValue<int>("MaxInt"));
    }

    [Fact]
    public void Get_IntMinValue_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, $"{{\"MinInt\": {int.MinValue}}}");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(int.MinValue, cfg.GetValue<int>("MinInt"));
    }

    [Fact]
    public void Get_BooleanStrings_Work()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"True1": true, "True2": "true", "True3": "True", "False1": false, "False2": "false"}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.GetValue<bool>("True1"));
        Assert.True(cfg.GetValue<bool>("True2"));
        Assert.True(cfg.GetValue<bool>("True3"));
        Assert.False(cfg.GetValue<bool>("False1"));
        Assert.False(cfg.GetValue<bool>("False2"));
    }

    [Fact]
    public void Get_DoubleSpecialValues_Work()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Zero": 0.0, "NegativeZero": -0.0, "Small": 0.0000001}""");

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(0.0, cfg.GetValue<double>("Zero"));
        Assert.Equal(0.0000001, cfg.GetValue<double>("Small"), 10);
    }

    #endregion

    #region Optional 文件测试

    [Fact]
    public void Build_OptionalMissingFile_Works()
    {
        // Arrange
        var existingPath = Path.Combine(_testDir, "existing.json");
        var missingPath = Path.Combine(_testDir, "missing.json");
        File.WriteAllText(existingPath, """{"Key": "Value"}""");

        // Act - optional: true 不应抛出异常
        using var cfg = new CfgBuilder()
            .AddJson(existingPath, level: 0, writeable: false, optional: true)
            .AddJson(missingPath, level: 1, writeable: false, optional: true)
            .Build();

        // Assert
        Assert.Equal("Value", cfg.Get("Key"));
    }

    [Fact]
    public void Build_RequiredMissingFile_ThrowsException()
    {
        // Arrange
        var missingPath = Path.Combine(_testDir, "missing.json");

        // Act & Assert - optional: false 应抛出异常
        var ex = Assert.ThrowsAny<Exception>(() =>
        {
            using var cfg = new CfgBuilder()
                .AddJson(missingPath, level: 0, writeable: false, optional: false)
                .Build();
            // 尝试访问配置以触发加载
            cfg.ToMicrosoftConfiguration();
        });

        // 验证是文件未找到相关的异常
        Assert.True(ex is FileNotFoundException || ex.InnerException is FileNotFoundException ||
                    ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("找不到", StringComparison.OrdinalIgnoreCase),
            $"Expected file not found exception, but got: {ex.GetType().Name}: {ex.Message}");
    }

    #endregion
}
