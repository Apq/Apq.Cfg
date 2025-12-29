#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto\.Providers](Apq.Cfg.Crypto.Providers.md 'Apq\.Cfg\.Crypto\.Providers')

## AesCbcCryptoProvider Class

AES\-CBC 加密提供者（使用 BouncyCastle 实现）

```csharp
public class AesCbcCryptoProvider : Apq.Cfg.Crypto.ICryptoProvider, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AesCbcCryptoProvider

Implements [ICryptoProvider](Apq.Cfg.Crypto.ICryptoProvider.md 'Apq\.Cfg\.Crypto\.ICryptoProvider'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

### Remarks
性能优化：
1\. 使用静态 SecureRandom 实例避免重复创建
2\. 使用 ArrayPool 减少内存分配