#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[EncryptionTransformer](Apq.Cfg.Crypto.EncryptionTransformer.md 'Apq\.Cfg\.Crypto\.EncryptionTransformer')

## EncryptionTransformer\.TransformOnRead\(string, string\) Method

读取时转换（解密）

```csharp
public string? TransformOnRead(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Crypto.EncryptionTransformer.TransformOnRead(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Crypto.EncryptionTransformer.TransformOnRead(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值

Implements [TransformOnRead\(string, string\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.security.ivaluetransformer.transformonread#apq-cfg-security-ivaluetransformer-transformonread(system-string-system-string) 'Apq\.Cfg\.Security\.IValueTransformer\.TransformOnRead\(System\.String,System\.String\)')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
解密后的值