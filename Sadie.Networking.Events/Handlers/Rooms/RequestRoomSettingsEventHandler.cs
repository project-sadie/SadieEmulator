using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RequestRoomSettingsEventHandler(
    RequestRoomSettingsEventParser eventParser, 
    IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomSettings;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        var (foundRoom, room) = roomRepository.TryGetRoomById(eventParser.RoomId);
        
        if (foundRoom)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter(room!).GetAllBytes());
        }
    }
}