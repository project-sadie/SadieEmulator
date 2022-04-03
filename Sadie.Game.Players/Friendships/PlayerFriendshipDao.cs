using Sadie.Database;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipDao : BaseDao, IPlayerFriendshipDao
{
    private readonly PlayerFriendshipFactory _friendshipFactory;

    public PlayerFriendshipDao(IDatabaseProvider databaseProvider, PlayerFriendshipFactory friendshipFactory) : base(databaseProvider)
    {
        _friendshipFactory = friendshipFactory;
    }
    
    public async Task<List<PlayerFriendshipData>> GetPendingFriendsAsync(long playerId)
    {
        return await GetFriendshipRecordByStatus(playerId, 1);
    }

    public async Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId)
    {
        return await GetFriendshipRecordByStatus(playerId, 2);
    }

    private async Task<List<PlayerFriendshipData>> GetFriendshipRecordByStatus(long playerId, int statusId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `players`.`id`, 
                   `players`.`username`, 
                   `player_avatar_data`.`figure_code`
            FROM `players` 
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `players`.`id` 
            WHERE `players`.`id` IN (SELECT `origin_player_id` FROM `player_friendships` WHERE `target_player_id` = @playerId AND `status` = @statusId);", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", statusId }
        });
        
        var data = new List<PlayerFriendshipData>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(_friendshipFactory.CreateFromRecord(record));
        }
        
        return data;
    }
}