#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.GetChildKeys\(\) Method

获取所有子节的键名

```csharp
System.Collections.Generic.IEnumerable<string> GetChildKeys();
```

#### Returns
[System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')  
子节键名集合

### Example

```csharp
var dbSection = cfg.GetSection("Database");
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection.Get(key)}");
}
```