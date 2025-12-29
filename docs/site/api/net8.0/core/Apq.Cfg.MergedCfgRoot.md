#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## MergedCfgRoot Class

合并配置根实现

```csharp
internal sealed class MergedCfgRoot : Apq.Cfg.ICfgRoot, System.IDisposable, System.IAsyncDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; MergedCfgRoot

Implements [ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable'), [System\.IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable 'System\.IAsyncDisposable')

| Properties | |
| :--- | :--- |
| [HasPendingChanges](Apq.Cfg.MergedCfgRoot.HasPendingChanges.md 'Apq\.Cfg\.MergedCfgRoot\.HasPendingChanges') | 检查是否有待处理的配置变更 |
| [ReloadErrors](Apq.Cfg.MergedCfgRoot.ReloadErrors.md 'Apq\.Cfg\.MergedCfgRoot\.ReloadErrors') | 重载错误事件（Rx 可观察序列） |

| Methods | |
| :--- | :--- |
| [ClearChangeHistory\(\)](Apq.Cfg.MergedCfgRoot.ClearChangeHistory().md 'Apq\.Cfg\.MergedCfgRoot\.ClearChangeHistory\(\)') | 清空配置变更历史记录 |
| [GetChangeHistory\(\)](Apq.Cfg.MergedCfgRoot.GetChangeHistory().md 'Apq\.Cfg\.MergedCfgRoot\.GetChangeHistory\(\)') | 获取配置变更历史记录 |
| [GetMasked\(string\)](Apq.Cfg.MergedCfgRoot.GetMasked(string).md 'Apq\.Cfg\.MergedCfgRoot\.GetMasked\(string\)') | 获取脱敏后的配置值（用于日志输出） |
| [GetMaskedSnapshot\(\)](Apq.Cfg.MergedCfgRoot.GetMaskedSnapshot().md 'Apq\.Cfg\.MergedCfgRoot\.GetMaskedSnapshot\(\)') | 获取所有配置的脱敏快照（用于调试） |
| [Reload\(\)](Apq.Cfg.MergedCfgRoot.Reload().md 'Apq\.Cfg\.MergedCfgRoot\.Reload\(\)') | 手动触发配置重载（用于 Manual 和 Lazy 策略） |
| [ToMicrosoftConfiguration\(\)](Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration() 'Apq\.Cfg\.MergedCfgRoot\.ToMicrosoftConfiguration\(\)') | 转换为 Microsoft Configuration（静态快照） |
| [ToMicrosoftConfiguration\(DynamicReloadOptions\)](Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions) 'Apq\.Cfg\.MergedCfgRoot\.ToMicrosoftConfiguration\(Apq\.Cfg\.Changes\.DynamicReloadOptions\)') | 转换为支持动态重载的 Microsoft Configuration |
