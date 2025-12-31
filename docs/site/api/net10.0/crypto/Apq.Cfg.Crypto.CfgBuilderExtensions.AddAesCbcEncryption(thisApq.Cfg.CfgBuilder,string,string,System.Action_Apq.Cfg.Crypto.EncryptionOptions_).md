#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddAesCbcEncryption\(this CfgBuilder, string, string, Action\<EncryptionOptions\>\) Method

添加 AES\-CBC 加密支持

```csharp
public static Apq.Cfg.CfgBuilder AddAesCbcEncryption(this Apq.Cfg.CfgBuilder builder, string base64EncryptionKey, string base64HmacKey, System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesCbcEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesCbcEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).base64EncryptionKey'></a>

`base64EncryptionKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesCbcEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).base64HmacKey'></a>

`base64HmacKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesCbcEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncryptionOptions](Apq.Cfg.Crypto.EncryptionOptions.md 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')