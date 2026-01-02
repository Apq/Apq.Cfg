using Apq.Cfg.Env;

namespace Apq.Cfg.Tests;

/// <summary>
/// .env 配置源测试
/// </summary>
public class EnvCfgTests : IDisposable
{
    private readonly string _testDir;

    public EnvCfgTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Fact]
    public void Get_EnvValue_ReturnsCorrectValue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            APP_NAME=TestApp
            APP_DEBUG=true
            DATABASE__HOST=localhost
            DATABASE__PORT=5432
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("TestApp", cfg.Get("APP_NAME"));
        Assert.Equal("true", cfg.Get("APP_DEBUG"));
        Assert.Equal("localhost", cfg.Get("DATABASE:HOST"));
        Assert.Equal("5432", cfg.Get("DATABASE:PORT"));
    }

    [Fact]
    public void Get_TypedValue_ReturnsCorrectType()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            MAX_RETRIES=5
            ENABLED=true
            TIMEOUT=30.5
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal(5, cfg.GetValue<int>("MAX_RETRIES"));
        Assert.True(cfg.GetValue<bool>("ENABLED"));
        Assert.Equal(30.5, cfg.GetValue<double>("TIMEOUT"));
    }

    [Fact]
    public void Get_QuotedValue_ReturnsUnquotedValue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            DOUBLE_QUOTED="Hello, World!"
            SINGLE_QUOTED='Hello, World!'
            WITH_SPACES="  spaces  "
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Hello, World!", cfg.Get("DOUBLE_QUOTED"));
        Assert.Equal("Hello, World!", cfg.Get("SINGLE_QUOTED"));
        Assert.Equal("  spaces  ", cfg.Get("WITH_SPACES"));
    }

    [Fact]
    public void Get_EscapedValue_ReturnsUnescapedValue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            MULTILINE="Line1\nLine2"
            WITH_TAB="Col1\tCol2"
            WITH_QUOTE="Say \"Hello\""
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("Line1\nLine2", cfg.Get("MULTILINE"));
        Assert.Equal("Col1\tCol2", cfg.Get("WITH_TAB"));
        Assert.Equal("Say \"Hello\"", cfg.Get("WITH_QUOTE"));
    }

    [Fact]
    public void Get_SingleQuotedValue_PreservesEscapeSequences()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            RAW_VALUE='Hello\nWorld'
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - 单引号不处理转义
        Assert.Equal("Hello\\nWorld", cfg.Get("RAW_VALUE"));
    }

    [Fact]
    public void Get_ExportPrefix_IgnoresExport()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            export API_KEY=secret123
            export DATABASE__URL=postgres://localhost
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("secret123", cfg.Get("API_KEY"));
        Assert.Equal("postgres://localhost", cfg.Get("DATABASE:URL"));
    }

    [Fact]
    public void Get_Comments_AreIgnored()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            # This is a comment
            APP_NAME=TestApp
            # Another comment
            APP_VERSION=1.0.0
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("TestApp", cfg.Get("APP_NAME"));
        Assert.Equal("1.0.0", cfg.Get("APP_VERSION"));
    }

    [Fact]
    public async Task Set_AndSave_PersistsValue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            ORIGINAL=Value
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("NEW_KEY", "NewValue");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("NewValue", cfg2.Get("NEW_KEY"));
        Assert.Equal("Value", cfg2.Get("ORIGINAL"));
    }

    [Fact]
    public async Task Set_NestedKey_UsesDoubleUnderscore()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, "");

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("DATABASE:HOST", "localhost");
        cfg.SetValue("DATABASE:PORT", "5432");
        await cfg.SaveAsync();

        // Assert - 读取文件内容验证格式
        var content = await File.ReadAllTextAsync(envPath);
        Assert.Contains("DATABASE__HOST=localhost", content);
        Assert.Contains("DATABASE__PORT=5432", content);

        // 验证可以正确读取
        using var cfg2 = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("localhost", cfg2.Get("DATABASE:HOST"));
        Assert.Equal("5432", cfg2.Get("DATABASE:PORT"));
    }

    [Fact]
    public void Exists_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            KEY=Value
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.True(cfg.Exists("KEY"));
        Assert.False(cfg.Exists("NonExistent"));
    }

    [Fact]
    public async Task Remove_AndSave_RemovesKey()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            TO_REMOVE=Value
            TO_KEEP=Value2
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.Remove("TO_REMOVE");
        await cfg.SaveAsync();

        // Assert
        using var cfg2 = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        var removedValue = cfg2.Get("TO_REMOVE");
        Assert.True(string.IsNullOrEmpty(removedValue));
        Assert.Equal("Value2", cfg2.Get("TO_KEEP"));
    }

    [Fact]
    public void GetSection_ReturnsNestedValues()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            DATABASE__HOST=localhost
            DATABASE__PORT=5432
            DATABASE__NAME=mydb
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act
        var dbSection = cfg.GetSection("DATABASE");

        // Assert
        Assert.Equal("localhost", dbSection.Get("HOST"));
        Assert.Equal("5432", dbSection.Get("PORT"));
        Assert.Equal("mydb", dbSection.Get("NAME"));
    }

    [Fact]
    public async Task Set_ValueWithSpecialChars_QuotesValue()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, "");

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // Act
        cfg.SetValue("MESSAGE", "Hello World");  // 包含空格
        cfg.SetValue("COMMENT", "Value # with hash");  // 包含 #
        await cfg.SaveAsync();

        // Assert
        var content = await File.ReadAllTextAsync(envPath);
        Assert.Contains("MESSAGE=\"Hello World\"", content);
        Assert.Contains("COMMENT=\"Value # with hash\"", content);

        // 验证可以正确读取
        using var cfg2 = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        Assert.Equal("Hello World", cfg2.Get("MESSAGE"));
        Assert.Equal("Value # with hash", cfg2.Get("COMMENT"));
    }

    [Fact]
    public void Optional_MissingFile_DoesNotThrow()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, "nonexistent.env");

        // Act & Assert - 不应抛出异常
        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false, optional: true)
            .Build();

        Assert.Null(cfg.Get("ANY_KEY"));
    }

    [Fact]
    public void EmptyValue_ReturnsEmptyString()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        File.WriteAllText(envPath, """
            EMPTY_VALUE=
            QUOTED_EMPTY=""
            """);

        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Equal("", cfg.Get("EMPTY_VALUE"));
        Assert.Equal("", cfg.Get("QUOTED_EMPTY"));
    }

    [Fact]
    public void SetEnvironmentVariables_True_SetsSystemEnvVars()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        var uniqueKey1 = $"APQ_TEST_ENV_{Guid.NewGuid():N}";
        var uniqueKey2 = $"APQ_TEST_NESTED__{Guid.NewGuid():N}";
        File.WriteAllText(envPath, $"""
            {uniqueKey1}=TestValue1
            {uniqueKey2}=TestValue2
            """);

        try
        {
            // Act
            using var cfg = new CfgBuilder()
                .AddEnv(envPath, level: 0, writeable: false, setEnvironmentVariables: true)
                .Build();

            // Assert - 验证系统环境变量已设置
            Assert.Equal("TestValue1", Environment.GetEnvironmentVariable(uniqueKey1));
            Assert.Equal("TestValue2", Environment.GetEnvironmentVariable(uniqueKey2));
        }
        finally
        {
            // Cleanup - 清理环境变量
            Environment.SetEnvironmentVariable(uniqueKey1, null);
            Environment.SetEnvironmentVariable(uniqueKey2, null);
        }
    }

    [Fact]
    public void SetEnvironmentVariables_False_DoesNotSetSystemEnvVars()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        var uniqueKey = $"APQ_TEST_NO_ENV_{Guid.NewGuid():N}";
        File.WriteAllText(envPath, $"""
            {uniqueKey}=TestValue
            """);

        // Act
        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false, setEnvironmentVariables: false)
            .Build();

        // Assert - 验证系统环境变量未设置
        Assert.Null(Environment.GetEnvironmentVariable(uniqueKey));
        // 但配置值应该可以正常读取
        Assert.Equal("TestValue", cfg.Get(uniqueKey));
    }

    [Fact]
    public void SetEnvironmentVariables_Default_DoesNotSetSystemEnvVars()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        var uniqueKey = $"APQ_TEST_DEFAULT_{Guid.NewGuid():N}";
        File.WriteAllText(envPath, $"""
            {uniqueKey}=TestValue
            """);

        // Act - 不指定 setEnvironmentVariables 参数（默认为 false）
        using var cfg = new CfgBuilder()
            .AddEnv(envPath, level: 0, writeable: false)
            .Build();

        // Assert - 验证系统环境变量未设置
        Assert.Null(Environment.GetEnvironmentVariable(uniqueKey));
        // 但配置值应该可以正常读取
        Assert.Equal("TestValue", cfg.Get(uniqueKey));
    }

    [Fact]
    public void SetEnvironmentVariables_NestedKey_PreservesOriginalFormat()
    {
        // Arrange
        var envPath = Path.Combine(_testDir, ".env");
        var uniquePrefix = $"APQ_TEST_{Guid.NewGuid():N}";
        var nestedKey = $"{uniquePrefix}__HOST";
        File.WriteAllText(envPath, $"""
            {nestedKey}=localhost
            """);

        try
        {
            // Act
            using var cfg = new CfgBuilder()
                .AddEnv(envPath, level: 0, writeable: false, setEnvironmentVariables: true)
                .Build();

            // Assert - 环境变量使用原始格式（双下划线）
            Assert.Equal("localhost", Environment.GetEnvironmentVariable(nestedKey));
            // 配置键使用冒号分隔
            Assert.Equal("localhost", cfg.Get($"{uniquePrefix}:HOST"));
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable(nestedKey, null);
        }
    }
}
