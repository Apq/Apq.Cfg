using Apq.Cfg.Snapshot;
using System.Text.Json;

namespace Apq.Cfg.Tests;

/// <summary>
/// 配置快照导出测试
/// </summary>
public class SnapshotTests
{
    private ICfgRoot CreateTestConfig()
    {
        var json = """
        {
            "App": {
                "Name": "TestApp",
                "Version": "1.0.0",
                "Environment": "Development"
            },
            "Database": {
                "Host": "localhost",
                "Port": "5432",
                "Password": "secret123"
            },
            "Logging": {
                "Level": "Information"
            }
        }
        """;

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        try
        {
            return new CfgBuilder()
                .AddJson(tempFile, level: 0, writeable: false)
                .Build();
        }
        finally
        {
            // 延迟删除，让配置加载完成
            Task.Delay(100).ContinueWith(_ =>
            {
                try { File.Delete(tempFile); } catch { }
            });
        }
    }

    [Fact]
    public void ExportSnapshot_DefaultOptions_ReturnsJson()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshot();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("App", result);
        Assert.Contains("TestApp", result);
        Assert.Contains("Database", result);

        // 验证是有效的 JSON
        var doc = JsonDocument.Parse(result);
        Assert.NotNull(doc);
    }

    [Fact]
    public void ExportSnapshot_JsonFormat_ReturnsNestedStructure()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            Indented = true,
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Json, options);

        // Assert
        var doc = JsonDocument.Parse(result);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("App", out var app));
        Assert.True(app.TryGetProperty("Name", out var name));
        Assert.Equal("TestApp", name.GetString());
    }

    [Fact]
    public void ExportSnapshot_KeyValueFormat_ReturnsKeyValuePairs()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.KeyValue, options);

        // Assert
        Assert.Contains("App:Name=TestApp", result);
        Assert.Contains("Database:Host=localhost", result);
        Assert.Contains("Database:Port=5432", result);
    }

    [Fact]
    public void ExportSnapshot_EnvFormat_ReturnsEnvVariables()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Env, options);

        // Assert
        Assert.Contains("APP__NAME=TestApp", result);
        Assert.Contains("DATABASE__HOST=localhost", result);
        Assert.Contains("DATABASE__PORT=5432", result);
    }

    [Fact]
    public void ExportSnapshot_EnvFormat_WithPrefix()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            EnvPrefix = "MYAPP_",
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Env, options);

        // Assert
        Assert.Contains("MYAPP_APP__NAME=TestApp", result);
        Assert.Contains("MYAPP_DATABASE__HOST=localhost", result);
    }

    [Fact]
    public void ExportSnapshot_WithMetadata_IncludesMetadata()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            IncludeMetadata = true,
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Json, options);

        // Assert
        var doc = JsonDocument.Parse(result);
        Assert.True(doc.RootElement.TryGetProperty("__metadata", out var metadata));
        Assert.True(metadata.TryGetProperty("exportedAt", out _));
        Assert.True(metadata.TryGetProperty("keyCount", out _));
    }

    [Fact]
    public void ExportSnapshot_IncludeKeys_FiltersKeys()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            IncludeKeys = new[] { "App:*" },
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.KeyValue, options);

        // Assert
        Assert.Contains("App:Name=TestApp", result);
        Assert.DoesNotContain("Database", result);
    }

    [Fact]
    public void ExportSnapshot_ExcludeKeys_FiltersKeys()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            ExcludeKeys = new[] { "Database:*" },
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.KeyValue, options);

        // Assert
        Assert.Contains("App:Name=TestApp", result);
        Assert.DoesNotContain("Database:Host", result);
        Assert.DoesNotContain("Database:Port", result);
    }

    [Fact]
    public void ExportSnapshotAsJson_ReturnsValidJson()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshotAsJson();

        // Assert
        var doc = JsonDocument.Parse(result);
        Assert.NotNull(doc);
    }

    [Fact]
    public void ExportSnapshotAsEnv_ReturnsEnvFormat()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshotAsEnv(prefix: "TEST_");

        // Assert
        Assert.Contains("TEST_APP__NAME=", result);
    }

    [Fact]
    public void ExportSnapshotAsDictionary_ReturnsFlatDictionary()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshotAsDictionary(maskSensitive: false);

        // Assert
        Assert.True(result.ContainsKey("App:Name"));
        Assert.Equal("TestApp", result["App:Name"]);
        Assert.True(result.ContainsKey("Database:Host"));
        Assert.Equal("localhost", result["Database:Host"]);
    }

    [Fact]
    public async Task ExportSnapshotToFileAsync_CreatesFile()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            await cfg.ExportSnapshotToFileAsync(tempFile, new ExportOptions
            {
                MaskSensitiveValues = false
            });

            // Assert
            Assert.True(File.Exists(tempFile));
            var content = await File.ReadAllTextAsync(tempFile);
            Assert.Contains("TestApp", content);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExportSnapshotAsync_WritesToStream()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        using var stream = new MemoryStream();

        // Act
        await cfg.ExportSnapshotAsync(stream, new ExportOptions
        {
            MaskSensitiveValues = false
        });

        // Assert
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Assert.Contains("TestApp", content);
    }

    [Fact]
    public void ExportSnapshot_WithConfigureAction_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshot(options =>
        {
            options.MaskSensitiveValues = false;
        });

        // Assert
        Assert.Contains("App", result);
        Assert.Contains("TestApp", result);
    }

    [Fact]
    public void ExportSnapshot_NotIndented_ReturnsCompactJson()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var options = new ExportOptions
        {
            Indented = false,
            MaskSensitiveValues = false
        };

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Json, options);

        // Assert
        // 紧凑格式不应包含换行符（除了可能的末尾）
        Assert.DoesNotContain("\n  ", result);
    }

    [Fact]
    public void ConfigExporter_Export_StaticMethod_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = ConfigExporter.Export(cfg);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("App", result);
    }

    [Fact]
    public void ConfigExporter_ExportToDictionary_StaticMethod_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = ConfigExporter.ExportToDictionary(cfg, new ExportOptions { MaskSensitiveValues = false });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #region 自定义导出器测试

    [Fact]
    public void ExportSnapshot_WithCustomExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act - 使用 Lambda 表达式作为自定义导出器
        var result = cfg.ExportSnapshot((data, ctx) =>
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[CUSTOM FORMAT]");
            foreach (var (key, value) in data.OrderBy(x => x.Key))
            {
                sb.AppendLine($"[CUSTOM] {key} => {value}");
            }
            return sb.ToString();
        });

        // Assert
        Assert.Contains("[CUSTOM]", result);
        Assert.Contains("App:Name", result);
    }

    [Fact]
    public void ExportSnapshot_WithJsonExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Json);

        // Assert
        Assert.Contains("App", result);
        Assert.Contains("TestApp", result);
    }

    [Fact]
    public void ExportSnapshot_WithKeyValueExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.KeyValue, new ExportOptions { MaskSensitiveValues = false });

        // Assert
        Assert.Contains("App:Name=TestApp", result);
    }

    [Fact]
    public void ExportSnapshot_WithEnvExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshot(SnapshotExporters.Env, new ExportOptions { MaskSensitiveValues = false });

        // Assert
        Assert.Contains("APP__NAME=TestApp", result);
    }

    [Fact]
    public void ExportSnapshotAsKeyValue_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ExportSnapshotAsKeyValue(maskSensitive: false);

        // Assert
        Assert.Contains("App:Name=TestApp", result);
        Assert.Contains("Database:Host=localhost", result);
    }

    [Fact]
    public void ExportContext_FromOptions_CreatesCorrectContext()
    {
        // Arrange
        var options = new ExportOptions
        {
            IncludeMetadata = true,
            Indented = false,
            EnvPrefix = "TEST_"
        };

        // Act
        var context = ExportContext.FromOptions(options, 10);

        // Assert
        Assert.True(context.IncludeMetadata);
        Assert.False(context.Indented);
        Assert.Equal("TEST_", context.EnvPrefix);
        Assert.Equal(10, context.KeyCount);
    }

    [Fact]
    public async Task ExportSnapshotToFileAsync_WithCustomExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            await cfg.ExportSnapshotToFileAsync((data, ctx) =>
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("[CUSTOM FORMAT]");
                foreach (var (key, value) in data.OrderBy(x => x.Key))
                {
                    sb.AppendLine($"[CUSTOM] {key} => {value}");
                }
                return sb.ToString();
            }, tempFile);

            // Assert
            var content = await File.ReadAllTextAsync(tempFile);
            Assert.Contains("[CUSTOM]", content);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExportSnapshotAsync_WithCustomExporter_Works()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        using var stream = new MemoryStream();

        // Act
        await cfg.ExportSnapshotAsync((data, ctx) =>
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[CUSTOM FORMAT]");
            foreach (var (key, value) in data.OrderBy(x => x.Key))
            {
                sb.AppendLine($"[CUSTOM] {key} => {value}");
            }
            return sb.ToString();
        }, stream);

        // Assert
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Assert.Contains("[CUSTOM]", content);
    }

    #endregion
}
