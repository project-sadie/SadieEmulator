using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Players.Badges;

namespace Sadie.Game.Players.DaosToDrop;

public class PlayerBadgeDao(IDatabaseProvider databaseProvider, IPlayerBadgeFactory badgeFactory)
    : BaseDao(databaseProvider), IPlayerBadgeDao
{
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
            
            data.Add(badgeFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("code"),
                record.Get<int>("slot")));
        }
        
        return data;
    }
}