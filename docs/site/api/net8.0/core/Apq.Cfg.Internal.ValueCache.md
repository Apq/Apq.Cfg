#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## ValueCache Class

配置值缓存，用于缓存类型转换结果

```csharp
internal sealed class ValueCache
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ValueCache

| Properties | |
| :--- | :--- |
| [Version](Apq.Cfg.Internal.ValueCache.Version.md 'Apq\.Cfg\.Internal\.ValueCache\.Version') | 当前缓存版本 |

| Methods | |
| :--- | :--- |
| [Clear\(\)](Apq.Cfg.Internal.ValueCache.Clear().md 'Apq\.Cfg\.Internal\.ValueCache\.Clear\(\)') | 清空所有缓存 |
| [GetStats\(\)](Apq.Cfg.Internal.ValueCache.GetStats().md 'Apq\.Cfg\.Internal\.ValueCache\.GetStats\(\)') | 获取缓存统计信息 |
| [Invalidate\(string\)](Apq.Cfg.Internal.ValueCache.Invalidate.md#Apq.Cfg.Internal.ValueCache.Invalidate(string) 'Apq\.Cfg\.Internal\.ValueCache\.Invalidate\(string\)') | 使指定键的所有类型缓存失效 |
| [Invalidate\(IEnumerable&lt;string&gt;\)](Apq.Cfg.Internal.ValueCache.Invalidate.md#Apq.Cfg.Internal.ValueCache.Invalidate(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.Internal\.ValueCache\.Invalidate\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 使多个键的缓存失效 |
| [Set&lt;T&gt;\(string, T\)](Apq.Cfg.Internal.ValueCache.Set_T_(string,T).md 'Apq\.Cfg\.Internal\.ValueCache\.Set\<T\>\(string, T\)') | 设置缓存值 |
| [TryGet&lt;T&gt;\(string, T\)](Apq.Cfg.Internal.ValueCache.TryGet_T_(string,T).md 'Apq\.Cfg\.Internal\.ValueCache\.TryGet\<T\>\(string, T\)') | 尝试从缓存获取值 |
