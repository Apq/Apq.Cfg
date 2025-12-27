using Apq.Cfg;
using Apq.Cfg.Apollo;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 14: Apollo 配置中心
/// 演示如何使用 Apollo 作为配置源
/// </summary>
public static class ApolloDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 14: Apollo 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Apollo 服务
        // Apollo 部署较复杂，建议参考官方文档：https://www.apolloconfig.com/
        
        Console.WriteLine("【Apollo 配置源说明】");
        Console.WriteLine("Apollo 是携程开源的分布式配置中心，支持配置的集中管理和动态推送。");
        Console.WriteLine("Apq.Cfg.Apollo 支持从 Apollo 配置中心读取配置。\n");

        // 示例 1: 基本配置
        Console.WriteLine("--- 示例 14.1: 基本配置 ---");
        ShowBasicConfiguration();

        // 示例 2: 多命名空间
        Console.WriteLine("\n--- 示例 14.2: 多命名空间 ---");
        ShowMultiNamespaceConfiguration();

        // 示例 3: 集群配置
        Console.WriteLine("\n--- 示例 14.3: 集群配置 ---");
        ShowClusterConfiguration();

        // 示例 4: 访问密钥
        Console.WriteLine("\n--- 示例 14.4: 访问密钥 ---");
        ShowSecretConfiguration();

        // 示例 5: 不同数据格式
        Console.WriteLine("\n--- 示例 14.5: 不同数据格式 ---");
        ShowDataFormatConfiguration();

        // 示例 6: 热重载
        Console.WriteLine("\n--- 示例 14.6: 热重载 ---");
        ShowHotReloadConfiguration();

        // 示例 7: 实际连接测试
        Console.WriteLine("\n--- 示例 14.7: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 14 完成\n");
    }

    private static void ShowBasicConfiguration()
    {
        Console.WriteLine("基本配置代码示例：");
        Console.WriteLine(@"
// 使用 Action 配置
var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";                     // 应用 ID
        options.MetaServer = ""http://localhost:8080""; // Meta Server 地址
        options.Namespaces = new[] { ""application"" }; // 命名空间
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg2 = new CfgBuilder()
    .AddApollo(""myapp"", metaServer: ""http://localhost:8080"")
    .Build();

// 读取配置
var host = cfg.Get(""Database:Host"");
");
    }

    private static void ShowMultiNamespaceConfiguration()
    {
        Console.WriteLine("多命名空间配置：");
        Console.WriteLine(@"
// Apollo 支持多个命名空间，可以实现配置的模块化管理
var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[]
        {
            ""application"",      // 默认命名空间
            ""database"",         // 数据库配置
            ""redis"",            // Redis 配置
            ""common.shared""     // 公共配置（关联命名空间）
        };
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg2 = new CfgBuilder()
    .AddApollo(
        ""myapp"",
        metaServer: ""http://localhost:8080"",
        namespaces: new[] { ""application"", ""database"" })
    .Build();
");
    }

    private static void ShowClusterConfiguration()
    {
        Console.WriteLine("集群配置：");
        Console.WriteLine(@"
// Apollo 支持集群，可以为不同机房/环境配置不同的值
var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://apollo-meta.example.com:8080"";
        options.Cluster = ""SHAJQ"";  // 上海机房集群
        options.Namespaces = new[] { ""application"" };
    }, level: 0)
    .Build();

// 北京机房配置
var cfgBJ = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://apollo-meta-bj.example.com:8080"";
        options.Cluster = ""BJZJY"";  // 北京机房集群
        options.Namespaces = new[] { ""application"" };
    }, level: 0)
    .Build();
");
    }

    private static void ShowSecretConfiguration()
    {
        Console.WriteLine("访问密钥配置：");
        Console.WriteLine(@"
// 当 Apollo 启用访问控制时，需要配置 Secret
var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[] { ""application"" };
        options.Secret = ""your-app-secret"";  // 访问密钥
        options.ConnectTimeout = TimeSpan.FromSeconds(10);
    }, level: 0)
    .Build();
");
    }

    private static void ShowDataFormatConfiguration()
    {
        Console.WriteLine("不同数据格式配置：");
        Console.WriteLine(@"
// 1. Properties 格式（默认）
var cfgProps = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[] { ""application"" };
        options.DataFormat = ApolloDataFormat.Properties;
    }, level: 0)
    .Build();

// 2. JSON 格式（需要在 Apollo 中创建 .json 后缀的命名空间）
var cfgJson = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[] { ""application.json"" };
        options.DataFormat = ApolloDataFormat.Json;
    }, level: 0)
    .Build();

// 3. YAML 格式（需要在 Apollo 中创建 .yaml 后缀的命名空间）
var cfgYaml = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[] { ""application.yaml"" };
        options.DataFormat = ApolloDataFormat.Yaml;
    }, level: 0)
    .Build();
");
    }

    private static void ShowHotReloadConfiguration()
    {
        Console.WriteLine("热重载配置：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddApollo(options =>
    {
        options.AppId = ""myapp"";
        options.MetaServer = ""http://localhost:8080"";
        options.Namespaces = new[] { ""application"" };
        options.EnableHotReload = true;                    // 启用热重载（默认 true）
        options.LongPollingTimeout = TimeSpan.FromSeconds(90); // 长轮询超时
    }, level: 0)
    .Build();

// 订阅配置变更
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($""配置变更: {change.Key}"");
    Console.WriteLine($""  旧值: {change.OldValue}"");
    Console.WriteLine($""  新值: {change.NewValue}"");
});

// 注意：Apollo 是只读配置源，不支持通过 API 写入配置
// 配置修改需要通过 Apollo 管理界面进行
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Apollo...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddApollo(options =>
                {
                    options.AppId = "apq-cfg-demo";
                    options.MetaServer = "http://localhost:8080";
                    options.Namespaces = new[] { "application" };
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0)
                .Build();

            // 尝试读取配置
            var testValue = cfg.Get("test");
            Console.WriteLine($"✓ 连接成功！test 键的值: {testValue ?? "(空)"}");
            
            // 显示所有键
            var keys = cfg.GetChildKeys();
            Console.WriteLine($"  配置键数量: {keys.Count()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 连接失败: {ex.Message}");
            Console.WriteLine("  提示: Apollo 部署较复杂，请参考官方文档");
            Console.WriteLine("  官方文档: https://www.apolloconfig.com/");
            Console.WriteLine("  Quick Start: https://www.apolloconfig.com/#/zh/deployment/quick-start");
        }
        
        await Task.CompletedTask;
    }
}
