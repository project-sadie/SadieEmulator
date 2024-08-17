using Sadie.Enums.Game.Rooms.Users;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserAction)]
public class RoomUserActionEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Action { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (Action == (int) RoomUserAction.Idle)
        {
            if (!roomUser.IsIdle)
            {
                roomUser.LastAction -= roomUser.IdleTime;
            }
            
            await room.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter
            {
                UserId = roomUser.Id,
                IsIdle = roomUser.IsIdle
            });
            
            return;
        }

        await room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Id,
            Action = Action
        });
    }
}