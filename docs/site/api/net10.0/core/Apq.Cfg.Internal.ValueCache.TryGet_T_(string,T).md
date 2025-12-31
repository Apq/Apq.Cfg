#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueCache](Apq.Cfg.Internal.ValueCache.md 'Apq\.Cfg\.Internal\.ValueCache')

## ValueCache\.TryGet\<T\>\(string, T\) Method

尝试从缓存获取值

```csharp
public bool TryGet<T>(string key, out T? value);
```
#### Type parameters

<a name='Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).value'></a>

`value` [T](Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).md#Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).T 'Apq\.Cfg\.Internal\.ValueCache\.TryGet\<T\>\(string, T\)\.T')

缓存的值

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
是否命中缓存