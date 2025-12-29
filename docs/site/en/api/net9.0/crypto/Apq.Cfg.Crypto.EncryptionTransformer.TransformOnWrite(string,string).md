#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[EncryptionTransformer](Apq.Cfg.Crypto.EncryptionTransformer.md 'Apq\.Cfg\.Crypto\.EncryptionTransformer')

## EncryptionTransformer\.TransformOnWrite\(string, string\) Method

写入时转换（加密）

```csharp
public string? TransformOnWrite(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Crypto.EncryptionTransformer.TransformOnWrite(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Crypto.EncryptionTransformer.TransformOnWrite(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值

Implements [TransformOnWrite\(string, string\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.security.ivaluetransformer.transformonwrite#apq-cfg-security-ivaluetransformer-transformonwrite(system-string-system-string) 'Apq\.Cfg\.Security\.IValueTransformer\.TransformOnWrite\(System\.String,System\.String\)')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
加密后的值