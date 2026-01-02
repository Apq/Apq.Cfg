namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 枚举值验证规则 - 验证值是否在允许的值列表中
/// </summary>
public sealed class EnumValuesRule : IValidationRule
{
    private readonly HashSet<string> _allowedValues;
    private readonly StringComparison _comparison;

    /// <summary>
    /// 创建枚举值验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="allowedValues">允许的值列表</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <param name="errorMessage">自定义错误消息</param>
    public EnumValuesRule(string key, IEnumerable<string> allowedValues, bool ignoreCase = false, string? errorMessage = null)
    {
        Key = key;
        _comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        _allowedValues = new HashSet<string>(allowedValues, ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc />
    public string Name => "EnumValues";

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
            // 空值不在此规则验证范围内，应由 Required 规则处理
            return null;
        }

        if (!_allowedValues.Contains(value))
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的值 '{value}' 必须是以下值之一: {string.Join(", ", _allowedValues)}";
            return new ValidationError(Key, message, Name, value);
        }

        return null;
    }
}
