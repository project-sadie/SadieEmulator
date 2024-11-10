using Sadie.API;
using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Game.Rooms.Users;

namespace CommandsPlugin.User;

public class StandCommand : AbstractRoomChatCommand
{
    public override string Trigger => "stand";
    public override string Description => "Makes your avatar stand up";
    
    public override Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        user.RemoveStatuses(RoomUserStatus.Sit);
        return Task.CompletedTask;
    }
}