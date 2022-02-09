using Sadie.Database;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipDao : BaseDao, IPlayerFriendshipDao
{
    public PlayerFriendshipDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
    }
    
    public async Task<List<PlayerFriendshipData>> GetFriendshipRequestsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `players`.`id`, 
                   `players`.`username`, 
                   `player_data`.`figure_code`
            FROM `players` 
                INNER JOIN `player_data` ON `player_data`.`profile_id` = `players`.`id` 
            WHERE `players`.`id` IN (SELECT `sender_id` FROM `player_friendships` WHERE `target_id` = @playerId AND `status` = 1);", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendshipData>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(PlayerFriendshipFactory.CreateFromRecord(record));
        }
        
        return data;
    }
}