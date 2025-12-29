#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Security](Apq.Cfg.Security.md 'Apq\.Cfg\.Security').[IValueTransformer](Apq.Cfg.Security.IValueTransformer.md 'Apq\.Cfg\.Security\.IValueTransformer')

## IValueTransformer\.TransformOnWrite\(string, string\) Method

写入时转换（如加密）

```csharp
string? TransformOnWrite(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Security.IValueTransformer.TransformOnWrite(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Security.IValueTransformer.TransformOnWrite(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
转换后的值