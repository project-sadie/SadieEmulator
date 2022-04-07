namespace Sadie.Game.Players.Badges;

public interface IPlayerBadgeRepository
{
    Task<List<PlayerBadge>> GetBadgesForPlayerAsync(int playerId);
}