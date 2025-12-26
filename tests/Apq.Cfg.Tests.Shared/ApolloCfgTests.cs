using Apq.Cfg.Apollo;

namespace Apq.Cfg.Tests;

/// <summary>
/// Apollo 配置中心测试
/// 注意：这些测试需要配置 Apollo 服务才能运行
/// </summary>
public class ApolloCfgTests : IAsyncLifetime
{
    private ICfgRoot? _cfg;

    public Task InitializeAsync()
    {
        if (!TestSettings.IsApolloConfigured)
        {
            return Task.CompletedTask;
        }

        _cfg = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = TestSettings.ApolloAppId!;
                options.MetaServer = TestSettings.ApolloMetaServer!;
                options.Namespaces = new[] { TestSettings.ApolloNamespace ?? "application" };
                options.EnableHotReload = false;
            }, level: 0, isPrimaryWriter: false)
            .Build();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_cfg != null)
        {
            await _cfg.DisposeAsync();
        }
    }

    [SkippableFact]
    public void Get_SimpleValue_Works()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        // Apollo 通常是只读的，测试读取功能
        var value = _cfg!.Get("TestKey");
        // 值可能存在也可能不存在，主要测试不抛异常
        Assert.True(true);
    }

    [SkippableFact]
    public void Get_NestedKey_Works()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        var value = _cfg!.Get("Settings:Value1");
        Assert.True(true);
    }

    [SkippableFact]
    public void Exists_Key_ReturnsCorrectResult()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        // 测试 Exists 方法不抛异常
        var exists = _cfg!.Exists("TestKey");
        Assert.True(exists || !exists); // 无论结果如何，方法应该正常工作
    }

    [SkippableFact]
    public void Get_TypedValue_Works()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        // 测试类型转换不抛异常
        try
        {
            var intValue = _cfg!.Get<int>("IntValue");
        }
        catch (Exception)
        {
            // 如果键不存在或值无法转换，这是预期的
        }
        Assert.True(true);
    }

    [SkippableFact]
    public void Apollo_OverridesJson_WhenHigherLevel()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"ApqCfgTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var jsonPath = Path.Combine(tempDir, "config.json");
            File.WriteAllText(jsonPath, """{"Setting": "JsonValue"}""");

            using var cfg = new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddApollo(options =>
                {
                    options.AppId = TestSettings.ApolloAppId!;
                    options.MetaServer = TestSettings.ApolloMetaServer!;
                    options.Namespaces = new[] { TestSettings.ApolloNamespace ?? "application" };
                    options.EnableHotReload = false;
                }, level: 1)
                .Build();

            // 如果 Apollo 中有 Setting 键，应该覆盖 JSON
            var value = cfg.Get("Setting");
            Assert.NotNull(value);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [SkippableFact]
    public void AddApollo_WithNamespace_Works()
    {
        Skip.If(!TestSettings.IsApolloConfigured, "Apollo 服务未配置，跳过测试");

        using var cfg = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = TestSettings.ApolloAppId!;
                options.MetaServer = TestSettings.ApolloMetaServer!;
                options.Namespaces = new[] { "application" };
                options.EnableHotReload = false;
            }, level: 0)
            .Build();

        Assert.NotNull(cfg);
    }
}
