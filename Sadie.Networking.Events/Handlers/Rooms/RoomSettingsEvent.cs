using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomSettingsEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();
        var (foundRoom, room) = roomRepository.TryGetRoomById(roomId);
        
        if (foundRoom)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter(room!).GetAllBytes());
        }
    }
}