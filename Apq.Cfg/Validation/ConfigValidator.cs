using Apq.Cfg.Validation.Rules;

namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证器实现
/// </summary>
public sealed class ConfigValidator : IConfigValidator
{
    private readonly List<IValidationRule> _rules;

    /// <summary>
    /// 创建配置验证器
    /// </summary>
    /// <param name="rules">验证规则列表</param>
    public ConfigValidator(IEnumerable<IValidationRule> rules)
    {
        _rules = rules.ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<IValidationRule> Rules => _rules;

    /// <inheritdoc />
    public ValidationResult Validate(ICfgRoot cfg)
    {
        var result = new ValidationResult();

        foreach (var rule in _rules)
        {
            var value = cfg[rule.Key];

            // 特殊处理依赖规则
            if (rule is DependsOnRule dependsOnRule)
            {
                var error = dependsOnRule.ValidateWithDependency(cfg, value);
                if (error != null)
                {
                    result.AddError(error);
                }
            }
            else
            {
                var error = rule.Validate(value);
                if (error != null)
                {
                    result.AddError(error);
                }
            }
        }

        return result;
    }
}
