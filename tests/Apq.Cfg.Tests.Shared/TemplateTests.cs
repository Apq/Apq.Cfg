using Apq.Cfg.Template;

namespace Apq.Cfg.Tests;

/// <summary>
/// 配置模板与变量替换测试
/// </summary>
public class TemplateTests
{
    private ICfgRoot CreateTestConfig()
    {
        var json = """
        {
            "App": {
                "Name": "TestApp",
                "Version": "1.0.0",
                "LogPath": "${App:Name}/logs",
                "DataPath": "${App:Name}/data/${App:Version}"
            },
            "Database": {
                "Host": "localhost",
                "Port": "5432",
                "ConnectionString": "Host=${Database:Host};Port=${Database:Port}"
            },
            "Paths": {
                "Home": "${ENV:USERPROFILE}",
                "Machine": "${SYS:MachineName}",
                "Combined": "${App:Name}-${SYS:MachineName}"
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
            Task.Delay(100).ContinueWith(_ =>
            {
                try { File.Delete(tempFile); } catch { }
            });
        }
    }

    [Fact]
    public void GetResolved_SimpleReference_ReturnsResolvedValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var logPath = cfg.GetResolved("App:LogPath");

        // Assert
        Assert.Equal("TestApp/logs", logPath);
    }

    [Fact]
    public void GetResolved_NestedReference_ReturnsResolvedValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var dataPath = cfg.GetResolved("App:DataPath");

        // Assert
        Assert.Equal("TestApp/data/1.0.0", dataPath);
    }

    [Fact]
    public void GetResolved_MultipleReferences_ReturnsResolvedValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var connStr = cfg.GetResolved("Database:ConnectionString");

        // Assert
        Assert.Equal("Host=localhost;Port=5432", connStr);
    }

    [Fact]
    public void GetResolved_NoVariables_ReturnsOriginalValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var host = cfg.GetResolved("Database:Host");

        // Assert
        Assert.Equal("localhost", host);
    }

    [Fact]
    public void GetResolved_SystemVariable_ReturnsSystemValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var machine = cfg.GetResolved("Paths:Machine");

        // Assert
        Assert.Equal(Environment.MachineName, machine);
    }

    [Fact]
    public void GetResolved_EnvironmentVariable_ReturnsEnvValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var expectedPath = Environment.GetEnvironmentVariable("USERPROFILE");

        // Act
        var homePath = cfg.GetResolved("Paths:Home");

        // Assert
        Assert.Equal(expectedPath, homePath);
    }

    [Fact]
    public void GetResolved_MixedVariables_ReturnsResolvedValue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var combined = cfg.GetResolved("Paths:Combined");

        // Assert
        Assert.Equal($"TestApp-{Environment.MachineName}", combined);
    }

    [Fact]
    public void ResolveVariables_Template_ReturnsResolvedString()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var template = "Application ${App:Name} v${App:Version} running on ${SYS:MachineName}";

        // Act
        var result = cfg.ResolveVariables(template);

        // Assert
        Assert.Equal($"Application TestApp v1.0.0 running on {Environment.MachineName}", result);
    }

    [Fact]
    public void ResolveVariables_NullTemplate_ReturnsNull()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ResolveVariables(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResolveVariables_EmptyTemplate_ReturnsEmpty()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.ResolveVariables("");

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void GetResolved_NonExistentKey_ReturnsNull()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var result = cfg.GetResolved("NonExistent:Key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetResolved_UnresolvedVariable_KeepsByDefault()
    {
        // Arrange
        var json = """
        {
            "Test": "${NonExistent:Key}/path"
        }
        """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        using var cfg = new CfgBuilder()
            .AddJson(tempFile, level: 0, writeable: false)
            .Build();

        try
        {
            // Act
            var result = cfg.GetResolved("Test");

            // Assert - 默认保留未解析的变量
            Assert.Equal("${NonExistent:Key}/path", result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetResolved_WithTypedResult_ReturnsConvertedValue()
    {
        // Arrange
        var json = """
        {
            "Base": "100",
            "Calculated": "${Base}"
        }
        """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        using var cfg = new CfgBuilder()
            .AddJson(tempFile, level: 0, writeable: false)
            .Build();

        try
        {
            // Act
            var result = cfg.GetResolved<int>("Calculated");

            // Assert
            Assert.Equal(100, result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void TryGetResolved_ExistingKey_ReturnsTrue()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var success = cfg.TryGetResolved("App:LogPath", out var value);

        // Assert
        Assert.True(success);
        Assert.Equal("TestApp/logs", value);
    }

    [Fact]
    public void TryGetResolved_NonExistentKey_ReturnsFalse()
    {
        // Arrange
        using var cfg = CreateTestConfig();

        // Act
        var success = cfg.TryGetResolved("NonExistent:Key", out var value);

        // Assert
        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void GetManyResolved_MultipleKeys_ReturnsAllResolved()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var keys = new[] { "App:LogPath", "App:DataPath", "Database:Host" };

        // Act
        var result = cfg.GetManyResolved(keys);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("TestApp/logs", result["App:LogPath"]);
        Assert.Equal("TestApp/data/1.0.0", result["App:DataPath"]);
        Assert.Equal("localhost", result["Database:Host"]);
    }

    [Fact]
    public void TemplateEngine_CircularReference_ThrowsException()
    {
        // Arrange
        var json = """
        {
            "A": "${B}",
            "B": "${A}"
        }
        """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        using var cfg = new CfgBuilder()
            .AddJson(tempFile, level: 0, writeable: false)
            .Build();

        try
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cfg.GetResolved("A"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void TemplateEngine_MaxRecursionDepth_ThrowsException()
    {
        // Arrange - 创建深度嵌套的引用，使用自定义选项限制深度为 1
        // 深度 0: 解析 L1 的值 "${L2}"
        // 深度 1: 解析 L2 的值 "${L3}"，此时 depth=1, MaxRecursionDepth=1, 1 > 1 = false，继续
        // 深度 2: 解析 L3 的值 "${L4}"，此时 depth=2, MaxRecursionDepth=1, 2 > 1 = true，抛出异常
        var json = """
        {
            "L1": "${L2}",
            "L2": "${L3}",
            "L3": "${L4}",
            "L4": "end"
        }
        """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        using var cfg = new CfgBuilder()
            .AddJson(tempFile, level: 0, writeable: false)
            .Build();

        var options = new VariableResolutionOptions
        {
            MaxRecursionDepth = 1
        };
        options.Resolvers.Add(VariableResolvers.Config);

        try
        {
            // Act & Assert - 深度超过 1 应该抛出异常
            Assert.Throws<InvalidOperationException>(() => cfg.GetResolved("L1", options));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetResolved_WithCustomOptions_UsesOptions()
    {
        // Arrange
        var json = """
        {
            "Test": "#{Value}#",
            "Value": "resolved"
        }
        """;
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        using var cfg = new CfgBuilder()
            .AddJson(tempFile, level: 0, writeable: false)
            .Build();

        var options = new VariableResolutionOptions
        {
            VariablePrefix = "#{",
            VariableSuffix = "}#"
        };
        options.Resolvers.Add(VariableResolvers.Config);

        try
        {
            // Act
            var result = cfg.GetResolved("Test", options);

            // Assert
            Assert.Equal("resolved", result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void SystemVariableResolver_AllProperties_ReturnValues()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var resolver = VariableResolvers.System;

        // Act & Assert - 测试所有系统属性
        Assert.Equal(Environment.MachineName, resolver.Resolve("MachineName", cfg));
        Assert.Equal(Environment.UserName, resolver.Resolve("UserName", cfg));
        Assert.Equal(Environment.ProcessorCount.ToString(), resolver.Resolve("ProcessorCount", cfg));
        Assert.Equal(Environment.Is64BitProcess.ToString(), resolver.Resolve("Is64BitProcess", cfg));
        Assert.NotNull(resolver.Resolve("Now", cfg));
        Assert.NotNull(resolver.Resolve("Today", cfg));
    }

    [Fact]
    public void SystemVariableResolver_UnknownProperty_ReturnsNull()
    {
        // Arrange
        using var cfg = CreateTestConfig();
        var resolver = VariableResolvers.System;

        // Act
        var result = resolver.Resolve("UnknownProperty", cfg);

        // Assert
        Assert.Null(result);
    }
}
