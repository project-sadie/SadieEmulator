namespace Sadie.Game.Players.Badges;

public class PlayerBadgeRepository : IPlayerBadgeRepository
{
    private readonly IPlayerBadgeDao _badgeDao;

    public PlayerBadgeRepository(IPlayerBadgeDao badgeDao)
    {
        _badgeDao = badgeDao;
    }
    
    public async Task<List<PlayerBadge>> GetBadgesForPlayerAsync(int playerId)
    {
        return await _badgeDao.GetBadgesForPlayerAsync(playerId);
    }
    
}