#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources').[JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

## JsonFileCfgSource\(string, int, bool, bool, bool, bool, EncodingOptions\) Constructor

初始化 JsonFileCfgSource 实例

```csharp
public JsonFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange, bool isPrimaryWriter, Apq.Cfg.EncodingSupport.EncodingOptions? encodingOptions=null);
```
#### Parameters

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

JSON 文件路径

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).writeable'></a>

`writeable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否可写

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).optional'></a>

`optional` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为可选文件

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).reloadOnChange'></a>

`reloadOnChange` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

文件变更时是否自动重载

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主要写入源

<a name='Apq.Cfg.Sources.JsonFileCfgSource.JsonFileCfgSource(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).encodingOptions'></a>

`encodingOptions` [EncodingOptions](Apq.Cfg.EncodingSupport.EncodingOptions.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions')

编码选项