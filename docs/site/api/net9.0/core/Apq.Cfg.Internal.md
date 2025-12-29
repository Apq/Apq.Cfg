#### [Apq\.Cfg](index.md 'index')

## Apq\.Cfg\.Internal Namespace

| Classes | |
| :--- | :--- |
| [ChangeCoordinator](Apq.Cfg.Internal.ChangeCoordinator.md 'Apq\.Cfg\.Internal\.ChangeCoordinator') | 变更协调器，负责协调多个配置源的变更事件 |
| [FastCollections](Apq.Cfg.Internal.FastCollections.md 'Apq\.Cfg\.Internal\.FastCollections') | 快速集合工厂方法 |
| [FastReadOnlyDictionary&lt;TKey,TValue&gt;](Apq.Cfg.Internal.FastReadOnlyDictionary_TKey,TValue_.md 'Apq\.Cfg\.Internal\.FastReadOnlyDictionary\<TKey,TValue\>') | 高性能只读字典包装器 在 \.NET 8\+ 使用 FrozenDictionary，其他版本使用普通 Dictionary |
| [FastReadOnlySet&lt;T&gt;](Apq.Cfg.Internal.FastReadOnlySet_T_.md 'Apq\.Cfg\.Internal\.FastReadOnlySet\<T\>') | 高性能只读集合包装器 在 \.NET 8\+ 使用 FrozenSet，其他版本使用 HashSet |
| [KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser') | 键路径解析工具，使用 Span 优化避免字符串分配 |
| [MergedConfigurationProvider](Apq.Cfg.Internal.MergedConfigurationProvider.md 'Apq\.Cfg\.Internal\.MergedConfigurationProvider') | 支持增量更新的合并配置提供者 |
| [MergedConfigurationSource](Apq.Cfg.Internal.MergedConfigurationSource.md 'Apq\.Cfg\.Internal\.MergedConfigurationSource') | 合并配置源 |
| [ValueCache](Apq.Cfg.Internal.ValueCache.md 'Apq\.Cfg\.Internal\.ValueCache') | 配置值缓存，用于缓存类型转换结果 |
| [ValueConverter](Apq.Cfg.Internal.ValueConverter.md 'Apq\.Cfg\.Internal\.ValueConverter') | 值类型转换工具类 |
| [ValueMaskerChain](Apq.Cfg.Internal.ValueMaskerChain.md 'Apq\.Cfg\.Internal\.ValueMaskerChain') | 值脱敏器链，按顺序执行脱敏器 |
| [ValueTransformerChain](Apq.Cfg.Internal.ValueTransformerChain.md 'Apq\.Cfg\.Internal\.ValueTransformerChain') | 值转换器链，按优先级顺序执行转换器 |

| Structs | |
| :--- | :--- |
| [KeyPathParser\.SegmentEnumerator](Apq.Cfg.Internal.KeyPathParser.SegmentEnumerator.md 'Apq\.Cfg\.Internal\.KeyPathParser\.SegmentEnumerator') | 键段枚举器（ref struct，零堆分配） |
| [ValueTransformerChain\.CacheEntry](Apq.Cfg.Internal.ValueTransformerChain.CacheEntry.md 'Apq\.Cfg\.Internal\.ValueTransformerChain\.CacheEntry') | 缓存条目 |
