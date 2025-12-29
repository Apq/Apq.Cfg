### [Apq\.Cfg\.Nacos](Apq.Cfg.Nacos.md 'Apq\.Cfg\.Nacos').[CfgBuilderExtensions](Apq.Cfg.Nacos.CfgBuilderExtensions.md 'Apq\.Cfg\.Nacos\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddNacosJson\(this CfgBuilder, string, string, string, int, bool\) Method

添加 Nacos JSON 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddNacosJson(this Apq.Cfg.CfgBuilder builder, string serverAddresses, string dataId, string group="DEFAULT_GROUP", int level=0, bool enableHotReload=false);
```
#### Parameters

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).serverAddresses'></a>

`serverAddresses` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Nacos 服务地址

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).dataId'></a>

`dataId` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置的 DataId

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).group'></a>

`group` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置分组

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacosJson(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器