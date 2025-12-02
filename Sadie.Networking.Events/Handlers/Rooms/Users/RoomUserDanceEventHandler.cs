using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserDance)]
public class RoomUserDanceEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int DanceId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room.BroadcastDataAsync(new RoomUserDanceWriter
        {
            UserId = roomUser.Player.Player.Id,
            DanceId = DanceId
        });
    }
}