using System.Collections.Concurrent;

namespace Sadie.Game.Rooms.Chat.Commands;

public class RoomChatCommandRepository(ConcurrentDictionary<string, IRoomChatCommand> commands) : IRoomChatCommandRepository
{
    public Tuple<bool, IRoomChatCommand?> TryGetCommandByTriggerWord(string trigger)
    {
        return commands.TryGetValue(trigger, out var command)
            ? new Tuple<bool, IRoomChatCommand?>(true, command)
            : new Tuple<bool, IRoomChatCommand?>(false, null);
    }
}