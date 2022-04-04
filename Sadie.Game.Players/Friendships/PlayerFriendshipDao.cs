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
        return await GetFriendshipRecordByStatus(playerId, PlayerFriendshipStatus.Pending);
    }

    public async Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId)
    {
        return await GetFriendshipRecordByStatus(playerId, PlayerFriendshipStatus.Accepted);
    }

    public async Task<int> GetActiveFriendsCountAsync(long playerId)
    {
        return await CountAsync("SELECT COUNT(*) FROM `player_friendships` WHERE (`origin_player_id` = @playerId OR `target_player_id` = @playerId) AND `status` = @statusId", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", 2 }
        });
    }

    public async Task<bool> DoesFriendshipExist(long player1Id, long player2Id, PlayerFriendshipStatus status)
    {
        return await CountAsync(
            "SELECT COUNT(*) FROM `player_friendships` WHERE ((`origin_player_id` = @player1Id AND `target_player_id` = @player2Id) OR (`origin_player_id` = @player2Id AND `target_player_id` = @player1Id)) AND `status` = @statusId",
            new Dictionary<string, object>
            {
                {"player1Id", player1Id},
                {"player2Id", player2Id},
                {"statusId", status}
            }) > 0;
    }

    private async Task<List<PlayerFriendshipData>> GetFriendshipRecordByStatus(long playerId, PlayerFriendshipStatus statusId)
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