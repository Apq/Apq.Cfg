using Apq.Cfg;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 17: 源生成器（零反射绑定）
/// 演示如何使用 Apq.Cfg.SourceGenerator 实现零反射配置绑定
/// </summary>
public static class SourceGeneratorDemo
{
    public static async Task RunAsync(string baseDir)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("示例 17: 源生成器（零反射绑定）");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("【源生成器说明】");
        Console.WriteLine("Apq.Cfg.SourceGenerator 使用 Roslyn 源生成器在编译时生成配置绑定代码。");
        Console.WriteLine("优势：零反射、Native AOT 兼容、编译时类型检查。\n");

        // 示例 1: 基本用法
        Console.WriteLine("--- 示例 17.1: 基本用法 ---");
        ShowBasicUsage();

        // 示例 2: 定义配置类
        Console.WriteLine("\n--- 示例 17.2: 定义配置类 ---");
        ShowConfigClassDefinition();

        // 示例 3: 使用生成的绑定方法
        Console.WriteLine("\n--- 示例 17.3: 使用生成的绑定方法 ---");
        ShowBindingMethods();

        // 示例 4: 支持的类型
        Console.WriteLine("\n--- 示例 17.4: 支持的类型 ---");
        ShowSupportedTypes();

        // 示例 5: 嵌套配置
        Console.WriteLine("\n--- 示例 17.5: 嵌套配置 ---");
        ShowNestedConfiguration();

        // 示例 6: 集合类型
        Console.WriteLine("\n--- 示例 17.6: 集合类型 ---");
        ShowCollectionTypes();

        // 示例 7: 查看生成的代码
        Console.WriteLine("\n--- 示例 17.7: 查看生成的代码 ---");
        ShowGeneratedCode();

        // 示例 8: 实际演示
        Console.WriteLine("\n--- 示例 17.8: 实际演示 ---");
        await DemoActualUsageAsync(baseDir);

        Console.WriteLine("\n示例 17 完成\n");
    }

    private static void ShowBasicUsage()
    {
        Console.WriteLine("基本用法：");
        Console.WriteLine(@"
// 1. 安装 NuGet 包
// dotnet add package Apq.Cfg.SourceGenerator

// 2. 定义配置类（必须是 partial 类）
[CfgSection(""AppSettings"")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public int Port { get; set; }
}

// 3. 使用生成的绑定方法
var cfg = new CfgBuilder()
    .AddJson(""config.json"")
    .Build();

var appConfig = AppConfig.BindFrom(cfg.GetSection(""AppSettings""));
Console.WriteLine($""Name: {appConfig.Name}, Port: {appConfig.Port}"");
");
    }

    private static void ShowConfigClassDefinition()
    {
        Console.WriteLine("配置类定义规则：");
        Console.WriteLine(@"
// 1. 使用 [CfgSection] 特性标记
// 2. 类必须是 partial 的
// 3. 属性必须有 public getter 和 setter

using Apq.Cfg;

// 指定配置节路径
[CfgSection(""Database"")]
public partial class DatabaseConfig
{
    public string? Host { get; set; }
    public int Port { get; set; } = 5432;  // 支持默认值
    public string? Name { get; set; }
}

// 不指定路径时，使用类名（去掉 Config/Settings 后缀）
[CfgSection]
public partial class LoggingConfig
{
    public string? Level { get; set; }
    public bool EnableConsole { get; set; }
}

// 禁用扩展方法生成
[CfgSection(""Cache"", GenerateExtension = false)]
public partial class CacheConfig
{
    public string? Provider { get; set; }
    public int ExpirationMinutes { get; set; }
}
");
    }

    private static void ShowBindingMethods()
    {
        Console.WriteLine("生成的绑定方法：");
        Console.WriteLine(@"
// 源生成器会为每个配置类生成以下方法：

// 1. BindFrom - 从配置节创建新实例
var config = DatabaseConfig.BindFrom(cfgRoot.GetSection(""Database""));

// 2. BindTo - 绑定到已有实例
var existingConfig = new DatabaseConfig();
DatabaseConfig.BindTo(cfgRoot.GetSection(""Database""), existingConfig);

// 3. 扩展方法（如果指定了 SectionPath）
var config2 = cfgRoot.GetDatabaseConfig();  // 自动生成的扩展方法
");
    }

    private static void ShowSupportedTypes()
    {
        Console.WriteLine("支持的类型：");
        Console.WriteLine(@"
[CfgSection(""TypeDemo"")]
public partial class TypeDemoConfig
{
    // 字符串
    public string? StringValue { get; set; }

    // 数值类型
    public int IntValue { get; set; }
    public long LongValue { get; set; }
    public double DoubleValue { get; set; }
    public decimal DecimalValue { get; set; }

    // 布尔
    public bool BoolValue { get; set; }

    // 日期时间
    public DateTime DateTimeValue { get; set; }
    public DateTimeOffset DateTimeOffsetValue { get; set; }
    public TimeSpan TimeSpanValue { get; set; }
    public DateOnly DateOnlyValue { get; set; }  // .NET 6+
    public TimeOnly TimeOnlyValue { get; set; }  // .NET 6+

    // 其他
    public Guid GuidValue { get; set; }
    public Uri? UriValue { get; set; }

    // 枚举
    public LogLevel LogLevel { get; set; }

    // 可空类型
    public int? NullableInt { get; set; }
    public DateTime? NullableDateTime { get; set; }
}

public enum LogLevel { Debug, Info, Warning, Error }
");
    }

    private static void ShowNestedConfiguration()
    {
        Console.WriteLine("嵌套配置：");
        Console.WriteLine(@"
// 嵌套的配置类也需要标记 [CfgSection]
[CfgSection(""App"")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public DatabaseConfig? Database { get; set; }  // 嵌套对象
    public LoggingConfig? Logging { get; set; }
}

[CfgSection]
public partial class DatabaseConfig
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
}

[CfgSection]
public partial class LoggingConfig
{
    public string? Level { get; set; }
    public bool EnableConsole { get; set; }
}

// 对应的 JSON 配置：
// {
//   ""App"": {
//     ""Name"": ""MyApp"",
//     ""Database"": {
//       ""ConnectionString"": ""Server=localhost;..."",
//       ""Timeout"": 60
//     },
//     ""Logging"": {
//       ""Level"": ""Info"",
//       ""EnableConsole"": true
//     }
//   }
// }
");
    }

    private static void ShowCollectionTypes()
    {
        Console.WriteLine("集合类型：");
        Console.WriteLine(@"
[CfgSection(""Collections"")]
public partial class CollectionsConfig
{
    // 数组
    public string[]? Tags { get; set; }
    public int[]? Ports { get; set; }

    // List
    public List<string>? Hosts { get; set; }

    // HashSet
    public HashSet<string>? AllowedOrigins { get; set; }

    // Dictionary
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, int>? Limits { get; set; }
}

// 对应的 JSON 配置：
// {
//   ""Collections"": {
//     ""Tags"": [""web"", ""api"", ""v2""],
//     ""Ports"": [80, 443, 8080],
//     ""Hosts"": [""host1.com"", ""host2.com""],
//     ""AllowedOrigins"": [""https://example.com""],
//     ""Headers"": {
//       ""X-Api-Key"": ""secret"",
//       ""X-Version"": ""1.0""
//     },
//     ""Limits"": {
//       ""MaxConnections"": 100,
//       ""MaxRequests"": 1000
//     }
//   }
// }
");
    }

    private static void ShowGeneratedCode()
    {
        Console.WriteLine("查看生成的代码：");
        Console.WriteLine(@"
// 在项目文件中添加以下配置可以保留生成的源代码：
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)\GeneratedFiles</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

// 生成的文件将位于 obj/GeneratedFiles/ 目录下

// 生成的代码示例：
partial class DatabaseConfig
{
    public static DatabaseConfig BindFrom(ICfgSection section)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        var result = new DatabaseConfig();
        BindTo(section, result);
        return result;
    }

    public static void BindTo(ICfgSection section, DatabaseConfig target)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        if (target == null) throw new ArgumentNullException(nameof(target));

        // ConnectionString: string?
        {
            var __value = section.Get(""ConnectionString"");
            if (__value != null)
            {
                target.ConnectionString = __value;
            }
        }

        // Timeout: int
        {
            var __value = section.Get(""Timeout"");
            if (__value != null)
            {
                var __converted = int.TryParse(__value, out var __intVal) ? __intVal : (int?)null;
                if (__converted != null) target.Timeout = __converted.Value;
            }
        }
    }
}
");
    }

    private static async Task DemoActualUsageAsync(string baseDir)
    {
        Console.WriteLine("实际演示（使用内存配置）：");
        
        // 创建一个简单的配置
        var cfg = new CfgBuilder()
            .AddJson(Path.Combine(baseDir, "config.json"), level: 0, writeable: false, optional: true, reloadOnChange: false)
            .Build();

        // 由于源生成器需要在编译时生成代码，这里只能展示概念
        Console.WriteLine(@"
// 假设我们有以下配置类：
// [CfgSection(""Database"")]
// public partial class DatabaseConfig
// {
//     public string? Host { get; set; }
//     public int Port { get; set; }
//     public string? Name { get; set; }
// }

// 使用方式：
// var dbConfig = DatabaseConfig.BindFrom(cfg.GetSection(""Database""));
// Console.WriteLine($""Host: {dbConfig.Host}"");
// Console.WriteLine($""Port: {dbConfig.Port}"");
// Console.WriteLine($""Name: {dbConfig.Name}"");
");

        // 使用传统方式读取配置作为对比
        var section = cfg.GetSection("Database");
        Console.WriteLine("\n使用传统方式读取配置（对比）：");
        Console.WriteLine($"  Host: {section["Host"] ?? "(未配置)"}");
        Console.WriteLine($"  Port: {section["Port"] ?? "(未配置)"}");
        Console.WriteLine($"  Name: {section["Name"] ?? "(未配置)"}");

        Console.WriteLine("\n源生成器的优势：");
        Console.WriteLine("  ✓ 零反射 - 编译时生成绑定代码");
        Console.WriteLine("  ✓ Native AOT 兼容 - 完全支持 AOT 发布");
        Console.WriteLine("  ✓ 编译时类型检查 - 属性名错误会在编译时报错");
        Console.WriteLine("  ✓ 更好的性能 - 无运行时反射开销");
        Console.WriteLine("  ✓ IntelliSense 支持 - IDE 自动补全");

        await Task.CompletedTask;
    }
}
