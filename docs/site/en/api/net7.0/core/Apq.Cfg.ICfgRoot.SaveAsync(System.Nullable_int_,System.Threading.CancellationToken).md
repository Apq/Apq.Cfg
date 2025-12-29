#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.SaveAsync\(Nullable\<int\>, CancellationToken\) Method

保存配置更改到持久化存储

```csharp
System.Threading.Tasks.Task SaveAsync(System.Nullable<int> targetLevel=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.SaveAsync(System.Nullable_int_,System.Threading.CancellationToken).targetLevel'></a>

`targetLevel` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

目标层级，为null时保存所有可写层级

<a name='Apq.Cfg.ICfgRoot.SaveAsync(System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

取消令牌

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')  
表示异步操作的任务

### Example

```csharp
await cfg.SaveAsync();
// 或指定特定层级
await cfg.SaveAsync(targetLevel: 1);
```