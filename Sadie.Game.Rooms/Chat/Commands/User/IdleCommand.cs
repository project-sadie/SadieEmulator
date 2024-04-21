using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class IdleCommand : AbstractRoomChatCommand
{
    public override string Trigger => "idle";
    public override string Description => "Your avatar falls asleep";
    
    public override async Task ExecuteAsync(IRoomUser user)
    {
        await user.Room.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(user.Id, true));
    }
}