namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证结果
/// </summary>
public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors;

    /// <summary>
    /// 创建验证结果
    /// </summary>
    /// <param name="errors">验证错误列表</param>
    public ValidationResult(IEnumerable<ValidationError>? errors = null)
    {
        _errors = errors?.ToList() ?? new List<ValidationError>();
    }

    /// <summary>
    /// 验证是否通过
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// 验证错误列表
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors;

    /// <summary>
    /// 错误数量
    /// </summary>
    public int ErrorCount => _errors.Count;

    /// <summary>
    /// 添加错误
    /// </summary>
    internal void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    /// <summary>
    /// 添加多个错误
    /// </summary>
    internal void AddErrors(IEnumerable<ValidationError> errors)
    {
        _errors.AddRange(errors);
    }

    /// <summary>
    /// 获取指定键的错误
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>该键的所有错误</returns>
    public IEnumerable<ValidationError> GetErrorsForKey(string key)
    {
        return _errors.Where(e => e.Key == key);
    }

    /// <summary>
    /// 检查指定键是否有错误
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>如果有错误返回 true</returns>
    public bool HasErrorsForKey(string key)
    {
        return _errors.Any(e => e.Key == key);
    }

    /// <summary>
    /// 成功的验证结果
    /// </summary>
    public static ValidationResult Success { get; } = new ValidationResult();

    /// <summary>
    /// 创建失败的验证结果
    /// </summary>
    /// <param name="errors">错误列表</param>
    public static ValidationResult Failure(params ValidationError[] errors)
    {
        return new ValidationResult(errors);
    }

    /// <summary>
    /// 创建失败的验证结果
    /// </summary>
    /// <param name="errors">错误列表</param>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors)
    {
        return new ValidationResult(errors);
    }

    /// <summary>
    /// 返回验证结果的字符串表示
    /// </summary>
    public override string ToString()
    {
        if (IsValid)
            return "Validation passed";

        return $"Validation failed with {ErrorCount} error(s):\n" +
               string.Join("\n", _errors.Select(e => $"  - {e}"));
    }
}
