using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerIds.RoomUserAction)]
public class RoomUserActionEventHandler(RoomUserActionEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (eventParser.Action == RoomUserAction.Idle)
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
        
        roomUser.UpdateLastAction();

        await room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Id,
            Action = (int) eventParser.Action
        });
    }
}