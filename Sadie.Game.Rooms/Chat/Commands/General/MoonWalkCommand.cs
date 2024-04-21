using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.General;

public class MoonWalkCommand : AbstractRoomChatCommand
{
    public override string Trigger => "moonwalk";
    public override string Description => "Your avatar falls asleep";
    
    public override async Task ExecuteAsync(IRoomUser user)
    {
        user.MoonWalking = !user.MoonWalking;
        var effectId = user.MoonWalking ? (int) EffectIds.Moonwalk : 0;
        await user.Room.UserRepository.BroadcastDataAsync(new RoomUserEffectWriter(user.Id, effectId));
    }
}