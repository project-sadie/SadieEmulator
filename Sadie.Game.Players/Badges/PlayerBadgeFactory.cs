using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Players.Badges;

public class PlayerBadgeFactory : IPlayerBadgeFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerBadgeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public PlayerBadge Create(int id, string code, int slot)
    {
        return ActivatorUtilities.CreateInstance<PlayerBadge>(
            _serviceProvider,
            id,
            code,
            slot);
    }
}