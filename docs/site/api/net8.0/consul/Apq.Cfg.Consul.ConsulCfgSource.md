### [Apq\.Cfg\.Consul](Apq.Cfg.Consul.md 'Apq\.Cfg\.Consul')

## ConsulCfgSource Class

Consul 配置源

```csharp
internal sealed class ConsulCfgSource : Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ConsulCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Consul.ConsulCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标 |
| [IsWriteable](Apq.Cfg.Consul.ConsulCfgSource.IsWriteable.md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.IsWriteable') | 获取是否可写，Consul 支持通过 API 写入配置，因此始终为 true |
| [Level](Apq.Cfg.Consul.ConsulCfgSource.Level.md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Consul.ConsulCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 Consul |
| [BuildSource\(\)](Apq.Cfg.Consul.ConsulCfgSource.BuildSource().md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的配置源 |
| [Dispose\(\)](Apq.Cfg.Consul.ConsulCfgSource.Dispose().md 'Apq\.Cfg\.Consul\.ConsulCfgSource\.Dispose\(\)') | 释放资源，取消所有异步操作并释放 Consul 客户端 |
