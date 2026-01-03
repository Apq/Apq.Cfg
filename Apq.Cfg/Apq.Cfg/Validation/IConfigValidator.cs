namespace Apq.Cfg.Validation;

/// <summary>
/// 配置验证器接口
/// </summary>
public interface IConfigValidator
{
    /// <summary>
    /// 验证配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <returns>验证结果</returns>
    ValidationResult Validate(ICfgRoot cfg);

    /// <summary>
    /// 获取所有验证规则
    /// </summary>
    IReadOnlyList<IValidationRule> Rules { get; }
}
