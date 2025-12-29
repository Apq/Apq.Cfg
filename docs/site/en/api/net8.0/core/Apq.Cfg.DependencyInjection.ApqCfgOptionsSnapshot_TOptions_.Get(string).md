#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection').[ApqCfgOptionsSnapshot&lt;TOptions&gt;](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>')

## ApqCfgOptionsSnapshot\<TOptions\>\.Get\(string\) Method

Returns a configured [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions') instance with the given [name](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).name 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.Get\(string\)\.name')\.

```csharp
public TOptions Get(string? name);
```
#### Parameters

<a name='Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions') instance, if [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')[Microsoft\.Extensions\.Options\.Options\.DefaultName](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.options.defaultname 'Microsoft\.Extensions\.Options\.Options\.DefaultName') is used\.

Implements [Get\(string\)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionssnapshot-1.get#microsoft-extensions-options-ioptionssnapshot-1-get(system-string) 'Microsoft\.Extensions\.Options\.IOptionsSnapshot\`1\.Get\(System\.String\)')

#### Returns
[TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions')  
The [TOptions](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.TOptions 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.TOptions') instance that matches the given [name](Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).md#Apq.Cfg.DependencyInjection.ApqCfgOptionsSnapshot_TOptions_.Get(string).name 'Apq\.Cfg\.DependencyInjection\.ApqCfgOptionsSnapshot\<TOptions\>\.Get\(string\)\.name')\.