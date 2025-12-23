namespace Apq.Cfg.Changes;

/// <summary>
/// 配置变更记录
/// </summary>
public readonly struct ConfigChange
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// 变更前的值
    /// </summary>
    public string? OldValue { get; init; }

    /// <summary>
    /// 变更后的值
    /// </summary>
    public string? NewValue { get; init; }

    /// <summary>
    /// 变更类型
    /// </summary>
    public ChangeType Type { get; init; }

    public override string ToString() =>
        $"[{Type}] {Key}: {OldValue ?? "(null)"} -> {NewValue ?? "(null)"}";
}
