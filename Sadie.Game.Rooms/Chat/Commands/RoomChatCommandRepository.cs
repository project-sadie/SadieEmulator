namespace Sadie.Game.Rooms.Chat.Commands;

public class RoomChatCommandRepository : IRoomChatCommandRepository
{
    private readonly Dictionary<string, IRoomChatCommand> _commands;

    public RoomChatCommandRepository(IEnumerable<IRoomChatCommand> commands)
    {
        _commands = commands.ToDictionary(x => x.Trigger, c => c);
    }

    public Tuple<bool, IRoomChatCommand?> TryGetCommandByTriggerWord(string trigger)
    {
        return _commands.TryGetValue(trigger, out var command)
            ? new Tuple<bool, IRoomChatCommand?>(true, command)
            : new Tuple<bool, IRoomChatCommand?>(false, null);
    }
}