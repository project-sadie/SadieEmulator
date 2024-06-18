using Sadie.API.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class IdleCommand : AbstractRoomChatCommand
{
    public override string Trigger => "idle";
    public override string Description => "Your avatar falls asleep";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var writer = new RoomUserIdleWriter
        {
            UserId = user.Id,
            IsIdle = true
        };
        
        await user.Room.UserRepository.BroadcastDataAsync(writer);
    }
}