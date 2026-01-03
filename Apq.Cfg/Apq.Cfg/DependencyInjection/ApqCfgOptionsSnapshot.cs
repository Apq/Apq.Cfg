using Microsoft.Extensions.Options;

namespace Apq.Cfg.DependencyInjection;

/// <summary>
/// 支持每次请求重新绑定的 IOptionsSnapshot 实现
/// </summary>
/// <typeparam name="TOptions">配置选项类型</typeparam>
public sealed class ApqCfgOptionsSnapshot<TOptions> : IOptionsSnapshot<TOptions>
    where TOptions : class, new()
{
    private readonly ICfgRoot _cfgRoot;
    private readonly string _sectionKey;
    private TOptions? _value;

    /// <summary>
    /// 创建 ApqCfgOptionsSnapshot 实例
    /// </summary>
    /// <param name="cfgRoot">配置根</param>
    /// <param name="sectionKey">配置节键名</param>
    public ApqCfgOptionsSnapshot(ICfgRoot cfgRoot, string sectionKey)
    {
        _cfgRoot = cfgRoot ?? throw new ArgumentNullException(nameof(cfgRoot));
        _sectionKey = sectionKey ?? throw new ArgumentNullException(nameof(sectionKey));
    }

    /// <inheritdoc />
    public TOptions Value => _value ??= BindOptions();

    /// <inheritdoc />
    public TOptions Get(string? name)
    {
        // 对于命名选项，目前只支持默认名称
        return Value;
    }

    private TOptions BindOptions()
    {
        var options = new TOptions();
        var section = _cfgRoot.GetSection(_sectionKey);
        ObjectBinder.BindSection(section, options);
        return options;
    }
}
