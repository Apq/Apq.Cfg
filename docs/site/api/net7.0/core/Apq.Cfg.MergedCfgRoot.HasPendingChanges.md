#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.HasPendingChanges Property

检查是否有待处理的配置变更

```csharp
public bool HasPendingChanges { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

### Example

```csharp
// 检查是否有待保存的更改
if (cfg.HasPendingChanges)
{
    await cfg.SaveAsync();
}
```

### Remarks
待处理的配置变更是指通过 Set 或 SetMany 方法设置但尚未通过 SaveAsync 保存的更改。
此属性在检查前会确保配置是最新的（Lazy 策略）。