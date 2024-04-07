using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Players.Navigator;

namespace Sadie.Game.Players.DaosToDrop;

public class PlayerGenericDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider)
{
    public async Task<List<PlayerSavedSearch>> GetSavedSearchesAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                player_id,
                search,
                filter 
            FROM player_saved_searches 
            WHERE player_id = @playerId", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerSavedSearch>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerSavedSearch(
                record.Get<long>("playerId"),
                record.Get<string>("search"),
                record.Get<string>("filter")));
        }
        
        return data;
    }

    public async Task<List<string>> GetPermissionsAsync(long roleId)
    {
        var reader = await GetReaderAsync(@"
            SELECT name 
            FROM player_permissions 
            WHERE id IN (SELECT permission_id FROM player_role_permissions WHERE player_role_permissions.role_id = @roleId)", 
            new Dictionary<string, object>
            {
                { "roleId", roleId }
            });
        
        var data = new List<string>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(record.Get<string>("name"));
        }

        return data;
    }

    public async Task<List<long>> GetLikedRoomsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT room_id FROM player_room_likes WHERE player_id = @playerId", 
            new Dictionary<string, object>
            {
                { "playerId", playerId }
            });
        
        var data = new List<long>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(record.Get<long>("id"));
        }

        return data;
    }

    public async Task CreatePlayerRoomLikeAsync(long playerId, long roomId)
    {
        await QueryAsync("INSERT INTO player_room_likes (player_id, room_id) VALUES (@playerId, @roomId);", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "roomId", roomId }
        });
    }

}