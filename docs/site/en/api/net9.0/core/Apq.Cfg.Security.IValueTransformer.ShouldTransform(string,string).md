#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Security](Apq.Cfg.Security.md 'Apq\.Cfg\.Security').[IValueTransformer](Apq.Cfg.Security.IValueTransformer.md 'Apq\.Cfg\.Security\.IValueTransformer')

## IValueTransformer\.ShouldTransform\(string, string\) Method

判断是否应该处理该键

```csharp
bool ShouldTransform(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Security.IValueTransformer.ShouldTransform(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Security.IValueTransformer.ShouldTransform(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
如果应该处理返回 true，否则返回 false