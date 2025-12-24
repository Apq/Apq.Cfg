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
    private readonly object _tokenLock = new(); // 用于保护 token 操作
    private readonly object _dataLock = new(); // 用于保护 Data 操作
    private int _disposed; // 改为 int 以支持 Interlocked

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
        lock (_dataLock)
        {
            Data.Clear();
            foreach (var (key, value) in _coordinator.GetSnapshot())
            {
                if (value != null)
                    Data[key] = value;
            }
        }
    }

    private void OnCoordinatorChanges(IReadOnlyDictionary<string, ConfigChange> changes)
    {
        if (Volatile.Read(ref _disposed) != 0) return;

        // 应用变更到 Data
        lock (_dataLock)
        {
            foreach (var (key, change) in changes)
            {
                if (change.Type == ChangeType.Removed)
                    Data.Remove(key);
                else
                    Data[key] = change.NewValue!;
            }
        }

        // 触发 reload token
        TriggerReload();

        // 触发事件
        OnConfigChanged?.Invoke(new ConfigChangeEvent(changes));
    }

    private void TriggerReload()
    {
        CancellationTokenSource oldSource;
        lock (_tokenLock)
        {
            oldSource = _reloadTokenSource;
            _reloadTokenSource = new CancellationTokenSource();
        }
        oldSource.Cancel();
        oldSource.Dispose();
    }

    public new IChangeToken GetReloadToken()
    {
        lock (_tokenLock)
        {
            return new CancellationChangeToken(_reloadTokenSource.Token);
        }
    }

    public void Dispose()
    {
        // 使用 Interlocked 确保原子性
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        _coordinator.OnMergedChanges -= OnCoordinatorChanges;
        lock (_tokenLock)
        {
            _reloadTokenSource.Dispose();
        }
    }
}
