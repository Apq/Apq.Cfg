namespace Apq.Cfg.Security;

/// <summary>
/// 配置值脱敏器接口，用于日志输出等场景
/// </summary>
public interface IValueMasker
{
    /// <summary>
    /// 判断是否应该脱敏该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>如果应该脱敏返回 true，否则返回 false</returns>
    bool ShouldMask(string key);

    /// <summary>
    /// 脱敏处理
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>脱敏后的值</returns>
    string Mask(string key, string? value);
}
