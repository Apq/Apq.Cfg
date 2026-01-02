namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 自定义验证规则
/// </summary>
public sealed class CustomRule : IValidationRule
{
    private readonly Func<string?, bool> _validator;

    /// <summary>
    /// 创建自定义验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="validator">验证函数，返回 true 表示验证通过</param>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="ruleName">规则名称</param>
    public CustomRule(string key, Func<string?, bool> validator, string errorMessage, string? ruleName = null)
    {
        Key = key;
        _validator = validator;
        ErrorMessage = errorMessage;
        Name = ruleName ?? "Custom";
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Key { get; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; }

    /// <inheritdoc />
    public ValidationError? Validate(string? value)
    {
        if (!_validator(value))
        {
            return new ValidationError(Key, ErrorMessage, Name, value);
        }
        return null;
    }
}
