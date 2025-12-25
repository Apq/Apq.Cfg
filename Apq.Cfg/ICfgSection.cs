namespace Apq.Cfg;

/// <summary>
/// 配置节接口，表示配置的一个子节
/// </summary>
public interface ICfgSection
{
    /// <summary>
    /// 获取此节的路径前缀
    /// </summary>
    string Path { get; }

    /// <summary>
    /// 获取配置值（相对于此节的键）
    /// </summary>
    string? Get(string key);

    /// <summary>
    /// 获取配置值并转换为指定类型
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// 检查配置键是否存在
    /// </summary>
    bool Exists(string key);

    /// <summary>
    /// 设置配置值
    /// </summary>
    void Set(string key, string? value, int? targetLevel = null);

    /// <summary>
    /// 移除配置键
    /// </summary>
    void Remove(string key, int? targetLevel = null);

    /// <summary>
    /// 获取子节
    /// </summary>
    ICfgSection GetSection(string key);

    /// <summary>
    /// 获取所有子节的键名
    /// </summary>
    IEnumerable<string> GetChildKeys();
}
