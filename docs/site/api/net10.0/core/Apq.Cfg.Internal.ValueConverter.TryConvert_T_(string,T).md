#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueConverter](Apq.Cfg.Internal.ValueConverter.md 'Apq\.Cfg\.Internal\.ValueConverter')

## ValueConverter\.TryConvert\<T\>\(string, T\) Method

尝试将字符串值转换为目标类型

```csharp
public static bool TryConvert<T>(string? value, out T? result);
```
#### Type parameters

<a name='Apq.Cfg.Internal.ValueConverter.TryConvert_T_(string,T).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.Internal.ValueConverter.TryConvert_T_(string,T).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

字符串值

<a name='Apq.Cfg.Internal.ValueConverter.TryConvert_T_(string,T).result'></a>

`result` [T](Apq.Cfg.Internal.ValueConverter.TryConvert_T_(string,T).md#Apq.Cfg.Internal.ValueConverter.TryConvert_T_(string,T).T 'Apq\.Cfg\.Internal\.ValueConverter\.TryConvert\<T\>\(string, T\)\.T')

转换结果

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
转换是否成功