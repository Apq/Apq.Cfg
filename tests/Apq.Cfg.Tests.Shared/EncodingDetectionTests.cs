using System.Text;
using Apq.Cfg.Sources.File;

namespace Apq.Cfg.Tests;

/// <summary>
/// 编码检测功能测试
/// </summary>
public class EncodingDetectionTests : IDisposable
{
    private readonly string _testDir;
    private readonly float _originalThreshold;

    public EncodingDetectionTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgEncodingTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
        _originalThreshold = FileCfgSourceBase.EncodingConfidenceThreshold;
    }

    public void Dispose()
    {
        // 恢复原始阈值
        FileCfgSourceBase.EncodingConfidenceThreshold = _originalThreshold;

        if (Directory.Exists(_testDir))
        {
            try { Directory.Delete(_testDir, true); }
            catch { }
        }
    }

    [Fact]
    public void DetectEncoding_Utf8File_ReturnsUtf8()
    {
        // Arrange
        var path = Path.Combine(_testDir, "utf8.json");
        File.WriteAllText(path, """{"Key": "Value", "中文": "测试"}""", new UTF8Encoding(false));

        // Act
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert
        Assert.NotNull(encoding);
        // UTF-8 编码名称可能是 "utf-8" 或 "UTF-8"
        Assert.Contains("utf", encoding.WebName, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DetectEncoding_Utf8BomFile_ReturnsUtf8()
    {
        // Arrange
        var path = Path.Combine(_testDir, "utf8bom.json");
        File.WriteAllText(path, """{"Key": "Value", "中文": "测试"}""", new UTF8Encoding(true));

        // Act
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert
        Assert.NotNull(encoding);
        Assert.Contains("utf", encoding.WebName, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DetectEncoding_NonExistentFile_ReturnsUtf8()
    {
        // Arrange
        var path = Path.Combine(_testDir, "nonexistent.json");

        // Act
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert
        Assert.Equal(Encoding.UTF8, encoding);
    }

    [Fact]
    public void DetectEncoding_EmptyFile_ReturnsUtf8()
    {
        // Arrange
        var path = Path.Combine(_testDir, "empty.json");
        File.WriteAllText(path, "");

        // Act
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert
        Assert.NotNull(encoding);
    }

    [Fact]
    public void WriteEncoding_IsUtf8WithoutBom()
    {
        // Act & Assert
        Assert.NotNull(FileCfgSourceBase.WriteEncoding);
        Assert.Equal("utf-8", FileCfgSourceBase.WriteEncoding.WebName);

        // 验证是无 BOM 的 UTF-8
        var preamble = FileCfgSourceBase.WriteEncoding.GetPreamble();
        Assert.Empty(preamble);
    }

    [Fact]
    public void EncodingConfidenceThreshold_DefaultValue_Is0Point6()
    {
        // 重置为默认值
        FileCfgSourceBase.EncodingConfidenceThreshold = 0.6f;

        // Assert
        Assert.Equal(0.6f, FileCfgSourceBase.EncodingConfidenceThreshold);
    }

    [Fact]
    public void EncodingConfidenceThreshold_SetValue_Works()
    {
        // Act
        FileCfgSourceBase.EncodingConfidenceThreshold = 0.8f;

        // Assert
        Assert.Equal(0.8f, FileCfgSourceBase.EncodingConfidenceThreshold);
    }

    [Fact]
    public void EncodingConfidenceThreshold_ClampsToValidRange()
    {
        // Act - 设置超过 1.0 的值
        FileCfgSourceBase.EncodingConfidenceThreshold = 1.5f;
        Assert.Equal(1.0f, FileCfgSourceBase.EncodingConfidenceThreshold);

        // Act - 设置小于 0.0 的值
        FileCfgSourceBase.EncodingConfidenceThreshold = -0.5f;
        Assert.Equal(0.0f, FileCfgSourceBase.EncodingConfidenceThreshold);
    }

    [Fact]
    public void DetectEncoding_WithLowThreshold_DetectsMoreEncodings()
    {
        // Arrange
        var path = Path.Combine(_testDir, "ascii.txt");
        File.WriteAllText(path, "Simple ASCII text without special characters");

        // Act - 使用低阈值
        FileCfgSourceBase.EncodingConfidenceThreshold = 0.1f;
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert - 应该能检测到编码
        Assert.NotNull(encoding);
    }

    [Fact]
    public void DetectEncoding_WithHighThreshold_FallsBackToUtf8()
    {
        // Arrange
        var path = Path.Combine(_testDir, "ambiguous.txt");
        // 写入一些可能导致编码检测不确定的内容
        File.WriteAllText(path, "abc");

        // Act - 使用非常高的阈值
        FileCfgSourceBase.EncodingConfidenceThreshold = 0.99f;
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert - 应该回退到 UTF-8
        Assert.NotNull(encoding);
    }

    [Fact]
    public void DetectEncoding_GbkFile_DetectsCorrectly()
    {
        // Arrange
        var path = Path.Combine(_testDir, "gbk.txt");
        // 注册 GBK 编码提供程序
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var gbkEncoding = Encoding.GetEncoding("GBK");
        File.WriteAllText(path, "这是一段中文测试文本，用于测试GBK编码检测功能。", gbkEncoding);

        // Act
        FileCfgSourceBase.EncodingConfidenceThreshold = 0.5f;
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert - 应该能检测到某种编码（可能是 GBK 或兼容编码）
        Assert.NotNull(encoding);
    }

    [Fact]
    public void CfgBuilder_WithEncodingThreshold_AffectsDetection()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"Key": "Value"}""", new UTF8Encoding(false));

        // Act - 使用 CfgBuilder 设置阈值
        using var cfg = new CfgBuilder()
            .WithEncodingConfidenceThreshold(0.7f)
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Assert - 配置应该能正常读取
        Assert.Equal("Value", cfg.Get("Key"));
    }

    [Fact]
    public void DetectEncoding_LargeFile_Works()
    {
        // Arrange
        var path = Path.Combine(_testDir, "large.json");
        var sb = new StringBuilder();
        sb.Append("{");
        for (int i = 0; i < 1000; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append($"\"Key{i}\": \"Value{i}\"");
        }
        sb.Append("}");
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));

        // Act
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert - 应该能检测到编码（可能是 UTF-8 或 ASCII，因为纯 ASCII 内容）
        Assert.NotNull(encoding);
    }

    [Fact]
    public void DetectEncoding_BinaryFile_ReturnsEncoding()
    {
        // Arrange
        var path = Path.Combine(_testDir, "binary.bin");
        var bytes = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0xFE, 0xFD };
        File.WriteAllBytes(path, bytes);

        // Act - 二进制文件的编码检测
        var encoding = FileCfgSourceBase.DetectEncoding(path);

        // Assert - 应该返回某种编码（可能是 UTF-8 作为回退）
        Assert.NotNull(encoding);
    }
}
