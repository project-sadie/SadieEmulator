using Sadie.Database;
using Sadie.Game.Rooms.Chat;

namespace Sadie.Game.Rooms;

public class RoomChatMessageDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider), IRoomChatMessageDao
{
    public async Task<int> CreateChatMessages(List<RoomChatMessage> messages)
    {
        var parameters = new Dictionary<string, object>();
        var query = "INSERT INTO room_chat_messages (room_id, player_id, message, chat_bubble_id, created_at) VALUES ";

        for (var i = 0; i < messages.Count; i++)
        {
            query += $"(@roomId{i}, @playerId{i}, @message{i}, @bubbleId{i}, @createdAt{i})";
            query += i + 1 >= messages.Count ? ";" : ",";
            
            parameters.Add($"roomId{i}", messages[i].Room.Id);
            parameters.Add($"playerId{i}", messages[i].Sender.Id);
            parameters.Add($"message{i}", messages[i].Message);
            parameters.Add($"bubbleId{i}", (int) messages[i].Bubble);
            parameters.Add($"createdAt{i}", messages[i].CreatedAt);
        }
        
        return await QueryAsync(query, parameters);
    }
}