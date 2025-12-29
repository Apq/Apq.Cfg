### [Apq\.Cfg\.Vault](Apq.Cfg.Vault.md 'Apq\.Cfg\.Vault').[CfgBuilderExtensions](Apq.Cfg.Vault.CfgBuilderExtensions.md 'Apq\.Cfg\.Vault\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddVaultUserPass\(this CfgBuilder, string, string, string, string, string, int, int\) Method

添加 Vault 配置源（使用 UserPass 认证）

```csharp
public static Apq.Cfg.CfgBuilder AddVaultUserPass(this Apq.Cfg.CfgBuilder builder, string address, string username, string password, string enginePath="kv", string path="", int kvVersion=2, int level=0);
```
#### Parameters

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).address'></a>

`address` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Vault 服务地址

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

用户名

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

密码

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).enginePath'></a>

`enginePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

KV 引擎路径

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

密钥路径

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).kvVersion'></a>

`kvVersion` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

KV 引擎版本 \(1 或 2\)

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVaultUserPass(thisApq.Cfg.CfgBuilder,string,string,string,string,string,int,int).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器