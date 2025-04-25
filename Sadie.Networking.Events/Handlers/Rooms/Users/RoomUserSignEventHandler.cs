using Sadie.API.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserSign)]
public class RoomUserSignEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int SignId { get; init; }
    
    public Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        roomUser.AddStatus(RoomUserStatus.Sign, SignId.ToString());
        roomUser.SignSet = DateTime.Now;
        
        return Task.CompletedTask;
    }
}