using Apq.Cfg.Etcd;

namespace Apq.Cfg.Tests;

/// <summary>
/// Etcd 配置中心测试
/// 注意：这些测试需要配置 Etcd 服务才能运行
/// </summary>
public class EtcdCfgTests : IAsyncLifetime
{
    private ICfgRoot? _cfg;
    private readonly string _keyPrefix;

    public EtcdCfgTests()
    {
        _keyPrefix = $"/apqcfgtest/{Guid.NewGuid():N}/";
    }

    public Task InitializeAsync()
    {
        if (!TestSettings.IsEtcdConfigured)
        {
            return Task.CompletedTask;
        }

        _cfg = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = TestSettings.EtcdConnectionString!.Split(',');
                options.Username = TestSettings.EtcdUsername;
                options.Password = TestSettings.EtcdPassword;
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
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.SetValue("TestKey", "TestValue");
        await _cfg.SaveAsync();

        // 重新创建配置实例来验证持久化
        await using var cfg2 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = TestSettings.EtcdConnectionString!.Split(',');
                options.Username = TestSettings.EtcdUsername;
                options.Password = TestSettings.EtcdPassword;
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
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.SetValue("Settings:Value1", "Value1");
        _cfg.SetValue("Settings:Value2", "Value2");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = TestSettings.EtcdConnectionString!.Split(',');
                options.Username = TestSettings.EtcdUsername;
                options.Password = TestSettings.EtcdPassword;
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
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange
        _cfg!.SetValue("ToRemove", "Value");
        await _cfg.SaveAsync();

        // Act
        _cfg.Remove("ToRemove");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = TestSettings.EtcdConnectionString!.Split(',');
                options.Username = TestSettings.EtcdUsername;
                options.Password = TestSettings.EtcdPassword;
                options.KeyPrefix = _keyPrefix;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.False(cfg2.Exists("ToRemove"));
    }

    [SkippableFact]
    public async Task Exists_Key_ReturnsCorrectResult()
    {
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange
        _cfg!.SetValue("ExistsKey", "Value");
        await _cfg.SaveAsync();

        // Assert
        Assert.True(_cfg.Exists("ExistsKey"));
        Assert.False(_cfg.Exists("NotExistsKey"));
    }

    [SkippableFact]
    public void Etcd_OverridesJson_WhenHigherLevel()
    {
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """{"Setting": "JsonValue"}""");

            // 先设置 Etcd 值
            _cfg!.SetValue("Setting", "EtcdValue");
            _cfg.SaveAsync().Wait();

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddEtcd(options =>
                {
                    options.Endpoints = TestSettings.EtcdConnectionString!.Split(',');
                    options.Username = TestSettings.EtcdUsername;
                    options.Password = TestSettings.EtcdPassword;
                    options.KeyPrefix = _keyPrefix;
                    options.EnableHotReload = false;
                }, level: 1)
                .Build();

            // Assert
            Assert.Equal("EtcdValue", cfg.Get("Setting"));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [SkippableFact]
    public async Task Get_TypedValue_Works()
    {
        Skip.If(!TestSettings.IsEtcdConfigured, "Etcd 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.SetValue("IntValue", "42");
        _cfg.SetValue("BoolValue", "true");
        _cfg.SetValue("DoubleValue", "3.14");
        await _cfg.SaveAsync();

        // Assert
        Assert.Equal(42, _cfg.GetValue<int>("IntValue"));
        Assert.True(_cfg.GetValue<bool>("BoolValue"));
        Assert.Equal(3.14, _cfg.GetValue<double>("DoubleValue"));
    }
}
