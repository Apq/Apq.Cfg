### [Apq\.Cfg\.Apollo](Apq.Cfg.Apollo.md 'Apq\.Cfg\.Apollo').[CfgBuilderExtensions](Apq.Cfg.Apollo.CfgBuilderExtensions.md 'Apq\.Cfg\.Apollo\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddApollo Method

| Overloads | |
| :--- | :--- |
| [AddApollo\(this CfgBuilder, string, string, string\[\], int, bool\)](Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo.md#Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool) 'Apq\.Cfg\.Apollo\.CfgBuilderExtensions\.AddApollo\(this Apq\.Cfg\.CfgBuilder, string, string, string\[\], int, bool\)') | 添加 Apollo 配置源（使用默认选项） |
| [AddApollo\(this CfgBuilder, Action&lt;ApolloCfgOptions&gt;, int, bool\)](Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo.md#Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool) 'Apq\.Cfg\.Apollo\.CfgBuilderExtensions\.AddApollo\(this Apq\.Cfg\.CfgBuilder, System\.Action\<Apq\.Cfg\.Apollo\.ApolloCfgOptions\>, int, bool\)') | 添加 Apollo 配置源 |

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool)'></a>

## CfgBuilderExtensions\.AddApollo\(this CfgBuilder, string, string, string\[\], int, bool\) Method

添加 Apollo 配置源（使用默认选项）

```csharp
public static Apq.Cfg.CfgBuilder AddApollo(this Apq.Cfg.CfgBuilder builder, string appId, string metaServer="http://localhost:8080", string[]? namespaces=null, int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).appId'></a>

`appId` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Apollo 应用 ID

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).metaServer'></a>

`metaServer` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Meta Server 地址

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).namespaces'></a>

`namespaces` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

命名空间列表

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,string,string,string[],int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool)'></a>

## CfgBuilderExtensions\.AddApollo\(this CfgBuilder, Action\<ApolloCfgOptions\>, int, bool\) Method

添加 Apollo 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddApollo(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Apollo.ApolloCfgOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[ApolloCfgOptions](Apq.Cfg.Apollo.ApolloCfgOptions.md 'Apq\.Cfg\.Apollo\.ApolloCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Apollo.CfgBuilderExtensions.AddApollo(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Apollo.ApolloCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源（Apollo 不支持写入）

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器