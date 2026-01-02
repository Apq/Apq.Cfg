using Apq.Cfg.Consul;

namespace Apq.Cfg.Tests;

/// <summary>
/// Consul 配置中心测试
/// 注意：这些测试需要配置 Consul 服务才能运行
/// </summary>
public class ConsulCfgTests : IAsyncLifetime
{
    private ICfgRoot? _cfg;
    private readonly string _keyPrefix;

    public ConsulCfgTests()
    {
        _keyPrefix = $"apqcfgtest/{Guid.NewGuid():N}";
    }

    public Task InitializeAsync()
    {
        if (!TestSettings.IsConsulConfigured)
        {
            return Task.CompletedTask;
        }

        _cfg = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = TestSettings.ConsulAddress!;
                options.Token = TestSettings.ConsulToken;
                options.KeyPrefix = _keyPrefix;
                options.EnableHotReload = false;
            }, level: 0, isPrimaryWriter: true)
            .Build();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_cfg != null)
        {
            // 清理测试数据
            try
            {
                _cfg.Remove("TestKey");
                _cfg.Remove("Settings:Value1");
                _cfg.Remove("Settings:Value2");
                await _cfg.SaveAsync();
            }
            catch
            {
                // 忽略清理错误
            }

            await _cfg.DisposeAsync();
        }
    }

    [SkippableFact]
    public async Task SetAndGet_SimpleValue_Works()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("TestKey", "TestValue");
        await _cfg.SaveAsync();

        // 重新创建配置实例来验证持久化
        await using var cfg2 = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = TestSettings.ConsulAddress!;
                options.Token = TestSettings.ConsulToken;
                options.KeyPrefix = _keyPrefix;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        // Assert
        Assert.Equal("TestValue", cfg2.Get("TestKey"));
    }

    [SkippableFact]
    public async Task SetAndGet_NestedKey_Works()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("Settings:Value1", "Value1");
        _cfg.Set("Settings:Value2", "Value2");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = TestSettings.ConsulAddress!;
                options.Token = TestSettings.ConsulToken;
                options.KeyPrefix = _keyPrefix;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.Equal("Value1", cfg2.Get("Settings:Value1"));
        Assert.Equal("Value2", cfg2.Get("Settings:Value2"));
    }

    [SkippableFact]
    public async Task Remove_Key_Works()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange
        _cfg!.Set("ToRemove", "Value");
        await _cfg.SaveAsync();

        // Act
        _cfg.Remove("ToRemove");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = TestSettings.ConsulAddress!;
                options.Token = TestSettings.ConsulToken;
                options.KeyPrefix = _keyPrefix;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.False(cfg2.Exists("ToRemove"));
    }

    [SkippableFact]
    public async Task Exists_Key_ReturnsCorrectResult()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange
        _cfg!.Set("ExistsKey", "Value");
        await _cfg.SaveAsync();

        // Assert
        Assert.True(_cfg.Exists("ExistsKey"));
        Assert.False(_cfg.Exists("NotExistsKey"));
    }

    [SkippableFact]
    public void Consul_OverridesJson_WhenHigherLevel()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """{"Setting": "JsonValue"}""");

            // 先设置 Consul 值
            _cfg!.Set("Setting", "ConsulValue");
            _cfg.SaveAsync().Wait();

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddConsul(options =>
                {
                    options.Address = TestSettings.ConsulAddress!;
                    options.Token = TestSettings.ConsulToken;
                    options.KeyPrefix = _keyPrefix;
                    options.EnableHotReload = false;
                }, level: 1)
                .Build();

            // Assert
            Assert.Equal("ConsulValue", cfg.Get("Setting"));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [SkippableFact]
    public async Task Get_TypedValue_Works()
    {
        Skip.If(!TestSettings.IsConsulConfigured, "Consul 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("IntValue", "42");
        _cfg.Set("BoolValue", "true");
        _cfg.Set("DoubleValue", "3.14");
        await _cfg.SaveAsync();

        // Assert
        Assert.Equal(42, _cfg.GetValue<int>("IntValue"));
        Assert.True(_cfg.GetValue<bool>("BoolValue"));
        Assert.Equal(3.14, _cfg.GetValue<double>("DoubleValue"));
    }
}
