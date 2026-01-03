namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证规则接口
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// 规则名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 要验证的配置键
    /// </summary>
    string Key { get; }

    /// <summary>
    /// 验证配置值
    /// </summary>
    /// <param name="value">配置值</param>
    /// <returns>验证错误，如果验证通过返回 null</returns>
    ValidationError? Validate(string? value);
}
