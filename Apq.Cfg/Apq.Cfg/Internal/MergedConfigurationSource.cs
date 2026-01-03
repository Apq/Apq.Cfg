using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Internal;

/// <summary>
/// 合并配置源
/// </summary>
internal sealed class MergedConfigurationSource : IConfigurationSource
{
    private readonly ChangeCoordinator _coordinator;

    public MergedConfigurationSource(ChangeCoordinator coordinator)
    {
        _coordinator = coordinator;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MergedConfigurationProvider(_coordinator);
    }
}
