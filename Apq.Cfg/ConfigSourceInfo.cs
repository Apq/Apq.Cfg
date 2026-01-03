namespace Apq.Cfg;

/// <summary>
/// 配置源信息（用于查询和展示）
/// </summary>
public sealed class ConfigSourceInfo
{
    /// <summary>
    /// 配置源层级（优先级，数字越大优先级越高）
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 配置源名称（同一层级内唯一）
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 配置源类型名称
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// 是否可写
    /// </summary>
    public bool IsWriteable { get; set; }

    /// <summary>
    /// 是否为主写入源
    /// </summary>
    public bool IsPrimaryWriter { get; set; }

    /// <summary>
    /// 配置项数量
    /// </summary>
    public int KeyCount { get; set; }
}
