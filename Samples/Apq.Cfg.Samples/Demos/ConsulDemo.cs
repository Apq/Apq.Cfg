#pragma warning disable CS0162 // 检测到无法访问的代码

using Apq.Cfg;
using Apq.Cfg.Consul;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 11: Consul 配置中心
/// 演示如何使用 Consul 作为配置源
/// 注意：需要运行 Consul 服务才能执行完整示例
/// 快速启动：docker run -d -p 8500:8500 consul
/// </summary>
public static class ConsulDemo
{
    // 是否启用实际连接测试（设为 true 需要 Consul 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 11: Consul 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Consul 是 HashiCorp 开发的服务发现和配置管理工具。");
        Console.WriteLine("【启动】docker run -d -p 8500:8500 consul\n");

        // 示例 11.1: 基本配置
        Demo_BasicConfiguration();

        // 示例 11.2: 带认证的配置
        Demo_AuthConfiguration();

        // 示例 11.3: 多数据中心配置
        Demo_MultiDatacenterConfiguration();

        // 示例 11.4: 监听配置变更
        Demo_WatchConfiguration();

        // 示例 11.5: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 11.5: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Consul 服务后可运行\n");
        }

        Console.WriteLine("[示例 11 完成]\n");
    }

    /// <summary>
    /// 示例 11.1: 基本配置
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 11.1: 基本配置 ---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = "http://localhost:8500";  // Consul 地址
                options.KeyPrefix = "myapp/config/";        // KV 前缀
            }, level: 0);
        
        Console.WriteLine("    方式1: AddConsul(options => { ... })");
        Console.WriteLine("      options.Address = \"http://localhost:8500\"");
        Console.WriteLine("      options.KeyPrefix = \"myapp/config/\"");

        // 方式2: 使用简化方法
        var builder2 = new CfgBuilder()
            .AddConsul("http://localhost:8500", keyPrefix: "myapp/config/", level: 0);
        
        Console.WriteLine("    方式2: AddConsul(\"http://localhost:8500\", keyPrefix: \"myapp/config/\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 11.2: 带 ACL Token 认证
    /// </summary>
    private static void Demo_AuthConfiguration()
    {
        Console.WriteLine("--- 示例 11.2: 带 ACL Token 认证 ---");
        
        var builder = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = "http://localhost:8500";
                options.KeyPrefix = "myapp/config/";
                options.Token = "your-acl-token";           // ACL Token
                options.Datacenter = "dc1";                 // 数据中心
                options.ConnectTimeout = TimeSpan.FromSeconds(10);
            }, level: 0);
        
        Console.WriteLine("    options.Token = \"your-acl-token\"");
        Console.WriteLine("    options.Datacenter = \"dc1\"");
        Console.WriteLine("    options.ConnectTimeout = TimeSpan.FromSeconds(10)");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 11.3: 多数据中心配置（层级覆盖）
    /// </summary>
    private static void Demo_MultiDatacenterConfiguration()
    {
        Console.WriteLine("--- 示例 11.3: 多数据中心配置 ---");
        
        // 可以从多个数据中心加载配置，实现配置的层级覆盖
        var builder = new CfgBuilder()
            // 基础配置（level: 0，最低优先级）
            .AddConsul(options =>
            {
                options.Address = "http://consul-dc1:8500";
                options.KeyPrefix = "shared/config/";
                options.Datacenter = "dc1";
            }, level: 0)
            // 应用特定配置（level: 1，覆盖基础配置）
            .AddConsul(options =>
            {
                options.Address = "http://consul-dc2:8500";
                options.KeyPrefix = "myapp/config/";
                options.Datacenter = "dc2";
            }, level: 1);
        
        Console.WriteLine("    level: 0 - 共享配置 (shared/config/)");
        Console.WriteLine("    level: 1 - 应用配置 (myapp/config/) - 覆盖共享配置");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 11.4: 监听配置变更
    /// </summary>
    private static void Demo_WatchConfiguration()
    {
        Console.WriteLine("--- 示例 11.4: 监听配置变更 ---");
        
        var builder = new CfgBuilder()
            .AddConsul(options =>
            {
                options.Address = "http://localhost:8500";
                options.KeyPrefix = "myapp/config/";
                options.EnableHotReload = true;              // 启用热重载（默认 true）
                options.WaitTime = TimeSpan.FromMinutes(5);  // Blocking Query 等待时间
                options.ReconnectInterval = TimeSpan.FromSeconds(5);  // 重连间隔
            }, level: 0);
        
        Console.WriteLine("    options.EnableHotReload = true");
        Console.WriteLine("    options.WaitTime = TimeSpan.FromMinutes(5)");
        Console.WriteLine("    options.ReconnectInterval = TimeSpan.FromSeconds(5)");
        Console.WriteLine();
        Console.WriteLine("    // 订阅配置变更");
        Console.WriteLine("    cfg.ConfigChanges.Subscribe(change =>");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.WriteLine($\"配置变更: {change.Key} = {change.NewValue}\");");
        Console.WriteLine("    });");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 11.5: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 11.5: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddConsul(options =>
                {
                    options.Address = "http://localhost:8500";
                    options.KeyPrefix = "apq-cfg-demo/";
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    ✓ 连接 Consul 成功");

            // 写入测试配置
            cfg.Set("App:Name", "ConsulDemo");
            cfg.Set("App:Version", "1.0.0");
            cfg.Set("Database:Host", "localhost");
            cfg.Set("Database:Port", "5432");
            await cfg.SaveAsync();
            Console.WriteLine("    ✓ 写入配置成功");

            // 读取配置
            Console.WriteLine($"    App:Name = {cfg.Get("App:Name")}");
            Console.WriteLine($"    App:Version = {cfg.Get("App:Version")}");
            Console.WriteLine($"    Database:Host = {cfg.Get("Database:Host")}");
            Console.WriteLine($"    Database:Port = {cfg.Get<int>("Database:Port")}");

            // 枚举所有键
            var keys = cfg.GetChildKeys();
            Console.WriteLine($"    配置键数量: {keys.Count()}");

            // 清理测试数据
            cfg.Remove("App:Name");
            cfg.Remove("App:Version");
            cfg.Remove("Database:Host");
            cfg.Remove("Database:Port");
            await cfg.SaveAsync();
            Console.WriteLine("    ✓ 清理测试数据成功");

            cfg.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ✗ 连接失败: {ex.Message}");
            Console.WriteLine("    提示: 请确保 Consul 服务正在运行");
        }
        
        Console.WriteLine();
    }
}
