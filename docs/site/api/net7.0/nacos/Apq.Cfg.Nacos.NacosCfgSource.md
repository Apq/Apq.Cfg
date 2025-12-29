### [Apq\.Cfg\.Nacos](Apq.Cfg.Nacos.md 'Apq\.Cfg\.Nacos')

## NacosCfgSource Class

Nacos 配置源，使用官方 SDK，支持热重载

```csharp
internal sealed class NacosCfgSource : Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; NacosCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Nacos.NacosCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标 |
| [IsWriteable](Apq.Cfg.Nacos.NacosCfgSource.IsWriteable.md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.IsWriteable') | 获取是否可写，Nacos 支持通过 API 写入配置，因此始终为 true |
| [Level](Apq.Cfg.Nacos.NacosCfgSource.Level.md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Nacos.NacosCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 Nacos |
| [BuildSource\(\)](Apq.Cfg.Nacos.NacosCfgSource.BuildSource().md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的配置源 |
| [Dispose\(\)](Apq.Cfg.Nacos.NacosCfgSource.Dispose().md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.Dispose\(\)') | 释放资源，移除监听器、关闭 Nacos 服务并释放服务提供程序 |
| [OnConfigChanged\(string\)](Apq.Cfg.Nacos.NacosCfgSource.OnConfigChanged(string).md 'Apq\.Cfg\.Nacos\.NacosCfgSource\.OnConfigChanged\(string\)') | 处理配置变更（由监听器调用） |
