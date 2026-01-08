namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 1: 基础用法 - JSON 配置与层级覆盖
/// </summary>
public static class BasicUsageDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 1: 基础用法 - JSON 配置与层级覆盖");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "config.json");
        var localConfigPath = Path.Combine(baseDir, "config.local.json");

        // 创建基础配置
        File.WriteAllText(configPath, """
        {
            "App": {
                "Name": "MyApp",
                "Version": "1.0.0",
                "Debug": false
            },
            "Database": {
                "Host": "localhost",
                "Port": 3306,
                "Name": "mydb"
            }
        }
        """);

        // 创建本地覆盖配置（高优先级）
        File.WriteAllText(localConfigPath, """
        {
            "App": {
                "Debug": true
            },
            "Database": {
                "Host": "192.168.1.100"
            }
        }
        """);

        // 构建配置：level 越大优先级越高
        // 注意：环境变量不可写，所以 isPrimaryWriter 设置在 JSON 配置源上
        var cfg = new CfgBuilder()
            .AddJsonFile(configPath, level: 0, writeable: false)
            .AddJsonFile(localConfigPath, level: 1, writeable: true, isPrimaryWriter: true)
            .AddEnvironmentVariables(level: 2, prefix: "MYAPP_")
            .Build();

        // 读取配置
        Console.WriteLine("1.1 读取配置值:");
        Console.WriteLine($"    App:Name = {cfg["App:Name"]}");
        Console.WriteLine($"    App:Version = {cfg["App:Version"]}");
        Console.WriteLine($"    App:Debug = {cfg["App:Debug"]} (被本地配置覆盖为 true)");
        Console.WriteLine($"    Database:Host = {cfg["Database:Host"]} (被本地配置覆盖)");
        Console.WriteLine($"    Database:Port = {cfg["Database:Port"]}");

        // 检查配置是否存在
        Console.WriteLine("\n1.2 检查配置是否存在:");
        Console.WriteLine($"    Exists(App:Name) = {cfg.Exists("App:Name")}");
        Console.WriteLine($"    Exists(NotExist:Key) = {cfg.Exists("NotExist:Key")}");

        // 修改配置（写入到 isPrimaryWriter 的配置源，需要指定 targetLevel）
        Console.WriteLine("\n1.3 修改配置:");
        cfg.SetValue("App:LastRun", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetLevel: 1);
        await cfg.SaveAsync(targetLevel: 1);
        Console.WriteLine($"    已设置 App:LastRun = {cfg["App:LastRun"]}");

        // 删除配置
        Console.WriteLine("\n1.4 删除配置:");
        cfg.SetValue("App:TempKey", "临时值", targetLevel: 1);
        Console.WriteLine($"    设置 App:TempKey = {cfg["App:TempKey"]}");
        cfg.Remove("App:TempKey", targetLevel: 1);
        await cfg.SaveAsync(targetLevel: 1);
        Console.WriteLine($"    删除后 App:TempKey = {cfg["App:TempKey"] ?? "(null)"}");

        // 转换为 Microsoft.Extensions.Configuration
        Console.WriteLine("\n1.5 转换为 IConfigurationRoot:");
        var msConfig = cfg.ToMicrosoftConfiguration();
        Console.WriteLine($"    msConfig[\"App:Name\"] = {msConfig["App:Name"]}");

        cfg.Dispose();
        File.Delete(configPath);
        File.Delete(localConfigPath);

        Console.WriteLine("\n[示例 1 完成]\n");
    }
}
