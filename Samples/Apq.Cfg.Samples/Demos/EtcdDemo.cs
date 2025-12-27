#pragma warning disable CS0162 // 检测到无法访问的代码

using Apq.Cfg;
using Apq.Cfg.Etcd;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 12: Etcd 配置中心
/// 演示如何使用 Etcd 作为配置源
/// 注意：需要运行 Etcd 服务才能执行完整示例
/// 快速启动：docker run -d -p 2379:2379 -p 2380:2380 --name etcd quay.io/coreos/etcd:latest /usr/local/bin/etcd --advertise-client-urls http://0.0.0.0:2379 --listen-client-urls http://0.0.0.0:2379
/// </summary>
public static class EtcdDemo
{
    // 是否启用实际连接测试（设为 true 需要 Etcd 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 12: Etcd 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Etcd 是一个分布式键值存储系统，常用于服务发现和配置管理。");
        Console.WriteLine("【启动】docker run -d -p 2379:2379 quay.io/coreos/etcd:latest\n");

        // 示例 12.1: 基本配置
        Demo_BasicConfiguration();

        // 示例 12.2: 集群配置
        Demo_ClusterConfiguration();

        // 示例 12.3: 认证配置
        Demo_AuthConfiguration();

        // 示例 12.4: TLS/mTLS 配置
        Demo_TlsConfiguration();

        // 示例 12.5: 监听配置变更
        Demo_WatchConfiguration();

        // 示例 12.6: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 12.6: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Etcd 服务后可运行\n");
        }

        Console.WriteLine("[示例 12 完成]\n");
    }

    /// <summary>
    /// 示例 12.1: 基本配置
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 12.1: 基本配置 ---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[] { "localhost:2379" };  // Etcd 端点
                options.KeyPrefix = "/myapp/config/";            // 键前缀
            }, level: 0);
        
        Console.WriteLine("    方式1: AddEtcd(options => { ... })");
        Console.WriteLine("      options.Endpoints = new[] { \"localhost:2379\" }");
        Console.WriteLine("      options.KeyPrefix = \"/myapp/config/\"");

        // 方式2: 使用简化方法
        var builder2 = new CfgBuilder()
            .AddEtcd("localhost:2379", keyPrefix: "/myapp/config/", level: 0);
        
        Console.WriteLine("    方式2: AddEtcd(\"localhost:2379\", keyPrefix: \"/myapp/config/\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 12.2: 集群配置（多端点）
    /// </summary>
    private static void Demo_ClusterConfiguration()
    {
        Console.WriteLine("--- 示例 12.2: 集群配置 ---");
        
        // Etcd 集群通常有多个节点，配置多个端点可提高可用性
        var builder = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[]
                {
                    "etcd1.example.com:2379",
                    "etcd2.example.com:2379",
                    "etcd3.example.com:2379"
                };
                options.KeyPrefix = "/myapp/config/";
                options.ConnectTimeout = TimeSpan.FromSeconds(10);
                options.ReconnectInterval = TimeSpan.FromSeconds(5);
            }, level: 0);
        
        Console.WriteLine("    options.Endpoints = new[] { \"etcd1:2379\", \"etcd2:2379\", \"etcd3:2379\" }");
        Console.WriteLine("    options.ConnectTimeout = TimeSpan.FromSeconds(10)");
        Console.WriteLine("    options.ReconnectInterval = TimeSpan.FromSeconds(5)");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 12.3: 用户名密码认证
    /// </summary>
    private static void Demo_AuthConfiguration()
    {
        Console.WriteLine("--- 示例 12.3: 用户名密码认证 ---");
        
        var builder = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[] { "localhost:2379" };
                options.KeyPrefix = "/myapp/config/";
                options.Username = "root";           // 用户名
                options.Password = "your-password";  // 密码
            }, level: 0);
        
        Console.WriteLine("    options.Username = \"root\"");
        Console.WriteLine("    options.Password = \"your-password\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 12.4: TLS/mTLS 安全连接
    /// </summary>
    private static void Demo_TlsConfiguration()
    {
        Console.WriteLine("--- 示例 12.4: TLS/mTLS 安全连接 ---");
        
        // TLS 连接（仅验证服务器证书）
        var builder1 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[] { "etcd.example.com:2379" };
                options.KeyPrefix = "/myapp/config/";
                options.CaCertPath = "/path/to/ca.crt";  // CA 证书
            }, level: 0);
        
        Console.WriteLine("    TLS 连接:");
        Console.WriteLine("      options.CaCertPath = \"/path/to/ca.crt\"");

        // mTLS 连接（双向认证）
        var builder2 = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[] { "etcd.example.com:2379" };
                options.KeyPrefix = "/myapp/config/";
                options.CaCertPath = "/path/to/ca.crt";          // CA 证书
                options.ClientCertPath = "/path/to/client.crt";  // 客户端证书
                options.ClientKeyPath = "/path/to/client.key";   // 客户端私钥
            }, level: 0);
        
        Console.WriteLine("    mTLS 连接:");
        Console.WriteLine("      options.CaCertPath = \"/path/to/ca.crt\"");
        Console.WriteLine("      options.ClientCertPath = \"/path/to/client.crt\"");
        Console.WriteLine("      options.ClientKeyPath = \"/path/to/client.key\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 12.5: 监听配置变更
    /// </summary>
    private static void Demo_WatchConfiguration()
    {
        Console.WriteLine("--- 示例 12.5: 监听配置变更 ---");
        
        var builder = new CfgBuilder()
            .AddEtcd(options =>
            {
                options.Endpoints = new[] { "localhost:2379" };
                options.KeyPrefix = "/myapp/config/";
                options.EnableHotReload = true;  // 启用热重载（默认 true）
            }, level: 0);
        
        Console.WriteLine("    options.EnableHotReload = true");
        Console.WriteLine();
        Console.WriteLine("    // 订阅配置变更（使用 Etcd Watch API）");
        Console.WriteLine("    cfg.ConfigChanges.Subscribe(change =>");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.WriteLine($\"配置变更: {change.Key}\");");
        Console.WriteLine("        Console.WriteLine($\"  旧值: {change.OldValue}\");");
        Console.WriteLine("        Console.WriteLine($\"  新值: {change.NewValue}\");");
        Console.WriteLine("    });");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 12.6: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 12.6: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddEtcd(options =>
                {
                    options.Endpoints = new[] { "localhost:2379" };
                    options.KeyPrefix = "/apq-cfg-demo/";
                    options.ConnectTimeout = TimeSpan.FromSeconds(3);
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    ✓ 连接 Etcd 成功");

            // 写入测试配置
            cfg.Set("App:Name", "EtcdDemo");
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
            Console.WriteLine("    提示: 请确保 Etcd 服务正在运行");
        }
        
        Console.WriteLine();
    }
}
