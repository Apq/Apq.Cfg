#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ChangeCoordinator](Apq.Cfg.Internal.ChangeCoordinator.md 'Apq\.Cfg\.Internal\.ChangeCoordinator')

## ChangeCoordinator\.OnMergedChanges Event

当合并后的配置发生变化时触发

```csharp
public event Action<IReadOnlyDictionary<string,ConfigChange>>? OnMergedChanges;
```

#### Event Type
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[ConfigChange](Apq.Cfg.Changes.ConfigChange.md 'Apq\.Cfg\.Changes\.ConfigChange')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')