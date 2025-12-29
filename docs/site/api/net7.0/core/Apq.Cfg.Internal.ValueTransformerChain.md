#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## ValueTransformerChain Class

值转换器链，按优先级顺序执行转换器

```csharp
internal sealed class ValueTransformerChain
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ValueTransformerChain

### Remarks
性能优化：
1\. 解密结果缓存 \- 首次解密后缓存明文，后续读取直接返回缓存
2\. 使用 StringComparison\.Ordinal 进行前缀检查
3\. 支持缓存失效（配置变更时）
4\. LRU 缓存策略，限制最大缓存数量

| Constructors | |
| :--- | :--- |
| [ValueTransformerChain\(IEnumerable&lt;IValueTransformer&gt;, ValueTransformerOptions\)](Apq.Cfg.Internal.ValueTransformerChain.ValueTransformerChain(System.Collections.Generic.IEnumerable_Apq.Cfg.Security.IValueTransformer_,Apq.Cfg.Security.ValueTransformerOptions).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.ValueTransformerChain\(System\.Collections\.Generic\.IEnumerable\<Apq\.Cfg\.Security\.IValueTransformer\>, Apq\.Cfg\.Security\.ValueTransformerOptions\)') | 初始化值转换器链 |

| Methods | |
| :--- | :--- |
| [AddToCache\(string, string\)](Apq.Cfg.Internal.ValueTransformerChain.AddToCache(string,string).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.AddToCache\(string, string\)') | 添加到解密缓存，带 LRU 淘汰 |
| [AddToNoTransformCache\(string\)](Apq.Cfg.Internal.ValueTransformerChain.AddToNoTransformCache(string).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.AddToNoTransformCache\(string\)') | 添加到不需要转换的缓存，带 LRU 淘汰 |
| [ClearCache\(\)](Apq.Cfg.Internal.ValueTransformerChain.ClearCache().md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.ClearCache\(\)') | 清除所有缓存 |
| [EvictOldestEntries\(ConcurrentDictionary&lt;string,CacheEntry&gt;, int\)](Apq.Cfg.Internal.ValueTransformerChain.EvictOldestEntries(System.Collections.Concurrent.ConcurrentDictionary_string,Apq.Cfg.Internal.ValueTransformerChain.CacheEntry_,int).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.EvictOldestEntries\(System\.Collections\.Concurrent\.ConcurrentDictionary\<string,Apq\.Cfg\.Internal\.ValueTransformerChain\.CacheEntry\>, int\)') | 淘汰最旧的缓存条目 |
| [EvictOldestNoTransformEntries\(int\)](Apq.Cfg.Internal.ValueTransformerChain.EvictOldestNoTransformEntries(int).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.EvictOldestNoTransformEntries\(int\)') | 淘汰最旧的不需要转换的缓存条目 |
| [GetCacheStats\(\)](Apq.Cfg.Internal.ValueTransformerChain.GetCacheStats().md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.GetCacheStats\(\)') | 获取缓存统计信息 |
| [InvalidateCache\(string\)](Apq.Cfg.Internal.ValueTransformerChain.InvalidateCache(string).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.InvalidateCache\(string\)') | 使指定键的缓存失效（配置变更时调用） |
| [TransformOnRead\(string, string\)](Apq.Cfg.Internal.ValueTransformerChain.TransformOnRead(string,string).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.TransformOnRead\(string, string\)') | 读取时转换（如解密），带缓存优化 |
| [TransformOnWrite\(string, string\)](Apq.Cfg.Internal.ValueTransformerChain.TransformOnWrite(string,string).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.TransformOnWrite\(string, string\)') | 写入时转换（如加密） |
| [WarmupCache\(IEnumerable&lt;string&gt;, Func&lt;string,string&gt;\)](Apq.Cfg.Internal.ValueTransformerChain.WarmupCache(System.Collections.Generic.IEnumerable_string_,System.Func_string,string_).md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.WarmupCache\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Func\<string,string\>\)') | 预热缓存：预先解密所有加密值 |
