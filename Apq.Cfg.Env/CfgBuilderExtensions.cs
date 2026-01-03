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
    /// <param name="level">配置层级，数值越大优先级越高，默认为 <see cref="CfgSourceLevels.Env"/> (5)</param>
    /// <param name="writeable">是否可写，默认为false</param>
    /// <param name="optional">文件不存在时是否忽略，默认为true</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载，默认为true</param>
    /// <param name="isPrimaryWriter">是否为默认写入目标，默认为false</param>
    /// <param name="setEnvironmentVariables">是否将配置写入系统环境变量，默认为false</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddEnv(this CfgBuilder builder, string path, int level = CfgSourceLevels.Env, bool writeable = false,
        bool optional = true, bool reloadOnChange = true, bool isPrimaryWriter = false,
        bool setEnvironmentVariables = false)
    {
        return builder.AddSource(new EnvFileCfgSource(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, setEnvironmentVariables));
    }
}
