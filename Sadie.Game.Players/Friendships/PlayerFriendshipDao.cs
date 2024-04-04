using Sadie.Database.LegacyAdoNet;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipDao(IDatabaseProvider databaseProvider, PlayerFriendshipFactory friendshipFactory)
    : BaseDao(databaseProvider), IPlayerFriendshipDao
{
    private static string GetDefaultSelectClause()
    {
        return @"
            player_friendships.id AS request_id,
            player_friendships.origin_player_id,
            player_friendships.target_player_id,
            player_friendships.status,
            (SELECT username FROM players WHERE id = player_avatar_data.player_id) AS username,
            player_avatar_data.player_id AS target_id,
            player_avatar_data.figure_code,
            player_avatar_data.motto,
            player_avatar_data.gender
        ";
    }

    private PlayerFriendship CreateFriendshipFromRecord(DatabaseRecord record)
    {
        var targetData = friendshipFactory.CreateFriendshipData(
            record.Get<int>("target_id"),
            record.Get<string>("username"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female);
        
        return friendshipFactory.CreateFriendship(
            record.Get<int>("request_id"),
            record.Get<int>("origin_player_id"),
            record.Get<int>("target_player_id"),
            (PlayerFriendshipStatus)record.Get<int>("status"),
            targetData);
    }
    
    public async Task<List<PlayerFriendship>> GetIncomingFriendRequestsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT " + GetDefaultSelectClause() + @"
            FROM player_friendships
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = origin_player_id
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
            
            data.Add(CreateFriendshipFromRecord(record));
        }
        
        return data;
    }

    public async Task<List<PlayerFriendship>> GetOutgoingFriendRequestsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT " + GetDefaultSelectClause() + @"
            FROM player_friendships
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = target_player_id
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

            data.Add(CreateFriendshipFromRecord(record));
        }
        
        return data;
    }

    public async Task<List<PlayerFriendship>> GetFriendsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT " + GetDefaultSelectClause() + @"
            FROM player_friendships
                INNER JOIN player_avatar_data ON 
                    player_avatar_data.player_id = target_player_id = 
                        if(@playerId = target_player_id, origin_player_id, target_player_id)
            WHERE (
                origin_player_id = @playerId OR 
                target_player_id = @playerId
            ) AND status = 2;", new Dictionary<string, object>
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
            
            data.Add(CreateFriendshipFromRecord(record));
        }
        
        return data;
    }

    public async Task AcceptFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync(@"
            UPDATE player_friendships 
            SET status = @newStatus WHERE 
                origin_player_id = @originId AND target_player_id = @targetId 
            LIMIT 1;", new Dictionary<string, object>
        {
            {"newStatus", (int) PlayerFriendshipStatus.Accepted},
            {"originId", originId},
            {"targetId", targetId}
        });
    }

    public async Task AcceptFriendRequestAsync(int requestId)
    {
        await QueryAsync("UPDATE player_friendships SET status = @newStatus WHERE id = @requestId LIMIT 1;", new Dictionary<string, object>()
        {
            {"newStatus", (int) PlayerFriendshipStatus.Accepted},
            {"requestId", requestId}
        });
    }

    public async Task DeclineFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync(@"
            DELETE FROM player_friendships 
            WHERE (
                origin_player_id = @originId AND target_player_id = @targetId
            ) 
            AND status = 1 LIMIT 1;", new Dictionary<string, object>
        {
            {"originId", originId},
            {"targetId", targetId}
        });
    }

    public async Task DeclineAllFriendRequestsAsync(int targetId)
    {
        await QueryAsync(@"
            DELETE FROM player_friendships 
            WHERE target_player_id = @targetId AND status = 1", new Dictionary<string, object>
        {
            {"targetId", targetId}
        });
    }

    public async Task CreateFriendRequestAsync(int originId, int targetId)
    {
        await QueryAsync(@"
            INSERT INTO player_friendships (
                origin_player_id, 
                target_player_id, 
                status, 
                created_at
            ) 
            VALUES (@originId, @targetId, @status, @createdAt);", new Dictionary<string, object>
        {
            { "originId", originId },
            { "targetId", targetId },
            { "status", (int) PlayerFriendshipStatus.Pending },
            { "createdAt", DateTime.Now }
        });
    }

    public async Task<bool> DoesFriendRequestExist(int playerId1, int playerId2)
    {
        return await Exists(@"
            SELECT NULL 
            FROM player_friendships 
            WHERE (
                (origin_player_id = @playerId1 AND target_player_id = @playerId2) OR 
                (origin_player_id = @playerId2 AND target_player_id = @playerId1)
            ) AND status = 1;", new Dictionary<string, object>()
        {
            {"playerId1", playerId1},
            {"playerId2", playerId2}
        });
    }

    public async Task<bool> DoesFriendshipExist(int playerId1, int playerId2)
    {
        return await Exists(@"
            SELECT NULL 
            FROM player_friendships 
            WHERE (
                (origin_player_id = @playerId1 AND target_player_id = @playerId2) OR 
                (origin_player_id = @playerId2 AND target_player_id = @playerId1)
            ) AND status = 2;", new Dictionary<string, object>()
        {
            {"playerId1", playerId1},
            {"playerId2", playerId2}
        });
    }

    public async Task<List<PlayerFriendship>> GetAllRecordsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT " + GetDefaultSelectClause() + @"
            FROM player_friendships
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = if(@playerId = target_player_id, origin_player_id, target_player_id)
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
            
            data.Add(CreateFriendshipFromRecord(record));
        }
        
        return data;
    }

    public async Task DeleteFriendshipAsync(int playerId1, int playerId2)
    {
        await QueryAsync(@"
            DELETE FROM player_friendships 
            WHERE (
                (origin_player_id = @playerId1 AND target_player_id = @playerId2) OR 
                (origin_player_id = @playerId2 AND target_player_id = @playerId1)
            ) AND status = 2 LIMIT 1;",
            new Dictionary<string, object>
            {
                {"playerId1", playerId1},
                {"playerId2", playerId2}
            });
    }
}