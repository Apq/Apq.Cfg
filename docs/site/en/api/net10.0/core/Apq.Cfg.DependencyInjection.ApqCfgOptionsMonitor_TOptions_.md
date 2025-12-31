#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection')

## ApqCfgOptionsMonitor\<TOptions\> Class

支持配置变更自动更新的 IOptionsMonitor 实现

```csharp
public sealed class ApqCfgOptionsMonitor<TOptions> : Microsoft.Extensions.Options.IOptionsMonitor<TOptions>, System.IDisposable
    where TOptions : class, new()
```
#### Type parameters

<a name='Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions'></a>

`TOptions`

配置选项类型

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ApqCfgOptionsMonitor\<TOptions\>

Implements [Microsoft\.Extensions\.Options\.IOptionsMonitor&lt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsmonitor-1 'Microsoft\.Extensions\.Options\.IOptionsMonitor\`1')[TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsmonitor-1 'Microsoft\.Extensions\.Options\.IOptionsMonitor\`1'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Constructors | |
| :--- | :--- |
| [ApqCfgOptionsMonitor\(ICfgRoot, string\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.ApqCfgOptionsMonitor(Apq.Cfg.ICfgRoot,string).md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.ApqCfgOptionsMonitor\(Apq\.Cfg\.ICfgRoot, string\)') | 创建 ApqCfgOptionsMonitor 实例 |

| Properties | |
| :--- | :--- |
| [CurrentValue](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.CurrentValue.md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.CurrentValue') | Gets the current [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions') instance with the [Microsoft\.Extensions\.Options\.Options\.DefaultName](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.options.defaultname 'Microsoft\.Extensions\.Options\.Options\.DefaultName')\. |

| Methods | |
| :--- | :--- |
| [Dispose\(\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.Dispose().md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.Dispose\(\)') | Performs application\-defined tasks associated with freeing, releasing, or resetting unmanaged resources\. |
| [Get\(string\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.Get(string).md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.Get\(string\)') | Returns a configured [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions') instance with the given [name](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.Get(string).md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.Get(string).name 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.Get\(string\)\.name')\. |
| [GetChangeToken\(\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.GetChangeToken().md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.GetChangeToken\(\)') | 获取变更令牌 |
| [OnChange\(Action&lt;TOptions,string&gt;\)](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.OnChange(System.Action_TOptions,string_).md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.OnChange\(System\.Action\<TOptions,string\>\)') | Registers a listener to be called whenever a named [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions') changes\. |
