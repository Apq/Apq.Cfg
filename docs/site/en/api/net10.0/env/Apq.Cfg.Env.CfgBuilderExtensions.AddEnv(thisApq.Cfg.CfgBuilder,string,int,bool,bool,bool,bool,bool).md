### [Apq\.Cfg\.Env](Apq.Cfg.Env.md 'Apq\.Cfg\.Env').[CfgBuilderExtensions](Apq.Cfg.Env.CfgBuilderExtensions.md 'Apq\.Cfg\.Env\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddEnv\(this CfgBuilder, string, int, bool, bool, bool, bool, bool\) Method

添加 \.env 文件配置源

```csharp
public static Apq.Cfg.CfgBuilder AddEnv(this Apq.Cfg.CfgBuilder builder, string path, int level, bool writeable=false, bool optional=true, bool reloadOnChange=true, bool isPrimaryWriter=false, bool setEnvironmentVariables=false);
```
#### Parameters

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

\.env 文件路径

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).writeable'></a>

`writeable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否可写

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).optional'></a>

`optional` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

文件不存在时是否忽略

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).reloadOnChange'></a>

`reloadOnChange` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

文件变更时是否自动重载

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为默认写入目标

<a name='Apq.Cfg.Env.CfgBuilderExtensions.AddEnv(thisApq.Cfg.CfgBuilder,string,int,bool,bool,bool,bool,bool).setEnvironmentVariables'></a>

`setEnvironmentVariables` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否将配置写入系统环境变量（默认为 false）

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器