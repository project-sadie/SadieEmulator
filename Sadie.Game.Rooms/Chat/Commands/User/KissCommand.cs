using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class KissCommand : AbstractRoomChatCommand
{
    public override string Trigger => "kiss";
    public override string Description => "Your avatar blows a kiss";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var actionWriter = new RoomUserActionWriter
        {
            UserId = user.Id,
            Action = (int) RoomUserAction.Kiss
        };
        
        await user.Room.UserRepository.BroadcastDataAsync(actionWriter);
    }
}