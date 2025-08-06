using Sadie.API.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserAction)]
public class RoomUserActionEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Action { get; init; }
    
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
                UserId = roomUser.Player.Id,
                IsIdle = roomUser.IsIdle
            });
            
            return;
        }

        await room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Player.Id,
            Action = Action
        });
    }
}