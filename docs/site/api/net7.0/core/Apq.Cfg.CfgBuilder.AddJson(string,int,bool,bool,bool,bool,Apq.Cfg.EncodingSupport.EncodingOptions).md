#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddJson\(string, int, bool, bool, bool, bool, EncodingOptions\) Method

添加JSON文件配置源

```csharp
public Apq.Cfg.CfgBuilder AddJson(string path, int level, bool writeable, bool optional=true, bool reloadOnChange=true, bool isPrimaryWriter=false, Apq.Cfg.EncodingSupport.EncodingOptions? encoding=null);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

JSON文件路径

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).writeable'></a>

`writeable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否可写

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).optional'></a>

`optional` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为可选文件，默认为true

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).reloadOnChange'></a>

`reloadOnChange` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

文件变更时是否自动重载，默认为true

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主要写入器，默认为false

<a name='Apq.Cfg.CfgBuilder.AddJson(string,int,bool,bool,bool,bool,Apq.Cfg.EncodingSupport.EncodingOptions).encoding'></a>

`encoding` [EncodingOptions](Apq.Cfg.EncodingSupport.EncodingOptions.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions')

编码选项，默认为null

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddJson("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .Build();
```