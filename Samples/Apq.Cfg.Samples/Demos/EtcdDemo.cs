using Apq.Cfg;
using Apq.Cfg.Etcd;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 12: Etcd 配置中心
/// 演示如何使用 Etcd 作为配置源
/// </summary>
public static class EtcdDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 12: Etcd 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Etcd 服务
        // 可以使用 Docker 快速启动：docker run -d -p 2379:2379 -p 2380:2380 \
        //   --name etcd quay.io/coreos/etcd:v3.5.0 \
        //   /usr/local/bin/etcd --advertise-client-urls http://0.0.0.0:2379 \
        //   --listen-client-urls http://0.0.0.0:2379
        
        Console.WriteLine("【Etcd 配置源说明】");
        Console.WriteLine("Etcd 是一个分布式键值存储系统，常用于服务发现和配置管理。");
        Console.WriteLine("Apq.Cfg.Etcd 支持从 Etcd v3 API 读取配置。\n");

        // 示例 1: 基本配置
        Console.WriteLine("--- 示例 12.1: 基本配置 ---");
        ShowBasicConfiguration();

        // 示例 2: 集群配置
        Console.WriteLine("\n--- 示例 12.2: 集群配置 ---");
        ShowClusterConfiguration();

        // 示例 3: 认证配置
        Console.WriteLine("\n--- 示例 12.3: 认证配置 ---");
        ShowAuthConfiguration();

        // 示例 4: TLS/mTLS 配置
        Console.WriteLine("\n--- 示例 12.4: TLS/mTLS 配置 ---");
        ShowTlsConfiguration();

        // 示例 5: 监听配置变更
        Console.WriteLine("\n--- 示例 12.5: 监听配置变更 ---");
        ShowWatchConfiguration();

        // 示例 6: 实际连接测试
        Console.WriteLine("\n--- 示例 12.6: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 12 完成\n");
    }

    private static void ShowBasicConfiguration()
    {
        Console.WriteLine("基本配置代码示例：");
        Console.WriteLine(@"
// 使用 Action 配置
var cfg = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""localhost:2379"" };  // Etcd 端点
        options.KeyPrefix = ""/myapp/config/"";            // 键前缀
    }, level: 0)
    .Build();

// 或使用简化方法（单端点）
var cfg2 = new CfgBuilder()
    .AddEtcd(""localhost:2379"", keyPrefix: ""/myapp/config/"")
    .Build();

// 读取配置
var host = cfg.Get(""Database:Host"");
");
    }

    private static void ShowClusterConfiguration()
    {
        Console.WriteLine("集群配置（多端点）：");
        Console.WriteLine(@"
// Etcd 集群通常有多个节点，配置多个端点可提高可用性
var cfg = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[]
        {
            ""etcd1.example.com:2379"",
            ""etcd2.example.com:2379"",
            ""etcd3.example.com:2379""
        };
        options.KeyPrefix = ""/myapp/config/"";
        options.ConnectTimeout = TimeSpan.FromSeconds(10);
        options.ReconnectInterval = TimeSpan.FromSeconds(5);
    }, level: 0)
    .Build();

// 或使用简化方法（多端点）
var cfg2 = new CfgBuilder()
    .AddEtcd(
        new[] { ""etcd1:2379"", ""etcd2:2379"", ""etcd3:2379"" },
        keyPrefix: ""/myapp/config/"")
    .Build();
");
    }

    private static void ShowAuthConfiguration()
    {
        Console.WriteLine("用户名密码认证：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""localhost:2379"" };
        options.KeyPrefix = ""/myapp/config/"";
        options.Username = ""root"";           // 用户名
        options.Password = ""your-password"";  // 密码
    }, level: 0)
    .Build();
");
    }

    private static void ShowTlsConfiguration()
    {
        Console.WriteLine("TLS/mTLS 安全连接：");
        Console.WriteLine(@"
// TLS 连接（仅验证服务器证书）
var cfg1 = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""etcd.example.com:2379"" };
        options.KeyPrefix = ""/myapp/config/"";
        options.CaCertPath = ""/path/to/ca.crt"";  // CA 证书
    }, level: 0)
    .Build();

// mTLS 连接（双向认证）
var cfg2 = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""etcd.example.com:2379"" };
        options.KeyPrefix = ""/myapp/config/"";
        options.CaCertPath = ""/path/to/ca.crt"";          // CA 证书
        options.ClientCertPath = ""/path/to/client.crt"";  // 客户端证书
        options.ClientKeyPath = ""/path/to/client.key"";   // 客户端私钥
    }, level: 0)
    .Build();
");
    }

    private static void ShowWatchConfiguration()
    {
        Console.WriteLine("监听配置变更：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""localhost:2379"" };
        options.KeyPrefix = ""/myapp/config/"";
        options.EnableHotReload = true;  // 启用热重载（默认 true）
    }, level: 0)
    .Build();

// 订阅配置变更（使用 Etcd Watch API）
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($""配置变更: {change.Key}"");
    Console.WriteLine($""  旧值: {change.OldValue}"");
    Console.WriteLine($""  新值: {change.NewValue}"");
    Console.WriteLine($""  类型: {change.ChangeType}"");
});

// 不同数据格式
var cfgJson = new CfgBuilder()
    .AddEtcd(options =>
    {
        options.Endpoints = new[] { ""localhost:2379"" };
        options.KeyPrefix = ""/myapp/"";
        options.DataFormat = EtcdDataFormat.Json;  // JSON 格式
        options.SingleKey = ""config"";            // 读取 /myapp/config
    }, level: 0)
    .Build();
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Etcd...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddEtcd(options =>
                {
                    options.Endpoints = new[] { "localhost:2379" };
                    options.KeyPrefix = "/apq-cfg-demo/";
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
            Console.WriteLine("  提示: 请确保 Etcd 服务正在运行");
            Console.WriteLine("  快速启动: docker run -d -p 2379:2379 quay.io/coreos/etcd:v3.5.0 \\");
            Console.WriteLine("    /usr/local/bin/etcd --advertise-client-urls http://0.0.0.0:2379 \\");
            Console.WriteLine("    --listen-client-urls http://0.0.0.0:2379");
        }
        
        await Task.CompletedTask;
    }
}
