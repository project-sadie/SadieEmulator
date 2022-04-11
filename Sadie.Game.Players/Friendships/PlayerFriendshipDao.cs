using Sadie.Database;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipDao : BaseDao, IPlayerFriendshipDao
{
    private readonly PlayerFriendshipFactory _friendshipFactory;

    public PlayerFriendshipDao(IDatabaseProvider databaseProvider, PlayerFriendshipFactory friendshipFactory) : base(databaseProvider)
    {
        _friendshipFactory = friendshipFactory;
    }

    /*public async Task<int> GetFriendshipCountAsync(int playerId, PlayerFriendshipStatus status)
    {
        return await CountAsync("SELECT COUNT(*) FROM `player_friendships` WHERE (`origin_player_id` = @playerId OR `target_player_id` = @playerId) AND `status` = @statusId", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", (int)status }
        });
    }

    public async Task<bool> DoesFriendshipExistAsync(int originId, int targetId, PlayerFriendshipStatus status)
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

    public async Task CreateAsync(int originId, int targetId, PlayerFriendshipStatus status)
    {
        await QueryAsync("INSERT INTO `player_friendships` (`origin_player_id`, `target_player_id`, `status`, `type_id`) VALUES (@originId, @targetId, @status, 0);", new Dictionary<string, object>
        {
            { "originId", originId },
            { "targetId", targetId },
            { "status", 1 },
        });
    }

    public async Task UpdateAsync(int originId, int targetId, PlayerFriendshipStatus newStatus)
    {
        await QueryAsync("UPDATE `player_friendships` SET `status` = @newStatus WHERE (`origin_player_id` = @originId AND `target_player_id` = @targetId) OR (`origin_player_id` = @targetId AND `target_player_id` = @originId) LIMIT 1;", new Dictionary<string, object>()
        {
            {"newStatus", (int) newStatus},
            {"originId", (int) originId},
            {"targetId", (int) targetId},
        });
    }

    public async Task UpdateAsync(int id, PlayerFriendshipStatus newStatus)
    {
        await QueryAsync("UPDATE `player_friendships` SET `status` = @newStatus WHERE `id` = @id LIMIT 1;", new Dictionary<string, object>
        {
            {"id", id},
        });
    }

    public async Task DeleteAsync(int originId, int targetId)
    {
        await QueryAsync("DELETE FROM `player_friendships` WHERE (`origin_player_id` = @originId AND `target_player_id` = @targetId) LIMIT 1;", new Dictionary<string, object>
        {
            {"originId", originId},
            {"targetId", targetId},
        });
    }

    public async Task DeleteAllAsync(int targetId)
    {
        await QueryAsync("DELETE FROM `player_friendships` WHERE (`target_player_id` = @targetId)", new Dictionary<string, object>
        {
            {"targetId", targetId},
        });
    }

    public async Task<List<PlayerFriendship>> GetFriendshipRecordsAsync(int playerId, PlayerFriendshipStatus status) // @
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`figure_code`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `target_player_id`
            WHERE (origin_player_id = @playerId OR target_player_id = @playerId) AND `status` = @status", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "statusId", status }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code")));
        }
        
        return data;
    }

    public async Task<List<PlayerFriendship>> GetFriendshipRecordsAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`figure_code`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `target_player_id`
            WHERE origin_player_id = @playerId OR target_player_id = @playerId", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code")));
        }
        
        return data;
    }*/
    public async Task<List<PlayerFriendship>> GetIncomingFriendRequestsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`player_id` AS `target_id`,
                `player_avatar_data`.`figure_code`,
                `player_avatar_data`.`motto`,
                `player_avatar_data`.`gender`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `origin_player_id`
            WHERE target_player_id = @playerId AND status = 1;", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            var targetData = _friendshipFactory.CreateData(
                record.Get<int>("target_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code"),
                record.Get<string>("motto"),
                record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female);
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                targetData));
        }
        
        return data;
    }

    public async Task<List<PlayerFriendship>> GetOutgoingFriendRequestsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`player_id` AS `target_id`,
                `player_avatar_data`.`figure_code`,
                `player_avatar_data`.`motto`,
                `player_avatar_data`.`gender`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `target_player_id`
            WHERE origin_player_id = @playerId AND status = 1;", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var targetData = _friendshipFactory.CreateData(
                record.Get<int>("target_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code"),
                record.Get<string>("motto"),
                record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female);
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                targetData));
        }
        
        return data;
    }

    public async Task<List<PlayerFriendship>> GetFriendsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`player_id` AS `target_id`,
                `player_avatar_data`.`figure_code`,
                `player_avatar_data`.`motto`,
                `player_avatar_data`.`gender`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `target_player_id` = if(@playerId = target_player_id, origin_player_id, target_player_id)
            WHERE (origin_player_id = @playerId OR target_player_id = @playerId) AND status = 2;", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            var targetData = _friendshipFactory.CreateData(
                record.Get<int>("target_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code"),
                record.Get<string>("motto"),
                record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female);
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                targetData));
        }
        
        return data;
    }

    public async Task AcceptFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync("UPDATE `player_friendships` SET `status` = @newStatus WHERE `origin_player_id` = @originId AND `target_player_id` = @targetId LIMIT 1;", new Dictionary<string, object>()
        {
            {"newStatus", (int) PlayerFriendshipStatus.Accepted},
            {"originId", originId},
            {"targetId", targetId},
        });
    }

    public async Task AcceptFriendRequestAsync(int requestId)
    {
        await QueryAsync("UPDATE `player_friendships` SET `status` = @newStatus WHERE `id` = @requestId LIMIT 1;", new Dictionary<string, object>()
        {
            {"newStatus", (int) PlayerFriendshipStatus.Accepted},
            {"requestId", requestId},
        });
    }

    public async Task DeclineFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync("DELETE FROM `player_friendships` WHERE (`origin_player_id` = @originId AND `target_player_id` = @targetId) AND `status` = 1 LIMIT 1;", new Dictionary<string, object>
        {
            {"originId", originId},
            {"targetId", targetId},
        });
    }

    public async Task DeclineAllFriendRequestsAsync(int targetId)
    {
        await QueryAsync("DELETE FROM `player_friendships` WHERE `target_player_id` = @targetId AND `status` = 1", new Dictionary<string, object>
        {
            {"targetId", targetId},
        });
    }

    public async Task CreateFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync("INSERT INTO `player_friendships` (`origin_player_id`, `target_player_id`, `status`, `type_id`) VALUES (@originId, @targetId, @status, 0);", new Dictionary<string, object>
        {
            { "originId", originId },
            { "targetId", targetId },
            { "status", (int) PlayerFriendshipStatus.Pending },
        });
    }

    public async Task<bool> DoesRequestExist(int playerId1, int playerId2)
    {
        return await Exists("SELECT NULL FROM `player_friendships` WHERE ((`origin_player_id` = @playerId1 AND `target_player_id` = @playerId2) OR (`origin_player_id` = @playerId2 AND `target_player_id` = @playerId1)) AND status = 1;", new Dictionary<string, object>()
        {
            {"playerId1", playerId1},
            {"playerId2", playerId2},
        });
    }

    public async Task<bool> DoesFriendshipExist(int playerId1, int playerId2)
    {
        return await Exists("SELECT NULL FROM `player_friendships` WHERE ((`origin_player_id` = @playerId1 AND `target_player_id` = @playerId2) OR (`origin_player_id` = @playerId2 AND `target_player_id` = @playerId1)) AND status = 2;", new Dictionary<string, object>()
        {
            {"playerId1", playerId1},
            {"playerId2", playerId2},
        });
    }

    public async Task<List<PlayerFriendship>> GetAllRecordsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `player_friendships`.`id` AS `request_id`,
                `player_friendships`.`origin_player_id`,
                `player_friendships`.`target_player_id`,
                `player_friendships`.`status`,
                `player_friendships`.`type_id`,
                (SELECT `username` FROM `players` WHERE `id` = `player_avatar_data`.`player_id`) AS `username`,
                `player_avatar_data`.`player_id` AS `target_id`,
                `player_avatar_data`.`figure_code`,
                `player_avatar_data`.`motto`,
                `player_avatar_data`.`gender`

            FROM player_friendships
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = if(@playerId = target_player_id, origin_player_id, target_player_id)
            WHERE (origin_player_id = @playerId OR target_player_id = @playerId);", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerFriendship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            var targetData = _friendshipFactory.CreateData(
                record.Get<int>("target_id"),
                record.Get<string>("username"),
                record.Get<string>("figure_code"),
                record.Get<string>("motto"),
                record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female);
            
            data.Add(_friendshipFactory.CreateFromRecord(
                record.Get<int>("request_id"),
                record.Get<int>("origin_player_id"),
                record.Get<int>("target_player_id"),
                (PlayerFriendshipStatus) record.Get<int>("status"),
                (PlayerFriendshipType) record.Get<int>("request_id"),
                targetData));
        }
        
        return data;
    }

    public async Task DeleteFriendshipAsync(int playerId1, int playerId2)
    {
        await QueryAsync("DELETE FROM `player_friendships` WHERE ((`origin_player_id` = @playerId1 AND `target_player_id` = @playerId2) OR (`origin_player_id` = @playerId2 AND `target_player_id` = @playerId1)) AND `status` = 2 LIMIT 1;", new Dictionary<string, object>
        {
            {"playerId1", playerId1},
            {"playerId2", playerId2},
        });
    }
}