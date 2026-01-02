namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证错误
/// </summary>
public sealed class ValidationError
{
    /// <summary>
    /// 创建验证错误
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="message">错误消息</param>
    /// <param name="ruleName">规则名称</param>
    /// <param name="actualValue">实际值</param>
    public ValidationError(string key, string message, string ruleName, string? actualValue = null)
    {
        Key = key;
        Message = message;
        RuleName = ruleName;
        ActualValue = actualValue;
    }

    /// <summary>
    /// 配置键
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 规则名称
    /// </summary>
    public string RuleName { get; }

    /// <summary>
    /// 实际值（可能为 null 或脱敏后的值）
    /// </summary>
    public string? ActualValue { get; }

    /// <summary>
    /// 返回错误的字符串表示
    /// </summary>
    public override string ToString() => $"[{RuleName}] {Key}: {Message}";
}
