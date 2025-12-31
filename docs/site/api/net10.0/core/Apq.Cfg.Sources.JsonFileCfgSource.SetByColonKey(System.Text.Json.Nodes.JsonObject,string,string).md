#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources').[JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

## JsonFileCfgSource\.SetByColonKey\(JsonObject, string, string\) Method

根据冒号分隔的键路径设置 JSON 对象中的值

```csharp
private static void SetByColonKey(System.Text.Json.Nodes.JsonObject root, string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Sources.JsonFileCfgSource.SetByColonKey(System.Text.Json.Nodes.JsonObject,string,string).root'></a>

`root` [System\.Text\.Json\.Nodes\.JsonObject](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject 'System\.Text\.Json\.Nodes\.JsonObject')

JSON 根对象

<a name='Apq.Cfg.Sources.JsonFileCfgSource.SetByColonKey(System.Text.Json.Nodes.JsonObject,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

冒号分隔的键路径（如 "Database:Connection:Timeout"）

<a name='Apq.Cfg.Sources.JsonFileCfgSource.SetByColonKey(System.Text.Json.Nodes.JsonObject,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

要设置的值，为 null 时删除该键