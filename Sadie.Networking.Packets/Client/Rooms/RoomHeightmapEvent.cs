using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomHeightmapEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomHeightmapEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var (found, room) = _roomRepository.TryGetRoomById(client.Player.LastRoomLoaded);
        
        if (!found || room == null)
        {
            return;
        }

        var roomLayout = room.Layout;
        
        await client.WriteToStreamAsync(new RoomRelativeMapWriter(roomLayout).GetAllBytes());
        await client.WriteToStreamAsync(new RoomHeightMapWriter(true, -1, roomLayout.HeightMap.Replace("\n", "\r")).GetAllBytes());
        await client.WriteToStreamAsync(new RoomUserDataWriter().GetAllBytes());
    }
}