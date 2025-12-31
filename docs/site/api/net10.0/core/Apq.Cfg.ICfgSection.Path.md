#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.Path Property

获取此节的路径前缀

```csharp
string Path { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Example

```csharp
var dbSection = cfg.GetSection("Database");
Console.WriteLine(dbSection.Path); // 输出: Database
```