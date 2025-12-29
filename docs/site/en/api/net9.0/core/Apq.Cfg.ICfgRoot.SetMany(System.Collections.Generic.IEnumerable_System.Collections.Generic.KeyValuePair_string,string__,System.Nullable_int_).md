#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.SetMany\(IEnumerable\<KeyValuePair\<string,string\>\>, Nullable\<int\>\) Method

批量设置多个配置值，减少锁竞争

```csharp
void SetMany(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,string?>> values, System.Nullable<int> targetLevel=null);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.SetMany(System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_string,string__,System.Nullable_int_).values'></a>

`values` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Collections\.Generic\.KeyValuePair&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.keyvaluepair-2 'System\.Collections\.Generic\.KeyValuePair\`2')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要设置的键值对

<a name='Apq.Cfg.ICfgRoot.SetMany(System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_string,string__,System.Nullable_int_).targetLevel'></a>

`targetLevel` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

目标层级