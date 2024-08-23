using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserDance)]
public class RoomUserDanceEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int DanceId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDanceWriter
        {
            UserId = roomUser.Id,
            DanceId = DanceId
        });
    }
}