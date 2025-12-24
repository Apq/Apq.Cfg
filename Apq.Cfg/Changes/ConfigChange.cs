namespace Apq.Cfg.Changes;

/// <summary>
/// 配置变更记录
/// </summary>
public readonly struct ConfigChange
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 变更前的值
    /// </summary>
    public string? OldValue { get; }

    /// <summary>
    /// 变更后的值
    /// </summary>
    public string? NewValue { get; }

    /// <summary>
    /// 变更类型
    /// </summary>
    public ChangeType Type { get; }

    public ConfigChange(string key, string? oldValue, string? newValue, ChangeType type)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
        Type = type;
    }

    public override string ToString() =>
        $"[{Type}] {Key}: {OldValue ?? "(null)"} -> {NewValue ?? "(null)"}";
}
