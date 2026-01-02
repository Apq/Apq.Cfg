using System.Text;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 8: 编码映射配置
/// </summary>
public static class EncodingMappingDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 8: 编码映射配置");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var configPath = Path.Combine(baseDir, "encoding-demo.json");
        File.WriteAllText(configPath, """
        {
            "App": {
                "Name": "编码测试应用",
                "Description": "支持中文和特殊字符: äöü ñ 日本語"
            }
        }
        """, Encoding.UTF8);

        Console.WriteLine("8.1 编码检测置信度阈值:");
        var cfg1 = new CfgBuilder()
            .WithEncodingConfidenceThreshold(0.7f)
            .AddJson(configPath, level: 0, writeable: false)
            .Build();
        Console.WriteLine($"    置信度阈值设置为 0.7");
        Console.WriteLine($"    App:Name = {cfg1["App:Name"]}");
        cfg1.Dispose();

        Console.WriteLine("\n8.2 编码检测日志:");
        var cfg2 = new CfgBuilder()
            .WithEncodingDetectionLogging(result =>
            {
                Console.WriteLine($"    [编码检测] 文件: {Path.GetFileName(result.FilePath)}");
                Console.WriteLine($"               编码: {result.Encoding.EncodingName}");
                Console.WriteLine($"               置信度: {result.Confidence:P0}");
                Console.WriteLine($"               方法: {result.Method}");
            })
            .AddJson(configPath, level: 0, writeable: false)
            .Build();
        cfg2.Dispose();

        Console.WriteLine("\n8.3 编码映射规则:");
        Console.WriteLine("    支持三种映射方式:");
        Console.WriteLine("    - 完整路径: AddReadEncodingMapping(path, encoding)");
        Console.WriteLine("    - 通配符:   AddReadEncodingMappingWildcard(\"*.json\", encoding)");
        Console.WriteLine("    - 正则:     AddReadEncodingMappingRegex(@\"config.*\\.json$\", encoding)");

        // 演示编码映射配置
        var cfg3 = new CfgBuilder()
            // 为特定文件指定编码
            .AddReadEncodingMapping(configPath, Encoding.UTF8, priority: 100)
            // 为所有 JSON 文件指定写入编码
            .AddWriteEncodingMappingWildcard("*.json", new UTF8Encoding(false), priority: 50)
            .AddJson(configPath, level: 0, writeable: false)
            .Build();
        Console.WriteLine("\n    已配置编码映射规则");
        Console.WriteLine($"    App:Description = {cfg3["App:Description"]}");
        cfg3.Dispose();

        File.Delete(configPath);

        Console.WriteLine("\n[示例 8 完成]\n");
        await Task.CompletedTask;
    }
}
