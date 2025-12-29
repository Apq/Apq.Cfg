### [Apq\.Cfg\.Yaml](Apq.Cfg.Yaml.md 'Apq\.Cfg\.Yaml').[YamlFileCfgSource](Apq.Cfg.Yaml.YamlFileCfgSource.md 'Apq\.Cfg\.Yaml\.YamlFileCfgSource')

## YamlFileCfgSource\.SetYamlByColonKey\(YamlMappingNode, string, string\) Method

根据冒号分隔的键路径设置 YAML 映射节点中的值

```csharp
private static void SetYamlByColonKey(YamlDotNet.RepresentationModel.YamlMappingNode root, string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Yaml.YamlFileCfgSource.SetYamlByColonKey(YamlDotNet.RepresentationModel.YamlMappingNode,string,string).root'></a>

`root` [YamlDotNet\.RepresentationModel\.YamlMappingNode](https://learn.microsoft.com/en-us/dotnet/api/yamldotnet.representationmodel.yamlmappingnode 'YamlDotNet\.RepresentationModel\.YamlMappingNode')

YAML 映射根节点

<a name='Apq.Cfg.Yaml.YamlFileCfgSource.SetYamlByColonKey(YamlDotNet.RepresentationModel.YamlMappingNode,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

冒号分隔的键路径（如 "Database:Connection:Timeout"）

<a name='Apq.Cfg.Yaml.YamlFileCfgSource.SetYamlByColonKey(YamlDotNet.RepresentationModel.YamlMappingNode,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

要设置的值，为 null 时设置为空字符串