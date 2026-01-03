namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 必填验证规则
/// </summary>
public sealed class RequiredRule : IValidationRule
{
    /// <summary>
    /// 创建必填验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="errorMessage">自定义错误消息</param>
    public RequiredRule(string key, string? errorMessage = null)
    {
        Key = key;
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc />
    public string Name => "Required";

    /// <inheritdoc />
    public string Key { get; }

    /// <summary>
    /// 自定义错误消息
    /// </summary>
    public string? ErrorMessage { get; }

    /// <inheritdoc />
    public ValidationError? Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 是必填的";
            return new ValidationError(Key, message, Name, value);
        }
        return null;
    }
}
