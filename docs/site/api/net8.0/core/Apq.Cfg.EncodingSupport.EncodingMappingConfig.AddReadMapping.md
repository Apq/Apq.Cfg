#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')

## EncodingMappingConfig\.AddReadMapping Method

| Overloads | |
| :--- | :--- |
| [AddReadMapping\(string, EncodingMappingType, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddReadMapping\(string, Apq\.Cfg\.EncodingSupport\.EncodingMappingType, System\.Text\.Encoding, int\)') | 添加读取编码映射规则 |
| [AddReadMapping\(string, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping.md#Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int) 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig\.AddReadMapping\(string, System\.Text\.Encoding, int\)') | 添加读取编码映射（完整路径） |

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int)'></a>

## EncodingMappingConfig\.AddReadMapping\(string, EncodingMappingType, Encoding, int\) Method

添加读取编码映射规则

```csharp
public Apq.Cfg.EncodingSupport.EncodingMappingConfig AddReadMapping(string pattern, Apq.Cfg.EncodingSupport.EncodingMappingType type, System.Text.Encoding encoding, int priority=0);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).pattern'></a>

`pattern` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

匹配模式

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).type'></a>

`type` [EncodingMappingType](Apq.Cfg.EncodingSupport.EncodingMappingType.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingType')

匹配类型

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

目标编码

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).priority'></a>

`priority` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

优先级（数值越大优先级越高）

#### Returns
[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int)'></a>

## EncodingMappingConfig\.AddReadMapping\(string, Encoding, int\) Method

添加读取编码映射（完整路径）

```csharp
public Apq.Cfg.EncodingSupport.EncodingMappingConfig AddReadMapping(string filePath, System.Text.Encoding encoding, int priority=0);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.AddReadMapping(string,System.Text.Encoding,int).priority'></a>

`priority` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')