#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection').[ApqCfgOptionsMonitor&lt;TOptions&gt;](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>')

## ApqCfgOptionsMonitor\<TOptions\>\.OnChange\(Action\<TOptions,string\>\) Method

Registers a listener to be called whenever a named [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions') changes\.

```csharp
public System.IDisposable? OnChange(System.Action<TOptions,string?> listener);
```
#### Parameters

<a name='Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.OnChange(System.Action_TOptions,string_).listener'></a>

`listener` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

The action to be invoked when [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsMonitor_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsMonitor\<TOptions\>\.TOptions') has changed\.

Implements [OnChange\(Action&lt;TOptions,string&gt;\)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsmonitor-1.onchange#microsoft-extensions-options-ioptionsmonitor-1-onchange(system-action{-0-system-string}) 'Microsoft\.Extensions\.Options\.IOptionsMonitor\`1\.OnChange\(System\.Action\{\`0,System\.String\}\)')

#### Returns
[System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')  
An [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable') that should be disposed to stop listening for changes\.