### [Apq\.Cfg\.Zookeeper](Apq.Cfg.Zookeeper.md 'Apq\.Cfg\.Zookeeper')

## ZookeeperCfgOptions Class

Zookeeper 配置选项

```csharp
public sealed class ZookeeperCfgOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ZookeeperCfgOptions

| Properties | |
| :--- | :--- |
| [AuthInfo](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.AuthInfo.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.AuthInfo') | 认证信息（可选），如 "user:password" |
| [AuthScheme](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.AuthScheme.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.AuthScheme') | 认证方案（可选），如 "digest" |
| [ConnectionString](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.ConnectionString.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.ConnectionString') | Zookeeper 连接字符串，默认 localhost:2181 支持多节点：host1:2181,host2:2181,host3:2181 |
| [ConnectTimeout](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.ConnectTimeout.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.ConnectTimeout') | 连接超时时间，默认 10 秒 |
| [DataFormat](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.DataFormat.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.DataFormat') | 配置数据格式，默认 KeyValue（每个节点一个值） |
| [EnableHotReload](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.EnableHotReload.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.EnableHotReload') | 是否启用热重载，默认 true |
| [ReconnectInterval](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.ReconnectInterval.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.ReconnectInterval') | 重连间隔，默认 5 秒 |
| [RootPath](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.RootPath.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.RootPath') | 根路径，默认 "/config" |
| [SessionTimeout](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.SessionTimeout.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.SessionTimeout') | 会话超时时间，默认 30 秒 |
| [SingleNode](Apq.Cfg.Zookeeper.ZookeeperCfgOptions.SingleNode.md 'Apq\.Cfg\.Zookeeper\.ZookeeperCfgOptions\.SingleNode') | 当 DataFormat 为 Json 时，指定要读取的节点路径（相对于 RootPath） |
