using Apq.Cfg.Database;

namespace Apq.Cfg.Tests;

/// <summary>
/// 数据库配置源测试
/// 注意：这些测试需要配置数据库连接字符串才能运行
/// </summary>
public class DatabaseCfgTests : IAsyncLifetime
{
    private ICfgRoot? _cfg;
    private readonly string _tableName;

    public DatabaseCfgTests()
    {
        _tableName = $"ApqCfgTest_{Guid.NewGuid():N}";
    }

    public Task InitializeAsync()
    {
        if (!TestSettings.IsDatabaseConfigured)
        {
            return Task.CompletedTask;
        }

        _cfg = new CfgBuilder()
            .AddDatabase(options =>
            {
                options.Provider = TestSettings.DatabaseProvider;
                options.ConnectionString = TestSettings.DatabaseConnectionString;
                options.Table = _tableName;
                options.KeyColumn = "ConfigKey";
                options.ValueColumn = "ConfigValue";
            }, level: 0, isPrimaryWriter: true)
            .Build();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_cfg != null)
        {
            await _cfg.DisposeAsync();
        }

        // 清理测试表
        if (TestSettings.IsDatabaseConfigured)
        {
            try
            {
                // 这里可以添加清理表的逻辑
                // 由于不同数据库语法不同，暂时跳过
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }

    [SkippableFact]
    public async Task SetAndGet_SimpleValue_Works()
    {
        Skip.If(!TestSettings.IsDatabaseConfigured, "数据库连接字符串未配置，跳过测试");

        // Arrange & Act
        _cfg!.SetValue("TestKey", "TestValue");
        await _cfg.SaveAsync();

        // 重新创建配置实例来验证持久化
        await using var cfg2 = new CfgBuilder()
            .AddDatabase(options =>
            {
                options.Provider = TestSettings.DatabaseProvider;
                options.ConnectionString = TestSettings.DatabaseConnectionString;
                options.Table = _tableName;
                options.KeyColumn = "ConfigKey";
                options.ValueColumn = "ConfigValue";
            }, level: 0)
            .Build();

        // Assert
        Assert.Equal("TestValue", cfg2["TestKey"]);
    }

    [SkippableFact]
    public async Task SetAndGet_NestedKey_Works()
    {
        Skip.If(!TestSettings.IsDatabaseConfigured, "数据库连接字符串未配置，跳过测试");

        // Arrange & Act
        _cfg!.SetValue("Settings:Value1", "Value1");
        _cfg.SetValue("Settings:Value2", "Value2");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddDatabase(options =>
            {
                options.Provider = TestSettings.DatabaseProvider;
                options.ConnectionString = TestSettings.DatabaseConnectionString;
                options.Table = _tableName;
                options.KeyColumn = "ConfigKey";
                options.ValueColumn = "ConfigValue";
            }, level: 0)
            .Build();

        Assert.Equal("Value1", cfg2["Settings:Value1"]);
        Assert.Equal("Value2", cfg2["Settings:Value2"]);
    }

    [SkippableFact]
    public async Task Remove_Key_Works()
    {
        Skip.If(!TestSettings.IsDatabaseConfigured, "数据库连接字符串未配置，跳过测试");

        // Arrange
        _cfg!.SetValue("ToRemove", "Value");
        await _cfg.SaveAsync();

        // Act
        _cfg.Remove("ToRemove");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddDatabase(options =>
            {
                options.Provider = TestSettings.DatabaseProvider;
                options.ConnectionString = TestSettings.DatabaseConnectionString;
                options.Table = _tableName;
                options.KeyColumn = "ConfigKey";
                options.ValueColumn = "ConfigValue";
            }, level: 0)
            .Build();

        Assert.False(cfg2.Exists("ToRemove"));
    }

    [SkippableFact]
    public async Task Exists_Key_ReturnsCorrectResult()
    {
        Skip.If(!TestSettings.IsDatabaseConfigured, "数据库连接字符串未配置，跳过测试");

        // Arrange
        _cfg!.SetValue("ExistsKey", "Value");
        await _cfg.SaveAsync();

        // Assert
        Assert.True(_cfg.Exists("ExistsKey"));
        Assert.False(_cfg.Exists("NotExistsKey"));
    }

    [SkippableFact]
    public void Database_OverridesJson_WhenHigherLevel()
    {
        Skip.If(!TestSettings.IsDatabaseConfigured, "数据库连接字符串未配置，跳过测试");

        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """{"Setting": "JsonValue"}""");

            // 先设置数据库值
            _cfg!.SetValue("Setting", "DatabaseValue");
            _cfg.SaveAsync().Wait();

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddDatabase(options =>
                {
                    options.Provider = TestSettings.DatabaseProvider;
                    options.ConnectionString = TestSettings.DatabaseConnectionString;
                    options.Table = _tableName;
                    options.KeyColumn = "ConfigKey";
                    options.ValueColumn = "ConfigValue";
                }, level: 1)
                .Build();

            // Assert
            Assert.Equal("DatabaseValue", cfg["Setting"]);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
