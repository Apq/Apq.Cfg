#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddRsaEncryption\(this CfgBuilder, string, Action\<EncryptionOptions\>\) Method

添加 RSA 加密支持（从 PEM 文件）

```csharp
public static Apq.Cfg.CfgBuilder AddRsaEncryption(this Apq.Cfg.CfgBuilder builder, string pemFilePath, System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddRsaEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddRsaEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).pemFilePath'></a>

`pemFilePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddRsaEncryption(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncryptionOptions](Apq.Cfg.Crypto.EncryptionOptions.md 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')