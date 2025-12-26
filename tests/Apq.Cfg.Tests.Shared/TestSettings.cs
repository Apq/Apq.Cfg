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

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public static string? RedisConnectionString => _configuration["TestConnections:Redis"];

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public static string? DatabaseConnectionString => _configuration["TestConnections:Database"];

    /// <summary>
    /// 数据库提供程序类型
    /// </summary>
    public static string? DatabaseProvider => _configuration["TestConnections:DatabaseProvider"];

    /// <summary>
    /// Zookeeper 连接字符串
    /// </summary>
    public static string? ZookeeperConnectionString => _configuration["TestConnections:Zookeeper"];

    /// <summary>
    /// 检查 Redis 连接是否已配置
    /// </summary>
    public static bool IsRedisConfigured => !string.IsNullOrWhiteSpace(RedisConnectionString);

    /// <summary>
    /// 检查数据库连接是否已配置
    /// </summary>
    public static bool IsDatabaseConfigured =>
        !string.IsNullOrWhiteSpace(DatabaseConnectionString) &&
        !string.IsNullOrWhiteSpace(DatabaseProvider);

    /// <summary>
    /// 检查 Zookeeper 连接是否已配置
    /// </summary>
    public static bool IsZookeeperConfigured => !string.IsNullOrWhiteSpace(ZookeeperConnectionString);
}
