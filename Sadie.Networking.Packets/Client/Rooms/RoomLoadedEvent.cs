using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomLoadedEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomLoadedEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var (roomId, password) = (reader.ReadInt(), reader.ReadString());
        var (found, room) = _roomRepository.TryGetRoomById(roomId);

        if (!found)
        {
            return;
        }

        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
        await client.WriteToStreamAsync(new RoomDataWriter(roomId, room.Model.Name).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
    }
}