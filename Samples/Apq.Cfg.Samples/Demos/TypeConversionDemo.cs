using Apq.Cfg.Samples.Models;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 5: 类型转换
/// </summary>
public static class TypeConversionDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 5: 类型转换");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "types-demo.json");
        File.WriteAllText(configPath, """
        {
            "Types": {
                "IntValue": "42",
                "LongValue": "9223372036854775807",
                "DoubleValue": "3.14159",
                "DecimalValue": "123.456",
                "BoolTrue": "true",
                "BoolFalse": "false",
                "DateValue": "2024-12-25",
                "GuidValue": "550e8400-e29b-41d4-a716-446655440000",
                "EnumValue": "Warning"
            }
        }
        """);

        using var cfg = new CfgBuilder()
            .AddJson(configPath, level: 0, writeable: false)
            .Build();

        Console.WriteLine("5.1 各种类型转换:");
        Console.WriteLine($"    int: {cfg.Get<int>("Types:IntValue")}");
        Console.WriteLine($"    long: {cfg.Get<long>("Types:LongValue")}");
        Console.WriteLine($"    double: {cfg.Get<double>("Types:DoubleValue")}");
        Console.WriteLine($"    decimal: {cfg.Get<decimal>("Types:DecimalValue")}");
        Console.WriteLine($"    bool (true): {cfg.Get<bool>("Types:BoolTrue")}");
        Console.WriteLine($"    bool (false): {cfg.Get<bool>("Types:BoolFalse")}");
        Console.WriteLine($"    DateTime: {cfg.Get<DateTime>("Types:DateValue"):yyyy-MM-dd}");
        Console.WriteLine($"    Guid: {cfg.Get<Guid>("Types:GuidValue")}");
        Console.WriteLine($"    Enum: {cfg.Get<LogLevel>("Types:EnumValue")}");

        Console.WriteLine("\n5.2 可空类型与默认值:");
        Console.WriteLine($"    不存在的键 (int?): {cfg.Get<int?>("Types:NotExist") ?? -1}");
        Console.WriteLine($"    不存在的键 (string): {cfg.Get("Types:NotExist") ?? "(null)"}");

        File.Delete(configPath);

        Console.WriteLine("\n[示例 5 完成]\n");
        await Task.CompletedTask;
    }
}
