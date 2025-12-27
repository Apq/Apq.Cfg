using Apq.Cfg;
using Apq.Cfg.Consul;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 11: Consul 配置中心
/// 演示如何使用 Consul 作为配置源
/// </summary>
public static class ConsulDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 11: Consul 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Consul 服务
        // 可以使用 Docker 快速启动：docker run -d -p 8500:8500 consul
        
        Console.WriteLine("【Consul 配置源说明】");
        Console.WriteLine("Consul 是 HashiCorp 开发的服务发现和配置管理工具。");
        Console.WriteLine("Apq.Cfg.Consul 支持从 Consul KV 存储读取配置。\n");

        // 示例 1: 基本配置
        Console.WriteLine("--- 示例 11.1: 基本配置 ---");
        ShowBasicConfiguration();

        // 示例 2: 带认证的配置
        Console.WriteLine("\n--- 示例 11.2: 带 ACL Token 认证 ---");
        ShowAuthConfiguration();

        // 示例 3: 监听配置变更
        Console.WriteLine("\n--- 示例 11.3: 监听配置变更 ---");
        ShowWatchConfiguration();

        // 示例 4: 多数据中心
        Console.WriteLine("\n--- 示例 11.4: 多数据中心配置 ---");
        ShowMultiDatacenterConfiguration();

        // 示例 5: 不同数据格式
        Console.WriteLine("\n--- 示例 11.5: 不同数据格式 ---");
        ShowDataFormatConfiguration();

        // 示例 6: 实际连接测试（如果 Consul 可用）
        Console.WriteLine("\n--- 示例 11.6: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 11 完成\n");
    }

    private static void ShowBasicConfiguration()
    {
        Console.WriteLine("基本配置代码示例：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";  // Consul 地址
        options.KeyPrefix = ""myapp/config/"";        // KV 前缀
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg2 = new CfgBuilder()
    .AddConsul(""http://localhost:8500"", keyPrefix: ""myapp/config/"")
    .Build();

// 读取配置（假设 Consul 中有 myapp/config/Database/Host）
var host = cfg.Get(""Database:Host"");
");
    }

    private static void ShowAuthConfiguration()
    {
        Console.WriteLine("带 ACL Token 认证的配置：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";
        options.KeyPrefix = ""myapp/config/"";
        options.Token = ""your-acl-token"";           // ACL Token
        options.Datacenter = ""dc1"";                 // 数据中心
    }, level: 0)
    .Build();
");
    }

    private static void ShowWatchConfiguration()
    {
        Console.WriteLine("监听配置变更：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";
        options.KeyPrefix = ""myapp/config/"";
        options.EnableHotReload = true;              // 启用热重载（默认 true）
        options.WaitTime = TimeSpan.FromMinutes(5);  // Blocking Query 等待时间
    }, level: 0)
    .Build();

// 订阅配置变更
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($""配置变更: {change.Key} = {change.NewValue}"");
});
");
    }

    private static void ShowMultiDatacenterConfiguration()
    {
        Console.WriteLine("多数据中心配置：");
        Console.WriteLine(@"
// 可以从多个数据中心加载配置，实现配置的层级覆盖
var cfg = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://consul-dc1:8500"";
        options.KeyPrefix = ""shared/config/"";
        options.Datacenter = ""dc1"";
    }, level: 0)  // 基础配置
    .AddConsul(options =>
    {
        options.Address = ""http://consul-dc2:8500"";
        options.KeyPrefix = ""myapp/config/"";
        options.Datacenter = ""dc2"";
    }, level: 1)  // 应用特定配置（覆盖基础配置）
    .Build();
");
    }

    private static void ShowDataFormatConfiguration()
    {
        Console.WriteLine("不同数据格式配置：");
        Console.WriteLine(@"
// 1. KeyValue 格式（默认）- 每个 KV 键对应一个配置项
var cfg1 = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";
        options.KeyPrefix = ""myapp/config/"";
        options.DataFormat = ConsulDataFormat.KeyValue;
    }, level: 0)
    .Build();

// 2. JSON 格式 - 单个 key 存储 JSON 配置
var cfg2 = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";
        options.KeyPrefix = ""myapp/"";
        options.DataFormat = ConsulDataFormat.Json;
        options.SingleKey = ""config.json"";  // 读取 myapp/config.json
    }, level: 0)
    .Build();

// 3. YAML 格式 - 单个 key 存储 YAML 配置
var cfg3 = new CfgBuilder()
    .AddConsul(options =>
    {
        options.Address = ""http://localhost:8500"";
        options.KeyPrefix = ""myapp/"";
        options.DataFormat = ConsulDataFormat.Yaml;
        options.SingleKey = ""config.yaml"";
    }, level: 0)
    .Build();
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Consul...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddConsul(options =>
                {
                    options.Address = "http://localhost:8500";
                    options.KeyPrefix = "apq-cfg-demo/";
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
            Console.WriteLine("  提示: 请确保 Consul 服务正在运行");
            Console.WriteLine("  快速启动: docker run -d -p 8500:8500 consul");
        }
        
        await Task.CompletedTask;
    }
}
