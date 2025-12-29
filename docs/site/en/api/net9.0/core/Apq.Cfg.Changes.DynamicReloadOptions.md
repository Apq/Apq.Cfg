#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Changes](Apq.Cfg.Changes.md 'Apq\.Cfg\.Changes')

## DynamicReloadOptions Class

动态重载选项

```csharp
public sealed class DynamicReloadOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; DynamicReloadOptions

| Properties | |
| :--- | :--- |
| [DebounceMs](Apq.Cfg.Changes.DynamicReloadOptions.DebounceMs.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.DebounceMs') | 防抖时间窗口（毫秒），默认 100ms |
| [EnableDynamicReload](Apq.Cfg.Changes.DynamicReloadOptions.EnableDynamicReload.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.EnableDynamicReload') | 是否启用动态重载，默认 true |
| [HistorySize](Apq.Cfg.Changes.DynamicReloadOptions.HistorySize.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.HistorySize') | 保留的变更历史记录数量（0 表示不记录），默认 0 |
| [KeyPrefixFilters](Apq.Cfg.Changes.DynamicReloadOptions.KeyPrefixFilters.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.KeyPrefixFilters') | 键前缀过滤器，只监听匹配这些前缀的键变更（null 或空表示监听所有） |
| [RollbackOnError](Apq.Cfg.Changes.DynamicReloadOptions.RollbackOnError.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.RollbackOnError') | 重载失败时是否回滚到之前的配置，默认 true |
| [Strategy](Apq.Cfg.Changes.DynamicReloadOptions.Strategy.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions\.Strategy') | 重载策略，默认 Eager（立即重载） |
