### [Apq\.Cfg\.Vault](Apq.Cfg.Vault.md 'Apq\.Cfg\.Vault').[CfgBuilderExtensions](Apq.Cfg.Vault.CfgBuilderExtensions.md 'Apq\.Cfg\.Vault\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddVaultV1\(this CfgBuilder, string, string, string, string, int, bool\) Method

添加 Vault KV V1 配置源（简化方法）

```csharp
public static Apq.Cfg.CfgBuilder AddVaultV1(this Apq.Cfg.CfgBuilder builder, string address, string token, string enginePath="kv", string path="", int level=0, bool enableHotReload=true);
```
#### Parameters

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).address'></a>

`address` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Vault 服务地址

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).token'></a>

`token` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Vault Token

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).enginePath'></a>

`enginePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

KV 引擎路径，默认 "kv"

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

密钥路径

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultV1(thisApq.Cfg.CfgBuilder,string,string,string,string,int,bool).enableHotReload'></a>

`enableHotReload` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否启用热重载

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器