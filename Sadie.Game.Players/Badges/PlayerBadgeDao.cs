using Sadie.Database;

namespace Sadie.Game.Players.Badges;

public class PlayerBadgeDao : BaseDao, IPlayerBadgeDao
{
    public PlayerBadgeDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
    }
    
    public async Task<List<PlayerBadge>> GetBadgesForPlayerAsync(long id)
    {
        var reader = await GetReaderAsync(@"SELECT 
            `badges`.`id`,
            `badges`.`code`,
            `player_badges`.`slot` FROM `player_badges` 
            INNER JOIN `badges` ON `player_badges`.`badge_id` = `badges`.`id` 
            WHERE `player_badges`.`player_id` = @profileId", new Dictionary<string, object>
        {
            { "profileId", id }
        });
        
        var data = new List<PlayerBadge>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerBadge(
                record.Get<int>("id"),
                record.Get<string>("code"),
                record.Get<int>("slot")));
        }
        
        return data;
    }
}