using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class SitCommand : AbstractRoomChatCommand
{
    public override string Trigger => "sit";
    public override string Description => "Makes your avatar sit down";
    
    public override Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        user.AddStatus(RoomUserStatus.Sit, "0");
        return Task.CompletedTask;
    }
}