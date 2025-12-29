#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources')

## JsonFileCfgSource Class

JSON 文件配置源

```csharp
internal sealed class JsonFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [FileCfgSourceBase](Apq.Cfg.Sources.File.FileCfgSourceBase.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; JsonFileCfgSource

Implements [IWritableCfgSource](Apq.Cfg.Sources.IWritableCfgSource.md 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [ICfgSource](Apq.Cfg.Sources.ICfgSource.md 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [JsonFileCfgSource\(string, int, bool, bool, bool, bool, EncodingOptions\)](Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).md 'Apq\.Cfg\.Sources\.JsonFileCfgSource\.JsonFileCfgSource\(string, int, bool, bool, bool, bool, Apq\.Cfg\.EncodingSupport\.EncodingOptions\)') | 初始化 JsonFileCfgSource 实例 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Sources.JsonFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Sources\.JsonFileCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 JSON 文件 |
| [BuildSource\(\)](Apq.Cfg.Sources.JsonFileCfgSource.BuildSource().md 'Apq\.Cfg\.Sources\.JsonFileCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的 JSON 配置源 |
| [SetByColonKey\(JsonObject, string, string\)](Apq.Cfg.Sources.JsonFileCfgSource.SetByColonKey(System.Text.Json.Nodes.JsonObject,string,string).md 'Apq\.Cfg\.Sources\.JsonFileCfgSource\.SetByColonKey\(System\.Text\.Json\.Nodes\.JsonObject, string, string\)') | 根据冒号分隔的键路径设置 JSON 对象中的值 |
