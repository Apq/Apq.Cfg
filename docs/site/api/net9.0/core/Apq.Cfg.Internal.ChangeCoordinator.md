#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## ChangeCoordinator Class

变更协调器，负责协调多个配置源的变更事件

```csharp
internal sealed class ChangeCoordinator : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ChangeCoordinator

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Properties | |
| :--- | :--- |
| [HasPendingChanges](Apq.Cfg.Internal.ChangeCoordinator.HasPendingChanges.md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.HasPendingChanges') | 检查是否有待处理的变更（用于 Lazy 策略） |

| Methods | |
| :--- | :--- |
| [ClearHistory\(\)](Apq.Cfg.Internal.ChangeCoordinator.ClearHistory().md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.ClearHistory\(\)') | 清空变更历史记录 |
| [CollectProviderKeys\(IConfigurationProvider, HashSet&lt;string&gt;\)](Apq.Cfg.Internal.ChangeCoordinator.CollectProviderKeys(Microsoft.Extensions.Configuration.IConfigurationProvider,System.Collections.Generic.HashSet_string_).md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.CollectProviderKeys\(Microsoft\.Extensions\.Configuration\.IConfigurationProvider, System\.Collections\.Generic\.HashSet\<string\>\)') | 收集 Provider 的所有键到目标集合（避免分配新集合） |
| [EnsureLatest\(\)](Apq.Cfg.Internal.ChangeCoordinator.EnsureLatest().md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.EnsureLatest\(\)') | 确保配置是最新的（用于 Lazy 策略，访问前调用） |
| [GetHistory\(\)](Apq.Cfg.Internal.ChangeCoordinator.GetHistory().md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.GetHistory\(\)') | 获取变更历史记录 |
| [GetSnapshot\(\)](Apq.Cfg.Internal.ChangeCoordinator.GetSnapshot().md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.GetSnapshot\(\)') | 获取当前合并快照（只读视图，无复制开销） |
| [Reload\(\)](Apq.Cfg.Internal.ChangeCoordinator.Reload().md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.Reload\(\)') | 手动触发重载（用于 Manual 和 Lazy 策略） |

| Events | |
| :--- | :--- |
| [OnMergedChanges](Apq.Cfg.Internal.ChangeCoordinator.OnMergedChanges.md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.OnMergedChanges') | 当合并后的配置发生变化时触发 |
| [OnMergedChangesAsync](Apq.Cfg.Internal.ChangeCoordinator.OnMergedChangesAsync.md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.OnMergedChangesAsync') | 当合并后的配置发生变化时触发（异步版本） |
| [OnReloadError](Apq.Cfg.Internal.ChangeCoordinator.OnReloadError.md 'Apq\.Cfg\.Internal\.ChangeCoordinator\.OnReloadError') | 当重载发生错误时触发 |
