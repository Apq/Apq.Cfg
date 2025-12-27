using Apq.Cfg;
using Apq.Cfg.Nacos;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 13: Nacos 配置中心
/// 演示如何使用 Nacos 作为配置源
/// </summary>
public static class NacosDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 13: Nacos 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Nacos 服务
        // 可以使用 Docker 快速启动：
        // docker run -d -p 8848:8848 -e MODE=standalone nacos/nacos-server:latest
        
        Console.WriteLine("【Nacos 配置源说明】");
        Console.WriteLine("Nacos 是阿里巴巴开源的服务发现和配置管理平台。");
        Console.WriteLine("Apq.Cfg.Nacos 支持从 Nacos 配置中心读取配置。\n");

        // 示例 1: 基本配置
        Console.WriteLine("--- 示例 13.1: 基本配置 ---");
        ShowBasicConfiguration();

        // 示例 2: 命名空间和分组
        Console.WriteLine("\n--- 示例 13.2: 命名空间和分组 ---");
        ShowNamespaceConfiguration();

        // 示例 3: 认证配置
        Console.WriteLine("\n--- 示例 13.3: 认证配置 ---");
        ShowAuthConfiguration();

        // 示例 4: 阿里云 MSE 配置
        Console.WriteLine("\n--- 示例 13.4: 阿里云 MSE 配置 ---");
        ShowAliyunMseConfiguration();

        // 示例 5: 多配置源
        Console.WriteLine("\n--- 示例 13.5: 多配置源 ---");
        ShowMultipleDataIdConfiguration();

        // 示例 6: 不同数据格式
        Console.WriteLine("\n--- 示例 13.6: 不同数据格式 ---");
        ShowDataFormatConfiguration();

        // 示例 7: 实际连接测试
        Console.WriteLine("\n--- 示例 13.7: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 13 完成\n");
    }

    private static void ShowBasicConfiguration()
    {
        Console.WriteLine("基本配置代码示例：");
        Console.WriteLine(@"
// 使用 Action 配置
var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";  // Nacos 服务地址
        options.DataId = ""myapp-config"";             // 配置 DataId
        options.Group = ""DEFAULT_GROUP"";             // 配置分组
    }, level: 0)
    .Build();

// 或使用简化方法
var cfg2 = new CfgBuilder()
    .AddNacos(""localhost:8848"", dataId: ""myapp-config"")
    .Build();

// 读取配置
var host = cfg.Get(""Database:Host"");
");
    }

    private static void ShowNamespaceConfiguration()
    {
        Console.WriteLine("命名空间和分组配置：");
        Console.WriteLine(@"
// Nacos 支持命名空间隔离不同环境的配置
var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.Namespace = ""dev"";           // 命名空间 ID（开发环境）
        options.DataId = ""myapp-config"";
        options.Group = ""MYAPP_GROUP"";       // 自定义分组
    }, level: 0)
    .Build();

// 生产环境配置
var cfgProd = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""nacos.prod.example.com:8848"";
        options.Namespace = ""prod"";          // 生产命名空间
        options.DataId = ""myapp-config"";
        options.Group = ""MYAPP_GROUP"";
    }, level: 0)
    .Build();
");
    }

    private static void ShowAuthConfiguration()
    {
        Console.WriteLine("用户名密码认证：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config"";
        options.Group = ""DEFAULT_GROUP"";
        options.Username = ""nacos"";          // 用户名
        options.Password = ""nacos"";          // 密码
        options.ConnectTimeoutMs = 10000;      // 连接超时（毫秒）
    }, level: 0)
    .Build();
");
    }

    private static void ShowAliyunMseConfiguration()
    {
        Console.WriteLine("阿里云 MSE（微服务引擎）配置：");
        Console.WriteLine(@"
// 使用阿里云 MSE 托管的 Nacos
var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""mse-xxx.nacos.mse.aliyuncs.com:8848"";
        options.Namespace = ""your-namespace-id"";
        options.DataId = ""myapp-config"";
        options.Group = ""DEFAULT_GROUP"";
        options.AccessKey = ""your-access-key"";   // 阿里云 AccessKey
        options.SecretKey = ""your-secret-key"";   // 阿里云 SecretKey
    }, level: 0)
    .Build();
");
    }

    private static void ShowMultipleDataIdConfiguration()
    {
        Console.WriteLine("多配置源（多 DataId）：");
        Console.WriteLine(@"
// 可以加载多个配置，实现配置的层级覆盖
var cfg = new CfgBuilder()
    // 公共配置（最低优先级）
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""common-config"";
        options.Group = ""DEFAULT_GROUP"";
    }, level: 0)
    // 应用配置（中等优先级）
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config"";
        options.Group = ""DEFAULT_GROUP"";
    }, level: 1)
    // 环境特定配置（最高优先级）
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config-dev"";
        options.Group = ""DEFAULT_GROUP"";
    }, level: 2)
    .Build();
");
    }

    private static void ShowDataFormatConfiguration()
    {
        Console.WriteLine("不同数据格式配置：");
        Console.WriteLine(@"
// 1. JSON 格式（默认）
var cfgJson = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config.json"";
        options.DataFormat = NacosDataFormat.Json;
    }, level: 0)
    .Build();

// 2. YAML 格式
var cfgYaml = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config.yaml"";
        options.DataFormat = NacosDataFormat.Yaml;
    }, level: 0)
    .Build();

// 3. Properties 格式（key=value）
var cfgProps = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = ""localhost:8848"";
        options.DataId = ""myapp-config.properties"";
        options.DataFormat = NacosDataFormat.Properties;
    }, level: 0)
    .Build();
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Nacos...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddNacos(options =>
                {
                    options.ServerAddresses = "localhost:8848";
                    options.DataId = "apq-cfg-demo";
                    options.Group = "DEFAULT_GROUP";
                    options.ConnectTimeoutMs = 3000;
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
            Console.WriteLine("  提示: 请确保 Nacos 服务正在运行");
            Console.WriteLine("  快速启动: docker run -d -p 8848:8848 -e MODE=standalone nacos/nacos-server:latest");
            Console.WriteLine("  控制台: http://localhost:8848/nacos (用户名/密码: nacos/nacos)");
        }
        
        await Task.CompletedTask;
    }
}
