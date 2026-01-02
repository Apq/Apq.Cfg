namespace Apq.Cfg.Validation.Rules;

/// <summary>
/// 依赖验证规则 - 当依赖键存在时，当前键也必须存在
/// </summary>
public sealed class DependsOnRule : IValidationRule
{
    private readonly string _dependencyKey;
    private readonly Func<ICfgRoot, string?>? _cfgAccessor;

    /// <summary>
    /// 创建依赖验证规则
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="dependencyKey">依赖的配置键</param>
    /// <param name="errorMessage">自定义错误消息</param>
    public DependsOnRule(string key, string dependencyKey, string? errorMessage = null)
    {
        Key = key;
        _dependencyKey = dependencyKey;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// 创建依赖验证规则（带配置访问器）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="dependencyKey">依赖的配置键</param>
    /// <param name="cfgAccessor">配置访问器，用于获取依赖键的值</param>
    /// <param name="errorMessage">自定义错误消息</param>
    internal DependsOnRule(string key, string dependencyKey, Func<ICfgRoot, string?> cfgAccessor, string? errorMessage = null)
    {
        Key = key;
        _dependencyKey = dependencyKey;
        _cfgAccessor = cfgAccessor;
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc />
    public string Name => "DependsOn";

    /// <inheritdoc />
    public string Key { get; }

    /// <summary>
    /// 依赖的配置键
    /// </summary>
    public string DependencyKey => _dependencyKey;

    /// <summary>
    /// 自定义错误消息
    /// </summary>
    public string? ErrorMessage { get; }

    /// <inheritdoc />
    public ValidationError? Validate(string? value)
    {
        // 基本验证：如果没有配置访问器，无法验证依赖关系
        // 实际的依赖验证在 ConfigValidator 中进行
        return null;
    }

    /// <summary>
    /// 验证依赖关系
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="value">当前键的值</param>
    /// <returns>验证错误，如果验证通过返回 null</returns>
    internal ValidationError? ValidateWithDependency(ICfgRoot cfg, string? value)
    {
        var dependencyValue = cfg[_dependencyKey];

        // 如果依赖键有值，当前键也必须有值
        if (!string.IsNullOrWhiteSpace(dependencyValue) && string.IsNullOrWhiteSpace(value))
        {
            var message = ErrorMessage ?? $"配置项 '{Key}' 依赖于 '{_dependencyKey}'，当 '{_dependencyKey}' 存在时，'{Key}' 也必须存在";
            return new ValidationError(Key, message, Name, value);
        }

        return null;
    }
}
