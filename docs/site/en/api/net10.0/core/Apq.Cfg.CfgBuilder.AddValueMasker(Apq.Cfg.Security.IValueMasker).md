#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddValueMasker\(IValueMasker\) Method

添加值脱敏器（供扩展包使用）

```csharp
public Apq.Cfg.CfgBuilder AddValueMasker(Apq.Cfg.Security.IValueMasker masker);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddValueMasker(Apq.Cfg.Security.IValueMasker).masker'></a>

`masker` [IValueMasker](Apq.Cfg.Security.IValueMasker.md 'Apq\.Cfg\.Security\.IValueMasker')

值脱敏器实例

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddValueMasker(new SensitiveMasker())
    .Build();
```