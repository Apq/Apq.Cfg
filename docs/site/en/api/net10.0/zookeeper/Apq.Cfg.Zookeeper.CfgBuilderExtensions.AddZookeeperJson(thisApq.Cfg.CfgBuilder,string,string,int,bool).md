### [Apq\.Cfg\.Zookeeper](Apq.Cfg.Zookeeper.md 'Apq\.Cfg\.Zookeeper').[CfgBuilderExtensions](Apq.Cfg.Zookeeper.CfgBuilderExtensions.md 'Apq\.Cfg\.Zookeeper\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddZookeeperJson\(this CfgBuilder, string, string, int, bool\) Method

添加 Zookeeper 配置源（JSON 格式）

```csharp
public static Apq.Cfg.CfgBuilder AddZookeeperJson(this Apq.Cfg.CfgBuilder builder, string connectionString, string nodePath, int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeperJson(thisApq.Cfg.CfgBuilder,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeperJson(thisApq.Cfg.CfgBuilder,string,string,int,bool).connectionString'></a>

`connectionString` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Zookeeper 连接字符串

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeperJson(thisApq.Cfg.CfgBuilder,string,string,int,bool).nodePath'></a>

`nodePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

存储 JSON 配置的节点路径

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeperJson(thisApq.Cfg.CfgBuilder,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Zookeeper.CfgBuilderExtensions.AddZookeeperJson(thisApq.Cfg.CfgBuilder,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器