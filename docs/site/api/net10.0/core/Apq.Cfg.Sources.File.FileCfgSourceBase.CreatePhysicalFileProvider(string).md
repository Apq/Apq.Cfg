#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources\.File](Apq.Cfg.Sources.File.md 'Apq\.Cfg\.Sources\.File').[FileCfgSourceBase](Apq.Cfg.Sources.File.FileCfgSourceBase.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase')

## FileCfgSourceBase\.CreatePhysicalFileProvider\(string\) Method

创建 PhysicalFileProvider 并跟踪以便后续释放

```csharp
protected (Microsoft.Extensions.FileProviders.PhysicalFileProvider Provider,string FileName) CreatePhysicalFileProvider(string path);
```
#### Parameters

<a name='Apq.Cfg.Sources.File.FileCfgSourceBase.CreatePhysicalFileProvider(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[Microsoft\.Extensions\.FileProviders\.PhysicalFileProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.fileproviders.physicalfileprovider 'Microsoft\.Extensions\.FileProviders\.PhysicalFileProvider')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')