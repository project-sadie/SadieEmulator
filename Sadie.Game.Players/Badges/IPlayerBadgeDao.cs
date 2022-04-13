namespace Sadie.Game.Players.Badges;

public interface IPlayerBadgeDao
{
    Task<List<PlayerBadge>> GetBadgesForPlayerAsync(int profileId);
}