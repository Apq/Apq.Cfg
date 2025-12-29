#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.Reload\(\) Method

手动触发配置重载（用于 Manual 和 Lazy 策略）

```csharp
public void Reload();
```

### Example

```csharp
// 手动重载配置
cfg.Reload();

// 在特定条件下重载
if (ShouldReload())
{
    cfg.Reload();
}
```

### Remarks
对于 Manual 和 Lazy 策略，此方法会立即检查所有配置源并应用更改。
对于 Automatic 策略，此方法不会产生额外效果，因为配置会自动重载。