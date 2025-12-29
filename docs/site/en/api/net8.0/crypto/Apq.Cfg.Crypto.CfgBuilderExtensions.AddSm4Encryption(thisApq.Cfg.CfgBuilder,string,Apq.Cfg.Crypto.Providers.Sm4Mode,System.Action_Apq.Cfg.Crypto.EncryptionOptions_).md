#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddSm4Encryption\(this CfgBuilder, string, Sm4Mode, Action\<EncryptionOptions\>\) Method

添加 SM4 国密加密支持

```csharp
public static Apq.Cfg.CfgBuilder AddSm4Encryption(this Apq.Cfg.CfgBuilder builder, string base64Key, Apq.Cfg.Crypto.Providers.Sm4Mode mode=Apq.Cfg.Crypto.Providers.Sm4Mode.CBC, System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSm4Encryption(thisApq.Cfg.CfgBuilder,string,Apq.Cfg.Crypto.Providers.Sm4Mode,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSm4Encryption(thisApq.Cfg.CfgBuilder,string,Apq.Cfg.Crypto.Providers.Sm4Mode,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).base64Key'></a>

`base64Key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSm4Encryption(thisApq.Cfg.CfgBuilder,string,Apq.Cfg.Crypto.Providers.Sm4Mode,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).mode'></a>

`mode` [Apq\.Cfg\.Crypto\.Providers\.Sm4Mode](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.providers.sm4mode 'Apq\.Cfg\.Crypto\.Providers\.Sm4Mode')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSm4Encryption(thisApq.Cfg.CfgBuilder,string,Apq.Cfg.Crypto.Providers.Sm4Mode,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncryptionOptions](Apq.Cfg.Crypto.EncryptionOptions.md 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')