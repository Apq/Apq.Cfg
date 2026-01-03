namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 范围验证规则
/// </summary>
/// <typeparam name="T">值类型，必须实现 IComparable</typeparam>
public sealed class RangeRule<T> : IValidationRule where T : struct, IComparable<T>
{
    private readonly T _min;
    private readonly T _max;
    private readonly Func<string, T?> _converter;

    /// <summary>
    /// 创建范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="converter">字符串到类型 T 的转换器</param>
    /// <param name="errorMessage">自定义错误消息</param>
    public RangeRule(string key, T min, T max, Func<string, T?> converter, string? errorMessage = null)
    {
        Key = key;
        _min = min;
        _max = max;
        _converter = converter;
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc />
    public string Name => "Range";

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

        T? convertedValue;
        try
        {
            convertedValue = _converter(value);
        }
        catch
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的值 '{value}' 无法转换为 {typeof(T).Name}";
            return new ValidationError(Key, message, Name, value);
        }

        if (!convertedValue.HasValue)
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的值 '{value}' 无法转换为 {typeof(T).Name}";
            return new ValidationError(Key, message, Name, value);
        }

        if (convertedValue.Value.CompareTo(_min) < 0 || convertedValue.Value.CompareTo(_max) > 0)
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的值 '{value}' 必须在 {_min} 和 {_max} 之间";
            return new ValidationError(Key, message, Name, value);
        }

        return null;
    }
}

/// <summary>
/// 范围验证规则工厂
/// </summary>
public static class RangeRule
{
    /// <summary>
    /// 创建整数范围验证规则
    /// </summary>
    public static RangeRule<int> ForInt(string key, int min, int max, string? errorMessage = null)
    {
        return new RangeRule<int>(key, min, max, s => int.TryParse(s, out var v) ? v : null, errorMessage);
    }

    /// <summary>
    /// 创建长整数范围验证规则
    /// </summary>
    public static RangeRule<long> ForLong(string key, long min, long max, string? errorMessage = null)
    {
        return new RangeRule<long>(key, min, max, s => long.TryParse(s, out var v) ? v : null, errorMessage);
    }

    /// <summary>
    /// 创建双精度浮点数范围验证规则
    /// </summary>
    public static RangeRule<double> ForDouble(string key, double min, double max, string? errorMessage = null)
    {
        return new RangeRule<double>(key, min, max, s => double.TryParse(s, out var v) ? v : null, errorMessage);
    }

    /// <summary>
    /// 创建十进制数范围验证规则
    /// </summary>
    public static RangeRule<decimal> ForDecimal(string key, decimal min, decimal max, string? errorMessage = null)
    {
        return new RangeRule<decimal>(key, min, max, s => decimal.TryParse(s, out var v) ? v : null, errorMessage);
    }

    /// <summary>
    /// 创建日期时间范围验证规则
    /// </summary>
    public static RangeRule<DateTime> ForDateTime(string key, DateTime min, DateTime max, string? errorMessage = null)
    {
        return new RangeRule<DateTime>(key, min, max, s => DateTime.TryParse(s, out var v) ? v : null, errorMessage);
    }
}
