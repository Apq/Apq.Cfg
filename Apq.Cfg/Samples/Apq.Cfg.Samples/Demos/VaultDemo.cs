#pragma warning disable CS0162 // 检测到无法访问的代码

using Apq.Cfg;
using Apq.Cfg.Vault;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 16: HashiCorp Vault 配置源
/// 演示如何使用 Vault 作为配置源（密钥管理）
/// 注意：需要运行 Vault 服务才能执行完整示例
/// 快速启动：docker run -d -p 8200:8200 -e VAULT_DEV_ROOT_TOKEN_ID=root vault:latest
/// </summary>
public static class VaultDemo
{
    // 是否启用实际连接测试（设为 true 需要 Vault 服务运行）
    private const bool EnableLiveTest = false;
    
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 16: HashiCorp Vault 配置源");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【说明】Vault 是 HashiCorp 开源的密钥管理和数据保护工具。");
        Console.WriteLine("【启动】docker run -d -p 8200:8200 -e VAULT_DEV_ROOT_TOKEN_ID=root vault\n");

        // 示例 16.1: 基本配置（Token 认证）
        Demo_BasicConfiguration();

        // 示例 16.2: UserPass 认证
        Demo_UserPassConfiguration();

        // 示例 16.3: AppRole 认证
        Demo_AppRoleConfiguration();

        // 示例 16.4: KV 引擎版本
        Demo_KvVersionConfiguration();

        // 示例 16.5: 企业版命名空间
        Demo_NamespaceConfiguration();

        // 示例 16.6: 监听配置变更
        Demo_WatchConfiguration();

        // 示例 16.7: 实际连接测试
        if (EnableLiveTest)
        {
            await Demo_LiveTestAsync();
        }
        else
        {
            Console.WriteLine("--- 示例 16.7: 实际连接测试 ---");
            Console.WriteLine("    [跳过] 设置 EnableLiveTest = true 并启动 Vault 服务后可运行\n");
        }

        Console.WriteLine("[示例 16 完成]\n");
    }

    /// <summary>
    /// 示例 16.1: 基本配置（Token 认证）
    /// </summary>
    private static void Demo_BasicConfiguration()
    {
        Console.WriteLine("--- 示例 16.1: 基本配置（Token 认证）---");
        
        // 方式1: 使用 Action 配置
        var builder1 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";  // Vault 地址
                options.Token = "root";                     // 认证令牌
                options.EnginePath = "secret";              // 密钥引擎路径
                options.Path = "myapp/config";              // 密钥路径
            }, level: 0);
        
        Console.WriteLine("    方式1: AddVault(options => { ... })");
        Console.WriteLine("      options.Address = \"http://localhost:8200\"");
        Console.WriteLine("      options.Token = \"root\"");
        Console.WriteLine("      options.EnginePath = \"secret\"");
        Console.WriteLine("      options.Path = \"myapp/config\"");

        // 方式2: 使用简化方法（KV V2）
        var builder2 = new CfgBuilder()
            .AddVaultV2("http://localhost:8200", "root", path: "myapp/config", level: 0);
        
        Console.WriteLine("    方式2: AddVaultV2(address, token, path: \"myapp/config\")");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.2: UserPass 认证
    /// </summary>
    private static void Demo_UserPassConfiguration()
    {
        Console.WriteLine("--- 示例 16.2: UserPass 认证 ---");
        
        var builder = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";
                options.AuthMethod = VaultAuthMethod.UserPass;  // 使用 UserPass 认证
                options.Username = "myuser";                    // 用户名
                options.Password = "mypassword";                // 密码
                options.EnginePath = "secret";
                options.Path = "myapp/config";
            }, level: 0);
        
        Console.WriteLine("    options.AuthMethod = VaultAuthMethod.UserPass");
        Console.WriteLine("    options.Username = \"myuser\"");
        Console.WriteLine("    options.Password = \"mypassword\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.3: AppRole 认证
    /// </summary>
    private static void Demo_AppRoleConfiguration()
    {
        Console.WriteLine("--- 示例 16.3: AppRole 认证 ---");
        
        var builder = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";
                options.AuthMethod = VaultAuthMethod.AppRole;   // 使用 AppRole 认证
                options.RoleId = "your-role-id";                // AppRole ID
                options.RoleSecret = "your-role-secret";        // AppRole Secret
                options.EnginePath = "secret";
                options.Path = "myapp/config";
            }, level: 0);
        
        Console.WriteLine("    options.AuthMethod = VaultAuthMethod.AppRole");
        Console.WriteLine("    options.RoleId = \"your-role-id\"");
        Console.WriteLine("    options.RoleSecret = \"your-role-secret\"");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.4: KV 引擎版本
    /// </summary>
    private static void Demo_KvVersionConfiguration()
    {
        Console.WriteLine("--- 示例 16.4: KV 引擎版本 ---");
        
        // KV V2（默认，支持版本控制）
        var builder1 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";
                options.Token = "root";
                options.EnginePath = "secret";
                options.Path = "myapp/config";
                options.KvVersion = 2;  // KV V2（默认）
            }, level: 0);
        
        Console.WriteLine("    KV V2: options.KvVersion = 2  // 支持版本控制");

        // KV V1（不支持版本控制）
        var builder2 = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";
                options.Token = "root";
                options.EnginePath = "kv";  // KV V1 引擎
                options.Path = "myapp/config";
                options.KvVersion = 1;  // KV V1
            }, level: 0);
        
        Console.WriteLine("    KV V1: options.KvVersion = 1  // 不支持版本控制");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.5: 企业版命名空间
    /// </summary>
    private static void Demo_NamespaceConfiguration()
    {
        Console.WriteLine("--- 示例 16.5: 企业版命名空间 ---");
        
        // Vault Enterprise 支持命名空间隔离
        var builder = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://vault.example.com:8200";
                options.Token = "your-token";
                options.Namespace = "myorg/myteam";  // 企业版命名空间
                options.EnginePath = "secret";
                options.Path = "myapp/config";
            }, level: 0);
        
        Console.WriteLine("    options.Namespace = \"myorg/myteam\"  // Vault Enterprise");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.6: 监听配置变更
    /// </summary>
    private static void Demo_WatchConfiguration()
    {
        Console.WriteLine("--- 示例 16.6: 监听配置变更 ---");
        
        var builder = new CfgBuilder()
            .AddVault(options =>
            {
                options.Address = "http://localhost:8200";
                options.Token = "root";
                options.EnginePath = "secret";
                options.Path = "myapp/config";
                options.EnableHotReload = true;                  // 启用热重载
                options.PollInterval = TimeSpan.FromSeconds(30); // 轮询间隔
                options.ReconnectInterval = TimeSpan.FromSeconds(5);
            }, level: 0);
        
        Console.WriteLine("    options.EnableHotReload = true");
        Console.WriteLine("    options.PollInterval = TimeSpan.FromSeconds(30)");
        Console.WriteLine("    options.ReconnectInterval = TimeSpan.FromSeconds(5)");
        Console.WriteLine();
        Console.WriteLine("    // 订阅配置变更");
        Console.WriteLine("    cfg.ConfigChanges.Subscribe(change =>");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.WriteLine($\"密钥变更: {change.Key} = {change.NewValue}\");");
        Console.WriteLine("    });");
        Console.WriteLine();
    }

    /// <summary>
    /// 示例 16.7: 实际连接测试
    /// </summary>
    private static async Task Demo_LiveTestAsync()
    {
        Console.WriteLine("--- 示例 16.7: 实际连接测试 ---");
        
        try
        {
            // 创建配置
            var cfg = new CfgBuilder()
                .AddVault(options =>
                {
                    options.Address = "http://localhost:8200";
                    options.Token = "root";  // 开发模式默认 token
                    options.EnginePath = "secret";
                    options.Path = "apq-cfg-demo";
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    ✓ 连接 Vault 成功");

            // 写入测试配置
            cfg.SetValue("App:Name", "VaultDemo");
            cfg.SetValue("App:Version", "1.0.0");
            cfg.SetValue("Database:Host", "localhost");
            cfg.SetValue("Database:Password", "secret-password");
            await cfg.SaveAsync();
            Console.WriteLine("    ✓ 写入密钥成功");

            // 读取配置
            Console.WriteLine($"    App:Name = {cfg["App:Name"]}");
            Console.WriteLine($"    App:Version = {cfg["App:Version"]}");
            Console.WriteLine($"    Database:Host = {cfg["Database:Host"]}");
            Console.WriteLine($"    Database:Password = {cfg["Database:Password"]}");

            // 枚举所有键
            var keys = cfg.GetChildKeys();
            Console.WriteLine($"    密钥数量: {keys.Count()}");

            // 清理测试数据
            cfg.Remove("App:Name");
            cfg.Remove("App:Version");
            cfg.Remove("Database:Host");
            cfg.Remove("Database:Password");
            await cfg.SaveAsync();
            Console.WriteLine("    ✓ 清理测试数据成功");

            cfg.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    ✗ 连接失败: {ex.Message}");
            Console.WriteLine("    提示: 请确保 Vault 服务正在运行");
        }
        
        Console.WriteLine();
    }
}
