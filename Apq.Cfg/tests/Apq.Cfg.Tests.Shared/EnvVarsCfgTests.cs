namespace Apq.Cfg.Tests;

/// <summary>
/// 环境变量配置源测试
/// </summary>
public class EnvVarsCfgTests : IDisposable
{
    private readonly string _prefix;
    private readonly List<string> _setVariables = new();

    public EnvVarsCfgTests()
    {
        _prefix = $"APQTEST_{Guid.NewGuid():N}_";
    }

    public void Dispose()
    {
        // 清理设置的环境变量
        foreach (var key in _setVariables)
        {
            Environment.SetEnvironmentVariable(key, null);
        }
    }

    private void SetEnvVar(string name, string value)
    {
        var fullName = _prefix + name;
        Environment.SetEnvironmentVariable(fullName, value);
        _setVariables.Add(fullName);
    }

    [Fact]
    public void Get_EnvironmentVariable_ReturnsValue()
    {
        // Arrange
        SetEnvVar("APP_NAME", "TestApp");
        SetEnvVar("APP_VERSION", "2.0.0");

        using var cfg = new CfgBuilder()
            .AddEnvironmentVariables(level: 0, prefix: _prefix)
            .Build();

        // Act & Assert
        Assert.Equal("TestApp", cfg["APP_NAME"]);
        Assert.Equal("2.0.0", cfg["APP_VERSION"]);
    }

    [Fact]
    public void Get_NestedKey_WithDoubleUnderscore()
    {
        // Arrange
        SetEnvVar("Database__Host", "localhost");
        SetEnvVar("Database__Port", "5432");

        using var cfg = new CfgBuilder()
            .AddEnvironmentVariables(level: 0, prefix: _prefix)
            .Build();

        // Act & Assert
        Assert.Equal("localhost", cfg["Database:Host"]);
        Assert.Equal("5432", cfg["Database:Port"]);
    }

    [Fact]
    public void EnvVars_OverridesJson_WhenHigherLevel()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """
                {
                    "Setting": "JsonValue"
                }
                """);

            SetEnvVar("Setting", "EnvValue");

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddEnvironmentVariables(level: 1, prefix: _prefix)
                .Build();

            // Act & Assert
            Assert.Equal("EnvValue", cfg["Setting"]);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void Exists_EnvironmentVariable_ReturnsTrue()
    {
        // Arrange
        SetEnvVar("EXISTS_TEST", "value");

        using var cfg = new CfgBuilder()
            .AddEnvironmentVariables(level: 0, prefix: _prefix)
            .Build();

        // Act & Assert
        Assert.True(cfg.Exists("EXISTS_TEST"));
        Assert.False(cfg.Exists("NOT_EXISTS"));
    }
}
