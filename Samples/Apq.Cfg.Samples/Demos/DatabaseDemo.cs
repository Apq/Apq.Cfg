using Apq.Cfg.Database;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 10: 数据库配置源
/// 注意：需要运行数据库服务才能执行此示例
/// </summary>
public static class DatabaseDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 10: 数据库配置源");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("注意：此示例需要运行数据库服务\n");

        // 使用 SQLite 作为示例（无需额外服务）
        var dbPath = Path.Combine(baseDir, "config.db");

        try
        {
            Console.WriteLine("10.1 使用 SQLite 数据库配置源:");
            Console.WriteLine($"    数据库文件: {dbPath}");

            var cfg = new CfgBuilder()
                .AddDatabase(options =>
                {
                    options.Provider = "SQLite";
                    options.ConnectionString = $"Data Source={dbPath}";
                    options.Table = "AppConfig";
                    options.KeyColumn = "ConfigKey";
                    options.ValueColumn = "ConfigValue";
                }, level: 0, isPrimaryWriter: true)
                .Build();

            Console.WriteLine("    已连接到 SQLite 数据库");

            // 写入配置
            Console.WriteLine("\n10.2 写入配置到数据库:");
            cfg.Set("App:Name", "DatabaseApp");
            cfg.Set("App:Version", "1.0.0");
            cfg.Set("Database:MaxConnections", "100");
            await cfg.SaveAsync();
            Console.WriteLine("    已写入 3 个配置项");

            // 读取配置
            Console.WriteLine("\n10.3 从数据库读取配置:");
            Console.WriteLine($"    App:Name = {cfg.Get("App:Name")}");
            Console.WriteLine($"    App:Version = {cfg.Get("App:Version")}");
            Console.WriteLine($"    Database:MaxConnections = {cfg.GetValue<int>("Database:MaxConnections")}");

            // 修改配置
            Console.WriteLine("\n10.4 修改配置:");
            cfg.Set("App:Version", "2.0.0");
            await cfg.SaveAsync();
            Console.WriteLine($"    修改后 App:Version = {cfg.Get("App:Version")}");

            // 删除配置
            Console.WriteLine("\n10.5 删除配置:");
            cfg.Remove("App:Name");
            cfg.Remove("App:Version");
            cfg.Remove("Database:MaxConnections");
            await cfg.SaveAsync();
            Console.WriteLine("    已清理测试配置");

            cfg.Dispose();

            // 等待数据库连接完全释放
            await Task.Delay(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // 清理数据库文件
            try
            {
                if (File.Exists(dbPath))
                    File.Delete(dbPath);
            }
            catch
            {
                // 忽略删除失败
            }

            Console.WriteLine("\n10.6 其他数据库支持:");
            Console.WriteLine("    - SqlServer: Provider=\"SqlServer\"");
            Console.WriteLine("    - MySQL: Provider=\"MySql\"");
            Console.WriteLine("    - PostgreSQL: Provider=\"PostgreSQL\"");
            Console.WriteLine("    - Oracle: Provider=\"Oracle\"");

            Console.WriteLine("\n[示例 10 完成]\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    [跳过] 数据库服务不可用: {ex.Message}");
            
            // 等待连接释放后再删除
            await Task.Delay(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            try
            {
                if (File.Exists(dbPath))
                    File.Delete(dbPath);
            }
            catch
            {
                // 忽略删除失败
            }
            Console.WriteLine("\n[示例 10 跳过]\n");
        }
    }
}
