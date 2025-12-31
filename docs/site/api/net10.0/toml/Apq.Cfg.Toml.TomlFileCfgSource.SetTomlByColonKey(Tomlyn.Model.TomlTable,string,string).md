### [Apq\.Cfg\.Toml](Apq.Cfg.Toml.md 'Apq\.Cfg\.Toml').[TomlFileCfgSource](Apq.Cfg.Toml.TomlFileCfgSource.md 'Apq\.Cfg\.Toml\.TomlFileCfgSource')

## TomlFileCfgSource\.SetTomlByColonKey\(TomlTable, string, string\) Method

根据冒号分隔的键路径设置 TOML 表中的值

```csharp
private static void SetTomlByColonKey(Tomlyn.Model.TomlTable root, string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Toml.TomlFileCfgSource.SetTomlByColonKey(Tomlyn.Model.TomlTable,string,string).root'></a>

`root` [Tomlyn\.Model\.TomlTable](https://learn.microsoft.com/en-us/dotnet/api/tomlyn.model.tomltable 'Tomlyn\.Model\.TomlTable')

TOML 根表

<a name='Apq.Cfg.Toml.TomlFileCfgSource.SetTomlByColonKey(Tomlyn.Model.TomlTable,string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

冒号分隔的键路径（如 "Database:Connection:Timeout"）

<a name='Apq.Cfg.Toml.TomlFileCfgSource.SetTomlByColonKey(Tomlyn.Model.TomlTable,string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

要设置的值，为 null 时设置为空字符串