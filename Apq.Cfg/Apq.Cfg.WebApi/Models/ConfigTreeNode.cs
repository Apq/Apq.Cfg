namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 配置树节点
/// </summary>
public sealed class ConfigTreeNode
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public bool HasValue { get; set; }
    public bool IsMasked { get; set; }
    public List<ConfigTreeNode> Children { get; set; } = new();
}
