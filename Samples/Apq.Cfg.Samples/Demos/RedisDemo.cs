using Apq.Cfg.Redis;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 9: Redis 配置源
/// 注意：需要运行 Redis 服务才能执行此示例
/// </summary>
public static class RedisDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 9: Redis 配置源");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("注意：此示例需要运行 Redis 服务 (localhost:6379)\n");

        try
        {
            // 构建 Redis 配置源
            Console.WriteLine("9.1 连接 Redis 配置源:");
            var cfg = new CfgBuilder()
                .AddRedis(options =>
                {
                    options.ConnectionString = "localhost:6379";
                    options.KeyPrefix = "apq:samples:";
                    options.Database = 0;
                    options.ConnectTimeoutMs = 3000;
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    已连接到 Redis");

            // 写入配置
            Console.WriteLine("\n9.2 写入配置到 Redis:");
            cfg.Set("App:Name", "RedisApp");
            cfg.Set("App:Version", "1.0.0");
            cfg.Set("Database:Host", "db.example.com");
            cfg.Set("Database:Port", "5432");
            await cfg.SaveAsync();
            Console.WriteLine("    已写入 4 个配置项");

            // 读取配置
            Console.WriteLine("\n9.3 从 Redis 读取配置:");
            Console.WriteLine($"    App:Name = {cfg.Get("App:Name")}");
            Console.WriteLine($"    App:Version = {cfg.Get("App:Version")}");
            Console.WriteLine($"    Database:Host = {cfg.Get("Database:Host")}");
            Console.WriteLine($"    Database:Port = {cfg.GetValue<int>("Database:Port")}");

            // 检查配置是否存在
            Console.WriteLine("\n9.4 检查配置是否存在:");
            Console.WriteLine($"    Exists(App:Name) = {cfg.Exists("App:Name")}");
            Console.WriteLine($"    Exists(NotExist) = {cfg.Exists("NotExist")}");

            // 删除配置
            Console.WriteLine("\n9.5 删除配置:");
            cfg.Remove("App:Name");
            cfg.Remove("App:Version");
            cfg.Remove("Database:Host");
            cfg.Remove("Database:Port");
            await cfg.SaveAsync();
            Console.WriteLine("    已清理测试配置");

            cfg.Dispose();
            Console.WriteLine("\n[示例 9 完成]\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    [跳过] Redis 服务不可用: {ex.Message}");
            Console.WriteLine("\n[示例 9 跳过]\n");
        }
    }
}
