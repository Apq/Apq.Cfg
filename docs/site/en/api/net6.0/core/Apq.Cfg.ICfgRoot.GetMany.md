#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.GetMany Method

| Overloads | |
| :--- | :--- |
| [GetMany\(IEnumerable&lt;string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 批量获取多个配置值，减少锁竞争 |
| [GetMany\(IEnumerable&lt;string&gt;, Action&lt;string,string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_,System.Action_string,string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Action\<string,string\>\)') | 高性能批量获取：通过回调方式返回结果，零堆分配 |
| [GetMany&lt;T&gt;\(IEnumerable&lt;string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 批量获取多个配置值并转换为指定类型 |
| [GetMany&lt;T&gt;\(IEnumerable&lt;string&gt;, Action&lt;string,T&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_) 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Action\<string,T\>\)') | 高性能批量获取：通过回调方式返回结果并转换类型，零堆分配 |

<a name='Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_)'></a>

## ICfgRoot\.GetMany\(IEnumerable\<string\>\) Method

批量获取多个配置值，减少锁竞争

```csharp
System.Collections.Generic.IReadOnlyDictionary<string,string?> GetMany(System.Collections.Generic.IEnumerable<string> keys);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要获取的键集合

#### Returns
[System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')  
键值对字典

<a name='Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_,System.Action_string,string_)'></a>

## ICfgRoot\.GetMany\(IEnumerable\<string\>, Action\<string,string\>\) Method

高性能批量获取：通过回调方式返回结果，零堆分配

```csharp
void GetMany(System.Collections.Generic.IEnumerable<string> keys, System.Action<string,string?> onValue);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_,System.Action_string,string_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要获取的键集合

<a name='Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_,System.Action_string,string_).onValue'></a>

`onValue` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

每个键值对的回调处理函数

### Remarks
此方法避免了 Dictionary 分配开销，适合高频调用场景。
回调会按键的顺序依次调用。

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_)'></a>

## ICfgRoot\.GetMany\<T\>\(IEnumerable\<string\>\) Method

批量获取多个配置值并转换为指定类型

```csharp
System.Collections.Generic.IReadOnlyDictionary<string,T?> GetMany<T>(System.Collections.Generic.IEnumerable<string> keys);
```
#### Type parameters

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要获取的键集合

#### Returns
[System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[T](Apq.Cfg.ICfgRoot.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_).T 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')  
键值对字典

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_)'></a>

## ICfgRoot\.GetMany\<T\>\(IEnumerable\<string\>, Action\<string,T\>\) Method

高性能批量获取：通过回调方式返回结果并转换类型，零堆分配

```csharp
void GetMany<T>(System.Collections.Generic.IEnumerable<string> keys, System.Action<string,T?> onValue);
```
#### Type parameters

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要获取的键集合

<a name='Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_).onValue'></a>

`onValue` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[T](Apq.Cfg.ICfgRoot.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_).T 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Action\<string,T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

每个键值对的回调处理函数

### Remarks
此方法避免了 Dictionary 分配开销，适合高频调用场景。
回调会按键的顺序依次调用。