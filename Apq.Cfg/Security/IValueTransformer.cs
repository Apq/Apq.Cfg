namespace Apq.Cfg.Security;

/// <summary>
/// 配置值转换器接口，用于加密/解密、脱敏等场景
/// </summary>
public interface IValueTransformer
{
    /// <summary>
    /// 转换器名称，用于标识
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 优先级，数值越大优先级越高
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 判断是否应该处理该键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>如果应该处理返回 true，否则返回 false</returns>
    bool ShouldTransform(string key, string? value);

    /// <summary>
    /// 读取时转换（如解密）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>转换后的值</returns>
    string? TransformOnRead(string key, string? value);

    /// <summary>
    /// 写入时转换（如加密）
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>转换后的值</returns>
    string? TransformOnWrite(string key, string? value);
}
