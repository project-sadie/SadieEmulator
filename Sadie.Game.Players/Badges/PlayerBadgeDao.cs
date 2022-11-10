using Sadie.Database;

namespace Sadie.Game.Players.Badges;

public class PlayerBadgeDao : BaseDao, IPlayerBadgeDao
{
    private readonly IPlayerBadgeFactory _badgeFactory;

    public PlayerBadgeDao(IDatabaseProvider databaseProvider, IPlayerBadgeFactory badgeFactory) : base(databaseProvider)
    {
        _badgeFactory = badgeFactory;
    }
    
    public async Task<List<PlayerBadge>> GetBadgesForPlayerAsync(int profileId)
    {
        var reader = await GetReaderAsync(@"SELECT 
            badges.id,
            badges.code,
            player_badges.slot FROM player_badges
            INNER JOIN badges ON player_badges.badge_id = badges.id
            WHERE player_badges.player_id = @profileId", new Dictionary<string, object>
        {
            { "profileId", profileId }
        });
        
        var data = new List<PlayerBadge>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(_badgeFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("code"),
                record.Get<int>("slot")));
        }
        
        return data;
    }
}