using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Players.Badges;

public class PlayerBadgeFactory(IServiceProvider serviceProvider) : IPlayerBadgeFactory
{
    public PlayerBadge Create(int id, string code, int slot)
    {
        return ActivatorUtilities.CreateInstance<PlayerBadge>(
            serviceProvider,
            id,
            code,
            slot);
    }
}