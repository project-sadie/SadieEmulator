using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomSettings)]
public class RequestRoomSettingsEventHandler(
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        var room = roomRepository.TryGetRoomById(eventParser.RoomId);
        
        if (room != null)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter
            {
                Room = room
            });
        }
    }
}