using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Internal;

/// <summary>
/// 支持增量更新的合并配置提供者
/// </summary>
internal sealed class MergedConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly ChangeCoordinator _coordinator;
    private CancellationTokenSource _reloadTokenSource;
    private volatile bool _disposed;

    /// <summary>
    /// 当配置变更时触发
    /// </summary>
    public event Action<ConfigChangeEvent>? OnConfigChanged;

    public MergedConfigurationProvider(ChangeCoordinator coordinator)
    {
        _coordinator = coordinator;
        _reloadTokenSource = new CancellationTokenSource();

        // 订阅协调器的变更事件
        _coordinator.OnMergedChanges += OnCoordinatorChanges;
    }

    public override void Load()
    {
        // 从协调器获取快照数据
        Data.Clear();
        foreach (var (key, value) in _coordinator.GetSnapshot())
        {
            if (value != null)
                Data[key] = value;
        }
    }

    private void OnCoordinatorChanges(IReadOnlyDictionary<string, ConfigChange> changes)
    {
        if (_disposed) return;

        // 应用变更到 Data
        foreach (var (key, change) in changes)
        {
            if (change.Type == ChangeType.Removed)
                Data.Remove(key);
            else
                Data[key] = change.NewValue!;
        }

        // 触发 reload token
        TriggerReload();

        // 触发事件
        OnConfigChanged?.Invoke(new ConfigChangeEvent
        {
            Changes = changes,
            Timestamp = DateTimeOffset.Now
        });
    }

    private void TriggerReload()
    {
        var oldSource = Interlocked.Exchange(
            ref _reloadTokenSource,
            new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
    }

    public new IChangeToken GetReloadToken()
    {
        return new CancellationChangeToken(_reloadTokenSource.Token);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _coordinator.OnMergedChanges -= OnCoordinatorChanges;
        _reloadTokenSource.Dispose();
    }
}
