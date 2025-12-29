### [Apq\.Cfg\.Ini](Apq.Cfg.Ini.md 'Apq\.Cfg\.Ini').[IniFileCfgSource](Apq.Cfg.Ini.IniFileCfgSource.md 'Apq\.Cfg\.Ini\.IniFileCfgSource')

## IniFileCfgSource\(string, int, bool, bool, bool, bool\) Constructor

初始化 IniFileCfgSource 实例

```csharp
public IniFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange, bool isPrimaryWriter);
```
#### Parameters

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

INI 文件路径

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).writeable'></a>

`writeable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否可写

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).optional'></a>

`optional` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为可选文件

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).reloadOnChange'></a>

`reloadOnChange` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

文件变更时是否自动重载

<a name='Apq.Cfg.Ini.IniFileCfgSource.IniFileCfgSource(string,int,bool,bool,bool,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主要写入源