### [Apq\.Cfg\.Database](Apq.Cfg.Database.md 'Apq\.Cfg\.Database').[SqlSugarDatabaseProvider](Apq.Cfg.Database.SqlSugarDatabaseProvider.md 'Apq\.Cfg\.Database\.SqlSugarDatabaseProvider')

## SqlSugarDatabaseProvider\.SafeIdentifierRegex\(\) Method

```csharp
private static System.Text.RegularExpressions.Regex SafeIdentifierRegex();
```

#### Returns
[System\.Text\.RegularExpressions\.Regex](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex 'System\.Text\.RegularExpressions\.Regex')

### Remarks
Pattern:<br/>

```csharp
^[a-zA-Z_][a-zA-Z0-9_]*$
```<br/>
Options:<br/>

```csharp
RegexOptions.Compiled
```<br/>
Explanation:<br/>

```csharp
○ Match if at the beginning of the string.<br/>
○ Match a character in the set [A-Z_a-z].<br/>
○ Match a character in the set [0-9A-Z_a-z] atomically any number of times.<br/>
○ Match if at the end of the string or if before an ending newline.<br/>
```