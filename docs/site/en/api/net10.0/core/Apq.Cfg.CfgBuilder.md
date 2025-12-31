#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## CfgBuilder Class

配置构建器，用于创建和管理配置源

```csharp
public sealed class CfgBuilder
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CfgBuilder

| Methods | |
| :--- | :--- |
| [AddEnvironmentVariables\(int, string\)](Apq.Cfg.CfgBuilder.AddEnvironmentVariables(int,string).md 'Apq\.Cfg\.CfgBuilder\.AddEnvironmentVariables\(int, string\)') | 添加环境变量配置源 |
| [AddJson\(string, int, bool, bool, bool, bool, EncodingOptions\)](Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).md 'Apq\.Cfg\.CfgBuilder\.AddJson\(string, int, bool, bool, bool, bool, Apq\.Cfg\.EncodingSupport\.EncodingOptions\)') | 添加JSON文件配置源 |
| [AddReadEncodingMapping\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddReadEncodingMapping(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddReadEncodingMapping\(string, System\.Text\.Encoding, int\)') | 添加读取编码映射（完整路径） |
| [AddReadEncodingMappingRegex\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddReadEncodingMappingRegex(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddReadEncodingMappingRegex\(string, System\.Text\.Encoding, int\)') | 添加读取编码映射（正则表达式） |
| [AddReadEncodingMappingWildcard\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddReadEncodingMappingWildcard(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddReadEncodingMappingWildcard\(string, System\.Text\.Encoding, int\)') | 添加读取编码映射（通配符） |
| [AddSource\(ICfgSource\)](Apq.Cfg.CfgBuilder.AddSource(Apq.Cfg.Sources.ICfgSource).md 'Apq\.Cfg\.CfgBuilder\.AddSource\(Apq\.Cfg\.Sources\.ICfgSource\)') | 添加自定义配置源（供扩展包使用） |
| [AddValueMasker\(IValueMasker\)](Apq.Cfg.CfgBuilder.AddValueMasker(Apq.Cfg.Security.IValueMasker).md 'Apq\.Cfg\.CfgBuilder\.AddValueMasker\(Apq\.Cfg\.Security\.IValueMasker\)') | 添加值脱敏器（供扩展包使用） |
| [AddValueTransformer\(IValueTransformer\)](Apq.Cfg.CfgBuilder.AddValueTransformer(Apq.Cfg.Security.IValueTransformer).md 'Apq\.Cfg\.CfgBuilder\.AddValueTransformer\(Apq\.Cfg\.Security\.IValueTransformer\)') | 添加值转换器（供扩展包使用） |
| [AddWriteEncodingMapping\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddWriteEncodingMapping(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddWriteEncodingMapping\(string, System\.Text\.Encoding, int\)') | 添加写入编码映射（完整路径） |
| [AddWriteEncodingMappingRegex\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddWriteEncodingMappingRegex(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddWriteEncodingMappingRegex\(string, System\.Text\.Encoding, int\)') | 添加写入编码映射（正则表达式） |
| [AddWriteEncodingMappingWildcard\(string, Encoding, int\)](Apq.Cfg.CfgBuilder.AddWriteEncodingMappingWildcard(string,System.Text.Encoding,int).md 'Apq\.Cfg\.CfgBuilder\.AddWriteEncodingMappingWildcard\(string, System\.Text\.Encoding, int\)') | 添加写入编码映射（通配符） |
| [Build\(\)](Apq.Cfg.CfgBuilder.Build().md 'Apq\.Cfg\.CfgBuilder\.Build\(\)') | 构建配置根实例 |
| [ConfigureEncodingMapping\(Action&lt;EncodingMappingConfig&gt;\)](Apq.Cfg.CfgBuilder.ConfigureEncodingMapping(System.Action_Apq.Cfg.EncodingSupport.EncodingMappingConfig_).md 'Apq\.Cfg\.CfgBuilder\.ConfigureEncodingMapping\(System\.Action\<Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\>\)') | 配置编码映射（使用 Action 委托进行更复杂的配置） |
| [ConfigureValueTransformer\(Action&lt;ValueTransformerOptions&gt;\)](Apq.Cfg.CfgBuilder.ConfigureValueTransformer(System.Action_Apq.Cfg.Security.ValueTransformerOptions_).md 'Apq\.Cfg\.CfgBuilder\.ConfigureValueTransformer\(System\.Action\<Apq\.Cfg\.Security\.ValueTransformerOptions\>\)') | 配置值转换选项 |
| [WithEncodingConfidenceThreshold\(float\)](Apq.Cfg.CfgBuilder.WithEncodingConfidenceThreshold(float).md 'Apq\.Cfg\.CfgBuilder\.WithEncodingConfidenceThreshold\(float\)') | 设置编码检测置信度阈值（0\.0\-1\.0） |
| [WithEncodingDetectionLogging\(Action&lt;EncodingDetectionResult&gt;\)](Apq.Cfg.CfgBuilder.WithEncodingDetectionLogging(System.Action_Apq.Cfg.EncodingSupport.EncodingDetectionResult_).md 'Apq\.Cfg\.CfgBuilder\.WithEncodingDetectionLogging\(System\.Action\<Apq\.Cfg\.EncodingSupport\.EncodingDetectionResult\>\)') | 启用编码检测日志 |
