#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingWriteStrategy Enum

编码写入策略

```csharp
public enum EncodingWriteStrategy
```
### Fields

<a name='Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8NoBom'></a>

`Utf8NoBom` 0

统一转换为 UTF\-8（无 BOM）

<a name='Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Utf8WithBom'></a>

`Utf8WithBom` 1

统一转换为 UTF\-8（带 BOM）

<a name='Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Preserve'></a>

`Preserve` 2

保持原文件编码

<a name='Apq.Cfg.EncodingSupport.EncodingWriteStrategy.Specified'></a>

`Specified` 3

使用指定的编码