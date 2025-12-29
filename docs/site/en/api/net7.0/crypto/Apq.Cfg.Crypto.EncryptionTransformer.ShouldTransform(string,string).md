#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[EncryptionTransformer](Apq.Cfg.Crypto.EncryptionTransformer.md 'Apq\.Cfg\.Crypto\.EncryptionTransformer')

## EncryptionTransformer\.ShouldTransform\(string, string\) Method

判断是否应该处理该键

```csharp
public bool ShouldTransform(string key, string? value);
```
#### Parameters

<a name='Apq.Cfg.Crypto.EncryptionTransformer.ShouldTransform(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.Crypto.EncryptionTransformer.ShouldTransform(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置值

Implements [ShouldTransform\(string, string\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.security.ivaluetransformer.shouldtransform#apq-cfg-security-ivaluetransformer-shouldtransform(system-string-system-string) 'Apq\.Cfg\.Security\.IValueTransformer\.ShouldTransform\(System\.String,System\.String\)')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
如果应该处理返回 true，否则返回 false