using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands;

public class RoomChatCommandParameterReader(Queue<string> parameters) : IRoomChatCommandParameterReader
{
    public bool GetWord(out string? value)
    {
        return parameters.TryDequeue(out value);
    }
    
    public bool GetSentence(out string? value)
    {
        if (parameters.Count == 0)
        {
            value = null;
            return false;
        }

        var parts = new List<string>();

        while (parameters.TryDequeue(out var part))
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                parts.Add(part);
            }
        }

        value = string.Join(" ", parts);
        return true;
    }

    public bool GetInt(out int value)
    {
        value = 0;
        
        return parameters.TryDequeue(out var s) && 
               int.TryParse(s, out value);
    }

    public bool TryGetUser(
        IRoomUserRepository userRepository, 
        out int userId, 
        out IRoomUser? user)
    {
        userId = 0;
        user = null;
        
        return parameters.TryDequeue(out var s) && 
               int.TryParse(s, out userId) &&
               userRepository.TryGetById(userId, out user);
    }
}