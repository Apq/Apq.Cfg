#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddValueTransformer\(IValueTransformer\) Method

添加值转换器（供扩展包使用）

```csharp
public Apq.Cfg.CfgBuilder AddValueTransformer(Apq.Cfg.Security.IValueTransformer transformer);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddValueTransformer(Apq.Cfg.Security.IValueTransformer).transformer'></a>

`transformer` [IValueTransformer](Apq.Cfg.Security.IValueTransformer.md 'Apq\.Cfg\.Security\.IValueTransformer')

值转换器实例

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddValueTransformer(new EncryptionTransformer(provider))
    .Build();
```