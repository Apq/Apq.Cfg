### [Apq\.Cfg\.Consul](Apq.Cfg.Consul.md 'Apq\.Cfg\.Consul')

## ConsulCfgOptions Class

Consul 配置选项

```csharp
public sealed class ConsulCfgOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ConsulCfgOptions

| Properties | |
| :--- | :--- |
| [Address](Apq.Cfg.Consul.ConsulCfgOptions.Address.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.Address') | Consul 服务地址，默认 http://localhost:8500 |
| [ConnectTimeout](Apq.Cfg.Consul.ConsulCfgOptions.ConnectTimeout.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.ConnectTimeout') | 连接超时时间，默认 10 秒 |
| [Datacenter](Apq.Cfg.Consul.ConsulCfgOptions.Datacenter.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.Datacenter') | 数据中心名称（可选） |
| [DataFormat](Apq.Cfg.Consul.ConsulCfgOptions.DataFormat.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.DataFormat') | 配置数据格式，默认 KeyValue（每个 key 一个值） |
| [EnableHotReload](Apq.Cfg.Consul.ConsulCfgOptions.EnableHotReload.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.EnableHotReload') | 是否启用热重载，默认 true |
| [KeyPrefix](Apq.Cfg.Consul.ConsulCfgOptions.KeyPrefix.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.KeyPrefix') | KV 键前缀，默认 "config/" |
| [ReconnectInterval](Apq.Cfg.Consul.ConsulCfgOptions.ReconnectInterval.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.ReconnectInterval') | 重连间隔，默认 5 秒 |
| [SingleKey](Apq.Cfg.Consul.ConsulCfgOptions.SingleKey.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.SingleKey') | 当 DataFormat 为 Json/Yaml 时，指定要读取的单个 key |
| [Token](Apq.Cfg.Consul.ConsulCfgOptions.Token.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.Token') | ACL Token（可选） |
| [WaitTime](Apq.Cfg.Consul.ConsulCfgOptions.WaitTime.md 'Apq\.Cfg\.Consul\.ConsulCfgOptions\.WaitTime') | Blocking Query 等待时间，默认 5 分钟 |
