#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.ConfigureValueTransformer\(Action\<ValueTransformerOptions\>\) Method

配置值转换选项

```csharp
public Apq.Cfg.CfgBuilder ConfigureValueTransformer(System.Action<Apq.Cfg.Security.ValueTransformerOptions> configure);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.ConfigureValueTransformer(System.Action_Apq.Cfg.Security.ValueTransformerOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[ValueTransformerOptions](Apq.Cfg.Security.ValueTransformerOptions.md 'Apq\.Cfg\.Security\.ValueTransformerOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置委托

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .ConfigureValueTransformer(options =>
    {
        options.EncryptedPrefix = "[ENCRYPTED]";
        options.SensitiveKeyPatterns.Add("*ApiSecret*");
    })
    .Build();
```