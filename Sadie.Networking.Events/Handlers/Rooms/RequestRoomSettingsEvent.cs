using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RequestRoomSettingsEvent(
    RequestRoomSettingsParser parser, 
    IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        var (foundRoom, room) = roomRepository.TryGetRoomById(parser.RoomId);
        
        if (foundRoom)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter(room!).GetAllBytes());
        }
    }
}