using Sadie.Database;

namespace Sadie.Game.Rooms;

public class RoomRightsDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider), IRoomRightsDao
{
    public async Task InsertRightsAsync(long roomId, long playerId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roomId", roomId },
            { "playerId", playerId },
            { "created", DateTime.Now }
        };
        
        await QueryAsync("INSERT INTO room_player_rights (room_id, player_id, created_at) VALUES (@roomId, @playerId, @created)", parameters);
    }

    public async Task DeleteRightsAsync(long roomId, long playerId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roomId", roomId },
            { "playerId", playerId }
        };
        
        await QueryAsync("DELETE FROM room_player_rights WHERE room_id = @roomId AND player_id = @playerId LIMIT 1;", parameters);
    }
}