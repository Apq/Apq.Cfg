using System.Text;
using Apq.Cfg;
using Apq.Cfg.Changes;
using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              Apq.Cfg 完整功能示例                            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

var baseDir = AppContext.BaseDirectory;

// ============================================================================
// 示例 1: 基础用法 - JSON 配置与层级覆盖
// ============================================================================
await Demo1_BasicUsage(baseDir);

// ============================================================================
// 示例 2: 多格式支持 - INI、XML、YAML、TOML
// ============================================================================
await Demo2_MultiFormat(baseDir);

// ============================================================================
// 示例 3: 配置节 (GetSection) 与子键枚举
// ============================================================================
await Demo3_ConfigSection(baseDir);

// ============================================================================
// 示例 4: 批量操作 - GetMany / SetMany
// ============================================================================
await Demo4_BatchOperations(baseDir);

// ============================================================================
// 示例 5: 类型转换
// ============================================================================
await Demo5_TypeConversion(baseDir);

// ============================================================================
// 示例 6: 动态配置重载
// ============================================================================
await Demo6_DynamicReload(baseDir);

// ============================================================================
// 示例 7: 依赖注入集成
// ============================================================================
await Demo7_DependencyInjection(baseDir);

// ============================================================================
// 示例 8: 编码映射配置
// ============================================================================
await Demo8_EncodingMapping(baseDir);

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              所有示例执行完成                                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

// ============================================================================
// 示例实现
// ============================================================================

static async Task Demo1_BasicUsage(string baseDir)
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
        .AddJson(configPath, level: 0, writeable: false)
        .AddJson(localConfigPath, level: 1, writeable: true, isPrimaryWriter: true)
        .AddEnvironmentVariables(level: 2, prefix: "MYAPP_")
        .Build();

    // 读取配置
    Console.WriteLine("1.1 读取配置值:");
    Console.WriteLine($"    App:Name = {cfg.Get("App:Name")}");
    Console.WriteLine($"    App:Version = {cfg.Get("App:Version")}");
    Console.WriteLine($"    App:Debug = {cfg.Get("App:Debug")} (被本地配置覆盖为 true)");
    Console.WriteLine($"    Database:Host = {cfg.Get("Database:Host")} (被本地配置覆盖)");
    Console.WriteLine($"    Database:Port = {cfg.Get("Database:Port")}");

    // 检查配置是否存在
    Console.WriteLine("\n1.2 检查配置是否存在:");
    Console.WriteLine($"    Exists(App:Name) = {cfg.Exists("App:Name")}");
    Console.WriteLine($"    Exists(NotExist:Key) = {cfg.Exists("NotExist:Key")}");

    // 修改配置（写入到 isPrimaryWriter 的配置源，需要指定 targetLevel）
    Console.WriteLine("\n1.3 修改配置:");
    cfg.Set("App:LastRun", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), targetLevel: 1);
    await cfg.SaveAsync(targetLevel: 1);
    Console.WriteLine($"    已设置 App:LastRun = {cfg.Get("App:LastRun")}");

    // 删除配置
    Console.WriteLine("\n1.4 删除配置:");
    cfg.Set("App:TempKey", "临时值", targetLevel: 1);
    Console.WriteLine($"    设置 App:TempKey = {cfg.Get("App:TempKey")}");
    cfg.Remove("App:TempKey", targetLevel: 1);
    await cfg.SaveAsync(targetLevel: 1);
    Console.WriteLine($"    删除后 App:TempKey = {cfg.Get("App:TempKey") ?? "(null)"}");

    // 转换为 Microsoft.Extensions.Configuration
    Console.WriteLine("\n1.5 转换为 IConfigurationRoot:");
    var msConfig = cfg.ToMicrosoftConfiguration();
    Console.WriteLine($"    msConfig[\"App:Name\"] = {msConfig["App:Name"]}");

    cfg.Dispose();
    File.Delete(configPath);
    File.Delete(localConfigPath);

    Console.WriteLine("\n[示例 1 完成]\n");
}

static async Task Demo2_MultiFormat(string baseDir)
{
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("示例 2: 多格式支持 - INI、XML、YAML、TOML");
    Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

    // INI 格式
    var iniPath = Path.Combine(baseDir, "config.ini");
    File.WriteAllText(iniPath, """
    [App]
    Name=IniApp
    Version=2.0.0

    [Database]
    Host=ini-server
    Port=5432
    """);

    // XML 格式
    var xmlPath = Path.Combine(baseDir, "config.xml");
    File.WriteAllText(xmlPath, """
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <App>
            <Name>XmlApp</Name>
            <Version>3.0.0</Version>
        </App>
        <Database>
            <Host>xml-server</Host>
            <Port>1433</Port>
        </Database>
    </configuration>
    """);

    // YAML 格式
    var yamlPath = Path.Combine(baseDir, "config.yaml");
    File.WriteAllText(yamlPath, """
    App:
      Name: YamlApp
      Version: 4.0.0
    Database:
      Host: yaml-server
      Port: 27017
    """);

    // TOML 格式
    var tomlPath = Path.Combine(baseDir, "config.toml");
    File.WriteAllText(tomlPath, """
    [App]
    Name = "TomlApp"
    Version = "5.0.0"

    [Database]
    Host = "toml-server"
    Port = 6379
    """);

    // 分别测试各格式
    Console.WriteLine("2.1 INI 格式:");
    using (var iniCfg = new CfgBuilder().AddIni(iniPath, level: 0, writeable: true).Build())
    {
        Console.WriteLine($"    App:Name = {iniCfg.Get("App:Name")}");
        Console.WriteLine($"    Database:Port = {iniCfg.Get("Database:Port")}");
    }

    Console.WriteLine("\n2.2 XML 格式:");
    using (var xmlCfg = new CfgBuilder().AddXml(xmlPath, level: 0, writeable: true).Build())
    {
        Console.WriteLine($"    App:Name = {xmlCfg.Get("App:Name")}");
        Console.WriteLine($"    Database:Port = {xmlCfg.Get("Database:Port")}");
    }

    Console.WriteLine("\n2.3 YAML 格式:");
    using (var yamlCfg = new CfgBuilder().AddYaml(yamlPath, level: 0, writeable: true).Build())
    {
        Console.WriteLine($"    App:Name = {yamlCfg.Get("App:Name")}");
        Console.WriteLine($"    Database:Port = {yamlCfg.Get("Database:Port")}");
    }

    Console.WriteLine("\n2.4 TOML 格式:");
    using (var tomlCfg = new CfgBuilder().AddToml(tomlPath, level: 0, writeable: true).Build())
    {
        Console.WriteLine($"    App:Name = {tomlCfg.Get("App:Name")}");
        Console.WriteLine($"    Database:Port = {tomlCfg.Get("Database:Port")}");
    }

    // 混合多种格式
    Console.WriteLine("\n2.5 混合多种格式（层级覆盖）:");
    using var mixedCfg = new CfgBuilder()
        .AddIni(iniPath, level: 0, writeable: false)
        .AddYaml(yamlPath, level: 1, writeable: false)
        .AddToml(tomlPath, level: 2, writeable: true, isPrimaryWriter: true)
        .Build();

    Console.WriteLine($"    App:Name = {mixedCfg.Get("App:Name")} (来自 TOML，最高优先级)");
    Console.WriteLine($"    App:Version = {mixedCfg.Get("App:Version")} (来自 TOML)");

    File.Delete(iniPath);
    File.Delete(xmlPath);
    File.Delete(yamlPath);
    File.Delete(tomlPath);

    Console.WriteLine("\n[示例 2 完成]\n");
    await Task.CompletedTask;
}

static async Task Demo3_ConfigSection(string baseDir)
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

static async Task Demo4_BatchOperations(string baseDir)
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

static async Task Demo5_TypeConversion(string baseDir)
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

static async Task Demo6_DynamicReload(string baseDir)
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

static async Task Demo7_DependencyInjection(string baseDir)
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
        .AddJson(configPath, level: 0, writeable: true, isPrimaryWriter: true));
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

    Console.WriteLine($"    ICfgRoot: Database:Host = {cfgRoot.Get("Database:Host")}");
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

static async Task Demo8_EncodingMapping(string baseDir)
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
    Console.WriteLine($"    App:Name = {cfg1.Get("App:Name")}");
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
    Console.WriteLine($"    App:Description = {cfg3.Get("App:Description")}");
    cfg3.Dispose();

    File.Delete(configPath);

    Console.WriteLine("\n[示例 8 完成]\n");
    await Task.CompletedTask;
}

// ============================================================================
// 强类型配置类
// ============================================================================

public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}

public class LoggingOptions
{
    public string? Level { get; set; }
    public bool EnableConsole { get; set; }
}

// 用于类型转换示例的枚举
public enum LogLevel { Debug, Info, Warning, Error }
