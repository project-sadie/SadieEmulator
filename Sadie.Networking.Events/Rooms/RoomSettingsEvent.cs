using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Rooms;

public class RoomSettingsEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomSettingsEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();
        var (foundRoom, room) = _roomRepository.TryGetRoomById(roomId);
        
        if (foundRoom)
        {
            await client.WriteToStreamAsync(new RoomSettingsWriter(room!).GetAllBytes());
        }
    }
}