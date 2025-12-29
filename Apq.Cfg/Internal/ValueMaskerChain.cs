using Apq.Cfg.Security;

namespace Apq.Cfg.Internal;

/// <summary>
/// 值脱敏器链，按顺序执行脱敏器
/// </summary>
internal sealed class ValueMaskerChain
{
    private readonly IValueMasker[] _maskers;

    /// <summary>
    /// 初始化值脱敏器链
    /// </summary>
    /// <param name="maskers">脱敏器集合</param>
    public ValueMaskerChain(IEnumerable<IValueMasker> maskers)
    {
        _maskers = maskers.ToArray();
    }

    /// <summary>
    /// 脱敏处理
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>脱敏后的值</returns>
    public string Mask(string key, string? value)
    {
        if (value == null)
            return "[null]";

        foreach (var masker in _maskers)
        {
            if (masker.ShouldMask(key))
            {
                return masker.Mask(key, value);
            }
        }
        return value;
    }
}
