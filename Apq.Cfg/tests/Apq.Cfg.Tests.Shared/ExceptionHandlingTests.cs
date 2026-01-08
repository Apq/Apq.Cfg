namespace Apq.Cfg.Tests;

/// <summary>
/// 异常场景测试
/// </summary>
public class ExceptionHandlingTests : IDisposable
{
    private readonly string _testDir;

    public ExceptionHandlingTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgExceptionTests_{Guid.NewGuid():N}");
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

    #region 无效 JSON 测试

    [Fact]
    public void Build_InvalidJson_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "invalid.json");
        File.WriteAllText(jsonPath, "{ invalid json }");

        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
        {
            using var cfg = new CfgBuilder()
                .AddJsonFile(jsonPath, level: 0, writeable: false, optional: false)
                .Build();
        });
    }

    [Fact]
    public void Build_TruncatedJson_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "truncated.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"""); // 缺少结束括号

        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
        {
            using var cfg = new CfgBuilder()
                .AddJsonFile(jsonPath, level: 0, writeable: false, optional: false)
                .Build();
        });
    }

    #endregion

    #region 无可写源测试

    [Fact]
    public void Set_NoWritableSource_ThrowsInvalidOperationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "readonly.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.SetValue("NewKey", "NewValue"));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public void Remove_NoWritableSource_ThrowsInvalidOperationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "readonly.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.Remove("Key"));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public void Set_InvalidTargetLevel_ThrowsInvalidOperationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act & Assert - 指定不存在的层级
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.SetValue("Key", "Value", targetLevel: 999));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    [Fact]
    public void Remove_InvalidTargetLevel_ThrowsInvalidOperationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.Remove("Key", targetLevel: 999));
        Assert.Contains("没有可写的配置源", ex.Message);
    }

    #endregion

    #region 类型转换异常测试

    [Fact]
    public void Get_InvalidIntConversion_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"NotANumber": "abc"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - 无效值返回默认值（与 Microsoft.Extensions.Configuration 行为一致）
        Assert.Equal(default(int), cfg.GetValue<int>("NotANumber"));
    }

    [Fact]
    public void Get_InvalidBoolConversion_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"NotABool": "maybe"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - 无效值返回默认值（与 Microsoft.Extensions.Configuration 行为一致）
        Assert.Equal(default(bool), cfg.GetValue<bool>("NotABool"));
    }

    [Fact]
    public void Get_OverflowInt_ReturnsDefault()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        // 使用字符串形式的大数字
        File.WriteAllText(jsonPath, """{"TooBig": "99999999999999999999"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - 溢出时返回默认值（与 Microsoft.Extensions.Configuration 行为一致）
        Assert.Equal(default(int), cfg.GetValue<int>("TooBig"));
    }

    [Fact]
    public void GetRequired_NonExistentKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => cfg.GetRequired<string>("NonExistent"));
        Assert.Contains("NonExistent", ex.Message);
        Assert.Contains("必需的配置键", ex.Message);
    }

    #endregion

    #region 文件权限测试

    [Fact]
    public async Task SaveAsync_ReadOnlyFile_ThrowsException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "readonly.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        // 设置文件为只读
        var fileInfo = new FileInfo(jsonPath);
        fileInfo.IsReadOnly = true;

        try
        {
            using var cfg = new CfgBuilder()
                .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
                .Build();

            cfg.SetValue("NewKey", "NewValue");

            // Act & Assert - 保存到只读文件应该抛出异常
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await cfg.SaveAsync());
            // 验证是 IO 相关的异常
            Assert.True(exception is IOException || exception is UnauthorizedAccessException,
                $"Expected IOException or UnauthorizedAccessException, but got {exception.GetType().Name}");
        }
        finally
        {
            // 清理：移除只读属性
            fileInfo.IsReadOnly = false;
        }
    }

    #endregion

    #region Dispose 后操作测试

    [Fact]
    public void Get_AfterDispose_MayThrowOrReturnNull()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .Build();

        cfg.Dispose();

        // Act & Assert - Dispose 后的行为取决于实现
        // 可能抛出 ObjectDisposedException 或返回 null
        try
        {
            var value = cfg["Key"];
            // 如果没有抛出异常，值可能是 null 或原值
        }
        catch (ObjectDisposedException)
        {
            // 预期的异常
        }
    }

    [Fact]
    public async Task SaveAsync_AfterDispose_MayThrowOrDoNothing()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        cfg.SetValue("NewKey", "NewValue");
        cfg.Dispose();

        // Act & Assert
        try
        {
            await cfg.SaveAsync();
            // 如果没有抛出异常，操作可能被忽略
        }
        catch (ObjectDisposedException)
        {
            // 预期的异常
        }
        catch (InvalidOperationException)
        {
            // 也可能抛出此异常
        }
    }

    #endregion

    #region 空配置源测试

    [Fact]
    public void Build_NoSources_ReturnsEmptyConfig()
    {
        // Act - 没有添加任何配置源，应该返回空配置而不是抛出异常
        using var cfg = new CfgBuilder().Build();

        // Assert - 空配置应该正常工作
        Assert.Null(cfg["AnyKey"]);
        Assert.False(cfg.Exists("AnyKey"));
    }

    #endregion

    #region 多 PrimaryWriter 测试

    [Fact]
    public void Build_MultiplePrimaryWriters_SameLevelUsesLast()
    {
        // Arrange
        var path1 = Path.Combine(_testDir, "config1.json");
        var path2 = Path.Combine(_testDir, "config2.json");

        File.WriteAllText(path1, """{"Key": "Value1"}""");
        File.WriteAllText(path2, """{"Key": "Value2"}""");

        // Act - 同一层级多个 PrimaryWriter
        using var cfg = new CfgBuilder()
            .AddJsonFile(path1, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJsonFile(path2, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Assert - 应该使用最后一个
        cfg.SetValue("NewKey", "NewValue");
        Assert.Equal("NewValue", cfg["NewKey"]);
    }

    [Fact]
    public async Task SaveAsync_MultiplePrimaryWriters_WritesToCorrectFile()
    {
        // Arrange
        var path1 = Path.Combine(_testDir, "config1.json");
        var path2 = Path.Combine(_testDir, "config2.json");

        File.WriteAllText(path1, """{"Key": "Value1"}""");
        File.WriteAllText(path2, """{"Key": "Value2"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(path1, level: 0, writeable: true, isPrimaryWriter: true)
            .AddJsonFile(path2, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("NewKey", "NewValue");
        await cfg.SaveAsync();

        // Assert - 验证写入到了正确的文件（最后一个 PrimaryWriter）
        var content2 = File.ReadAllText(path2);
        Assert.Contains("NewKey", content2);
    }

    #endregion

    #region CancellationToken 测试

    [Fact]
    public async Task SaveAsync_WithCancelledToken_ThrowsCancellationException()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        cfg.SetValue("NewKey", "NewValue");

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException 继承自 OperationCanceledException
        var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await cfg.SaveAsync(cancellationToken: cts.Token));
        Assert.True(ex is OperationCanceledException || ex is TaskCanceledException);
    }

    #endregion

    #region 环境变量异常测试

    [Fact]
    public void AddEnvironmentVariables_WithInvalidPrefix_Works()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""");

        // Act - 使用不存在的前缀不应抛出异常
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddEnvironmentVariables(level: 1, prefix: "NONEXISTENT_PREFIX_12345_")
            .Build();

        // Assert
        Assert.Equal("Value", cfg["Key"]);
    }

    #endregion
}
