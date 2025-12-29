### [Apq\.Cfg\.Env](Apq.Cfg.Env.md 'Apq\.Cfg\.Env').[EnvFileCfgSource](Apq.Cfg.Env.EnvFileCfgSource.md 'Apq\.Cfg\.Env\.EnvFileCfgSource')

## EnvFileCfgSource\.CreatePhysicalFileProviderForEnv\(string\) Method

创建 PhysicalFileProvider，允许访问以点开头的文件（如 \.env）

```csharp
private (Microsoft.Extensions.FileProviders.PhysicalFileProvider Provider,string FileName) CreatePhysicalFileProviderForEnv(string path);
```
#### Parameters

<a name='Apq.Cfg.Env.EnvFileCfgSource.CreatePhysicalFileProviderForEnv(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[Microsoft\.Extensions\.FileProviders\.PhysicalFileProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.fileproviders.physicalfileprovider 'Microsoft\.Extensions\.FileProviders\.PhysicalFileProvider')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')