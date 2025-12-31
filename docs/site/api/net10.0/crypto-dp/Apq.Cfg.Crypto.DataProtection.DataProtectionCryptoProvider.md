### [Apq\.Cfg\.Crypto\.DataProtection](Apq.Cfg.Crypto.DataProtection.md 'Apq\.Cfg\.Crypto\.DataProtection')

## DataProtectionCryptoProvider Class

使用 ASP\.NET Core Data Protection 的加密提供者

```csharp
public class DataProtectionCryptoProvider : Apq.Cfg.Crypto.ICryptoProvider
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; DataProtectionCryptoProvider

Implements [Apq\.Cfg\.Crypto\.ICryptoProvider](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.icryptoprovider 'Apq\.Cfg\.Crypto\.ICryptoProvider')

### Remarks
Data Protection 提供了跨机器、跨应用的密钥管理和加密功能。
适用于 ASP\.NET Core 应用程序，支持密钥轮换和撤销。

| Constructors | |
| :--- | :--- |
| [DataProtectionCryptoProvider\(IDataProtectionProvider, string\)](Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.DataProtectionCryptoProvider(Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string).md 'Apq\.Cfg\.Crypto\.DataProtection\.DataProtectionCryptoProvider\.DataProtectionCryptoProvider\(Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider, string\)') | 初始化 Data Protection 加密提供者 |

| Methods | |
| :--- | :--- |
| [Decrypt\(string\)](Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.Decrypt(string).md 'Apq\.Cfg\.Crypto\.DataProtection\.DataProtectionCryptoProvider\.Decrypt\(string\)') | 解密密文 |
| [Encrypt\(string\)](Apq.Cfg.Crypto.DataProtection.DataProtectionCryptoProvider.Encrypt(string).md 'Apq\.Cfg\.Crypto\.DataProtection\.DataProtectionCryptoProvider\.Encrypt\(string\)') | 加密明文 |
