#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.ClearChangeHistory\(\) Method

清空配置变更历史记录

```csharp
public void ClearChangeHistory();
```

### Example

```csharp
// 在特定条件下清空历史记录
if (historyTooLarge)
{
    cfg.ClearChangeHistory();
}
```

### Remarks
清空历史记录不会影响当前配置值，只是删除变更事件的记录。
此操作不可逆，清空后无法恢复历史记录。