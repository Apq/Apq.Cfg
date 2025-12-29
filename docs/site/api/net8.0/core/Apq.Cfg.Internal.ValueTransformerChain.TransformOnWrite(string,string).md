#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueTransformerChain](Apq.Cfg.Internal.ValueTransformerChain.md 'Apq\.Cfg\.Internal\.ValueTransformerChain')

## ValueTransformerChain\.TransformOnWrite\(string, string\) Method

写入时转换（如加密）

```csharp
public string? TransformOnWrite(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueTransformerChain.TransformOnWrite(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Internal.ValueTransformerChain.TransformOnWrite(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值（明文）

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
转换后的值（加密后）