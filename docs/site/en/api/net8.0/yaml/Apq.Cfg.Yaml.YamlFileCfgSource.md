### [Apq\.Cfg\.Yaml](Apq.Cfg.Yaml.md 'Apq\.Cfg\.Yaml')

## YamlFileCfgSource Class

YAML 文件配置源，支持读取和写入 YAML 格式的配置文件

```csharp
internal sealed class YamlFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Apq\.Cfg\.Sources\.File\.FileCfgSourceBase](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.file.filecfgsourcebase 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; YamlFileCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [YamlFileCfgSource\(string, int, bool, bool, bool, bool\)](Apq.Cfg.Yaml.YamlFileCfgSource.YamlFileCfgSource(string,int,bool,bool,bool,bool).md 'Apq\.Cfg\.Yaml\.YamlFileCfgSource\.YamlFileCfgSource\(string, int, bool, bool, bool, bool\)') | 初始化 YamlFileCfgSource 实例 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Yaml.YamlFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Yaml\.YamlFileCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 YAML 文件 |
| [BuildSource\(\)](Apq.Cfg.Yaml.YamlFileCfgSource.BuildSource().md 'Apq\.Cfg\.Yaml\.YamlFileCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的 YAML 配置源 |
| [SetYamlByColonKey\(YamlMappingNode, string, string\)](Apq.Cfg.Yaml.YamlFileCfgSource.SetYamlByColonKey(YamlDotNet.RepresentationModel.YamlMappingNode,string,string).md 'Apq\.Cfg\.Yaml\.YamlFileCfgSource\.SetYamlByColonKey\(YamlDotNet\.RepresentationModel\.YamlMappingNode, string, string\)') | 根据冒号分隔的键路径设置 YAML 映射节点中的值 |
