#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## FastReadOnlyDictionary\<TKey,TValue\> Class

高性能只读字典包装器
在 \.NET 8\+ 使用 FrozenDictionary，其他版本使用普通 Dictionary

```csharp
internal sealed class FastReadOnlyDictionary<TKey,TValue> : System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IEnumerable, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>
    where TKey : notnull
```
#### Type parameters

<a name='Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TKey'></a>

`TKey`

键类型

<a name='Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TValue'></a>

`TValue`

值类型

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FastReadOnlyDictionary\<TKey,TValue\>

Implements [System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[TKey](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TKey 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TKey')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[TValue](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TValue 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TValue')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2'), [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Collections\.Generic\.KeyValuePair&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TKey](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TKey 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TKey')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TValue](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TValue 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TValue')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1'), [System\.Collections\.IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable 'System\.Collections\.IEnumerable'), [System\.Collections\.Generic\.IReadOnlyCollection&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1 'System\.Collections\.Generic\.IReadOnlyCollection\`1')[System\.Collections\.Generic\.KeyValuePair&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TKey](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TKey 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TKey')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TValue](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md#Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.TValue 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>\.TValue')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1 'System\.Collections\.Generic\.IReadOnlyCollection\`1')