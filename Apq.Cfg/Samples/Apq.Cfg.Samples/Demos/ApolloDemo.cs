#pragma warning disable CS0162 // 检测到无法访问的代码

using Apq.Cfg;
using Apq.Cfg.Apollo;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 14: Apollo 配置中心
/// 演示如何使用 Apollo 作为配置源
/// 注意：需要运行 Apollo 服务才能执行完整示例
/// 快速启动：参考 https://www.apolloconfig.com/#/zh/deployment/quick-start
/// </summary>
public static class ApolloDemo
{
    // 是否启用实际连接测试（设为 true 需要 Apollo 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 14: Apollo 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Apollo 是携程开源的分布式配置中心。");
        Console.WriteLine("【文档】https://www.apolloconfig.com/\n");

        // 示例 14.1: 基本配置
        Demo_BasicConfiguration();

        // 示例 14.2: 多命名空间
        Demo_MultiNamespaceConfiguration();

        // 示例 14.3: 集群配置
        Demo_ClusterConfiguration();

        // 示例 14.4: 访问密钥认证
        Demo_SecretConfiguration();

        // 示例 14.5: 不同数据格式
        Demo_DataFormatConfiguration();

        // 示例 14.6: 监听配置变更
        Demo_WatchConfiguration();

        // 示例 14.7: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 14.7: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Apollo 服务后可运行\n");
        }

        Console.WriteLine("[示例 14 完成]\n");
    }

    /// <summary>
    /// 示例 14.1: 基本配置
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 14.1: 基本配置 ---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";                         // 应用 ID
                options.MetaServer = "http://localhost:8080";    // Meta Server 地址
                options.Namespaces = new[] { "application" };    // 命名空间
            }, level: 0);
        
        Console.WriteLine("    方式1: AddApollo(options => { ... })");
        Console.WriteLine("      options.AppId = \"myapp\"");
        Console.WriteLine("      options.MetaServer = \"http://localhost:8080\"");
        Console.WriteLine("      options.Namespaces = new[] { \"application\" }");

        // 方式2: 使用简化方法
        var builder2 = new CfgBuilder()
            .AddApollo("myapp", metaServer: "http://localhost:8080", level: 0);
        
        Console.WriteLine("    方式2: AddApollo(\"myapp\", metaServer: \"http://localhost:8080\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.2: 多命名空间配置
    /// </summary>
    private static void Demo_MultiNamespaceConfiguration()
    {
        Console.WriteLine("--- 示例 14.2: 多命名空间 ---");
        
        // Apollo 支持多命名空间，可以将配置分类管理
        var builder = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[]
                {
                    "application",      // 默认命名空间
                    "database",         // 数据库配置
                    "redis",            // Redis 配置
                    "common.json"       // 公共配置（JSON 格式）
                };
            }, level: 0);
        
        Console.WriteLine("    options.Namespaces = new[]");
        Console.WriteLine("    {");
        Console.WriteLine("        \"application\",  // 默认命名空间");
        Console.WriteLine("        \"database\",     // 数据库配置");
        Console.WriteLine("        \"redis\",        // Redis 配置");
        Console.WriteLine("        \"common.json\"   // 公共配置（JSON 格式）");
        Console.WriteLine("    }");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.3: 集群配置
    /// </summary>
    private static void Demo_ClusterConfiguration()
    {
        Console.WriteLine("--- 示例 14.3: 集群配置 ---");
        
        // Apollo 支持集群，可以为不同环境配置不同的集群
        var builder = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://apollo.prod.example.com:8080";
                options.Cluster = "SHAJQ";  // 上海机房集群
                options.Namespaces = new[] { "application" };
            }, level: 0);
        
        Console.WriteLine("    options.Cluster = \"SHAJQ\"  // 上海机房集群");
        Console.WriteLine("    // 常见集群命名: default, SHAJQ, SHAOY, NTGXH 等");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.4: 访问密钥认证
    /// </summary>
    private static void Demo_SecretConfiguration()
    {
        Console.WriteLine("--- 示例 14.4: 访问密钥认证 ---");
        
        var builder = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[] { "application" };
                options.Secret = "your-app-secret";  // 访问密钥
                options.ConnectTimeout = TimeSpan.FromSeconds(10);
            }, level: 0);
        
        Console.WriteLine("    options.Secret = \"your-app-secret\"");
        Console.WriteLine("    options.ConnectTimeout = TimeSpan.FromSeconds(10)");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.5: 不同数据格式
    /// </summary>
    private static void Demo_DataFormatConfiguration()
    {
        Console.WriteLine("--- 示例 14.5: 不同数据格式 ---");
        
        // Properties 格式（默认）
        var builder1 = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[] { "application" };
                options.DataFormat = ApolloDataFormat.Properties;
            }, level: 0);
        
        Console.WriteLine("    Properties: options.DataFormat = ApolloDataFormat.Properties");

        // JSON 格式
        var builder2 = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[] { "application.json" };
                options.DataFormat = ApolloDataFormat.Json;
            }, level: 0);
        
        Console.WriteLine("    JSON: options.DataFormat = ApolloDataFormat.Json");

        // YAML 格式
        var builder3 = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[] { "application.yaml" };
                options.DataFormat = ApolloDataFormat.Yaml;
            }, level: 0);
        
        Console.WriteLine("    YAML: options.DataFormat = ApolloDataFormat.Yaml");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.6: 监听配置变更
    /// </summary>
    private static void Demo_WatchConfiguration()
    {
        Console.WriteLine("--- 示例 14.6: 监听配置变更 ---");
        
        var builder = new CfgBuilder()
            .AddApollo(options =>
            {
                options.AppId = "myapp";
                options.MetaServer = "http://localhost:8080";
                options.Namespaces = new[] { "application" };
                options.EnableHotReload = true;                      // 启用热重载（默认 true）
                options.LongPollingTimeout = TimeSpan.FromSeconds(90);  // 长轮询超时
            }, level: 0);
        
        Console.WriteLine("    options.EnableHotReload = true");
        Console.WriteLine("    options.LongPollingTimeout = TimeSpan.FromSeconds(90)");
        Console.WriteLine();
        Console.WriteLine("    // 订阅配置变更");
        Console.WriteLine("    cfg.ConfigChanges.Subscribe(change =>");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.WriteLine($\"配置变更: {change.Key} = {change.NewValue}\");");
        Console.WriteLine("    });");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 14.7: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 14.7: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddApollo(options =>
                {
                    options.AppId = "apq-cfg-demo";
                    options.MetaServer = "http://localhost:8080";
                    options.Namespaces = new[] { "application" };
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0)
                .Build();

            Console.WriteLine("    ✓ 连接 Apollo 成功");

            // 读取配置
            Console.WriteLine($"    App:Name = {cfg["App:Name"] ?? "(未配置)"}");
            Console.WriteLine($"    App:Version = {cfg["App:Version"] ?? "(未配置)"}");

            // 枚举所有键
            var keys = cfg.GetChildKeys();
            Console.WriteLine($"    配置键数量: {keys.Count()}");

            cfg.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ✗ 连接失败: {ex.Message}");
            Console.WriteLine("    提示: 请确保 Apollo 服务正在运行");
        }
        
        Console.WriteLine();
        await Task.CompletedTask;
    }
}
