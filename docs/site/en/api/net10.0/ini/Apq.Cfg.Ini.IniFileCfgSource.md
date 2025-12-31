### [Apq\.Cfg\.Ini](Apq.Cfg.Ini.md 'Apq\.Cfg\.Ini')

## IniFileCfgSource Class

INI 文件配置源，支持读取和写入 INI 格式的配置文件

```csharp
internal sealed class IniFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Apq\.Cfg\.Sources\.File\.FileCfgSourceBase](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.file.filecfgsourcebase 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; IniFileCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [IniFileCfgSource\(string, int, bool, bool, bool, bool\)](Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).md 'Apq\.Cfg\.Ini\.IniFileCfgSource\.IniFileCfgSource\(string, int, bool, bool, bool, bool\)') | 初始化 IniFileCfgSource 实例 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Ini.IniFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Ini\.IniFileCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 INI 文件 |
| [BuildSource\(\)](Apq.Cfg.Ini.IniFileCfgSource.BuildSource().md 'Apq\.Cfg\.Ini\.IniFileCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的 INI 配置源 |
