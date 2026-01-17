namespace Apq.Cfg;

/// <summary>
/// 配置源默认层级常量
/// </summary>
/// <remarks>
/// <para>层级规划原则：数值越大优先级越高，高层级覆盖低层级</para>
/// <para>
/// | 层级范围 | 用途 | 配置源 |
/// |----------|------|--------|
/// | 0 | 基础配置文件 | Json, Ini, Xml, Yaml, Toml |
/// | 100 | 远程存储 | Redis, Database |
/// | 200 | 配置中心 | Consul, Etcd, Nacos, Apollo, Zookeeper |
/// | 300 | 密钥管理 | Vault |
/// | 400 | 环境相关 | .env, EnvironmentVariables（最高优先级覆盖） |
/// </para>
/// </remarks>
public static class CfgSourceLevels
{
    #region 本地文件配置源 (0-99)

    /// <summary>
    /// JSON 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件，通常作为应用程序的默认配置</remarks>
    public const int Json = 0;

    /// <summary>
    /// INI 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件</remarks>
    public const int Ini = 0;

    /// <summary>
    /// XML 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件</remarks>
    public const int Xml = 0;

    /// <summary>
    /// YAML 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件</remarks>
    public const int Yaml = 0;

    /// <summary>
    /// TOML 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件</remarks>
    public const int Toml = 0;

    /// <summary>
    /// HCL 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件，HashiCorp Configuration Language</remarks>
    public const int Hcl = 0;

    /// <summary>
    /// Properties 文件默认层级 (0)
    /// </summary>
    /// <remarks>基础配置文件，Java Properties 格式</remarks>
    public const int Properties = 0;

    #endregion

    #region 远程存储 (100-199)

    /// <summary>
    /// Redis 配置源默认层级 (100)
    /// </summary>
    /// <remarks>远程存储配置源，优先级高于本地文件</remarks>
    public const int Redis = 100;

    /// <summary>
    /// 数据库配置源默认层级 (100)
    /// </summary>
    /// <remarks>远程存储配置源，优先级高于本地文件</remarks>
    public const int Database = 100;

    #endregion

    #region 配置中心 (200-299)

    /// <summary>
    /// Consul 配置中心默认层级 (200)
    /// </summary>
    /// <remarks>配置中心，优先级高于远程存储</remarks>
    public const int Consul = 200;

    /// <summary>
    /// Etcd 配置中心默认层级 (200)
    /// </summary>
    /// <remarks>配置中心，优先级高于远程存储</remarks>
    public const int Etcd = 200;

    /// <summary>
    /// Nacos 配置中心默认层级 (200)
    /// </summary>
    /// <remarks>配置中心，优先级高于远程存储</remarks>
    public const int Nacos = 200;

    /// <summary>
    /// Apollo 配置中心默认层级 (200)
    /// </summary>
    /// <remarks>配置中心，优先级高于远程存储</remarks>
    public const int Apollo = 200;

    /// <summary>
    /// Zookeeper 配置中心默认层级 (200)
    /// </summary>
    /// <remarks>配置中心，优先级高于远程存储</remarks>
    public const int Zookeeper = 200;

    #endregion

    #region 密钥管理 (300-399)

    /// <summary>
    /// HashiCorp Vault 密钥管理默认层级 (300)
    /// </summary>
    /// <remarks>密钥管理，优先级高于配置中心，用于敏感配置</remarks>
    public const int Vault = 300;

    #endregion

    #region 环境相关 (400+)

    /// <summary>
    /// .env 文件默认层级 (400)
    /// </summary>
    /// <remarks>
    /// .env 文件用于环境特定配置，与环境变量同级。
    /// 当 setEnvironmentVariables=true 时，.env 内容会写入系统环境变量。
    /// </remarks>
    public const int Env = 400;

    /// <summary>
    /// 环境变量默认层级 (400)
    /// </summary>
    /// <remarks>环境变量具有最高优先级，用于运行时覆盖</remarks>
    public const int EnvironmentVariables = 400;

    #endregion
}
