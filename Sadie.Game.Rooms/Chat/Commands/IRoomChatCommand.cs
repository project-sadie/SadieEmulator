using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands;

public interface IRoomChatCommand
{
    string Trigger { get; }
    Task ExecuteAsync(IRoomUser user);
}