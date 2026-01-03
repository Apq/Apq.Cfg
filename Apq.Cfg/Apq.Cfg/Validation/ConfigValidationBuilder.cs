using System.Text.RegularExpressions;
using Apq.Cfg.Validation.Rules;

namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证构建器，提供流式 API 构建验证规则
/// </summary>
public sealed class ConfigValidationBuilder
{
    private readonly List<IValidationRule> _rules = new();

    /// <summary>
    /// 添加必填验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Required(string key, string? errorMessage = null)
    {
        _rules.Add(new RequiredRule(key, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加多个必填验证规则
    /// </summary>
    /// <param name="keys">配置键列表</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Required(params string[] keys)
    {
        foreach (var key in keys)
        {
            _rules.Add(new RequiredRule(key));
        }
        return this;
    }

    /// <summary>
    /// 添加整数范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Range(string key, int min, int max, string? errorMessage = null)
    {
        _rules.Add(RangeRule.ForInt(key, min, max, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加长整数范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Range(string key, long min, long max, string? errorMessage = null)
    {
        _rules.Add(RangeRule.ForLong(key, min, max, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加双精度浮点数范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Range(string key, double min, double max, string? errorMessage = null)
    {
        _rules.Add(RangeRule.ForDouble(key, min, max, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加十进制数范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Range(string key, decimal min, decimal max, string? errorMessage = null)
    {
        _rules.Add(RangeRule.ForDecimal(key, min, max, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加日期时间范围验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Range(string key, DateTime min, DateTime max, string? errorMessage = null)
    {
        _rules.Add(RangeRule.ForDateTime(key, min, max, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加正则表达式验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <param name="options">正则表达式选项</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Regex(string key, string pattern, string? errorMessage = null, RegexOptions options = RegexOptions.None)
    {
        _rules.Add(new RegexRule(key, pattern, errorMessage, options));
        return this;
    }

    /// <summary>
    /// 添加自定义验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="validator">验证函数，返回 true 表示验证通过</param>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="ruleName">规则名称</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Custom(string key, Func<string?, bool> validator, string errorMessage, string? ruleName = null)
    {
        _rules.Add(new CustomRule(key, validator, errorMessage, ruleName));
        return this;
    }

    /// <summary>
    /// 添加依赖验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="dependencyKey">依赖的配置键</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder DependsOn(string key, string dependencyKey, string? errorMessage = null)
    {
        _rules.Add(new DependsOnRule(key, dependencyKey, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加枚举值验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="allowedValues">允许的值列表</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder OneOf(string key, IEnumerable<string> allowedValues, bool ignoreCase = false, string? errorMessage = null)
    {
        _rules.Add(new EnumValuesRule(key, allowedValues, ignoreCase, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加枚举值验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="allowedValues">允许的值列表</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder OneOf(string key, params string[] allowedValues)
    {
        _rules.Add(new EnumValuesRule(key, allowedValues));
        return this;
    }

    /// <summary>
    /// 添加字符串长度验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="minLength">最小长度（null 表示不限制）</param>
    /// <param name="maxLength">最大长度（null 表示不限制）</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder Length(string key, int? minLength = null, int? maxLength = null, string? errorMessage = null)
    {
        _rules.Add(new LengthRule(key, minLength, maxLength, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加最小长度验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder MinLength(string key, int minLength, string? errorMessage = null)
    {
        _rules.Add(new LengthRule(key, minLength, null, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加最大长度验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="errorMessage">自定义错误消息</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder MaxLength(string key, int maxLength, string? errorMessage = null)
    {
        _rules.Add(new LengthRule(key, null, maxLength, errorMessage));
        return this;
    }

    /// <summary>
    /// 添加自定义验证规则
    /// </summary>
    /// <param name="rule">验证规则</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder AddRule(IValidationRule rule)
    {
        _rules.Add(rule);
        return this;
    }

    /// <summary>
    /// 添加多个自定义验证规则
    /// </summary>
    /// <param name="rules">验证规则列表</param>
    /// <returns>构建器实例，支持链式调用</returns>
    public ConfigValidationBuilder AddRules(IEnumerable<IValidationRule> rules)
    {
        _rules.AddRange(rules);
        return this;
    }

    /// <summary>
    /// 构建配置验证器
    /// </summary>
    /// <returns>配置验证器</returns>
    public IConfigValidator Build()
    {
        return new ConfigValidator(_rules);
    }

    /// <summary>
    /// 获取所有规则（内部使用）
    /// </summary>
    internal IReadOnlyList<IValidationRule> Rules => _rules;
}
