#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources')

## IWritableCfgSource Interface

可写配置源接口，继承自 ICfgSource，增加了写入配置的能力

```csharp
public interface IWritableCfgSource : Apq.Cfg.Sources.ICfgSource
```

Derived  
&#8627; [JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

Implements [ICfgSource](Apq.Cfg.Sources.ICfgSource.md 'Apq\.Cfg\.Sources\.ICfgSource')

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Sources.IWritableCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Sources\.IWritableCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改 |
