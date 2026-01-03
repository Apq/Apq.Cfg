namespace Apq.Cfg.Sources;

/// <summary>
/// 配置源信息（用于序列化和 API 响应）
/// </summary>
public sealed class ConfigSourceInfo
{
    /// <summary>
    /// 配置源层级（优先级，数字越大优先级越高）
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// 配置源名称（同一层级内唯一）
    /// </summary>
    public string Name { get; init; } = "";

    /// <summary>
    /// 配置源类型名称
    /// </summary>
    public string Type { get; init; } = "";

    /// <summary>
    /// 是否可写
    /// </summary>
    public bool IsWriteable { get; init; }

    /// <summary>
    /// 是否为主写入源
    /// </summary>
    public bool IsPrimaryWriter { get; init; }

    /// <summary>
    /// 配置项数量（所有叶子节点的总数）
    /// </summary>
    public int KeyCount { get; init; }

    /// <summary>
    /// 顶级配置键数量（只统计第一层节点）
    /// </summary>
    public int TopLevelKeyCount { get; init; }
}
