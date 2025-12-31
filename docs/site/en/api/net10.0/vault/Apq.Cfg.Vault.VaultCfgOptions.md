### [Apq\.Cfg\.Vault](Apq.Cfg.Vault.md 'Apq\.Cfg\.Vault')

## VaultCfgOptions Class

Vault 配置选项

```csharp
public class VaultCfgOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; VaultCfgOptions

| Properties | |
| :--- | :--- |
| [Address](Apq.Cfg.Vault.VaultCfgOptions.Address.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Address') | Vault 服务器地址，例如 http://localhost:8200 |
| [AuthMethod](Apq.Cfg.Vault.VaultCfgOptions.AuthMethod.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.AuthMethod') | 认证方式 |
| [EnableHotReload](Apq.Cfg.Vault.VaultCfgOptions.EnableHotReload.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.EnableHotReload') | 是否启用热重载 |
| [EnginePath](Apq.Cfg.Vault.VaultCfgOptions.EnginePath.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.EnginePath') | 密钥引擎路径，默认为 "secret" |
| [KvVersion](Apq.Cfg.Vault.VaultCfgOptions.KvVersion.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.KvVersion') | KV Secret 引擎版本（V1 或 V2） |
| [Namespace](Apq.Cfg.Vault.VaultCfgOptions.Namespace.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Namespace') | 命名空间（Vault Enterprise） |
| [Password](Apq.Cfg.Vault.VaultCfgOptions.Password.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Password') | 密码（用于 UserPass 认证） |
| [Path](Apq.Cfg.Vault.VaultCfgOptions.Path.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Path') | 密钥路径前缀，例如 "/data/app/config" |
| [PollInterval](Apq.Cfg.Vault.VaultCfgOptions.PollInterval.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.PollInterval') | 轮询间隔（仅在不支持 Watch 的引擎中使用） |
| [ReconnectInterval](Apq.Cfg.Vault.VaultCfgOptions.ReconnectInterval.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.ReconnectInterval') | 重连间隔 |
| [RoleId](Apq.Cfg.Vault.VaultCfgOptions.RoleId.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.RoleId') | AppRole ID（用于 AppRole 认证） |
| [RoleSecret](Apq.Cfg.Vault.VaultCfgOptions.RoleSecret.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.RoleSecret') | AppRole Secret（用于 AppRole 认证） |
| [Token](Apq.Cfg.Vault.VaultCfgOptions.Token.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Token') | 认证令牌 |
| [Username](Apq.Cfg.Vault.VaultCfgOptions.Username.md 'Apq\.Cfg\.Vault\.VaultCfgOptions\.Username') | 用户名（用于 UserPass 认证） |
