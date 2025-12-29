### [Apq\.Cfg\.Crypto\.DataProtection](Apq.Cfg.Crypto.DataProtection.md 'Apq\.Cfg\.Crypto\.DataProtection').[CfgBuilderExtensions](Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.md 'Apq\.Cfg\.Crypto\.DataProtection\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddDataProtectionEncryption Method

| Overloads | |
| :--- | :--- |
| [AddDataProtectionEncryption\(this CfgBuilder, IDataProtectionProvider, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption.md#Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_) 'Apq\.Cfg\.Crypto\.DataProtection\.CfgBuilderExtensions\.AddDataProtectionEncryption\(this Apq\.Cfg\.CfgBuilder, Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 Data Protection 加密支持 |
| [AddDataProtectionEncryption\(this CfgBuilder, string, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption.md#Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_) 'Apq\.Cfg\.Crypto\.DataProtection\.CfgBuilderExtensions\.AddDataProtectionEncryption\(this Apq\.Cfg\.CfgBuilder, string, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 Data Protection 加密支持（使用默认提供者） |
| [AddDataProtectionEncryption\(this CfgBuilder, DirectoryInfo, string, string, Action&lt;EncryptionOptions&gt;\)](Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption.md#Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_) 'Apq\.Cfg\.Crypto\.DataProtection\.CfgBuilderExtensions\.AddDataProtectionEncryption\(this Apq\.Cfg\.CfgBuilder, System\.IO\.DirectoryInfo, string, string, System\.Action\<Apq\.Cfg\.Crypto\.EncryptionOptions\>\)') | 添加 Data Protection 加密支持（使用指定目录存储密钥） |

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_)'></a>

## CfgBuilderExtensions\.AddDataProtectionEncryption\(this CfgBuilder, IDataProtectionProvider, string, Action\<EncryptionOptions\>\) Method

添加 Data Protection 加密支持

```csharp
public static Apq.Cfg.CfgBuilder AddDataProtectionEncryption(this Apq.Cfg.CfgBuilder builder, Microsoft.AspNetCore.DataProtection.IDataProtectionProvider provider, string purpose="Apq.Cfg", System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).provider'></a>

`provider` [Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.dataprotection.idataprotectionprovider 'Microsoft\.AspNetCore\.DataProtection\.IDataProtectionProvider')

Data Protection 提供者

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).purpose'></a>

`purpose` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

保护目的，用于隔离不同用途的加密数据

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,Microsoft.AspNetCore.DataProtection.IDataProtectionProvider,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[Apq\.Cfg\.Crypto\.EncryptionOptions](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.encryptionoptions 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

加密选项配置委托

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var dataProtectionProvider = DataProtectionProvider.Create("MyApp");
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddDataProtectionEncryption(dataProtectionProvider)
    .Build();
```

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_)'></a>

## CfgBuilderExtensions\.AddDataProtectionEncryption\(this CfgBuilder, string, string, Action\<EncryptionOptions\>\) Method

添加 Data Protection 加密支持（使用默认提供者）

```csharp
public static Apq.Cfg.CfgBuilder AddDataProtectionEncryption(this Apq.Cfg.CfgBuilder builder, string applicationName, string purpose="Apq.Cfg", System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).applicationName'></a>

`applicationName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

应用程序名称，用于密钥隔离

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).purpose'></a>

`purpose` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

保护目的

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[Apq\.Cfg\.Crypto\.EncryptionOptions](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.encryptionoptions 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

加密选项配置委托

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddDataProtectionEncryption("MyApp")
    .Build();
```

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_)'></a>

## CfgBuilderExtensions\.AddDataProtectionEncryption\(this CfgBuilder, DirectoryInfo, string, string, Action\<EncryptionOptions\>\) Method

添加 Data Protection 加密支持（使用指定目录存储密钥）

```csharp
public static Apq.Cfg.CfgBuilder AddDataProtectionEncryption(this Apq.Cfg.CfgBuilder builder, System.IO.DirectoryInfo keyDirectory, string applicationName, string purpose="Apq.Cfg", System.Action<Apq.Cfg.Crypto.EncryptionOptions>? configure=null);
```
#### Parameters

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).keyDirectory'></a>

`keyDirectory` [System\.IO\.DirectoryInfo](https://learn.microsoft.com/en-us/dotnet/api/system.io.directoryinfo 'System\.IO\.DirectoryInfo')

密钥存储目录

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).applicationName'></a>

`applicationName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

应用程序名称

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).purpose'></a>

`purpose` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

保护目的

<a name='Apq.Cfg.Crypto.DataProtection.CfgBuilderExtensions.AddDataProtectionEncryption(thisApq.Cfg.CfgBuilder,System.IO.DirectoryInfo,string,string,System.Action_Apq.Cfg.Crypto.EncryptionOptions_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[Apq\.Cfg\.Crypto\.EncryptionOptions](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.encryptionoptions 'Apq\.Cfg\.Crypto\.EncryptionOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

加密选项配置委托

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddDataProtectionEncryption(new DirectoryInfo("/keys"), "MyApp")
    .Build();
```