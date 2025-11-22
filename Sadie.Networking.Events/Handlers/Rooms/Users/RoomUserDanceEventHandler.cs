using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Attributes;

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
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDanceWriter
        {
            UserId = roomUser.Player.Id,
            DanceId = DanceId
        });
    }
}