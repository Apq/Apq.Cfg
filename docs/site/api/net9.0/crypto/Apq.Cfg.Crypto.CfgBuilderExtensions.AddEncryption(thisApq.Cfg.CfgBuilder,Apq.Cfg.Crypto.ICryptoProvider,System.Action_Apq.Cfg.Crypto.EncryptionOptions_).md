#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddEncryption\(this CfgBuilder, ICryptoProvider, Action\<EncryptionOptions\>\) Method

添加加密支持

```csharp
public static Apq.Cfg.CfgBuilder AddEncryption(this Apq.Cfg.CfgBuilder builder, Apq.Cfg.Crypto.ICryptoProvider provider, System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddEncryption(thisApq.Cfg.CfgBuilder,Apq.Cfg.Crypto.ICryptoProvider,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddEncryption(thisApq.Cfg.CfgBuilder,Apq.Cfg.Crypto.ICryptoProvider,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).provider'></a>

`provider` [ICryptoProvider](Apq.Cfg.Crypto.ICryptoProvider.md 'Apq\.Cfg\.Crypto\.ICryptoProvider')

加密提供者

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddEncryption(thisApq.Cfg.CfgBuilder,Apq.Cfg.Crypto.ICryptoProvider,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncryptionOptions](Apq.Cfg.Crypto.EncryptionOptions.md 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

加密选项配置委托

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddEncryption(new AesGcmCryptoProvider(key), options =>
    {
        options.EncryptedPrefix = "[ENCRYPTED]";
    })
    .Build();
```