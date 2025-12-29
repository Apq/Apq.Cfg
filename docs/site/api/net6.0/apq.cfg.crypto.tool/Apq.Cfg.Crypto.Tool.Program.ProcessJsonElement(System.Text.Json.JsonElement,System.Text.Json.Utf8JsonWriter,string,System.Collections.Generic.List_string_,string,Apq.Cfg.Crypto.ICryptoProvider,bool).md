### [Apq\.Cfg\.Crypto\.Tool](Apq.Cfg.Crypto.Tool.md 'Apq\.Cfg\.Crypto\.Tool').[Program](Apq.Cfg.Crypto.Tool.Program.md 'Apq\.Cfg\.Crypto\.Tool\.Program')

## Program\.ProcessJsonElement\(JsonElement, Utf8JsonWriter, string, List\<string\>, string, ICryptoProvider, bool\) Method

递归处理 JSON 元素

```csharp
private static int ProcessJsonElement(System.Text.Json.JsonElement element, System.Text.Json.Utf8JsonWriter writer, string currentPath, System.Collections.Generic.List<string> patterns, string prefix, Apq.Cfg.Crypto.ICryptoProvider provider, bool dryRun);
```
#### Parameters

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).element'></a>

`element` [System\.Text\.Json\.JsonElement](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement 'System\.Text\.Json\.JsonElement')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).writer'></a>

`writer` [System\.Text\.Json\.Utf8JsonWriter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonwriter 'System\.Text\.Json\.Utf8JsonWriter')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).currentPath'></a>

`currentPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).patterns'></a>

`patterns` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).prefix'></a>

`prefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).provider'></a>

`provider` [Apq\.Cfg\.Crypto\.ICryptoProvider](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.crypto.icryptoprovider 'Apq\.Cfg\.Crypto\.ICryptoProvider')

<a name='Apq.Cfg.Crypto.Tool.Program.ProcessJsonElement(System.Text.Json.JsonElement,System.Text.Json.Utf8JsonWriter,string,System.Collections.Generic.List_string_,string,Apq.Cfg.Crypto.ICryptoProvider,bool).dryRun'></a>

`dryRun` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')