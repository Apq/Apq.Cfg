using Apq.Cfg;
using Apq.Cfg.Zookeeper;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 15: Zookeeper 配置中心
/// 演示如何使用 Zookeeper 作为配置源
/// </summary>
public static class ZookeeperDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 15: Zookeeper 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Zookeeper 服务
        // 可以使用 Docker 快速启动：docker run -d -p 2181:2181 zookeeper
        
        Console.WriteLine("【Zookeeper 配置源说明】");
        Console.WriteLine("Zookeeper 是 Apache 开源的分布式协调服务，常用于配置管理。");
        Console.WriteLine("Apq.Cfg.Zookeeper 支持从 Zookeeper 节点读取配置。\n");

        // 示例 1: 基本配置
        Console.WriteLine("--- 示例 15.1: 基本配置 ---");
        ShowBasicConfiguration();

        // 示例 2: 集群配置
        Console.WriteLine("\n--- 示例 15.2: 集群配置 ---");
        ShowClusterConfiguration();

        // 示例 3: 认证配置
        Console.WriteLine("\n--- 示例 15.3: 认证配置 ---");
        ShowAuthConfiguration();

        // 示例 4: JSON 格式
        Console.WriteLine("\n--- 示例 15.4: JSON 格式 ---");
        ShowJsonFormatConfiguration();

        // 示例 5: 监听配置变更
        Console.WriteLine("\n--- 示例 15.5: 监听配置变更 ---");
        ShowWatchConfiguration();

        // 示例 6: 实际连接测试
        Console.WriteLine("\n--- 示例 15.6: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 15 完成\n");
    }

    private static void ShowBasicConfiguration()
    {
        Console.WriteLine("基本配置代码示例：");
        Console.WriteLine(@"
// 使用 Action 配置
var cfg = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";  // Zookeeper 连接字符串
        options.RootPath = ""/myapp/config"";           // 根路径
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg2 = new CfgBuilder()
    .AddZookeeper(""localhost:2181"", rootPath: ""/myapp/config"")
    .Build();

// 读取配置（假设 Zookeeper 中有 /myapp/config/Database/Host 节点）
var host = cfg.Get(""Database:Host"");
");
    }

    private static void ShowClusterConfiguration()
    {
        Console.WriteLine("集群配置（多节点）：");
        Console.WriteLine(@"
// Zookeeper 集群通常有多个节点，配置多个地址可提高可用性
var cfg = new CfgBuilder()
    .AddZookeeper(options =>
    {
        // 多节点连接字符串，用逗号分隔
        options.ConnectionString = ""zk1.example.com:2181,zk2.example.com:2181,zk3.example.com:2181"";
        options.RootPath = ""/myapp/config"";
        options.SessionTimeout = TimeSpan.FromSeconds(30);   // 会话超时
        options.ConnectTimeout = TimeSpan.FromSeconds(10);   // 连接超时
        options.ReconnectInterval = TimeSpan.FromSeconds(5); // 重连间隔
    }, level: 0)
    .Build();
");
    }

    private static void ShowAuthConfiguration()
    {
        Console.WriteLine("认证配置：");
        Console.WriteLine(@"
// Zookeeper 支持 digest 认证方案
var cfg = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/myapp/config"";
        options.AuthScheme = ""digest"";           // 认证方案
        options.AuthInfo = ""user:password"";      // 认证信息
    }, level: 0)
    .Build();
");
    }

    private static void ShowJsonFormatConfiguration()
    {
        Console.WriteLine("JSON 格式配置：");
        Console.WriteLine(@"
// 1. KeyValue 格式（默认）- 每个 ZNode 对应一个配置项
var cfg1 = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/myapp/config"";
        options.DataFormat = ZookeeperDataFormat.KeyValue;
    }, level: 0)
    .Build();

// 2. JSON 格式 - 单个节点存储 JSON 配置
var cfg2 = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/myapp"";
        options.DataFormat = ZookeeperDataFormat.Json;
        options.SingleNode = ""config"";  // 读取 /myapp/config 节点的 JSON 内容
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg3 = new CfgBuilder()
    .AddZookeeperJson(""localhost:2181"", nodePath: ""/myapp/config.json"")
    .Build();
");
    }

    private static void ShowWatchConfiguration()
    {
        Console.WriteLine("监听配置变更：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/myapp/config"";
        options.EnableHotReload = true;  // 启用热重载（默认 true）
    }, level: 0)
    .Build();

// 订阅配置变更（使用 Zookeeper Watch 机制）
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($""配置变更: {change.Key}"");
    Console.WriteLine($""  旧值: {change.OldValue}"");
    Console.WriteLine($""  新值: {change.NewValue}"");
    Console.WriteLine($""  类型: {change.ChangeType}"");
});

// 多层级配置覆盖
var cfgMulti = new CfgBuilder()
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/shared/config"";  // 公共配置
    }, level: 0)
    .AddZookeeper(options =>
    {
        options.ConnectionString = ""localhost:2181"";
        options.RootPath = ""/myapp/config"";   // 应用配置（覆盖公共配置）
    }, level: 1)
    .Build();
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Zookeeper...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddZookeeper(options =>
                {
                    options.ConnectionString = "localhost:2181";
                    options.RootPath = "/apq-cfg-demo";
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
            Console.WriteLine("  提示: 请确保 Zookeeper 服务正在运行");
            Console.WriteLine("  快速启动: docker run -d -p 2181:2181 zookeeper");
        }
        
        await Task.CompletedTask;
    }
}
