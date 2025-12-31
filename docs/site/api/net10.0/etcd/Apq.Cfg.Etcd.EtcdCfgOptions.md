### [Apq\.Cfg\.Etcd](Apq.Cfg.Etcd.md 'Apq\.Cfg\.Etcd')

## EtcdCfgOptions Class

Etcd 配置选项

```csharp
public sealed class EtcdCfgOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EtcdCfgOptions

| Properties | |
| :--- | :--- |
| [CaCertPath](Apq.Cfg.Etcd.EtcdCfgOptions.CaCertPath.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.CaCertPath') | CA 证书路径（可选，用于 TLS） |
| [ClientCertPath](Apq.Cfg.Etcd.EtcdCfgOptions.ClientCertPath.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.ClientCertPath') | 客户端证书路径（可选，用于 mTLS） |
| [ClientKeyPath](Apq.Cfg.Etcd.EtcdCfgOptions.ClientKeyPath.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.ClientKeyPath') | 客户端私钥路径（可选，用于 mTLS） |
| [ConnectTimeout](Apq.Cfg.Etcd.EtcdCfgOptions.ConnectTimeout.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.ConnectTimeout') | 连接超时时间，默认 10 秒 |
| [DataFormat](Apq.Cfg.Etcd.EtcdCfgOptions.DataFormat.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.DataFormat') | 配置数据格式，默认 KeyValue（每个 key 一个值） |
| [EnableHotReload](Apq.Cfg.Etcd.EtcdCfgOptions.EnableHotReload.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.EnableHotReload') | 是否启用热重载，默认 true |
| [Endpoints](Apq.Cfg.Etcd.EtcdCfgOptions.Endpoints.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.Endpoints') | Etcd 服务端点列表，默认 \["localhost:2379"\] |
| [KeyPrefix](Apq.Cfg.Etcd.EtcdCfgOptions.KeyPrefix.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.KeyPrefix') | KV 键前缀，默认 "/config/" |
| [Password](Apq.Cfg.Etcd.EtcdCfgOptions.Password.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.Password') | 密码（可选） |
| [ReconnectInterval](Apq.Cfg.Etcd.EtcdCfgOptions.ReconnectInterval.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.ReconnectInterval') | 重连间隔，默认 5 秒 |
| [SingleKey](Apq.Cfg.Etcd.EtcdCfgOptions.SingleKey.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.SingleKey') | 当 DataFormat 为 Json 时，指定要读取的单个 key |
| [Username](Apq.Cfg.Etcd.EtcdCfgOptions.Username.md 'Apq\.Cfg\.Etcd\.EtcdCfgOptions\.Username') | 用户名（可选） |
