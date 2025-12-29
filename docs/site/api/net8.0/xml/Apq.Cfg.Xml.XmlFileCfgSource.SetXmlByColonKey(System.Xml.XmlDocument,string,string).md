### [Apq\.Cfg\.Xml](Apq.Cfg.Xml.md 'Apq\.Cfg\.Xml').[XmlFileCfgSource](Apq.Cfg.Xml.XmlFileCfgSource.md 'Apq\.Cfg\.Xml\.XmlFileCfgSource')

## XmlFileCfgSource\.SetXmlByColonKey\(XmlDocument, string, string\) Method

根据冒号分隔的键路径设置 XML 文档中的节点值

```csharp
private static void SetXmlByColonKey(System.Xml.XmlDocument doc, string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Xml.XmlFileCfgSource.SetXmlByColonKey(System.Xml.XmlDocument,string,string).doc'></a>

`doc` [System\.Xml\.XmlDocument](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmldocument 'System\.Xml\.XmlDocument')

XML 文档

<a name='Apq.Cfg.Xml.XmlFileCfgSource.SetXmlByColonKey(System.Xml.XmlDocument,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

冒号分隔的键路径（如 "Database:Connection:Timeout"）

<a name='Apq.Cfg.Xml.XmlFileCfgSource.SetXmlByColonKey(System.Xml.XmlDocument,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

要设置的值，为 null 时设置为空字符串