#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingMappingConfig Class

编码映射配置

```csharp
public sealed class EncodingMappingConfig
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EncodingMappingConfig

| Properties | |
| :--- | :--- |
| [ReadRules](Apq.Cfg.EncodingSupport.EncodingMappingConfig.ReadRules.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.ReadRules') | 读取编码映射规则（只读） |
| [WriteRules](Apq.Cfg.EncodingSupport.EncodingMappingConfig.WriteRules.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.WriteRules') | 写入编码映射规则（只读） |

| Methods | |
| :--- | :--- |
| [AddReadMapping\(string, EncodingMappingType, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddReadMapping\(string, Apq\.Cfg\.EncodingSupport\.EncodingMappingType, System\.Text\.Encoding, int\)') | 添加读取编码映射规则 |
| [AddReadMapping\(string, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddReadMapping\(string, System\.Text\.Encoding, int\)') | 添加读取编码映射（完整路径） |
| [AddWriteMapping\(string, EncodingMappingType, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddWriteMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddWriteMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddWriteMapping\(string, Apq\.Cfg\.EncodingSupport\.EncodingMappingType, System\.Text\.Encoding, int\)') | 添加写入编码映射规则 |
| [AddWriteMapping\(string, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddWriteMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddWriteMapping(string,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddWriteMapping\(string, System\.Text\.Encoding, int\)') | 添加写入编码映射（完整路径） |
| [Clear\(\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.Clear().md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.Clear\(\)') | 清除所有映射规则 |
| [ClearReadMappings\(\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.ClearReadMappings().md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.ClearReadMappings\(\)') | 清除所有读取映射规则 |
| [ClearWriteMappings\(\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.ClearWriteMappings().md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.ClearWriteMappings\(\)') | 清除所有写入映射规则 |
| [GetReadEncoding\(string\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.GetReadEncoding(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.GetReadEncoding\(string\)') | 获取文件的读取编码 |
| [GetStats\(\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.GetStats().md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.GetStats\(\)') | 获取统计信息 |
| [GetWriteEncoding\(string\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.GetWriteEncoding(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.GetWriteEncoding\(string\)') | 获取文件的写入编码 |
| [RemoveReadMapping\(string, Nullable&lt;EncodingMappingType&gt;\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.RemoveReadMapping(string,System.Nullable_Apq.Cfg.EncodingSupport.EncodingMappingType_).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.RemoveReadMapping\(string, System\.Nullable\<Apq\.Cfg\.EncodingSupport\.EncodingMappingType\>\)') | 移除读取编码映射规则 |
| [RemoveWriteMapping\(string, Nullable&lt;EncodingMappingType&gt;\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.RemoveWriteMapping(string,System.Nullable_Apq.Cfg.EncodingSupport.EncodingMappingType_).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.RemoveWriteMapping\(string, System\.Nullable\<Apq\.Cfg\.EncodingSupport\.EncodingMappingType\>\)') | 移除写入编码映射规则 |
