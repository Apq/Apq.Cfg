namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 批量更新请求
/// </summary>
public sealed class BatchUpdateRequest
{
    public Dictionary<string, string?> Values { get; set; } = new();

    /// <summary>
    /// 目标配置源的层级（可选，不指定则写入主写入源）
    /// </summary>
    public int? TargetLevel { get; set; }

    /// <summary>
    /// 目标配置源的名称（可选，不指定则写入主写入源）
    /// </summary>
    public string? TargetName { get; set; }
}
