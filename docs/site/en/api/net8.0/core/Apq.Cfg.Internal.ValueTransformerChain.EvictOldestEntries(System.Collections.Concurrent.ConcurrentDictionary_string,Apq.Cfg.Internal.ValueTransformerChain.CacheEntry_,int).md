#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueTransformerChain](Apq.Cfg.Internal.ValueTransformerChain.md 'Apq\.Cfg\.Internal\.ValueTransformerChain')

## ValueTransformerChain\.EvictOldestEntries\(ConcurrentDictionary\<string,CacheEntry\>, int\) Method

淘汰最旧的缓存条目

```csharp
private void EvictOldestEntries(System.Collections.Concurrent.ConcurrentDictionary<string,Apq.Cfg.Internal.ValueTransformerChain.CacheEntry> cache, int count);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueTransformerChain.EvictOldestEntries(System.Collections.Concurrent.ConcurrentDictionary_string,Apq.Cfg.Internal.ValueTransformerChain.CacheEntry_,int).cache'></a>

`cache` [System\.Collections\.Concurrent\.ConcurrentDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2 'System\.Collections\.Concurrent\.ConcurrentDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2 'System\.Collections\.Concurrent\.ConcurrentDictionary\`2')[CacheEntry](Apq.Cfg.Internal.ValueTransformerChain.CacheEntry.md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.CacheEntry')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2 'System\.Collections\.Concurrent\.ConcurrentDictionary\`2')

<a name='Apq.Cfg.Internal.ValueTransformerChain.EvictOldestEntries(System.Collections.Concurrent.ConcurrentDictionary_string,Apq.Cfg.Internal.ValueTransformerChain.CacheEntry_,int).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')