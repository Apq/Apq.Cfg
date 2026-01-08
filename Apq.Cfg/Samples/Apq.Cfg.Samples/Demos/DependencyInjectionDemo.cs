using Apq.Cfg.Samples.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 7: 依赖注入集成
/// </summary>
public static class DependencyInjectionDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 7: 依赖注入集成");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "di-demo.json");
        File.WriteAllText(configPath, """
        {
            "Database": {
                "Host": "db.example.com",
                "Port": "5432",
                "Name": "production"
            },
            "Logging": {
                "Level": "Information",
                "EnableConsole": "true"
            }
        }
        """);

        // 配置服务容器
        var services = new ServiceCollection();

        // 方式1: 使用 AddApqCfg 注册配置
        Console.WriteLine("7.1 注册 Apq.Cfg 到 DI 容器:");
        services.AddApqCfg(cfg => cfg
            .AddJsonFile(configPath, level: 0, writeable: true, isPrimaryWriter: true));
        Console.WriteLine("    已注册 ICfgRoot 和 IConfigurationRoot");

        // 方式2: 绑定强类型配置
        Console.WriteLine("\n7.2 绑定强类型配置:");
        services.ConfigureApqCfg<DatabaseOptions>("Database");
        services.ConfigureApqCfg<LoggingOptions>("Logging");
        Console.WriteLine("    已绑定 DatabaseOptions 和 LoggingOptions");

        // 构建服务提供者
        var provider = services.BuildServiceProvider();

        // 获取服务
        Console.WriteLine("\n7.3 从 DI 容器获取服务:");
        var cfgRoot = provider.GetRequiredService<ICfgRoot>();
        var msConfig = provider.GetRequiredService<IConfigurationRoot>();
        var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        var logOptions = provider.GetRequiredService<IOptions<LoggingOptions>>().Value;

        Console.WriteLine($"    ICfgRoot: Database:Host = {cfgRoot["Database:Host"]}");
        Console.WriteLine($"    IConfigurationRoot: Database:Host = {msConfig["Database:Host"]}");
        Console.WriteLine($"    DatabaseOptions: Host={dbOptions.Host}, Port={dbOptions.Port}, Name={dbOptions.Name}");
        Console.WriteLine($"    LoggingOptions: Level={logOptions.Level}, EnableConsole={logOptions.EnableConsole}");

        // 清理
        if (provider is IDisposable disposable)
            disposable.Dispose();
        File.Delete(configPath);

        Console.WriteLine("\n[示例 7 完成]\n");
        await Task.CompletedTask;
    }
}
