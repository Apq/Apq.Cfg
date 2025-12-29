#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto')

## CfgBuilderExtensions Class

CfgBuilder 的加密脱敏扩展方法

```csharp
public static class CfgBuilderExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CfgBuilderExtensions

| Methods | |
| :--- | :--- |
| [AddAesCbcEncryption\(this CfgBuilder, string, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesCbcEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddAesCbcEncryption\(this Apq\.Cfg\.CfgBuilder, string, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 AES\-CBC 加密支持 |
| [AddAesGcmEncryption\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesGcmEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddAesGcmEncryption\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 AES\-GCM 加密支持 |
| [AddAesGcmEncryptionFromEnv\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesGcmEncryptionFromEnv(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddAesGcmEncryptionFromEnv\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 AES\-GCM 加密支持（从环境变量读取密钥） |
| [AddChaCha20Encryption\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddChaCha20Encryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddChaCha20Encryption\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 ChaCha20\-Poly1305 加密支持 |
| [AddEncryption\(this CfgBuilder, ICryptoProvider, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddEncryption(thisApq.Cfg.CfgBuilder,Apq.Cfg.Crypto.ICryptoProvider,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddEncryption\(this Apq\.Cfg\.CfgBuilder, Apq\.Cfg\.Crypto\.ICryptoProvider, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加加密支持 |
| [AddRsaEncryption\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddRsaEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddRsaEncryption\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 RSA 加密支持（从 PEM 文件） |
| [AddRsaEncryptionFromPem\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddRsaEncryptionFromPem(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddRsaEncryptionFromPem\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 RSA 加密支持（从 PEM 字符串） |
| [AddSensitiveMasking\(this CfgBuilder, Action&lt;MaskingOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddSensitiveMasking(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Crypto.MaskingOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddSensitiveMasking\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Crypto\.MaskingOptions\>\)') | 添加敏感值脱敏 |
| [AddSm4Encryption\(this CfgBuilder, string, Sm4Mode, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddSm4Encryption(thisApq.Cfg.CfgBuilder,string,Apq.Cfg.Crypto.Providers.Sm4Mode,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddSm4Encryption\(this Apq\.Cfg\.CfgBuilder, string, Apq\.Cfg\.Crypto\.Providers\.Sm4Mode, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 SM4 国密加密支持 |
| [AddTripleDesEncryption\(this CfgBuilder, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.CfgBuilderExtensions.AddTripleDesEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions\.AddTripleDesEncryption\(this Apq\.Cfg\.CfgBuilder, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 Triple DES 加密支持（仅用于遗留系统兼容） |
