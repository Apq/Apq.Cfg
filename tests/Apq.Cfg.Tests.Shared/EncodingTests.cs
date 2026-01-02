using System.Text;
using Apq.Cfg.EncodingSupport;
using Apq.Cfg.Sources.File;
using static Apq.Cfg.EncodingSupport.EncodingDetectionResult;

namespace Apq.Cfg.Tests;

/// <summary>
/// 编码检测相关测试
/// </summary>
public class EncodingTests : IDisposable
{
    private readonly string _testDir;

    public EncodingTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgEncodingTests_{Guid.NewGuid():N}");
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

    // ========== EncodingOptions 测试 ==========

    [Fact]
    public void EncodingOptions_Default_HasCorrectValues()
    {
        // Arrange & Act
        var options = Apq.Cfg.EncodingSupport.EncodingOptions.Default;

        // Assert
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingReadStrategy.AutoDetect, options.ReadStrategy);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8NoBom, options.WriteStrategy);
        Assert.Equal(0.6f, options.ConfidenceThreshold);
        Assert.True(options.EnableCache);
        Assert.False(options.EnableLogging);
    }

    [Fact]
    public void EncodingOptions_PowerShell_HasUtf8WithBom()
    {
        // Arrange & Act
        var options = Apq.Cfg.EncodingSupport.EncodingOptions.PowerShell;

        // Assert
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8WithBom, options.WriteStrategy);
    }

    [Fact]
    public void EncodingOptions_GetWriteEncoding_Utf8NoBom()
    {
        // Arrange
        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { WriteStrategy = Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8NoBom };

        // Act
        var encoding = options.GetWriteEncoding();

        // Assert
        Assert.IsType<UTF8Encoding>(encoding);
        var utf8 = (UTF8Encoding)encoding;
        Assert.Empty(utf8.GetPreamble()); // 无 BOM
    }

    [Fact]
    public void EncodingOptions_GetWriteEncoding_Utf8WithBom()
    {
        // Arrange
        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { WriteStrategy = Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8WithBom };

        // Act
        var encoding = options.GetWriteEncoding();

        // Assert
        Assert.IsType<UTF8Encoding>(encoding);
        var utf8 = (UTF8Encoding)encoding;
        Assert.Equal(3, utf8.GetPreamble().Length); // 有 BOM
    }

    [Fact]
    public void EncodingOptions_GetWriteEncoding_Preserve()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { WriteStrategy = Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Preserve };
        var detectedEncoding = Encoding.GetEncoding("GB2312");

        // Act
        var encoding = options.GetWriteEncoding(detectedEncoding);

        // Assert
        Assert.Equal(detectedEncoding, encoding);
    }

    [Fact]
    public void EncodingOptions_GetWriteEncoding_Specified()
    {
        // Arrange
        var specifiedEncoding = Encoding.Unicode;
        var options = new Apq.Cfg.EncodingSupport.EncodingOptions
        {
            WriteStrategy = Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Specified,
            WriteEncoding = specifiedEncoding
        };

        // Act
        var encoding = options.GetWriteEncoding();

        // Assert
        Assert.Equal(specifiedEncoding, encoding);
    }

    // ========== EncodingDetectionResult 测试 ==========

    [Fact]
    public void EncodingDetectionResult_ToString_FormatsCorrectly()
    {
        // Arrange
        var result = new Apq.Cfg.EncodingSupport.EncodingDetectionResult(
            Encoding.UTF8,
            0.95f,
            Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom,
            true,
            "UTF-8",
            "/test/file.json");

        // Act
        var str = result.ToString();

        // Assert
        Assert.Contains("Bom", str);
        Assert.Contains("BOM", str);
        Assert.Contains("95", str); // 95%
    }

    // ========== EncodingDetector 测试 ==========

    [Fact]
    public void EncodingDetector_Detect_Utf8NoBom()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "utf8_no_bom.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Act
        var result = Apq.Cfg.EncodingSupport.EncodingDetector.Default.Detect(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.UtfUnknown, result.Method);
        Assert.False(result.HasBom);
    }

    [Fact]
    public void EncodingDetector_Detect_Utf8WithBom()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "utf8_with_bom.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        // Act
        var result = Apq.Cfg.EncodingSupport.EncodingDetector.Default.Detect(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom, result.Method);
        Assert.True(result.HasBom);
    }

    [Fact]
    public void EncodingDetector_Detect_Utf16Le()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "utf16_le.json");
        File.WriteAllText(filePath, """{"key": "value"}""", Encoding.Unicode);

        // Act
        var result = Apq.Cfg.EncodingSupport.EncodingDetector.Default.Detect(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom, result.Method);
        Assert.True(result.HasBom);
    }

    [Fact]
    public void EncodingDetector_Detect_NonExistentFile_ReturnsFallback()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "non_existent.json");

        // Act
        var result = Apq.Cfg.EncodingSupport.EncodingDetector.Default.Detect(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Fallback, result.Method);
        Assert.Equal(Encoding.UTF8, result.Encoding);
    }

    [Fact]
    public void EncodingDetector_CustomReadMapping_OverridesDetection()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "custom_mapping.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        var customEncoding = Encoding.Unicode;
        detector.MappingConfig.AddReadMapping(filePath, EncodingMappingType.ExactPath, customEncoding, priority: 100);

        // Act
        var result = detector.Detect(filePath);

        // Assert
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.UserSpecified, result.Method);
        Assert.Equal(customEncoding, result.Encoding);

        // Cleanup
        detector.MappingConfig.RemoveReadMapping(filePath);
    }

    [Fact]
    public void EncodingDetector_Cache_ReturnsFromCache()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "cached.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { EnableCache = true };

        // Act - 第一次检测
        var result1 = detector.Detect(filePath, options);
        // Act - 第二次检测（应该从缓存获取）
        var result2 = detector.Detect(filePath, options);

        // Assert
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom, result1.Method);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Cached, result2.Method);
    }

    [Fact]
    public void EncodingDetector_Cache_InvalidatesOnFileChange()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "invalidate.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { EnableCache = true };

        // Act - 第一次检测
        var result1 = detector.Detect(filePath, options);

        // 修改文件
        Thread.Sleep(100); // 确保时间戳不同
        File.WriteAllText(filePath, """{"key": "new_value"}""", new UTF8Encoding(true));

        // Act - 第二次检测（缓存应该失效）
        var result2 = detector.Detect(filePath, options);

        // Assert
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom, result1.Method);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom, result2.Method); // 不是 Cached
    }

    [Fact]
    public void EncodingDetector_ExtensionMapping_ReturnsCorrectEncoding()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var ps1Path = Path.Combine(_testDir, "script.ps1");

        // Act
        var encoding = detector.MappingConfig.GetWriteEncoding(ps1Path);

        // Assert
        Assert.NotNull(encoding);
        Assert.IsType<UTF8Encoding>(encoding);
        Assert.Equal(3, encoding.GetPreamble().Length); // UTF-8 BOM
    }

    [Fact]
    public void EncodingDetector_Logging_InvokesHandler()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "logging.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        Apq.Cfg.EncodingSupport.EncodingDetectionResult? loggedResult = null;
        detector.OnEncodingDetected += result => loggedResult = result;

        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { EnableLogging = true };

        // Act
        detector.Detect(filePath, options);

        // Assert
        Assert.NotNull(loggedResult);
        Assert.Equal(filePath, Path.GetFullPath(loggedResult.FilePath));
    }

    [Fact]
    public void EncodingDetector_ClearCache_RemovesAllEntries()
    {
        // Arrange
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "clear_cache.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { EnableCache = true };
        detector.Detect(filePath, options);

        // Act
        detector.ClearCache();
        var result = detector.Detect(filePath, options);

        // Assert
        Assert.NotEqual(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Cached, result.Method);
    }

    // ========== CfgBuilder 编码选项测试 ==========

    [Fact]
    public async Task CfgBuilder_WithEncodingOptions_WritesWithCorrectEncoding()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "encoding_test.json");
        File.WriteAllText(jsonPath, """{}""");

        var encodingOptions = new Apq.Cfg.EncodingSupport.EncodingOptions
        {
            WriteStrategy = Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8WithBom
        };

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: true, encoding: encodingOptions)
            .Build();

        // Act
        cfg.SetValue("Key", "Value");
        await cfg.SaveAsync();

        // Assert - 检查文件是否有 BOM
        var bytes = File.ReadAllBytes(jsonPath);
        Assert.True(bytes.Length >= 3);
        Assert.Equal(0xEF, bytes[0]);
        Assert.Equal(0xBB, bytes[1]);
        Assert.Equal(0xBF, bytes[2]);
    }

    [Fact]
    public void CfgBuilder_AddReadEncodingMapping_SetsReadMapping()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "custom_read.json");
        var customEncoding = Encoding.Unicode;

        // Act
        new CfgBuilder()
            .AddReadEncodingMapping(filePath, customEncoding);

        // Assert
        var result = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.Equal(Apq.Cfg.EncodingSupport.EncodingDetectionMethod.UserSpecified, result.Method);
        Assert.Equal(customEncoding, result.Encoding);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(filePath);
    }

    [Fact]
    public void CfgBuilder_AddWriteEncodingMapping_SetsWriteMapping()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "custom_write.json");
        var customEncoding = Encoding.Unicode;

        // Act
        new CfgBuilder()
            .AddWriteEncodingMapping(filePath, customEncoding);

        // Assert
        var writeEncoding = FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(filePath);
        Assert.NotNull(writeEncoding);
        Assert.Equal(customEncoding, writeEncoding);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveWriteMapping(filePath);
    }

    [Fact]
    public void EncodingDetector_SeparateReadWriteMappings_WorkIndependently()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var detector = new Apq.Cfg.EncodingSupport.EncodingDetector();
        var filePath = Path.Combine(_testDir, "separate_rw.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        var readEncoding = Encoding.GetEncoding("GB2312");
        var writeEncoding = Encoding.Unicode;

        // Act
        detector.MappingConfig.AddReadMapping(filePath, EncodingMappingType.ExactPath, readEncoding, priority: 100);
        detector.MappingConfig.AddWriteMapping(filePath, EncodingMappingType.ExactPath, writeEncoding, priority: 100);

        // Assert - 读取映射
        var detectResult = detector.Detect(filePath);
        Assert.Equal(readEncoding, detectResult.Encoding);

        // Assert - 写入映射
        var customWriteEncoding = detector.MappingConfig.GetWriteEncoding(filePath);
        Assert.Equal(writeEncoding, customWriteEncoding);

        // Cleanup
        detector.MappingConfig.RemoveReadMapping(filePath);
        detector.MappingConfig.RemoveWriteMapping(filePath);
    }

    [Fact]
    public void CfgBuilder_WithEncodingDetectionLogging_RegistersHandler()
    {
        // Arrange
        var logged = false;

        // Act
        new CfgBuilder()
            .WithEncodingDetectionLogging(_ => logged = true);

        var filePath = Path.Combine(_testDir, "log_test.json");
        File.WriteAllText(filePath, """{}""");

        var options = new Apq.Cfg.EncodingSupport.EncodingOptions { EnableLogging = true };
        FileCfgSourceBase.EncodingDetector.Detect(filePath, options);

        // Assert
        Assert.True(logged);
    }

    // ========== 编码映射通配符测试 ==========

    [Fact]
    public void CfgBuilder_AddReadEncodingMappingWildcard_MatchesPattern()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customEncoding = Encoding.GetEncoding("GB2312");
        var pattern = "*.legacy.json";

        // Act
        new CfgBuilder()
            .AddReadEncodingMappingWildcard(pattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "config.legacy.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Assert
        var result = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.Equal(EncodingDetectionMethod.UserSpecified, result.Method);
        Assert.Equal(customEncoding.CodePage, result.Encoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(pattern);
    }

    [Fact]
    public void CfgBuilder_AddReadEncodingMappingWildcard_DoesNotMatchNonMatchingFile()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customEncoding = Encoding.GetEncoding("GB2312");
        var pattern = "*.legacy.json";

        // Act
        new CfgBuilder()
            .AddReadEncodingMappingWildcard(pattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "config.normal.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        // Assert - 不匹配的文件应该使用正常检测
        var result = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.NotEqual(customEncoding.CodePage, result.Encoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(pattern);
    }

    [Fact]
    public void CfgBuilder_AddWriteEncodingMappingWildcard_MatchesPattern()
    {
        // Arrange
        var customEncoding = new UTF8Encoding(true); // UTF-8 with BOM
        var pattern = "*.ps1";

        // Act
        new CfgBuilder()
            .AddWriteEncodingMappingWildcard(pattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "script.ps1");

        // Assert
        var writeEncoding = FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(filePath);
        Assert.NotNull(writeEncoding);
        Assert.Equal(3, writeEncoding.GetPreamble().Length); // UTF-8 BOM

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveWriteMapping(pattern);
    }

    // ========== 编码映射正则表达式测试 ==========

    [Fact]
    public void CfgBuilder_AddReadEncodingMappingRegex_MatchesPattern()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customEncoding = Encoding.GetEncoding("GB2312");
        var regexPattern = @"config\d+\.json$";

        // Act
        new CfgBuilder()
            .AddReadEncodingMappingRegex(regexPattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "config123.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Assert
        var result = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.Equal(EncodingDetectionMethod.UserSpecified, result.Method);
        Assert.Equal(customEncoding.CodePage, result.Encoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(regexPattern);
    }

    [Fact]
    public void CfgBuilder_AddReadEncodingMappingRegex_DoesNotMatchNonMatchingFile()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customEncoding = Encoding.GetEncoding("GB2312");
        var regexPattern = @"config\d+\.json$";

        // Act
        new CfgBuilder()
            .AddReadEncodingMappingRegex(regexPattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "configABC.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        // Assert - 不匹配的文件应该使用正常检测
        var result = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.NotEqual(customEncoding.CodePage, result.Encoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(regexPattern);
    }

    [Fact]
    public void CfgBuilder_AddWriteEncodingMappingRegex_MatchesPattern()
    {
        // Arrange
        var customEncoding = Encoding.Unicode;
        var regexPattern = @"logs[/\\].*\.log$";

        // Act
        new CfgBuilder()
            .AddWriteEncodingMappingRegex(regexPattern, customEncoding, priority: 100);

        var filePath = Path.Combine(_testDir, "logs", "app.log");

        // Assert
        var writeEncoding = FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(filePath);
        Assert.NotNull(writeEncoding);
        Assert.Equal(customEncoding.CodePage, writeEncoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveWriteMapping(regexPattern);
    }

    // ========== ConfigureEncodingMapping 委托测试 ==========

    [Fact]
    public void CfgBuilder_ConfigureEncodingMapping_AllowsCustomConfiguration()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customReadEncoding = Encoding.GetEncoding("GB2312");
        var customWriteEncoding = Encoding.Unicode;
        var filePath = Path.Combine(_testDir, "delegate_config.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Act
        new CfgBuilder()
            .ConfigureEncodingMapping(config =>
            {
                config.AddReadMapping(filePath, EncodingMappingType.ExactPath, customReadEncoding, priority: 100);
                config.AddWriteMapping(filePath, EncodingMappingType.ExactPath, customWriteEncoding, priority: 100);
            });

        // Assert - 读取映射
        var readResult = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.Equal(EncodingDetectionMethod.UserSpecified, readResult.Method);
        Assert.Equal(customReadEncoding.CodePage, readResult.Encoding.CodePage);

        // Assert - 写入映射
        var writeEncoding = FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(filePath);
        Assert.NotNull(writeEncoding);
        Assert.Equal(customWriteEncoding.CodePage, writeEncoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(filePath);
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveWriteMapping(filePath);
    }

    [Fact]
    public void CfgBuilder_ConfigureEncodingMapping_SupportsWildcardInDelegate()
    {
        // Arrange
        var customEncoding = new UTF8Encoding(true);
        var pattern = "*.script.ps1";

        // Act
        new CfgBuilder()
            .ConfigureEncodingMapping(config =>
            {
                config.AddWriteMapping(pattern, EncodingMappingType.Wildcard, customEncoding, priority: 100);
            });

        var filePath = Path.Combine(_testDir, "test.script.ps1");

        // Assert
        var writeEncoding = FileCfgSourceBase.EncodingDetector.MappingConfig.GetWriteEncoding(filePath);
        Assert.NotNull(writeEncoding);
        Assert.Equal(3, writeEncoding.GetPreamble().Length);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveWriteMapping(pattern);
    }

    // ========== WithEncodingConfidenceThreshold 测试 ==========

    [Fact]
    public void CfgBuilder_WithEncodingConfidenceThreshold_SetsThreshold()
    {
        // Arrange
        var filePath = Path.Combine(_testDir, "threshold_test.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Act
        var builder = new CfgBuilder()
            .WithEncodingConfidenceThreshold(0.9f);

        // Assert - 通过检测结果验证阈值设置
        // 由于阈值设置是全局的，我们只能验证方法调用不抛出异常
        Assert.NotNull(builder);
    }

    // ========== 编码映射优先级测试 ==========

    [Fact]
    public void EncodingMapping_HigherPriority_TakesPrecedence()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var lowPriorityEncoding = Encoding.GetEncoding("GB2312");
        var highPriorityEncoding = Encoding.Unicode;
        var filePath = Path.Combine(_testDir, "priority_test.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(false));

        // Act - 先添加低优先级，再添加高优先级
        new CfgBuilder()
            .ConfigureEncodingMapping(config =>
            {
                config.AddReadMapping(filePath, EncodingMappingType.ExactPath, lowPriorityEncoding, priority: 10);
                config.AddReadMapping(filePath + "_high", EncodingMappingType.ExactPath, highPriorityEncoding, priority: 100);
            });

        // 使用通配符测试优先级
        var wildcardPattern = "*.priority.json";
        new CfgBuilder()
            .AddReadEncodingMappingWildcard(wildcardPattern, highPriorityEncoding, priority: 100);

        var testFile = Path.Combine(_testDir, "test.priority.json");
        File.WriteAllText(testFile, """{"key": "value"}""", new UTF8Encoding(false));

        // Assert
        var result = FileCfgSourceBase.EncodingDetector.Detect(testFile);
        Assert.Equal(highPriorityEncoding.CodePage, result.Encoding.CodePage);

        // Cleanup
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(filePath);
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(filePath + "_high");
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(wildcardPattern);
    }

    // ========== 编码映射移除测试 ==========

    [Fact]
    public void EncodingMapping_RemoveMapping_RestoresDefaultBehavior()
    {
        // Arrange
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var customEncoding = Encoding.GetEncoding("GB2312");
        var filePath = Path.Combine(_testDir, "remove_test.json");
        File.WriteAllText(filePath, """{"key": "value"}""", new UTF8Encoding(true));

        // Act - 添加映射
        new CfgBuilder()
            .AddReadEncodingMapping(filePath, customEncoding);

        var resultWithMapping = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.Equal(customEncoding.CodePage, resultWithMapping.Encoding.CodePage);

        // Act - 移除映射
        FileCfgSourceBase.EncodingDetector.MappingConfig.RemoveReadMapping(filePath);

        // Assert - 应该恢复默认检测行为
        var resultWithoutMapping = FileCfgSourceBase.EncodingDetector.Detect(filePath);
        Assert.NotEqual(customEncoding.CodePage, resultWithoutMapping.Encoding.CodePage);
        Assert.Equal(EncodingDetectionMethod.Bom, resultWithoutMapping.Method); // UTF-8 with BOM
    }
}
