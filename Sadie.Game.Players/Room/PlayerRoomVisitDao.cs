using Sadie.Database.LegacyAdoNet;

namespace Sadie.Game.Players.Room;

public class PlayerRoomVisitDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider), IPlayerRoomVisitDao
{
    public async Task<int> CreateAsync(List<PlayerRoomVisit> roomVisits)
    {
        var parameters = new Dictionary<string, object>();
        var query = "INSERT INTO player_room_visits (player_id, room_id, created_at) VALUES ";

        for (var i = 0; i < roomVisits.Count; i++)
        {
            query += $"(@playerId{i}, @roomId{i}, @createdAt{i})";
            query += i + 1 >= roomVisits.Count ? ";" : ",";
            
            parameters.Add($"playerId{i}", roomVisits[i].PlayerId);
            parameters.Add($"roomId{i}", roomVisits[i].RoomId);
            parameters.Add($"createdAt{i}", roomVisits[i].CreatedAt);
        }
        
        return await QueryAsync(query, parameters);
    }
}