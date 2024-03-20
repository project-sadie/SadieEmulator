using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands;

public interface IRoomChatCommand
{
    Task ExecuteAsync(IRoomUser user);
}