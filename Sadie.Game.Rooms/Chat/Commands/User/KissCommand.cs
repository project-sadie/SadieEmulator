using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class KissCommand : AbstractRoomChatCommand
{
    public override string Trigger => "kiss";
    public override string Description => "Your avatar blows a kiss";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        await user.Room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(user.Id, (int) RoomUserAction.Kiss));
    }
}