using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class StandCommand : AbstractRoomChatCommand, IRoomChatCommand
{
    public override string Trigger => "stand";
    public override string Description => "Makes your avatar stand up";
    
    public override Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        user.RemoveStatuses(RoomUserStatus.Sit);
        return Task.CompletedTask;
    }
}