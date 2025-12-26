namespace Apq.Cfg.Vault;

/// <summary>
/// Vault 配置选项
/// </summary>
public class VaultCfgOptions
{
    /// <summary>
    /// Vault 服务器地址，例如 http://localhost:8200
    /// </summary>
    public string Address { get; set; } = "http://localhost:8200";

    /// <summary>
    /// 认证令牌
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 密钥引擎路径，默认为 "secret"
    /// </summary>
    public string EnginePath { get; set; } = "secret";

    /// <summary>
    /// 密钥路径前缀，例如 "/data/app/config"
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 命名空间（Vault Enterprise）
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// 是否启用热重载
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

    /// <summary>
    /// 轮询间隔（仅在不支持 Watch 的引擎中使用）
    /// </summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 重连间隔
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 认证方式
    /// </summary>
    public VaultAuthMethod AuthMethod { get; set; } = VaultAuthMethod.Token;

    /// <summary>
    /// 用户名（用于 UserPass 认证）
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码（用于 UserPass 认证）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// AppRole ID（用于 AppRole 认证）
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// AppRole Secret（用于 AppRole 认证）
    /// </summary>
    public string? RoleSecret { get; set; }

    /// <summary>
    /// KV Secret 引擎版本（V1 或 V2）
    /// </summary>
    public int KvVersion { get; set; } = 2;
}

/// <summary>
/// Vault 认证方式
/// </summary>
public enum VaultAuthMethod
{
    /// <summary>
    /// Token 认证
    /// </summary>
    Token,

    /// <summary>
    /// UserPass 认证
    /// </summary>
    UserPass,

    /// <summary>
    /// AppRole 认证
    /// </summary>
    AppRole
}
