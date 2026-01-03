using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Tests;

/// <summary>
/// 测试配置读取器，用于读取 appsettings.json 中的测试配置
/// </summary>
public static class TestSettings
{
    private static readonly IConfigurationRoot _configuration;

    static TestSettings()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        _configuration = builder.Build();
    }

    #region Redis

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public static string? RedisConnectionString => _configuration["TestConnections:Redis"];

    /// <summary>
    /// 检查 Redis 连接是否已配置
    /// </summary>
    public static bool IsRedisConfigured => !string.IsNullOrWhiteSpace(RedisConnectionString);

    #endregion

    #region Database

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public static string? DatabaseConnectionString => _configuration["TestConnections:Database"];

    /// <summary>
    /// 数据库提供程序类型
    /// </summary>
    public static string? DatabaseProvider => _configuration["TestConnections:DatabaseProvider"];

    /// <summary>
    /// 检查数据库连接是否已配置
    /// </summary>
    public static bool IsDatabaseConfigured =>
        !string.IsNullOrWhiteSpace(DatabaseConnectionString) &&
        !string.IsNullOrWhiteSpace(DatabaseProvider);

    #endregion

    #region Zookeeper

    /// <summary>
    /// Zookeeper 连接字符串
    /// </summary>
    public static string? ZookeeperConnectionString => _configuration["TestConnections:Zookeeper"];

    /// <summary>
    /// 检查 Zookeeper 连接是否已配置
    /// </summary>
    public static bool IsZookeeperConfigured => !string.IsNullOrWhiteSpace(ZookeeperConnectionString);

    #endregion

    #region Apollo

    /// <summary>
    /// Apollo AppId
    /// </summary>
    public static string? ApolloAppId => _configuration["TestConnections:Apollo:AppId"];

    /// <summary>
    /// Apollo Meta Server 地址
    /// </summary>
    public static string? ApolloMetaServer => _configuration["TestConnections:Apollo:MetaServer"];

    /// <summary>
    /// Apollo 命名空间
    /// </summary>
    public static string? ApolloNamespace => _configuration["TestConnections:Apollo:Namespace"];

    /// <summary>
    /// 检查 Apollo 是否已配置
    /// </summary>
    public static bool IsApolloConfigured =>
        !string.IsNullOrWhiteSpace(ApolloAppId) &&
        !string.IsNullOrWhiteSpace(ApolloMetaServer);

    #endregion

    #region Consul

    /// <summary>
    /// Consul 服务地址
    /// </summary>
    public static string? ConsulAddress => _configuration["TestConnections:Consul:Address"];

    /// <summary>
    /// Consul ACL Token
    /// </summary>
    public static string? ConsulToken => _configuration["TestConnections:Consul:Token"];

    /// <summary>
    /// Consul KV 前缀
    /// </summary>
    public static string? ConsulKeyPrefix => _configuration["TestConnections:Consul:KeyPrefix"];

    /// <summary>
    /// 检查 Consul 是否已配置
    /// </summary>
    public static bool IsConsulConfigured => !string.IsNullOrWhiteSpace(ConsulAddress);

    #endregion

    #region Etcd

    /// <summary>
    /// Etcd 连接字符串
    /// </summary>
    public static string? EtcdConnectionString => _configuration["TestConnections:Etcd:ConnectionString"];

    /// <summary>
    /// Etcd 用户名
    /// </summary>
    public static string? EtcdUsername => _configuration["TestConnections:Etcd:Username"];

    /// <summary>
    /// Etcd 密码
    /// </summary>
    public static string? EtcdPassword => _configuration["TestConnections:Etcd:Password"];

    /// <summary>
    /// Etcd 键前缀
    /// </summary>
    public static string? EtcdKeyPrefix => _configuration["TestConnections:Etcd:KeyPrefix"];

    /// <summary>
    /// 检查 Etcd 是否已配置
    /// </summary>
    public static bool IsEtcdConfigured => !string.IsNullOrWhiteSpace(EtcdConnectionString);

    #endregion

    #region Nacos

    /// <summary>
    /// Nacos 服务地址
    /// </summary>
    public static string? NacosServerAddress => _configuration["TestConnections:Nacos:ServerAddress"];

    /// <summary>
    /// Nacos 命名空间
    /// </summary>
    public static string? NacosNamespace => _configuration["TestConnections:Nacos:Namespace"];

    /// <summary>
    /// Nacos 分组
    /// </summary>
    public static string? NacosGroup => _configuration["TestConnections:Nacos:Group"];

    /// <summary>
    /// Nacos DataId
    /// </summary>
    public static string? NacosDataId => _configuration["TestConnections:Nacos:DataId"];

    /// <summary>
    /// 检查 Nacos 是否已配置
    /// </summary>
    public static bool IsNacosConfigured => !string.IsNullOrWhiteSpace(NacosServerAddress);

    #endregion

    #region Vault

    /// <summary>
    /// Vault 服务地址
    /// </summary>
    public static string? VaultAddress => _configuration["TestConnections:Vault:Address"];

    /// <summary>
    /// Vault Token
    /// </summary>
    public static string? VaultToken => _configuration["TestConnections:Vault:Token"];

    /// <summary>
    /// Vault KV 引擎路径
    /// </summary>
    public static string? VaultEnginePath => _configuration["TestConnections:Vault:EnginePath"];

    /// <summary>
    /// Vault 密钥路径
    /// </summary>
    public static string? VaultPath => _configuration["TestConnections:Vault:Path"];

    /// <summary>
    /// Vault KV 版本 (1 或 2)
    /// </summary>
    public static int VaultKvVersion => int.TryParse(_configuration["TestConnections:Vault:KvVersion"], out var v) ? v : 2;

    /// <summary>
    /// 检查 Vault 是否已配置
    /// </summary>
    public static bool IsVaultConfigured =>
        !string.IsNullOrWhiteSpace(VaultAddress) &&
        !string.IsNullOrWhiteSpace(VaultToken);

    #endregion
}
