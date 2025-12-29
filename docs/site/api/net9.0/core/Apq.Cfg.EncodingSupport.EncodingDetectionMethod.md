#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingDetectionMethod Enum

编码检测方法

```csharp
public enum EncodingDetectionMethod
```
### Fields

<a name='Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Bom'></a>

`Bom` 0

通过 BOM 标记检测

<a name='Apq.Cfg.EncodingSupport.EncodingDetectionMethod.UtfUnknown'></a>

`UtfUnknown` 1

通过 UTF\.Unknown 库检测

<a name='Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Fallback'></a>

`Fallback` 2

使用回退编码

<a name='Apq.Cfg.EncodingSupport.EncodingDetectionMethod.UserSpecified'></a>

`UserSpecified` 3

使用用户指定编码

<a name='Apq.Cfg.EncodingSupport.EncodingDetectionMethod.Cached'></a>

`Cached` 4

从缓存获取