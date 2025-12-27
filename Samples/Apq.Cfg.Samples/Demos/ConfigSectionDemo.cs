namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 3: 配置节 (GetSection) 与子键枚举
/// </summary>
public static class ConfigSectionDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 3: 配置节 (GetSection) 与子键枚举");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "section-demo.json");
        File.WriteAllText(configPath, """
        {
            "Database": {
                "Primary": {
                    "Host": "primary.db.local",
                    "Port": 3306,
                    "Username": "admin"
                },
                "Replica": {
                    "Host": "replica.db.local",
                    "Port": 3307,
                    "Username": "reader"
                }
            },
            "Cache": {
                "Redis": {
                    "Host": "redis.local",
                    "Port": 6379
                }
            }
        }
        """);

        using var cfg = new CfgBuilder()
            .AddJson(configPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 获取配置节
        Console.WriteLine("3.1 使用 GetSection 简化嵌套访问:");
        var dbSection = cfg.GetSection("Database");
        var primarySection = dbSection.GetSection("Primary");

        Console.WriteLine($"    Database:Primary:Host = {primarySection.Get("Host")}");
        Console.WriteLine($"    Database:Primary:Port = {primarySection.Get<int>("Port")}");

        // 枚举子键
        Console.WriteLine("\n3.2 枚举配置节的子键:");
        Console.WriteLine("    Database 的子键:");
        foreach (var key in dbSection.GetChildKeys())
        {
            Console.WriteLine($"      - {key}");
        }

        Console.WriteLine("\n    顶级配置键:");
        foreach (var key in cfg.GetChildKeys())
        {
            Console.WriteLine($"      - {key}");
        }

        // 通过配置节修改值
        Console.WriteLine("\n3.3 通过配置节修改值:");
        var replicaSection = dbSection.GetSection("Replica");
        replicaSection.Set("Port", "3308");
        await cfg.SaveAsync();
        Console.WriteLine($"    修改后 Database:Replica:Port = {replicaSection.Get("Port")}");

        File.Delete(configPath);

        Console.WriteLine("\n[示例 3 完成]\n");
    }
}
