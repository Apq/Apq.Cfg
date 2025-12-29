### [Apq\.Cfg\.Zookeeper](Apq.Cfg.Zookeeper.md 'Apq\.Cfg\.Zookeeper').[CfgBuilderExtensions](Apq.Cfg.Zookeeper.CfgBuilderExtensions.md 'Apq\.Cfg\.Zookeeper\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddZookeeper Method

| Overloads | |
| :--- | :--- |
| [AddZookeeper\(this CfgBuilder, string, string, int, bool\)](Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper.md#Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool) 'Apq\.Cfg\.Zookeeper\.CfgBuilderExtensions\.AddZookeeper\(this Apq\.Cfg\.CfgBuilder, string, string, int, bool\)') | 添加 Zookeeper 配置源（使用默认选项） |
| [AddZookeeper\(this CfgBuilder, Action&lt;ZookeeperCfgOptions&gt;, int, bool\)](Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper.md#Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool) 'Apq\.Cfg\.Zookeeper\.CfgBuilderExtensions\.AddZookeeper\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\>, int, bool\)') | 添加 Zookeeper 配置源 |

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool)'></a>

## CfgBuilderExtensions\.AddZookeeper\(this CfgBuilder, string, string, int, bool\) Method

添加 Zookeeper 配置源（使用默认选项）

```csharp
public static Apq.Cfg.CfgBuilder AddZookeeper(this Apq.Cfg.CfgBuilder builder, string connectionString, string rootPath="/config", int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool).connectionString'></a>

`connectionString` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Zookeeper 连接字符串，如 localhost:2181

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool).rootPath'></a>

`rootPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

根路径，默认 /config

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool)'></a>

## CfgBuilderExtensions\.AddZookeeper\(this CfgBuilder, Action\<ZookeeperCfgOptions\>, int, bool\) Method

添加 Zookeeper 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddZookeeper(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Zookeeper.ZookeeperCfgOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[ZookeeperCfgOptions](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeper(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Zookeeper.ZookeeperCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器