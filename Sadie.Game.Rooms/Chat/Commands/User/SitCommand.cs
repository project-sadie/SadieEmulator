using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class SitCommand : AbstractRoomChatCommand, IRoomChatCommand
{
    public override string Trigger => "sit";
    public override string Description => "Makes your avatar sit down";
    
    public override Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        if ((int) user.Direction % 2 != 0)
        {
            return Task.CompletedTask;
        }
        
        user.AddStatus(RoomUserStatus.Sit, "0.5");
        return Task.CompletedTask;
    }
}