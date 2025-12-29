#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## FastReadOnlySet\<T\> Class

高性能只读集合包装器
在 \.NET 8\+ 使用 FrozenSet，其他版本使用 HashSet

```csharp
internal sealed class FastReadOnlySet<T> : System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable
    where T : notnull
```
#### Type parameters

<a name='Apq.Cfg.Internal.FastReadOnlySet_T_.T'></a>

`T`

元素类型

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FastReadOnlySet\<T\>

Implements [System\.Collections\.Generic\.IReadOnlyCollection&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1 'System\.Collections\.Generic\.IReadOnlyCollection\`1')[T](Apq.Cfg.Internal.FastReadOnlySet_T_.md#Apq.Cfg.Internal.FastReadOnlySet_T_.T 'Apq\.Cfg\.Internal\.FastReadOnlySet\<T\>\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1 'System\.Collections\.Generic\.IReadOnlyCollection\`1'), [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[T](Apq.Cfg.Internal.FastReadOnlySet_T_.md#Apq.Cfg.Internal.FastReadOnlySet_T_.T 'Apq\.Cfg\.Internal\.FastReadOnlySet\<T\>\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1'), [System\.Collections\.IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable 'System\.Collections\.IEnumerable')