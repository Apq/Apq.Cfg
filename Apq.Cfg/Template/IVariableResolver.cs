namespace Apq.Cfg.Template;

/// <summary>
/// 变量解析器接口
/// </summary>
/// <remarks>
/// 用于解析配置模板中的变量引用，如 ${App:Name} 或 ${ENV:PATH}
/// </remarks>
public interface IVariableResolver
{
    /// <summary>
    /// 解析器前缀（如 "ENV"、"SYS"），为 null 表示默认解析器
    /// </summary>
    string? Prefix { get; }

    /// <summary>
    /// 解析变量
    /// </summary>
    /// <param name="variableName">变量名（不含前缀）</param>
    /// <param name="cfg">配置根，用于引用其他配置</param>
    /// <returns>解析后的值，null 表示无法解析</returns>
    string? Resolve(string variableName, ICfgRoot cfg);
}
