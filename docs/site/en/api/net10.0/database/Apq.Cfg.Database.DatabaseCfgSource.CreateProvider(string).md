### [Apq\.Cfg\.Database](Apq.Cfg.Database.md 'Apq\.Cfg\.Database').[DatabaseCfgSource](Apq.Cfg.Database.DatabaseCfgSource.md 'Apq\.Cfg\.Database\.DatabaseCfgSource')

## DatabaseCfgSource\.CreateProvider\(string\) Method

根据提供程序名称创建 SqlSugar 数据库提供程序

```csharp
private static Apq.Cfg.Database.SqlSugarDatabaseProvider CreateProvider(string providerName);
```
#### Parameters

<a name='Apq.Cfg.Database.DatabaseCfgSource.CreateProvider(string).providerName'></a>

`providerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

数据库提供程序名称

#### Returns
[SqlSugarDatabaseProvider](Apq.Cfg.Database.SqlSugarDatabaseProvider.md 'Apq\.Cfg\.Database\.SqlSugarDatabaseProvider')  
SqlSugar 数据库提供程序实例

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
当提供程序名称不受支持时抛出