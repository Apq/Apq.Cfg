using Apq.Cfg;
using Apq.Cfg.Vault;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 16: HashiCorp Vault 密钥管理
/// 演示如何使用 Vault 作为配置源
/// </summary>
public static class VaultDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 16: HashiCorp Vault 密钥管理");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // 注意：此示例需要运行中的 Vault 服务
        // 可以使用 Docker 快速启动：
        // docker run -d -p 8200:8200 -e VAULT_DEV_ROOT_TOKEN_ID=myroot vault
        
        Console.WriteLine("【Vault 配置源说明】");
        Console.WriteLine("HashiCorp Vault 是一个密钥管理和数据保护工具。");
        Console.WriteLine("Apq.Cfg.Vault 支持从 Vault KV 引擎读取配置和密钥。\n");

        // 示例 1: Token 认证
        Console.WriteLine("--- 示例 16.1: Token 认证 ---");
        ShowTokenAuthConfiguration();

        // 示例 2: KV V1 vs V2
        Console.WriteLine("\n--- 示例 16.2: KV V1 vs V2 ---");
        ShowKvVersionConfiguration();

        // 示例 3: UserPass 认证
        Console.WriteLine("\n--- 示例 16.3: UserPass 认证 ---");
        ShowUserPassAuthConfiguration();

        // 示例 4: AppRole 认证
        Console.WriteLine("\n--- 示例 16.4: AppRole 认证 ---");
        ShowAppRoleAuthConfiguration();

        // 示例 5: 企业版命名空间
        Console.WriteLine("\n--- 示例 16.5: 企业版命名空间 ---");
        ShowNamespaceConfiguration();

        // 示例 6: 热重载
        Console.WriteLine("\n--- 示例 16.6: 热重载 ---");
        ShowHotReloadConfiguration();

        // 示例 7: 实际连接测试
        Console.WriteLine("\n--- 示例 16.7: 连接测试 ---");
        await TestConnectionAsync();

        Console.WriteLine("\n示例 16 完成\n");
    }

    private static void ShowTokenAuthConfiguration()
    {
        Console.WriteLine("Token 认证配置：");
        Console.WriteLine(@"
// 使用 Action 配置
var cfg = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://localhost:8200"";  // Vault 地址
        options.Token = ""your-vault-token"";         // Vault Token
        options.EnginePath = ""secret"";              // KV 引擎路径
        options.Path = ""myapp/config"";              // 密钥路径
        options.KvVersion = 2;                        // KV 版本（1 或 2）
    })
    .Build();

// 读取密钥
var dbPassword = cfg.Get(""Database:Password"");
var apiKey = cfg.Get(""ApiKey"");
");
    }

    private static void ShowKvVersionConfiguration()
    {
        Console.WriteLine("KV V1 和 V2 引擎：");
        Console.WriteLine(@"
// KV V1 引擎（简化方法）
var cfgV1 = new CfgBuilder()
    .AddVaultV1(
        address: ""http://localhost:8200"",
        token: ""your-token"",
        enginePath: ""kv"",
        path: ""myapp/config"")
    .Build();

// KV V2 引擎（简化方法，推荐）
var cfgV2 = new CfgBuilder()
    .AddVaultV2(
        address: ""http://localhost:8200"",
        token: ""your-token"",
        enginePath: ""secret"",
        path: ""myapp/config"")
    .Build();

// KV V2 支持版本控制，可以查看密钥的历史版本
// 在 Vault CLI 中：vault kv get -version=1 secret/myapp/config
");
    }

    private static void ShowUserPassAuthConfiguration()
    {
        Console.WriteLine("UserPass 认证：");
        Console.WriteLine(@"
// 使用用户名密码认证
var cfg = new CfgBuilder()
    .AddVaultUserPass(
        address: ""http://localhost:8200"",
        username: ""myuser"",
        password: ""mypassword"",
        enginePath: ""secret"",
        path: ""myapp/config"")
    .Build();

// 或使用 Action 配置
var cfg2 = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://localhost:8200"";
        options.AuthMethod = VaultAuthMethod.UserPass;
        options.Username = ""myuser"";
        options.Password = ""mypassword"";
        options.EnginePath = ""secret"";
        options.Path = ""myapp/config"";
    })
    .Build();
");
    }

    private static void ShowAppRoleAuthConfiguration()
    {
        Console.WriteLine("AppRole 认证（推荐用于生产环境）：");
        Console.WriteLine(@"
// AppRole 是 Vault 推荐的机器认证方式
var cfg = new CfgBuilder()
    .AddVaultAppRole(
        address: ""http://localhost:8200"",
        roleId: ""your-role-id"",
        roleSecret: ""your-role-secret"",
        enginePath: ""secret"",
        path: ""myapp/config"")
    .Build();

// 或使用 Action 配置
var cfg2 = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://localhost:8200"";
        options.AuthMethod = VaultAuthMethod.AppRole;
        options.RoleId = ""your-role-id"";
        options.RoleSecret = ""your-role-secret"";
        options.EnginePath = ""secret"";
        options.Path = ""myapp/config"";
    })
    .Build();

// 在 Vault 中创建 AppRole：
// vault auth enable approle
// vault write auth/approle/role/myapp policies=""myapp-policy""
// vault read auth/approle/role/myapp/role-id
// vault write -f auth/approle/role/myapp/secret-id
");
    }

    private static void ShowNamespaceConfiguration()
    {
        Console.WriteLine("企业版命名空间配置：");
        Console.WriteLine(@"
// Vault Enterprise 支持命名空间隔离
var cfg = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://vault.example.com:8200"";
        options.Token = ""your-token"";
        options.Namespace = ""myteam"";           // 企业版命名空间
        options.EnginePath = ""secret"";
        options.Path = ""myapp/config"";
    })
    .Build();

// 多层级配置（不同命名空间）
var cfgMulti = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://vault.example.com:8200"";
        options.Token = ""your-token"";
        options.Namespace = ""shared"";           // 共享命名空间
        options.EnginePath = ""secret"";
        options.Path = ""common/config"";
    }, level: 0)
    .AddVault(options =>
    {
        options.Address = ""http://vault.example.com:8200"";
        options.Token = ""your-token"";
        options.Namespace = ""myteam"";           // 团队命名空间
        options.EnginePath = ""secret"";
        options.Path = ""myapp/config"";
    }, level: 1)
    .Build();
");
    }

    private static void ShowHotReloadConfiguration()
    {
        Console.WriteLine("热重载配置：");
        Console.WriteLine(@"
var cfg = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = ""http://localhost:8200"";
        options.Token = ""your-token"";
        options.EnginePath = ""secret"";
        options.Path = ""myapp/config"";
        options.EnableHotReload = true;              // 启用热重载
        options.PollInterval = TimeSpan.FromSeconds(30); // 轮询间隔
        options.ReconnectInterval = TimeSpan.FromSeconds(5); // 重连间隔
    })
    .Build();

// 订阅配置变更
cfg.ConfigChanges.Subscribe(change =>
{
    Console.WriteLine($""密钥变更: {change.Key}"");
    // 注意：出于安全考虑，不要打印密钥值
});

// 注意：Vault KV 引擎不支持原生 Watch，
// Apq.Cfg.Vault 使用轮询方式检测变更
");
    }

    private static async Task TestConnectionAsync()
    {
        Console.WriteLine("尝试连接本地 Vault（开发模式）...");
        
        try
        {
            var cfg = new CfgBuilder()
                .AddVault(options =>
                {
                    options.Address = "http://localhost:8200";
                    options.Token = "myroot";  // 开发模式默认 Token
                    options.EnginePath = "secret";
                    options.Path = "apq-cfg-demo";
                    options.KvVersion = 2;
                })
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
            Console.WriteLine("  提示: 请确保 Vault 服务正在运行");
            Console.WriteLine("  快速启动（开发模式）:");
            Console.WriteLine("    docker run -d -p 8200:8200 -e VAULT_DEV_ROOT_TOKEN_ID=myroot vault");
            Console.WriteLine("  访问 UI: http://localhost:8200 (Token: myroot)");
        }
        
        await Task.CompletedTask;
    }
}
