#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddAesGcmEncryptionFromEnv\(this CfgBuilder, string, Action\<EncryptionOptions\>\) Method

添加 AES\-GCM 加密支持（从环境变量读取密钥）

```csharp
public static Apq.Cfg.CfgBuilder AddAesGcmEncryptionFromEnv(this Apq.Cfg.CfgBuilder builder, string envVarName="APQ_CFG_ENCRYPTION_KEY", System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesGcmEncryptionFromEnv(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesGcmEncryptionFromEnv(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).envVarName'></a>

`envVarName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddAesGcmEncryptionFromEnv(thisApq.Cfg.CfgBuilder,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncryptionOptions](Apq.Cfg.Crypto.EncryptionOptions.md 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')