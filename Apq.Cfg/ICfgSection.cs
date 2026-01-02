namespace Apq.Cfg;

/// <summary>
/// 配置节接口，表示配置的一个子节
/// </summary>
public interface ICfgSection
{
    /// <summary>
    /// 通过索引器获取或设置配置值（相对于此节的键）
    /// </summary>
    /// <param name="key">相对于此节的键名</param>
    /// <returns>配置值，不存在时返回null</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    ///
    /// // 读取配置
    /// var host = dbSection["Host"]; // 等同于 cfg["Database:Host"]
    ///
    /// // 写入配置
    /// dbSection["Host"] = "localhost"; // 等同于 cfg["Database:Host"] = "localhost"
    /// </code>
    /// </example>
    string? this[string key] { get; set; }

    /// <summary>
    /// 获取此节的路径前缀
    /// </summary>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// Console.WriteLine(dbSection.Path); // 输出: Database
    /// </code>
    /// </example>
    string Path { get; }

    /// <summary>
    /// 获取配置值（相对于此节的键）
    /// </summary>
    /// <param name="key">相对于此节的键名</param>
    /// <returns>配置值，不存在时返回null</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// var host = dbSection.Get("Host"); // 等同于 cfg.Get("Database:Host")
    /// </code>
    /// </example>
    string? Get(string key);

    /// <summary>
    /// 获取配置值并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">相对于此节的键名</param>
    /// <returns>转换后的配置值，不存在或转换失败时返回默认值</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// var port = dbSection.GetValue&lt;int&gt;("Port"); // 等同于 cfg.GetValue&lt;int&gt;("Database:Port")
    /// </code>
    /// </example>
    T? GetValue<T>(string key);

    /// <summary>
    /// 检查配置键是否存在
    /// </summary>
    /// <param name="key">相对于此节的键名</param>
    /// <returns>存在返回true，否则返回false</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// if (dbSection.Exists("ConnectionString"))
    /// {
    ///     // 处理连接字符串
    /// }
    /// </code>
    /// </example>
    bool Exists(string key);

    /// <summary>
    /// 设置配置值
    /// </summary>
    /// <param name="key">相对于此节的键名</param>
    /// <param name="value">配置值</param>
    /// <param name="targetLevel">目标层级，为null时使用可写的最高层级</param>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// dbSection.SetValue("Timeout", "60"); // 等同于 cfg.SetValue("Database:Timeout", "60")
    /// </code>
    /// </example>
    void SetValue(string key, string? value, int? targetLevel = null);

    /// <summary>
    /// 移除配置键
    /// </summary>
    /// <param name="key">相对于此节的键名</param>
    /// <param name="targetLevel">目标层级，为null时从所有层级移除</param>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// dbSection.Remove("OldSetting"); // 等同于 cfg.Remove("Database:OldSetting")
    /// </code>
    /// </example>
    void Remove(string key, int? targetLevel = null);

    /// <summary>
    /// 获取子节
    /// </summary>
    /// <param name="key">子节键名</param>
    /// <returns>子节对象</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// var connSection = dbSection.GetSection("Connection"); // 等同于 cfg.GetSection("Database:Connection")
    /// </code>
    /// </example>
    ICfgSection GetSection(string key);

    /// <summary>
    /// 获取所有子节的键名
    /// </summary>
    /// <returns>子节键名集合</returns>
    /// <example>
    /// <code>
    /// var dbSection = cfg.GetSection("Database");
    /// foreach (var key in dbSection.GetChildKeys())
    /// {
    ///     Console.WriteLine($"{key}: {dbSection.Get(key)}");
    /// }
    /// </code>
    /// </example>
    IEnumerable<string> GetChildKeys();
}
