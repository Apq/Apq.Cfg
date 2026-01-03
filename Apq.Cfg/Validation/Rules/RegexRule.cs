using System.Text.RegularExpressions;

namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 正则表达式验证规则
/// </summary>
public sealed class RegexRule : IValidationRule
{
    private readonly Regex _regex;

    /// <summary>
    /// 创建正则表达式验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <param name="options">正则表达式选项</param>
    public RegexRule(string key, string pattern, string? errorMessage = null, RegexOptions options = RegexOptions.None)
    {
        Key = key;
        Pattern = pattern;
        ErrorMessage = errorMessage;
        _regex = new Regex(pattern, options | RegexOptions.Compiled);
    }

    /// <inheritdoc />
    public string Name => "Regex";

    /// <inheritdoc />
    public string Key { get; }

    /// <summary>
    /// 正则表达式模式
    /// </summary>
    public string Pattern { get; }

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

        if (!_regex.IsMatch(value))
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 的值 '{value}' 不匹配模式 '{Pattern}'";
            return new ValidationError(Key, message, Name, value);
        }

        return null;
    }
}
