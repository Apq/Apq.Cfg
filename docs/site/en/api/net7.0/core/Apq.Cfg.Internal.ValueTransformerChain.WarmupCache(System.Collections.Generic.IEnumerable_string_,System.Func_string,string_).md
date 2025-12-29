#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueTransformerChain](Apq.Cfg.Internal.ValueTransformerChain.md 'Apq\.Cfg\.Internal\.ValueTransformerChain')

## ValueTransformerChain\.WarmupCache\(IEnumerable\<string\>, Func\<string,string\>\) Method

预热缓存：预先解密所有加密值

```csharp
public void WarmupCache(System.Collections.Generic.IEnumerable<string> keys, System.Func<string,string?> valueGetter);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueTransformerChain.WarmupCache(System.Collections.Generic.IEnumerable_string_,System.Func_string,string_).keys'></a>

`keys` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

要预热的键集合

<a name='Apq.Cfg.Internal.ValueTransformerChain.WarmupCache(System.Collections.Generic.IEnumerable_string_,System.Func_string,string_).valueGetter'></a>

`valueGetter` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

获取原始值的委托

### Remarks
在应用启动时调用此方法可以避免首次访问时的解密延迟