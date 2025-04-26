using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomSettings)]
public class RequestRoomSettingsEventHandler(
    IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int RoomId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);
        
        if (room != null)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter
            {
                Room = room
            });
        }
    }
}