using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.DependencyInjection;

/// <summary>
/// 支持配置变更自动更新的 IOptionsMonitor 实现
/// </summary>
/// <typeparam name="TOptions">配置选项类型</typeparam>
public sealed class ApqCfgOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>, IDisposable
    where TOptions : class, new()
{
    private readonly ICfgRoot _cfgRoot;
    private readonly string _sectionKey;
    private readonly object _lock = new();
    private TOptions _currentValue;
    private readonly List<Action<TOptions, string>> _listeners = new();
    private readonly IDisposable? _subscription;
    private ConfigurationReloadToken _changeToken;
    private readonly IConfigurationRoot _dynamicConfig;

    /// <summary>
    /// 创建 ApqCfgOptionsMonitor 实例
    /// </summary>
    /// <param name="cfgRoot">配置根</param>
    /// <param name="sectionKey">配置节键名</param>
    public ApqCfgOptionsMonitor(ICfgRoot cfgRoot, string sectionKey)
    {
        _cfgRoot = cfgRoot ?? throw new ArgumentNullException(nameof(cfgRoot));
        _sectionKey = sectionKey ?? throw new ArgumentNullException(nameof(sectionKey));
        _changeToken = new ConfigurationReloadToken();

        // 启用动态重载以触发 ConfigChanges
        _dynamicConfig = _cfgRoot.ToMicrosoftConfiguration(new DynamicReloadOptions
        {
            EnableDynamicReload = true,
            DebounceMs = 100
        });

        // 初始绑定
        _currentValue = BindOptions();

        // 订阅配置变更
        _subscription = _cfgRoot.ConfigChanges.Subscribe(OnConfigChanged);
    }

    /// <inheritdoc />
    public TOptions CurrentValue
    {
        get
        {
            lock (_lock)
            {
                return _currentValue;
            }
        }
    }

    /// <inheritdoc />
    public TOptions Get(string? name)
    {
        // 对于命名选项，目前只支持默认名称
        return CurrentValue;
    }

    /// <inheritdoc />
    public IDisposable? OnChange(Action<TOptions, string?> listener)
    {
        var wrapper = new Action<TOptions, string>((options, name) => listener(options, name));

        lock (_lock)
        {
            _listeners.Add(wrapper);
        }

        return new ChangeTrackerDisposable(this, wrapper);
    }

    /// <summary>
    /// 获取变更令牌
    /// </summary>
    public IChangeToken GetChangeToken() => _changeToken;

    private void OnConfigChanged(Changes.ConfigChangeEvent e)
    {
        // 检查是否有相关的配置变更
        var hasRelevantChange = e.Changes.Keys.Any(key =>
            key.StartsWith(_sectionKey + ":", StringComparison.OrdinalIgnoreCase) ||
            key.Equals(_sectionKey, StringComparison.OrdinalIgnoreCase));

        if (!hasRelevantChange)
            return;

        TOptions newValue;
        List<Action<TOptions, string>> listenersCopy;
        ConfigurationReloadToken previousToken;

        lock (_lock)
        {
            newValue = BindOptions();
            _currentValue = newValue;
            listenersCopy = new List<Action<TOptions, string>>(_listeners);

            // 触发 IChangeToken
            previousToken = _changeToken;
            _changeToken = new ConfigurationReloadToken();
        }

        // 触发旧令牌的变更通知
        previousToken.OnReload();

        // 通知所有监听器
        foreach (var listener in listenersCopy)
        {
            try
            {
                listener(newValue, Options.DefaultName);
            }
            catch
            {
                // 忽略监听器异常
            }
        }
    }

    private TOptions BindOptions()
    {
        var options = new TOptions();
        var section = _cfgRoot.GetSection(_sectionKey);
        ObjectBinder.BindSection(section, options);
        return options;
    }

    private void RemoveListener(Action<TOptions, string> listener)
    {
        lock (_lock)
        {
            _listeners.Remove(listener);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _subscription?.Dispose();
        lock (_lock)
        {
            _listeners.Clear();
        }
    }

    private sealed class ChangeTrackerDisposable : IDisposable
    {
        private readonly ApqCfgOptionsMonitor<TOptions> _monitor;
        private readonly Action<TOptions, string> _listener;

        public ChangeTrackerDisposable(ApqCfgOptionsMonitor<TOptions> monitor, Action<TOptions, string> listener)
        {
            _monitor = monitor;
            _listener = listener;
        }

        public void Dispose()
        {
            _monitor.RemoveListener(_listener);
        }
    }
}

/// <summary>
/// 配置重载令牌
/// </summary>
internal sealed class ConfigurationReloadToken : IChangeToken
{
    private CancellationTokenSource _cts = new();

    public bool ActiveChangeCallbacks => true;

    public bool HasChanged => _cts.IsCancellationRequested;

    public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
    {
        return _cts.Token.Register(callback, state);
    }

    public void OnReload()
    {
        var previousCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        previousCts.Cancel();
        previousCts.Dispose();
    }
}
