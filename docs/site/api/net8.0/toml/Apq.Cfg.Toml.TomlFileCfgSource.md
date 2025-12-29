### [Apq\.Cfg\.Toml](Apq.Cfg.Toml.md 'Apq\.Cfg\.Toml')

## TomlFileCfgSource Class

TOML 文件配置源，支持读取和写入 TOML 格式的配置文件

```csharp
internal sealed class TomlFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Apq\.Cfg\.Sources\.File\.FileCfgSourceBase](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.file.filecfgsourcebase 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; TomlFileCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [TomlFileCfgSource\(string, int, bool, bool, bool, bool\)](Apq.Cfg.Toml.TomlFileCfgSource.TomlFileCfgSource(string,int,bool,bool,bool,bool).md 'Apq\.Cfg\.Toml\.TomlFileCfgSource\.TomlFileCfgSource\(string, int, bool, bool, bool, bool\)') | 初始化 TomlFileCfgSource 实例 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Toml.TomlFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Toml\.TomlFileCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 TOML 文件 |
| [BuildSource\(\)](Apq.Cfg.Toml.TomlFileCfgSource.BuildSource().md 'Apq\.Cfg\.Toml\.TomlFileCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的 TOML 配置源 |
| [SetTomlByColonKey\(TomlTable, string, string\)](Apq.Cfg.Toml.TomlFileCfgSource.SetTomlByColonKey(Tomlyn.Model.TomlTable,string,string).md 'Apq\.Cfg\.Toml\.TomlFileCfgSource\.SetTomlByColonKey\(Tomlyn\.Model\.TomlTable, string, string\)') | 根据冒号分隔的键路径设置 TOML 表中的值 |
