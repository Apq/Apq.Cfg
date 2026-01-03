namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证异常
/// </summary>
public sealed class ConfigValidationException : Exception
{
    /// <summary>
    /// 创建配置验证异常
    /// </summary>
    /// <param name="result">验证结果</param>
    public ConfigValidationException(ValidationResult result)
        : base(BuildMessage(result))
    {
        ValidationResult = result;
    }

    /// <summary>
    /// 创建配置验证异常
    /// </summary>
    /// <param name="result">验证结果</param>
    /// <param name="message">自定义消息</param>
    public ConfigValidationException(ValidationResult result, string message)
        : base(message)
    {
        ValidationResult = result;
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public ValidationResult ValidationResult { get; }

    /// <summary>
    /// 验证错误列表
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => ValidationResult.Errors;

    private static string BuildMessage(ValidationResult result)
    {
        if (result.IsValid)
            return "配置验证通过";

        var errorMessages = result.Errors.Select(e => $"  - [{e.RuleName}] {e.Key}: {e.Message}");
        return $"配置验证失败，共 {result.ErrorCount} 个错误:\n{string.Join("\n", errorMessages)}";
    }
}
