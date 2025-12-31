namespace Apq.Cfg.Env;

/// <summary>
/// CfgBuilder 的 .env 文件扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 .env 文件配置源
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="path">.env 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">文件不存在时是否忽略</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为默认写入目标</param>
    /// <param name="setEnvironmentVariables">是否将配置写入系统环境变量（默认为 false）</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddEnv(this CfgBuilder builder, string path, int level, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false,
        bool setEnvironmentVariables = false)
    {
        return builder.AddSource(new EnvFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, setEnvironmentVariables));
    }
}
