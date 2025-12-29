#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources').[JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

## JsonFileCfgSource\.ApplyChangesAsync\(IReadOnlyDictionary\<string,string\>, CancellationToken\) Method

应用配置更改到 JSON 文件

```csharp
public System.Threading.Tasks.Task ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary<string,string?> changes, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='Apq.Cfg.Sources.JsonFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).changes'></a>

`changes` [System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')

要应用的配置更改

<a name='Apq.Cfg.Sources.JsonFileCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

取消令牌

Implements [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Sources.IWritableCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Sources\.IWritableCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)')

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')  
表示异步操作的任务

#### Exceptions

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
当配置源不可写或 JSON 根节点不是对象时抛出