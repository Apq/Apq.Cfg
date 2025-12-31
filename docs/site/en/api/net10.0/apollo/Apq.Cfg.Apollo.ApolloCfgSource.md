### [Apq\.Cfg\.Apollo](Apq.Cfg.Apollo.md 'Apq\.Cfg\.Apollo')

## ApolloCfgSource Class

Apollo 配置源

```csharp
internal sealed class ApolloCfgSource : Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ApolloCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Apollo.ApolloCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，Apollo 不支持写入，此值用于标识 |
| [IsWriteable](Apq.Cfg.Apollo.ApolloCfgSource.IsWriteable.md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.IsWriteable') | 获取是否可写，Apollo 不支持通过 API 写入配置，因此始终为 false |
| [Level](Apq.Cfg.Apollo.ApolloCfgSource.Level.md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Apollo.ApolloCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改（Apollo 不支持通过 API 写入配置） |
| [BuildSource\(\)](Apq.Cfg.Apollo.ApolloCfgSource.BuildSource().md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的配置源 |
| [Dispose\(\)](Apq.Cfg.Apollo.ApolloCfgSource.Dispose().md 'Apq\.Cfg\.Apollo\.ApolloCfgSource\.Dispose\(\)') | 释放资源，取消所有异步操作并释放 HTTP 客户端 |
