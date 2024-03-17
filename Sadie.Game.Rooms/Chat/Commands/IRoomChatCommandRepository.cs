namespace Sadie.Game.Rooms.Chat.Commands;

public interface IRoomChatCommandRepository
{
    Tuple<bool, IRoomChatCommand?> TryGetCommandByTriggerWord(string trigger);
}