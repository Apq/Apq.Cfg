#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.ConfigureEncodingMapping\(Action\<EncodingMappingConfig\>\) Method

配置编码映射（使用 Action 委托进行更复杂的配置）

```csharp
public Apq.Cfg.CfgBuilder ConfigureEncodingMapping(System.Action<Apq.Cfg.EncodingSupport.EncodingMappingConfig> configure);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.ConfigureEncodingMapping(System.Action_Apq.Cfg.EncodingSupport.EncodingMappingConfig_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置委托

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')