namespace Sadie.Game.Players.Badges;

public class PlayerBadgeRepository(IPlayerBadgeDao badgeDao) : IPlayerBadgeRepository
{
    public async Task<List<PlayerBadge>> GetBadgesForPlayerAsync(int playerId)
    {
        return await badgeDao.GetBadgesForPlayerAsync(playerId);
    }
    
}