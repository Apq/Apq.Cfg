#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueCache](Apq.Cfg.Internal.ValueCache.md 'Apq\.Cfg\.Internal\.ValueCache')

## ValueCache\.Invalidate Method

| Overloads | |
| :--- | :--- |
| [Invalidate\(string\)](Apq.Cfg.Internal.ValueCache.Invalidate.md#Apq.Cfg.Internal.ValueCache.Invalidate(string) 'Apq\.Cfg\.Internal\.ValueCache\.Invalidate\(string\)') | 使指定键的所有类型缓存失效 |
| [Invalidate\(IEnumerable&lt;string&gt;\)](Apq.Cfg.Internal.ValueCache.Invalidate.md#Apq.Cfg.Internal.ValueCache.Invalidate(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.Internal\.ValueCache\.Invalidate\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 使多个键的缓存失效 |

<a name='Apq.Cfg.Internal.ValueCache.Invalidate(string)'></a>

## ValueCache\.Invalidate\(string\) Method

使指定键的所有类型缓存失效

```csharp
public void Invalidate(string key);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueCache.Invalidate(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Internal.ValueCache.Invalidate(System.Collections.Generic.IEnumerable_string_)'></a>

## ValueCache\.Invalidate\(IEnumerable\<string\>\) Method

使多个键的缓存失效

```csharp
public void Invalidate(System.Collections.Generic.IEnumerable<string> keys);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueCache.Invalidate(System.Collections.Generic.IEnumerable_string_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

配置键集合