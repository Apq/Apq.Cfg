### [Apq\.Cfg\.Nacos](Apq.Cfg.Nacos.md 'Apq\.Cfg\.Nacos').[CfgBuilderExtensions](Apq.Cfg.Nacos.CfgBuilderExtensions.md 'Apq\.Cfg\.Nacos\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddNacos Method

| Overloads | |
| :--- | :--- |
| [AddNacos\(this CfgBuilder, string, string, string, int, bool\)](Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos.md#Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool) 'Apq\.Cfg\.Nacos\.CfgBuilderExtensions\.AddNacos\(this Apq\.Cfg\.CfgBuilder, string, string, string, int, bool\)') | 添加 Nacos 配置源（使用默认选项） |
| [AddNacos\(this CfgBuilder, Action&lt;NacosCfgOptions&gt;, int, bool\)](Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos.md#Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool) 'Apq\.Cfg\.Nacos\.CfgBuilderExtensions\.AddNacos\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Nacos\.NacosCfgOptions\>, int, bool\)') | 添加 Nacos 配置源 |

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool)'></a>

## CfgBuilderExtensions\.AddNacos\(this CfgBuilder, string, string, string, int, bool\) Method

添加 Nacos 配置源（使用默认选项）

```csharp
public static Apq.Cfg.CfgBuilder AddNacos(this Apq.Cfg.CfgBuilder builder, string serverAddresses, string dataId, string group="DEFAULT_GROUP", int level=0, bool enableHotReload=false);
```
#### Parameters

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).serverAddresses'></a>

`serverAddresses` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Nacos 服务地址

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).dataId'></a>

`dataId` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置的 DataId

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).group'></a>

`group` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置分组

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,string,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool)'></a>

## CfgBuilderExtensions\.AddNacos\(this CfgBuilder, Action\<NacosCfgOptions\>, int, bool\) Method

添加 Nacos 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddNacos(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Nacos.NacosCfgOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[NacosCfgOptions](Apq.Cfg.Nacos.NacosCfgOptions.md 'Apq\.Cfg\.Nacos\.NacosCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Nacos.CfgBuilderExtensions.AddNacos(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Nacos.NacosCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器