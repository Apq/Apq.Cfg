#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto').[CfgBuilderExtensions](Apq.Cfg.Crypto.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddSensitiveMasking\(this CfgBuilder, Action\<MaskingOptions\>\) Method

添加敏感值脱敏

```csharp
public static Apq.Cfg.CfgBuilder AddSensitiveMasking(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Crypto.MaskingOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSensitiveMasking(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Crypto.MaskingOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Crypto.CfgBuilderExtensions.AddSensitiveMasking(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Crypto.MaskingOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[MaskingOptions](Apq.Cfg.Crypto.MaskingOptions.md 'Apq\.Cfg\.Crypto\.MaskingOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

脱敏选项配置委托

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddSensitiveMasking(options =>
    {
        options.MaskString = "****";
        options.VisibleChars = 2;
    })
    .Build();
```