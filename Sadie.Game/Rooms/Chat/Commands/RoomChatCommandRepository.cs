using Sadie.API.Game.Rooms.Chat.Commands;

namespace Sadie.Game.Rooms.Chat.Commands;

public class RoomChatCommandRepository : IRoomChatCommandRepository
{
    private readonly Dictionary<string, IRoomChatCommand> _commands;

    public RoomChatCommandRepository(IEnumerable<IRoomChatCommand> commands)
    {
        _commands = commands.ToDictionary(x => x.Trigger, c => c);
    }

    public IRoomChatCommand? TryGetCommandByTriggerWord(string trigger)
    {
        return _commands.GetValueOrDefault(trigger);
    }

    public ICollection<IRoomChatCommand> GetRegisteredCommands()
    {
        return _commands.Values;
    }
}