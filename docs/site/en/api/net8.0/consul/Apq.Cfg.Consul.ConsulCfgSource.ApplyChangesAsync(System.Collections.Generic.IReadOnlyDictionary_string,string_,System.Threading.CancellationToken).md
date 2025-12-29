### [Apq\.Cfg\.Consul](Apq.Cfg.Consul.md 'Apq\.Cfg\.Consul').[ConsulCfgSource](Apq.Cfg.Consul.ConsulCfgSource.md 'Apq\.Cfg\.Consul\.ConsulCfgSource')

## ConsulCfgSource\.ApplyChangesAsync\(IReadOnlyDictionary\<string,string\>, CancellationToken\) Method

应用配置更改到 Consul

```csharp
public System.Threading.Tasks.Task ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary<string,string?> changes, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='Apq.Cfg.Consul.ConsulCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).changes'></a>

`changes` [System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')

要应用的配置更改

<a name='Apq.Cfg.Consul.ConsulCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

取消令牌

Implements [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource.applychangesasync#apq-cfg-sources-iwritablecfgsource-applychangesasync(system-collections-generic-ireadonlydictionary{system-string-system-string}-system-threading-cancellationtoken) 'Apq\.Cfg\.Sources\.IWritableCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\{System\.String,System\.String\},System\.Threading\.CancellationToken\)')

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')  
表示异步操作的任务

#### Exceptions

[System\.ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception 'System\.ObjectDisposedException')  
当对象已释放时抛出