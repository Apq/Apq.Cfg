#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[FastCollections](Apq.Cfg.Internal.FastCollections.md 'Apq\.Cfg\.Internal\.FastCollections')

## FastCollections\.ToFastReadOnly Method

| Overloads | |
| :--- | :--- |
| [ToFastReadOnly&lt;TKey,TValue&gt;\(this Dictionary&lt;TKey,TValue&gt;\)](Apq.Cfg.Internal.FastCollections.ToFastReadOnly.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_) 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.Dictionary\<TKey,TValue\>\)') | 创建高性能只读字典 |
| [ToFastReadOnly&lt;TKey,TValue&gt;\(this IEnumerable&lt;KeyValuePair&lt;TKey,TValue&gt;&gt;\)](Apq.Cfg.Internal.FastCollections.ToFastReadOnly.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__) 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<TKey,TValue\>\>\)') | 创建高性能只读字典 |

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_)'></a>

## FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this Dictionary\<TKey,TValue\>\) Method

创建高性能只读字典

```csharp
public static Apq.Cfg.Internal.FastReadOnlyDictionary<TKey,TValue> ToFastReadOnly<TKey,TValue>(this System.Collections.Generic.Dictionary<TKey,TValue> source)
    where TKey : notnull;
```
#### Type parameters

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TKey'></a>

`TKey`

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TValue'></a>

`TValue`
#### Parameters

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).source'></a>

`source` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[TKey](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TKey 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.Dictionary\<TKey,TValue\>\)\.TKey')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[TValue](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TValue 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.Dictionary\<TKey,TValue\>\)\.TValue')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

#### Returns
[Apq\.Cfg\.Internal\.FastReadOnlyDictionary&lt;](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')[TKey](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TKey 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.Dictionary\<TKey,TValue\>\)\.TKey')[,](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')[TValue](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.Dictionary_TKey,TValue_).TValue 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.Dictionary\<TKey,TValue\>\)\.TValue')[&gt;](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__)'></a>

## FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this IEnumerable\<KeyValuePair\<TKey,TValue\>\>\) Method

创建高性能只读字典

```csharp
public static Apq.Cfg.Internal.FastReadOnlyDictionary<TKey,TValue> ToFastReadOnly<TKey,TValue>(this System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey,TValue>> source)
    where TKey : notnull;
```
#### Type parameters

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TKey'></a>

`TKey`

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TValue'></a>

`TValue`
#### Parameters

<a name='Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Collections\.Generic\.KeyValuePair&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TKey](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TKey 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<TKey,TValue\>\>\)\.TKey')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[TValue](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TValue 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<TKey,TValue\>\>\)\.TValue')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

#### Returns
[Apq\.Cfg\.Internal\.FastReadOnlyDictionary&lt;](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')[TKey](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TKey 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<TKey,TValue\>\>\)\.TKey')[,](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')[TValue](Apq.Cfg.Internal.FastCollections.md#Apq.Cfg.Internal.FastCollections.ToFastReadOnly_TKey,TValue_(thisSystem.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_TKey,TValue__).TValue 'Apq\.Cfg\.Internal\.FastCollections\.ToFastReadOnly\<TKey,TValue\>\(this System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<TKey,TValue\>\>\)\.TValue')[&gt;](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>')