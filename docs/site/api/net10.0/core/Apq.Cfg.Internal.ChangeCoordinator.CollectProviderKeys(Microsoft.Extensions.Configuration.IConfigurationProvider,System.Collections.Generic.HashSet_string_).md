#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ChangeCoordinator](Apq.Cfg.Internal.ChangeCoordinator.md 'Apq\.Cfg\.Internal\.ChangeCoordinator')

## ChangeCoordinator\.CollectProviderKeys\(IConfigurationProvider, HashSet\<string\>\) Method

收集 Provider 的所有键到目标集合（避免分配新集合）

```csharp
private static void CollectProviderKeys(Microsoft.Extensions.Configuration.IConfigurationProvider provider, System.Collections.Generic.HashSet<string> targetKeys);
```
#### Parameters

<a name='Apq.Cfg.Internal.ChangeCoordinator.CollectProviderKeys(Microsoft.Extensions.Configuration.IConfigurationProvider,System.Collections.Generic.HashSet_string_).provider'></a>

`provider` [Microsoft\.Extensions\.Configuration\.IConfigurationProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationprovider 'Microsoft\.Extensions\.Configuration\.IConfigurationProvider')

<a name='Apq.Cfg.Internal.ChangeCoordinator.CollectProviderKeys(Microsoft.Extensions.Configuration.IConfigurationProvider,System.Collections.Generic.HashSet_string_).targetKeys'></a>

`targetKeys` [System\.Collections\.Generic\.HashSet&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1 'System\.Collections\.Generic\.HashSet\`1')