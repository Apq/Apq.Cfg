### [Apq\.Cfg\.Etcd](Apq.Cfg.Etcd.md 'Apq\.Cfg\.Etcd').[CfgBuilderExtensions](Apq.Cfg.Etcd.CfgBuilderExtensions.md 'Apq\.Cfg\.Etcd\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddEtcd Method

| Overloads | |
| :--- | :--- |
| [AddEtcd\(this CfgBuilder, string, string, int, bool\)](Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd.md#Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool) 'Apq\.Cfg\.Etcd\.CfgBuilderExtensions\.AddEtcd\(this Apq\.Cfg\.CfgBuilder, string, string, int, bool\)') | 添加 Etcd 配置源（单端点） |
| [AddEtcd\(this CfgBuilder, string\[\], string, int, bool\)](Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd.md#Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool) 'Apq\.Cfg\.Etcd\.CfgBuilderExtensions\.AddEtcd\(this Apq\.Cfg\.CfgBuilder, string\[\], string, int, bool\)') | 添加 Etcd 配置源（使用默认选项） |
| [AddEtcd\(this CfgBuilder, Action&lt;EtcdCfgOptions&gt;, int, bool\)](Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd.md#Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool) 'Apq\.Cfg\.Etcd\.CfgBuilderExtensions\.AddEtcd\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Etcd\.EtcdCfgOptions\>, int, bool\)') | 添加 Etcd 配置源 |

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool)'></a>

## CfgBuilderExtensions\.AddEtcd\(this CfgBuilder, string, string, int, bool\) Method

添加 Etcd 配置源（单端点）

```csharp
public static Apq.Cfg.CfgBuilder AddEtcd(this Apq.Cfg.CfgBuilder builder, string endpoint, string keyPrefix="/config/", int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool).endpoint'></a>

`endpoint` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Etcd 服务端点

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool).keyPrefix'></a>

`keyPrefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

KV 键前缀

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool)'></a>

## CfgBuilderExtensions\.AddEtcd\(this CfgBuilder, string\[\], string, int, bool\) Method

添加 Etcd 配置源（使用默认选项）

```csharp
public static Apq.Cfg.CfgBuilder AddEtcd(this Apq.Cfg.CfgBuilder builder, string[] endpoints, string keyPrefix="/config/", int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool).endpoints'></a>

`endpoints` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Etcd 服务端点列表

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool).keyPrefix'></a>

`keyPrefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

KV 键前缀

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,string[],string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool)'></a>

## CfgBuilderExtensions\.AddEtcd\(this CfgBuilder, Action\<EtcdCfgOptions\>, int, bool\) Method

添加 Etcd 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddEtcd(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Etcd.EtcdCfgOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[EtcdCfgOptions](Apq.Cfg.Etcd.EtcdCfgOptions.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Etcd.CfgBuilderExtensions.AddEtcd(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Etcd.EtcdCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器