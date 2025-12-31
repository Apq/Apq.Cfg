### [Apq\.Cfg\.Crypto\.DataProtection](Apq.Cfg.Crypto.DataProtection.md 'Apq\.Cfg\.Crypto\.DataProtection').[DataProtectionCryptoProvider](Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.md 'Apq\.Cfg\.Crypto\.DataProtection\.DataProtectionCryptoProvider')

## DataProtectionCryptoProvider\(IDataProtectionProvider, string\) Constructor

初始化 Data Protection 加密提供者

```csharp
public DataProtectionCryptoProvider(Microsoft.AspNetCore.DataProtection.IDataProtectionProvider provider, string purpose="Apq.Cfg");
```
#### Parameters

<a name='Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.DataProtectionCryptoProvider(Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string).provider'></a>

`provider` [Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.dataprotection.idataprotectionprovider 'Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider')

Data Protection 提供者

<a name='Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.DataProtectionCryptoProvider(Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string).purpose'></a>

`purpose` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

保护目的，用于隔离不同用途的加密数据