#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Security](Apq.Cfg.Security.md 'Apq\.Cfg\.Security').[IValueMasker](Apq.Cfg.Security.IValueMasker.md 'Apq\.Cfg\.Security\.IValueMasker')

## IValueMasker\.ShouldMask\(string\) Method

判断是否应该脱敏该键

```csharp
bool ShouldMask(string key);
```
#### Parameters

<a name='Apq.Cfg.Security.IValueMasker.ShouldMask(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
如果应该脱敏返回 true，否则返回 false