### [Apq\.Cfg\.Consul](Apq.Cfg.Consul.md 'Apq\.Cfg\.Consul').[CfgBuilderExtensions](Apq.Cfg.Consul.CfgBuilderExtensions.md 'Apq\.Cfg\.Consul\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddConsul Method

| Overloads | |
| :--- | :--- |
| [AddConsul\(this CfgBuilder, string, string, int, bool\)](Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul.md#Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool) 'Apq\.Cfg\.Consul\.CfgBuilderExtensions\.AddConsul\(this Apq\.Cfg\.CfgBuilder, string, string, int, bool\)') | 添加 Consul 配置源（使用默认选项） |
| [AddConsul\(this CfgBuilder, Action&lt;ConsulCfgOptions&gt;, int, bool\)](Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul.md#Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool) 'Apq\.Cfg\.Consul\.CfgBuilderExtensions\.AddConsul\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Consul\.ConsulCfgOptions\>, int, bool\)') | 添加 Consul 配置源 |

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool)'></a>

## CfgBuilderExtensions\.AddConsul\(this CfgBuilder, string, string, int, bool\) Method

添加 Consul 配置源（使用默认选项）

```csharp
public static Apq.Cfg.CfgBuilder AddConsul(this Apq.Cfg.CfgBuilder builder, string address, string keyPrefix="config/", int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool).address'></a>

`address` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Consul 服务地址

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool).keyPrefix'></a>

`keyPrefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

KV 键前缀

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool)'></a>

## CfgBuilderExtensions\.AddConsul\(this CfgBuilder, Action\<ConsulCfgOptions\>, int, bool\) Method

添加 Consul 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddConsul(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Consul.ConsulCfgOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[ConsulCfgOptions](Apq.Cfg.Consul.ConsulCfgOptions.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Consul.CfgBuilderExtensions.AddConsul(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Consul.ConsulCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器