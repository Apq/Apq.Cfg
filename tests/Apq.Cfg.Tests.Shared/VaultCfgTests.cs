using Apq.Cfg.Vault;

namespace Apq.Cfg.Tests;

/// <summary>
/// Vault 密钥管理测试
/// 注意：这些测试需要配置 Vault 服务才能运行
/// </summary>
public class VaultCfgTests : IAsyncLifetime
{
    private ICfgRoot? _cfg;
    private readonly string _testPath;

    public VaultCfgTests()
    {
        _testPath = $"apqcfgtest/{Guid.NewGuid():N}";
    }

    public Task InitializeAsync()
    {
        if (!TestSettings.IsVaultConfigured)
        {
            return Task.CompletedTask;
        }

        _cfg = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = TestSettings.VaultAddress!;
                options.Token = TestSettings.VaultToken!;
                options.EnginePath = TestSettings.VaultEnginePath ?? "kv";
                options.Path = _testPath;
                options.KvVersion = TestSettings.VaultKvVersion;
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
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("TestKey", "TestValue");
        await _cfg.SaveAsync();

        // 重新创建配置实例来验证持久化
        await using var cfg2 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = TestSettings.VaultAddress!;
                options.Token = TestSettings.VaultToken!;
                options.EnginePath = TestSettings.VaultEnginePath ?? "kv";
                options.Path = _testPath;
                options.KvVersion = TestSettings.VaultKvVersion;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        // Assert
        Assert.Equal("TestValue", cfg2.Get("TestKey"));
    }

    [SkippableFact]
    public async Task SetAndGet_NestedKey_Works()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("Settings:Value1", "Value1");
        _cfg.Set("Settings:Value2", "Value2");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = TestSettings.VaultAddress!;
                options.Token = TestSettings.VaultToken!;
                options.EnginePath = TestSettings.VaultEnginePath ?? "kv";
                options.Path = _testPath;
                options.KvVersion = TestSettings.VaultKvVersion;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.Equal("Value1", cfg2.Get("Settings:Value1"));
        Assert.Equal("Value2", cfg2.Get("Settings:Value2"));
    }

    [SkippableFact]
    public async Task Remove_Key_Works()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange
        _cfg!.Set("ToRemove", "Value");
        await _cfg.SaveAsync();

        // Act
        _cfg.Remove("ToRemove");
        await _cfg.SaveAsync();

        // Assert
        await using var cfg2 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = TestSettings.VaultAddress!;
                options.Token = TestSettings.VaultToken!;
                options.EnginePath = TestSettings.VaultEnginePath ?? "kv";
                options.Path = _testPath;
                options.KvVersion = TestSettings.VaultKvVersion;
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.False(cfg2.Exists("ToRemove"));
    }

    [SkippableFact]
    public async Task Exists_Key_ReturnsCorrectResult()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange
        _cfg!.Set("ExistsKey", "Value");
        await _cfg.SaveAsync();

        // Assert
        Assert.True(_cfg.Exists("ExistsKey"));
        Assert.False(_cfg.Exists("NotExistsKey"));
    }

    [SkippableFact]
    public void Vault_OverridesJson_WhenHigherLevel()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """{"Setting": "JsonValue"}""");

            // 先设置 Vault 值
            _cfg!.Set("Setting", "VaultValue");
            _cfg.SaveAsync().Wait();

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddVault(options =>
                {
                    options.Address = TestSettings.VaultAddress!;
                    options.Token = TestSettings.VaultToken!;
                    options.EnginePath = TestSettings.VaultEnginePath ?? "kv";
                    options.Path = _testPath;
                    options.KvVersion = TestSettings.VaultKvVersion;
                    options.EnableHotReload = false;
                }, level: 1)
                .Build();

            // Assert
            Assert.Equal("VaultValue", cfg.Get("Setting"));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [SkippableFact]
    public async Task Get_TypedValue_Works()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        // Arrange & Act
        _cfg!.Set("IntValue", "42");
        _cfg.Set("BoolValue", "true");
        _cfg.Set("DoubleValue", "3.14");
        await _cfg.SaveAsync();

        // Assert
        Assert.Equal(42, _cfg.Get<int>("IntValue"));
        Assert.True(_cfg.Get<bool>("BoolValue"));
        Assert.Equal(3.14, _cfg.Get<double>("DoubleValue"));
    }

    [SkippableFact]
    public void AddVaultV1_Works()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        using var cfg = new CfgBuilder()
            .AddVaultV1(
                address: TestSettings.VaultAddress!,
                token: TestSettings.VaultToken!,
                enginePath: TestSettings.VaultEnginePath ?? "kv",
                path: _testPath,
                level: 0)
            .Build();

        Assert.NotNull(cfg);
    }

    [SkippableFact]
    public void AddVaultV2_Works()
    {
        Skip.If(!TestSettings.IsVaultConfigured, "Vault 服务未配置，跳过测试");

        using var cfg = new CfgBuilder()
            .AddVaultV2(
                address: TestSettings.VaultAddress!,
                token: TestSettings.VaultToken!,
                enginePath: TestSettings.VaultEnginePath ?? "kv",
                path: _testPath,
                level: 0)
            .Build();

        Assert.NotNull(cfg);
    }
}
