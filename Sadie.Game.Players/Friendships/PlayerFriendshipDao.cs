using Sadie.Database;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipDao : BaseDao, IPlayerFriendshipDao
{
    private readonly PlayerFriendshipFactory _friendshipFactory;

    public PlayerFriendshipDao(IDatabaseProvider databaseProvider, PlayerFriendshipFactory friendshipFactory) : base(databaseProvider)
    {
        _friendshipFactory = friendshipFactory;
    }

    public async Task<int> GetFriendshipCountAsync(long playerId, PlayerFriendshipStatus status)
    {
        return await CountAsync("SELECT COUNT(*) FROM `player_friendships` WHERE (`origin_player_id` = @playerId OR `target_player_id` = @playerId) AND `status` = @statusId", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", (int)status }
        });
    }

    public async Task<bool> DoesFriendshipExistAsync(long originId, long targetId, PlayerFriendshipStatus status)
    {
        return await CountAsync(
            "SELECT COUNT(*) FROM `player_friendships` WHERE ((`origin_player_id` = @player1Id AND `target_player_id` = @player2Id) OR (`origin_player_id` = @player2Id AND `target_player_id` = @player1Id)) AND `status` = @statusId",
            new Dictionary<string, object>
            {
                {"player1Id", originId},
                {"player2Id", targetId},
                {"statusId", status}
            }) > 0;
    }

    public async Task CreateAsync(long originId, long targetId, PlayerFriendshipStatus status)
    {
        await QueryAsync("INSERT INTO `player_friendships` (`origin_player_id`, `target_player_id`, `status`) VALUES (@originId, @targetId, @status);", new Dictionary<string, object>()
        {
            { "originId", originId },
            { "targetId", targetId },
            { "status", 1 },
        });
    }

    public async Task UpdateAsync(long originId, long targetId, PlayerFriendshipStatus newStatus)
    {
        await QueryAsync("UPDATE `player_friendships` SET `status` = @newStatus WHERE (`origin_player_id` = @originId AND `target_player_id` = @targetId) OR (`origin_player_id` = @targetId AND `target_player_id` = @originId) LIMIT 1;", new Dictionary<string, object>()
        {
            {"newStatus", (int) newStatus},
            {"originId", (int) originId},
            {"targetId", (int) targetId},
        });
    }

    public async Task<List<PlayerFriendshipData>> GetFriendshipRecordsAsync(long playerId, PlayerFriendshipStatus status)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `players`.`id`, 
                   `players`.`username`, 
                   `player_avatar_data`.`figure_code`
            FROM `players` 
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `players`.`id` 
            WHERE (`players`.`id` IN (SELECT `origin_player_id` FROM `player_friendships` WHERE `target_player_id` = @playerId AND `status` = @statusId)) OR 
                  (`players`.`id` IN (SELECT `target_player_id` FROM `player_friendships` WHERE `origin_player_id` = @playerId AND `status` = @statusId));", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", status }
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

    public async Task<List<PlayerFriendshipData>> GetFriendshipRecordsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `players`.`id`, 
                   `players`.`username`, 
                   `player_avatar_data`.`figure_code`
            FROM `players` 
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `players`.`id` 
            WHERE (`players`.`id` IN (SELECT `origin_player_id` FROM `player_friendships` WHERE `target_player_id` = @playerId)) OR 
                  (`players`.`id` IN (SELECT `target_player_id` FROM `player_friendships` WHERE `origin_player_id` = @playerId));", new Dictionary<string, object>
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
            
            data.Add(_friendshipFactory.CreateFromRecord(record));
        }
        
        return data;
    }
}