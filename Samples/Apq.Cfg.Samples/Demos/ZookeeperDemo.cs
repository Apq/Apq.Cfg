#pragma warning disable CS0162 // 检测到无法访问的代码

using Apq.Cfg;
using Apq.Cfg.Zookeeper;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 15: Zookeeper 配置中心
/// 演示如何使用 Zookeeper 作为配置源
/// 注意：需要运行 Zookeeper 服务才能执行完整示例
/// 快速启动：docker run -d -p 2181:2181 zookeeper:latest
/// </summary>
public static class ZookeeperDemo
{
    // 是否启用实际连接测试（设为 true 需要 Zookeeper 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 15: Zookeeper 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Zookeeper 是 Apache 开源的分布式协调服务。");
        Console.WriteLine("【启动】docker run -d -p 2181:2181 zookeeper:latest\n");

        // 示例 15.1: 基本配置
        Demo_BasicConfiguration();

        // 示例 15.2: 集群配置
        Demo_ClusterConfiguration();

        // 示例 15.3: 认证配置
        Demo_AuthConfiguration();

        // 示例 15.4: 不同数据格式
        Demo_DataFormatConfiguration();

        // 示例 15.5: 监听配置变更
        Demo_WatchConfiguration();

        // 示例 15.6: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 15.6: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Zookeeper 服务后可运行\n");
        }

        Console.WriteLine("[示例 15 完成]\n");
    }

    /// <summary>
    /// 示例 15.1: 基本配置
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 15.1: 基本配置 ---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddZookeeper(options =>
            {
                options.ConnectionString = "localhost:2181";  // Zookeeper 连接字符串
                options.RootPath = "/myapp/config";           // 根路径
            }, level: 0);
        
        Console.WriteLine("    方式1: AddZookeeper(options => { ... })");
        Console.WriteLine("      options.ConnectionString = \"localhost:2181\"");
        Console.WriteLine("      options.RootPath = \"/myapp/config\"");

        // 方式2: 使用简化方法
        var builder2 = new CfgBuilder()
            .AddZookeeper("localhost:2181", rootPath: "/myapp/config", level: 0);
        
        Console.WriteLine("    方式2: AddZookeeper(\"localhost:2181\", rootPath: \"/myapp/config\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 15.2: 集群配置（多节点）
    /// </summary>
    private static void Demo_ClusterConfiguration()
    {
        Console.WriteLine("--- 示例 15.2: 集群配置 ---");
        
        // Zookeeper 集群通常有多个节点
        var builder = new CfgBuilder()
            .AddZookeeper(options =>
            {
                // 多节点连接字符串，用逗号分隔
                options.ConnectionString = "zk1.example.com:2181,zk2.example.com:2181,zk3.example.com:2181";
                options.RootPath = "/myapp/config";
                options.SessionTimeout = TimeSpan.FromSeconds(30);
                options.ConnectTimeout = TimeSpan.FromSeconds(10);
                options.ReconnectInterval = TimeSpan.FromSeconds(5);
            }, level: 0);
        
        Console.WriteLine("    options.ConnectionString = \"zk1:2181,zk2:2181,zk3:2181\"");
        Console.WriteLine("    options.SessionTimeout = TimeSpan.FromSeconds(30)");
        Console.WriteLine("    options.ConnectTimeout = TimeSpan.FromSeconds(10)");
        Console.WriteLine("    options.ReconnectInterval = TimeSpan.FromSeconds(5)");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 15.3: 认证配置
    /// </summary>
    private static void Demo_AuthConfiguration()
    {
        Console.WriteLine("--- 示例 15.3: 认证配置 ---");
        
        var builder = new CfgBuilder()
            .AddZookeeper(options =>
            {
                options.ConnectionString = "localhost:2181";
                options.RootPath = "/myapp/config";
                options.AuthScheme = "digest";           // 认证方案
                options.AuthInfo = "user:password";      // 认证信息
            }, level: 0);
        
        Console.WriteLine("    options.AuthScheme = \"digest\"");
        Console.WriteLine("    options.AuthInfo = \"user:password\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 15.4: 不同数据格式
    /// </summary>
    private static void Demo_DataFormatConfiguration()
    {
        Console.WriteLine("--- 示例 15.4: 不同数据格式 ---");
        
        // KeyValue 格式（默认）- 每个 ZNode 对应一个配置项
        var builder1 = new CfgBuilder()
            .AddZookeeper(options =>
            {
                options.ConnectionString = "localhost:2181";
                options.RootPath = "/myapp/config";
                options.DataFormat = ZookeeperDataFormat.KeyValue;
            }, level: 0);
        
        Console.WriteLine("    KeyValue: options.DataFormat = ZookeeperDataFormat.KeyValue");
        Console.WriteLine("      // 节点路径即为配置键，如 /myapp/config/Database/Host");

        // JSON 格式 - 单个节点存储 JSON 配置
        var builder2 = new CfgBuilder()
            .AddZookeeper(options =>
            {
                options.ConnectionString = "localhost:2181";
                options.RootPath = "/myapp";
                options.DataFormat = ZookeeperDataFormat.Json;
                options.SingleNode = "config";  // 读取 /myapp/config 节点
            }, level: 0);
        
        Console.WriteLine("    JSON: options.DataFormat = ZookeeperDataFormat.Json");
        Console.WriteLine("      options.SingleNode = \"config\"  // 读取 /myapp/config 节点");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 15.5: 监听配置变更
    /// </summary>
    private static void Demo_WatchConfiguration()
    {
        Console.WriteLine("--- 示例 15.5: 监听配置变更 ---");
        
        var builder = new CfgBuilder()
            .AddZookeeper(options =>
            {
                options.ConnectionString = "localhost:2181";
                options.RootPath = "/myapp/config";
                options.EnableHotReload = true;  // 启用热重载（默认 true）
            }, level: 0);
        
        Console.WriteLine("    options.EnableHotReload = true");
        Console.WriteLine();
        Console.WriteLine("    // 订阅配置变更（使用 Zookeeper Watch 机制）");
        Console.WriteLine("    cfg.ConfigChanges.Subscribe(change =>");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.WriteLine($\"配置变更: {change.Key}\");");
        Console.WriteLine("        Console.WriteLine($\"  旧值: {change.OldValue}\");");
        Console.WriteLine("        Console.WriteLine($\"  新值: {change.NewValue}\");");
        Console.WriteLine("    });");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 15.6: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 15.6: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddZookeeper(options =>
                {
                    options.ConnectionString = "localhost:2181";
                    options.RootPath = "/apq-cfg-demo";
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    ✓ 连接 Zookeeper 成功");

            // 写入测试配置
            cfg.Set("App:Name", "ZookeeperDemo");
            cfg.Set("App:Version", "1.0.0");
            cfg.Set("Database:Host", "localhost");
            cfg.Set("Database:Port", "5432");
            await cfg.SaveAsync();
            Console.WriteLine("    ✓ 写入配置成功");

            // 读取配置
            Console.WriteLine($"    App:Name = {cfg.Get("App:Name")}");
            Console.WriteLine($"    App:Version = {cfg.Get("App:Version")}");
            Console.WriteLine($"    Database:Host = {cfg.Get("Database:Host")}");
            Console.WriteLine($"    Database:Port = {cfg.GetValue<int>("Database:Port")}");

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
            Console.WriteLine("    提示: 请确保 Zookeeper 服务正在运行");
        }
        
        Console.WriteLine();
    }
}
