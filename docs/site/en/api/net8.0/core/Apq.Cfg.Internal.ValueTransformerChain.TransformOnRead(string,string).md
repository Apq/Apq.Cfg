#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueTransformerChain](Apq.Cfg.Internal.ValueTransformerChain.md 'Apq\.Cfg\.Internal\.ValueTransformerChain')

## ValueTransformerChain\.TransformOnRead\(string, string\) Method

读取时转换（如解密），带缓存优化

```csharp
public string? TransformOnRead(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueTransformerChain.TransformOnRead(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Internal.ValueTransformerChain.TransformOnRead(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值（原始值，可能是加密的）

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
转换后的值（明文）

### Remarks
性能优化策略：
1\. 快速路径：如果缓存中有解密结果，直接返回
2\. 快速路径：如果键已知不需要转换，直接返回原值
3\. 慢路径：执行转换并缓存结果