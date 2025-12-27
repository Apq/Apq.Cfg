using Apq.Cfg;
using Apq.Cfg.Nacos;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 13: Nacos 配置中心
/// 演示如何使用 Nacos 作为配置源
/// 注意：需要运行 Nacos 服务才能执行完整示例
/// 快速启动：docker run -d -p 8848:8848 -e MODE=standalone nacos/nacos-server:latest
/// </summary>
public static class NacosDemo
{
    // 是否启用实际连接测试（设为 true 需要 Nacos 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 13: Nacos 配置中心");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Nacos 是阿里巴巴开源的服务发现和配置管理平台。");
        Console.WriteLine("【启动】docker run -d -p 8848:8848 -e MODE=standalone nacos/nacos-server\n");

        // 示例 13.1: 基本配置
        Demo_BasicConfiguration();

        // 示例 13.2: 命名空间和分组
        Demo_NamespaceConfiguration();

        // 示例 13.3: 认证配置
        Demo_AuthConfiguration();

        // 示例 13.4: 阿里云 MSE 配置
        Demo_AliyunMseConfiguration();

        // 示例 13.5: 多配置源
        Demo_MultiSourceConfiguration();

        // 示例 13.6: 不同数据格式
        Demo_DataFormatConfiguration();

        // 示例 13.7: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 13.7: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Nacos 服务后可运行\n");
        }

        Console.WriteLine("[示例 13 完成]\n");
    }

    /// <summary>
    /// 示例 13.1: 基本配置
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 13.1: 基本配置 ---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";  // Nacos 服务地址
                options.DataId = "myapp-config";             // 配置 DataId
                options.Group = "DEFAULT_GROUP";             // 配置分组
            }, level: 0);
        
        Console.WriteLine("    方式1: AddNacos(options => { ... })");
        Console.WriteLine("      options.ServerAddresses = \"localhost:8848\"");
        Console.WriteLine("      options.DataId = \"myapp-config\"");
        Console.WriteLine("      options.Group = \"DEFAULT_GROUP\"");

        // 方式2: 使用简化方法
        var builder2 = new CfgBuilder()
            .AddNacos("localhost:8848", dataId: "myapp-config", level: 0);
        
        Console.WriteLine("    方式2: AddNacos(\"localhost:8848\", dataId: \"myapp-config\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.2: 命名空间和分组配置
    /// </summary>
    private static void Demo_NamespaceConfiguration()
    {
        Console.WriteLine("--- 示例 13.2: 命名空间和分组 ---");
        
        // Nacos 支持命名空间隔离不同环境的配置
        var builder = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.Namespace = "dev";           // 命名空间 ID（开发环境）
                options.DataId = "myapp-config";
                options.Group = "MYAPP_GROUP";       // 自定义分组
            }, level: 0);
        
        Console.WriteLine("    options.Namespace = \"dev\"  // 命名空间 ID");
        Console.WriteLine("    options.Group = \"MYAPP_GROUP\"  // 自定义分组");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.3: 用户名密码认证
    /// </summary>
    private static void Demo_AuthConfiguration()
    {
        Console.WriteLine("--- 示例 13.3: 用户名密码认证 ---");
        
        var builder = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config";
                options.Group = "DEFAULT_GROUP";
                options.Username = "nacos";          // 用户名
                options.Password = "nacos";          // 密码
                options.ConnectTimeoutMs = 10000;    // 连接超时（毫秒）
            }, level: 0);
        
        Console.WriteLine("    options.Username = \"nacos\"");
        Console.WriteLine("    options.Password = \"nacos\"");
        Console.WriteLine("    options.ConnectTimeoutMs = 10000");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.4: 阿里云 MSE（微服务引擎）配置
    /// </summary>
    private static void Demo_AliyunMseConfiguration()
    {
        Console.WriteLine("--- 示例 13.4: 阿里云 MSE 配置 ---");
        
        // 使用阿里云 MSE 托管的 Nacos
        var builder = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "mse-xxx.nacos.mse.aliyuncs.com:8848";
                options.Namespace = "your-namespace-id";
                options.DataId = "myapp-config";
                options.Group = "DEFAULT_GROUP";
                options.AccessKey = "your-access-key";   // 阿里云 AccessKey
                options.SecretKey = "your-secret-key";   // 阿里云 SecretKey
            }, level: 0);
        
        Console.WriteLine("    options.ServerAddresses = \"mse-xxx.nacos.mse.aliyuncs.com:8848\"");
        Console.WriteLine("    options.AccessKey = \"your-access-key\"");
        Console.WriteLine("    options.SecretKey = \"your-secret-key\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.5: 多配置源（多 DataId）
    /// </summary>
    private static void Demo_MultiSourceConfiguration()
    {
        Console.WriteLine("--- 示例 13.5: 多配置源 ---");
        
        // 可以加载多个配置，实现配置的层级覆盖
        var builder = new CfgBuilder()
            // 公共配置（最低优先级）
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "common-config";
                options.Group = "DEFAULT_GROUP";
            }, level: 0)
            // 应用配置（中等优先级）
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config";
                options.Group = "DEFAULT_GROUP";
            }, level: 1)
            // 环境特定配置（最高优先级）
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config-dev";
                options.Group = "DEFAULT_GROUP";
            }, level: 2);
        
        Console.WriteLine("    level: 0 - common-config (公共配置)");
        Console.WriteLine("    level: 1 - myapp-config (应用配置)");
        Console.WriteLine("    level: 2 - myapp-config-dev (环境配置) - 最高优先级");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.6: 不同数据格式
    /// </summary>
    private static void Demo_DataFormatConfiguration()
    {
        Console.WriteLine("--- 示例 13.6: 不同数据格式 ---");
        
        // JSON 格式（默认）
        var builder1 = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config.json";
                options.DataFormat = NacosDataFormat.Json;
            }, level: 0);
        
        Console.WriteLine("    JSON: options.DataFormat = NacosDataFormat.Json");

        // YAML 格式
        var builder2 = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config.yaml";
                options.DataFormat = NacosDataFormat.Yaml;
            }, level: 0);
        
        Console.WriteLine("    YAML: options.DataFormat = NacosDataFormat.Yaml");

        // Properties 格式（key=value）
        var builder3 = new CfgBuilder()
            .AddNacos(options =>
            {
                options.ServerAddresses = "localhost:8848";
                options.DataId = "myapp-config.properties";
                options.DataFormat = NacosDataFormat.Properties;
            }, level: 0);
        
        Console.WriteLine("    Properties: options.DataFormat = NacosDataFormat.Properties");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 13.7: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 13.7: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddNacos(options =>
                {
                    options.ServerAddresses = "localhost:8848";
                    options.DataId = "apq-cfg-demo";
                    options.Group = "DEFAULT_GROUP";
                    options.ConnectTimeoutMs = 3000;
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    ✓ 连接 Nacos 成功");

            // 写入测试配置
            cfg.Set("App:Name", "NacosDemo");
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

            cfg.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ✗ 连接失败: {ex.Message}");
            Console.WriteLine("    提示: 请确保 Nacos 服务正在运行");
        }
        
        Console.WriteLine();
    }
}
