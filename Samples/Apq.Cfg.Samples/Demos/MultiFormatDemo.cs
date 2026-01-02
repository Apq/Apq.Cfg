using Apq.Cfg.Ini;
using Apq.Cfg.Xml;
using Apq.Cfg.Yaml;
using Apq.Cfg.Toml;

namespace Apq.Cfg.Samples.Demos;

/// <summary>
/// 示例 2: 多格式支持 - INI、XML、YAML、TOML
/// </summary>
public static class MultiFormatDemo
{
    public static async Task RunAsync(string baseDir)
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
            Console.WriteLine($"    App:Name = {iniCfg["App:Name"]}");
            Console.WriteLine($"    Database:Port = {iniCfg["Database:Port"]}");
        }

        Console.WriteLine("\n2.2 XML 格式:");
        using (var xmlCfg = new CfgBuilder().AddXml(xmlPath, level: 0, writeable: true).Build())
        {
            Console.WriteLine($"    App:Name = {xmlCfg["App:Name"]}");
            Console.WriteLine($"    Database:Port = {xmlCfg["Database:Port"]}");
        }

        Console.WriteLine("\n2.3 YAML 格式:");
        using (var yamlCfg = new CfgBuilder().AddYaml(yamlPath, level: 0, writeable: true).Build())
        {
            Console.WriteLine($"    App:Name = {yamlCfg["App:Name"]}");
            Console.WriteLine($"    Database:Port = {yamlCfg["Database:Port"]}");
        }

        Console.WriteLine("\n2.4 TOML 格式:");
        using (var tomlCfg = new CfgBuilder().AddToml(tomlPath, level: 0, writeable: true).Build())
        {
            Console.WriteLine($"    App:Name = {tomlCfg["App:Name"]}");
            Console.WriteLine($"    Database:Port = {tomlCfg["Database:Port"]}");
        }

        // 混合多种格式
        Console.WriteLine("\n2.5 混合多种格式（层级覆盖）:");
        using var mixedCfg = new CfgBuilder()
            .AddIni(iniPath, level: 0, writeable: false)
            .AddYaml(yamlPath, level: 1, writeable: false)
            .AddToml(tomlPath, level: 2, writeable: true, isPrimaryWriter: true)
            .Build();

        Console.WriteLine($"    App:Name = {mixedCfg["App:Name"]} (来自 TOML，最高优先级)");
        Console.WriteLine($"    App:Version = {mixedCfg["App:Version"]} (来自 TOML)");

        File.Delete(iniPath);
        File.Delete(xmlPath);
        File.Delete(yamlPath);
        File.Delete(tomlPath);

        Console.WriteLine("\n[示例 2 完成]\n");
        await Task.CompletedTask;
    }
}
