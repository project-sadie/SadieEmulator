using Sadie.API;
using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Writers.Rooms.Users;

namespace CommandsPlugin.User;

public class MoonWalkCommand : AbstractRoomChatCommand
{
    public override string Trigger => "moonwalk";
    public override string Description => "Your avatar falls asleep";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        user.MoonWalking = !user.MoonWalking;
        
        var effectId = user.MoonWalking ? (int) EffectIds.Moonwalk : 0;
        
        var writer = new RoomUserEffectWriter
        {
            UserId = user.Id,
            EffectId = effectId,
            DelayMs = 0
        };
        
        await user.Room.UserRepository.BroadcastDataAsync(writer);
    }
}