#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection')

## ApqCfgOptionsSnapshot\<TOptions\> Class

支持每次请求重新绑定的 IOptionsSnapshot 实现

```csharp
public sealed class ApqCfgOptionsSnapshot<TOptions> : Microsoft.Extensions.Options.IOptionsSnapshot<TOptions>, Microsoft.Extensions.Options.IOptions<TOptions>
    where TOptions : class, new()
```
#### Type parameters

<a name='Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions'></a>

`TOptions`

配置选项类型

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ApqCfgOptionsSnapshot\<TOptions\>

Implements [Microsoft\.Extensions\.Options\.IOptionsSnapshot&lt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionssnapshot-1 'Microsoft\.Extensions\.Options\.IOptionsSnapshot\`1')[TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionssnapshot-1 'Microsoft\.Extensions\.Options\.IOptionsSnapshot\`1'), [Microsoft\.Extensions\.Options\.IOptions&lt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptions-1 'Microsoft\.Extensions\.Options\.IOptions\`1')[TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptions-1 'Microsoft\.Extensions\.Options\.IOptions\`1')

| Constructors | |
| :--- | :--- |
| [ApqCfgOptionsSnapshot\(ICfgRoot, string\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.ApqCfgOptionsSnapshot(Apq.Cfg.ICfgRoot,string).md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.ApqCfgOptionsSnapshot\(Apq\.Cfg\.ICfgRoot, string\)') | 创建 ApqCfgOptionsSnapshot 实例 |

| Properties | |
| :--- | :--- |
| [Value](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Value.md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.Value') | The default configured [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions') instance |

| Methods | |
| :--- | :--- |
| [Get\(string\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.Get\(string\)') | Returns a configured [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions') instance with the given [name](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).name 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.Get\(string\)\.name')\. |
