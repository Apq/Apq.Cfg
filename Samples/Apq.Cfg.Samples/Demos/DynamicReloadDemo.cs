using Apq.Cfg.Changes;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 6: 动态配置重载
/// </summary>
public static class DynamicReloadDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 6: 动态配置重载");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "reload-demo.json");
        File.WriteAllText(configPath, """
        {
            "App": {
                "RefreshInterval": "30"
            }
        }
        """);

        // 启用 reloadOnChange
        var cfg = new CfgBuilder()
            .AddJson(configPath, level: 0, writeable: true, isPrimaryWriter: true, reloadOnChange: true)
            .Build();

        Console.WriteLine("6.1 配置动态重载选项:");
        var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            DebounceMs = 100,                    // 防抖时间
            EnableDynamicReload = true,          // 启用动态重载
            Strategy = ReloadStrategy.Eager,     // 立即重载
            RollbackOnError = true,              // 错误时回滚
            HistorySize = 5                      // 保留 5 条历史
        });
        Console.WriteLine("    已配置: DebounceMs=100, Strategy=Eager, HistorySize=5");

        // 使用 IChangeToken 监听变更
        Console.WriteLine("\n6.2 使用 IChangeToken 监听变更:");
        var changeCount = 0;
        ChangeToken.OnChange(
            () => msConfig.GetReloadToken(),
            () =>
            {
                changeCount++;
                Console.WriteLine($"    [IChangeToken] 配置已更新 (第 {changeCount} 次)");
            });
        Console.WriteLine("    已注册 IChangeToken 回调");

        // 使用 Rx 订阅配置变更
        Console.WriteLine("\n6.3 使用 Rx 订阅配置变更:");
        using var subscription = cfg.ConfigChanges.Subscribe(e =>
        {
            Console.WriteLine($"    [Rx] 批次 {e.BatchId} - {e.Changes.Count} 个变更:");
            foreach (var (key, change) in e.Changes)
            {
                Console.WriteLine($"         [{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
            }
        });
        Console.WriteLine("    已订阅 ConfigChanges");

        // 模拟配置变更
        Console.WriteLine("\n6.4 模拟配置变更:");
        Console.WriteLine("    修改 App:RefreshInterval 为 60...");
        cfg.Set("App:RefreshInterval", "60");
        await cfg.SaveAsync();

        // 等待变更通知
        await Task.Delay(200);

        Console.WriteLine($"\n    当前值: App:RefreshInterval = {cfg.Get("App:RefreshInterval")}");

        cfg.Dispose();
        File.Delete(configPath);

        Console.WriteLine("\n[示例 6 完成]\n");
    }
}
