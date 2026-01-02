namespace Apq.Cfg.Vault;

/// <summary>
/// CfgBuilder 的 Vault 扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Vault 配置源（通用方法）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">配置选项</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Vault"/> (18)</param>
    /// <param name="isPrimaryWriter">是否为主写入源，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddVault(
        this CfgBuilder builder,
        Action<VaultCfgOptions> configure,
        int level = CfgSourceLevels.Vault,
        bool isPrimaryWriter = false)
    {
        var options = new VaultCfgOptions();
        configure?.Invoke(options);
        builder.AddSource(new VaultCfgSource(options, level, isPrimaryWriter));
        return builder;
    }

    /// <summary>
    /// 添加 Vault KV V1 配置源（简化方法）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="address">Vault 服务地址</param>
    /// <param name="token">Vault Token</param>
    /// <param name="enginePath">KV 引擎路径，默认为 "kv"</param>
    /// <param name="path">密钥路径，默认为空</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Vault"/> (18)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为true</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddVaultV1(
        this CfgBuilder builder,
        string address,
        string token,
        string enginePath = "kv",
        string path = "",
        int level = CfgSourceLevels.Vault,
        bool enableHotReload = true)
    {
        return builder.AddVault(options =>
        {
            options.Address = address;
            options.Token = token;
            options.EnginePath = enginePath;
            options.Path = path;
            options.EnableHotReload = enableHotReload;
            options.KvVersion = 1;
        }, level);
    }

    /// <summary>
    /// 添加 Vault KV V2 配置源（简化方法）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="address">Vault 服务地址</param>
    /// <param name="token">Vault Token</param>
    /// <param name="enginePath">KV 引擎路径，默认为 "kv"</param>
    /// <param name="path">密钥路径，默认为空</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Vault"/> (18)</param>
    /// <param name="enableHotReload">是否启用热重载，默认为true</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddVaultV2(
        this CfgBuilder builder,
        string address,
        string token,
        string enginePath = "kv",
        string path = "",
        int level = CfgSourceLevels.Vault,
        bool enableHotReload = true)
    {
        return builder.AddVault(options =>
        {
            options.Address = address;
            options.Token = token;
            options.EnginePath = enginePath;
            options.Path = path;
            options.EnableHotReload = enableHotReload;
            options.KvVersion = 2;
        }, level);
    }

    /// <summary>
    /// 添加 Vault 配置源（使用 UserPass 认证）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="address">Vault 服务地址</param>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="enginePath">KV 引擎路径，默认为 "kv"</param>
    /// <param name="path">密钥路径，默认为空</param>
    /// <param name="kvVersion">KV 引擎版本 (1 或 2)，默认为 2</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Vault"/> (18)</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddVaultUserPass(
        this CfgBuilder builder,
        string address,
        string username,
        string password,
        string enginePath = "kv",
        string path = "",
        int kvVersion = 2,
        int level = CfgSourceLevels.Vault)
    {
        return builder.AddVault(options =>
        {
            options.Address = address;
            options.AuthMethod = VaultAuthMethod.UserPass;
            options.Username = username;
            options.Password = password;
            options.EnginePath = enginePath;
            options.Path = path;
            options.KvVersion = kvVersion;
            options.EnableHotReload = true;
        }, level);
    }

    /// <summary>
    /// 添加 Vault 配置源（使用 AppRole 认证）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="address">Vault 服务地址</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="roleSecret">Role Secret</param>
    /// <param name="enginePath">KV 引擎路径，默认为 "kv"</param>
    /// <param name="path">密钥路径，默认为空</param>
    /// <param name="kvVersion">KV 引擎版本 (1 或 2)，默认为 2</param>
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Vault"/> (18)</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddVaultAppRole(
        this CfgBuilder builder,
        string address,
        string roleId,
        string roleSecret,
        string enginePath = "kv",
        string path = "",
        int kvVersion = 2,
        int level = CfgSourceLevels.Vault)
    {
        return builder.AddVault(options =>
        {
            options.Address = address;
            options.AuthMethod = VaultAuthMethod.AppRole;
            options.RoleId = roleId;
            options.RoleSecret = roleSecret;
            options.EnginePath = enginePath;
            options.Path = path;
            options.KvVersion = kvVersion;
            options.EnableHotReload = true;
        }, level);
    }
}
