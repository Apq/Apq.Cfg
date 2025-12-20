using Apq.Cfg;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

Console.WriteLine("=== Apq.Cfg 示例 ===\n");

// 创建示例配置文件
var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
var localSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");

// 写入基础配置
File.WriteAllText(appSettingsPath, """
{
    "App": {
        "Name": "MyApp",
        "Version": "1.0.0"
    },
    "Database": {
        "ConnectionString": "Server=localhost;Database=mydb",
        "Timeout": 30
    },
    "Logging": {
        "Level": "Information"
    }
}
""");

// 写入本地覆盖配置
File.WriteAllText(localSettingsPath, """
{
    "Database": {
        "ConnectionString": "Server=192.168.1.100;Database=proddb"
    },
    "Logging": {
        "Level": "Debug"
    }
}
""");

// 构建配置
var cfg = new CfgBuilder()
    .AddJson(appSettingsPath, level: 0, writeable: false)
    .AddJson(localSettingsPath, level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
Console.WriteLine("1. 读取配置值:");
Console.WriteLine($"   App:Name = {cfg.Get("App:Name")}");
Console.WriteLine($"   App:Version = {cfg.Get("App:Version")}");
Console.WriteLine($"   Database:ConnectionString = {cfg.Get("Database:ConnectionString")}");
Console.WriteLine($"   Database:Timeout = {cfg.Get<int>("Database:Timeout")}");
Console.WriteLine($"   Logging:Level = {cfg.Get("Logging:Level")} (被本地配置覆盖)");

// 检查配置是否存在
Console.WriteLine("\n2. 检查配置是否存在:");
Console.WriteLine($"   Exists(App:Name) = {cfg.Exists("App:Name")}");
Console.WriteLine($"   Exists(NotExist:Key) = {cfg.Exists("NotExist:Key")}");

// 修改配置
Console.WriteLine("\n3. 修改配置:");
cfg.Set("App:LastRun", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
await cfg.SaveAsync();
Console.WriteLine($"   已设置 App:LastRun = {cfg.Get("App:LastRun")}");

// 转换为 Microsoft.Extensions.Configuration
Console.WriteLine("\n4. 转换为 IConfigurationRoot:");
var msConfig = cfg.ToMicrosoftConfiguration();
Console.WriteLine($"   msConfig[\"App:Name\"] = {msConfig["App:Name"]}");

// 清理
cfg.Dispose();
File.Delete(appSettingsPath);
File.Delete(localSettingsPath);

Console.WriteLine("\n=== 示例完成 ===");
