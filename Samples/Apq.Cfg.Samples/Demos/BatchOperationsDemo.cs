namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 4: 批量操作 - GetMany / SetMany
/// </summary>
public static class BatchOperationsDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 4: 批量操作 - GetMany / SetMany");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "batch-demo.json");
        File.WriteAllText(configPath, """
        {
            "Settings": {
                "Theme": "dark",
                "Language": "zh-CN",
                "FontSize": "14",
                "AutoSave": "true"
            }
        }
        """);

        using var cfg = new CfgBuilder()
            .AddJson(configPath, level: 0, writeable: true, isPrimaryWriter: true)
            .Build();

        // 批量获取
        Console.WriteLine("4.1 批量获取 (GetMany):");
        var keys = new[] { "Settings:Theme", "Settings:Language", "Settings:FontSize" };
        var values = cfg.GetMany(keys);
        foreach (var kv in values)
        {
            Console.WriteLine($"    {kv.Key} = {kv.Value}");
        }

        // 批量获取并转换类型
        Console.WriteLine("\n4.2 批量获取并转换类型 (GetMany<T>):");
        var intKeys = new[] { "Settings:FontSize" };
        var intValues = cfg.GetMany<int>(intKeys);
        foreach (var kv in intValues)
        {
            Console.WriteLine($"    {kv.Key} = {kv.Value} (int)");
        }

        // 批量设置
        Console.WriteLine("\n4.3 批量设置 (SetMany):");
        var newValues = new Dictionary<string, string?>
        {
            ["Settings:Theme"] = "light",
            ["Settings:FontSize"] = "16",
            ["Settings:NewOption"] = "enabled"
        };
        cfg.SetMany(newValues);
        await cfg.SaveAsync();

        Console.WriteLine("    批量设置后的值:");
        var updatedValues = cfg.GetMany(new[] { "Settings:Theme", "Settings:FontSize", "Settings:NewOption" });
        foreach (var kv in updatedValues)
        {
            Console.WriteLine($"    {kv.Key} = {kv.Value}");
        }

        File.Delete(configPath);

        Console.WriteLine("\n[示例 4 完成]\n");
    }
}
