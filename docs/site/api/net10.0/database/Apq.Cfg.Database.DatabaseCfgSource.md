### [Apq\.Cfg\.Database](Apq.Cfg.Database.md 'Apq\.Cfg\.Database')

## DatabaseCfgSource Class

数据库配置源

```csharp
internal sealed class DatabaseCfgSource : Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; DatabaseCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [DatabaseCfgSource\(DatabaseOptions, int, bool\)](Apq.Cfg.Database.DatabaseCfgSource.DatabaseCfgSource(Apq.Cfg.Database.DatabaseOptions,int,bool).md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.DatabaseCfgSource\(Apq\.Cfg\.Database\.DatabaseOptions, int, bool\)') | 初始化 DatabaseCfgSource 实例 |

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Database.DatabaseCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标 |
| [IsWriteable](Apq.Cfg.Database.DatabaseCfgSource.IsWriteable.md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.IsWriteable') | 获取是否可写，数据库支持通过 API 写入配置，因此始终为 true |
| [Level](Apq.Cfg.Database.DatabaseCfgSource.Level.md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Database.DatabaseCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到数据库 |
| [BuildSource\(\)](Apq.Cfg.Database.DatabaseCfgSource.BuildSource().md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的内存配置源，从数据库加载数据 |
| [CreateProvider\(string\)](Apq.Cfg.Database.DatabaseCfgSource.CreateProvider(string).md 'Apq\.Cfg\.Database\.DatabaseCfgSource\.CreateProvider\(string\)') | 根据提供程序名称创建 SqlSugar 数据库提供程序 |
