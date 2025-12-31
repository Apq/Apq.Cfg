#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.GetChangeHistory\(\) Method

获取配置变更历史记录

```csharp
public System.Collections.Generic.IReadOnlyList<Apq.Cfg.Changes.ConfigChangeEvent> GetChangeHistory();
```

#### Returns
[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[ConfigChangeEvent](Apq.Cfg.Changes.ConfigChangeEvent.md 'Apq\.Cfg\.Changes\.ConfigChangeEvent')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')  
配置变更事件的只读列表，按时间顺序排列

### Example

```csharp
// 获取最近的配置变更
var changes = cfg.GetChangeHistory();
foreach (var change in changes.Take(10))
{
    Console.WriteLine($"[{change.Timestamp}] {change.Key}: {change.OldValue} -> {change.NewValue}");
}
```

### Remarks
变更历史记录包含所有已应用的配置更改，包括通过 SaveAsync 保存的更改和自动重载的更改。
历史记录的数量受 DynamicReloadOptions\.HistoryLimit 限制。