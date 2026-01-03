namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 字符串长度验证规则
/// </summary>
public sealed class LengthRule : IValidationRule
{
    private readonly int? _minLength;
    private readonly int? _maxLength;

    /// <summary>
    /// 创建字符串长度验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="minLength">最小长度（null 表示不限制）</param>
    /// <param name="maxLength">最大长度（null 表示不限制）</param>
    /// <param name="errorMessage">自定义错误消息</param>
    public LengthRule(string key, int? minLength = null, int? maxLength = null, string? errorMessage = null)
    {
        Key = key;
        _minLength = minLength;
        _maxLength = maxLength;
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc />
    public string Name => "Length";

    /// <inheritdoc />
    public string Key { get; }

    /// <summary>
    /// 自定义错误消息
    /// </summary>
    public string? ErrorMessage { get; }

    /// <inheritdoc />
    public ValidationError? Validate(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            // 空值不在此规则验证范围内，应由 Required 规则处理
            return null;
        }

        var length = value.Length;

        if (_minLength.HasValue && length < _minLength.Value)
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的长度 {length} 小于最小长度 {_minLength.Value}";
            return new ValidationError(Key, message, Name, value);
        }

        if (_maxLength.HasValue && length > _maxLength.Value)
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的长度 {length} 大于最大长度 {_maxLength.Value}";
            return new ValidationError(Key, message, Name, value);
        }

        return null;
    }
}
