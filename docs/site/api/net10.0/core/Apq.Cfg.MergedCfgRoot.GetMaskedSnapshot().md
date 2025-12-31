#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.GetMaskedSnapshot\(\) Method

获取所有配置的脱敏快照（用于调试）

```csharp
public System.Collections.Generic.IReadOnlyDictionary<string,string> GetMaskedSnapshot();
```

#### Returns
[System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')  
脱敏后的配置键值对字典

### Example

```csharp
// 获取脱敏快照用于调试
var snapshot = cfg.GetMaskedSnapshot();
foreach (var (key, value) in snapshot)
{
    Console.WriteLine($"{key}: {value}");
}
```