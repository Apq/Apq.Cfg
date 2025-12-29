### [Apq\.Cfg\.Xml](Apq.Cfg.Xml.md 'Apq\.Cfg\.Xml')

## XmlFileCfgSource Class

XML 文件配置源，支持读取和写入 XML 格式的配置文件

```csharp
internal sealed class XmlFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Apq\.Cfg\.Sources\.File\.FileCfgSourceBase](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.file.filecfgsourcebase 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; XmlFileCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [XmlFileCfgSource\(string, int, bool, bool, bool, bool\)](Apq.Cfg.Xml.XmlFileCfgSource.XmlFileCfgSource(string,int,bool,bool,bool,bool).md 'Apq\.Cfg\.Xml\.XmlFileCfgSource\.XmlFileCfgSource\(string, int, bool, bool, bool, bool\)') | 初始化 XmlFileCfgSource 实例 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Xml.XmlFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Xml\.XmlFileCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 XML 文件 |
| [BuildSource\(\)](Apq.Cfg.Xml.XmlFileCfgSource.BuildSource().md 'Apq\.Cfg\.Xml\.XmlFileCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的 XML 配置源 |
| [SetXmlByColonKey\(XmlDocument, string, string\)](Apq.Cfg.Xml.XmlFileCfgSource.SetXmlByColonKey(System.Xml.XmlDocument,string,string).md 'Apq\.Cfg\.Xml\.XmlFileCfgSource\.SetXmlByColonKey\(System\.Xml\.XmlDocument, string, string\)') | 根据冒号分隔的键路径设置 XML 文档中的节点值 |
