using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Rooms;

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
        var userRepository = room.UserRepository;
        
        await client.WriteToStreamAsync(new RoomRelativeMapWriter(roomLayout).GetAllBytes());
        await client.WriteToStreamAsync(new RoomHeightMapWriter(true, -1, roomLayout.HeightMap.Replace("\n", "\r")).GetAllBytes());
        
        await userRepository.BroadcastDataAsync(new RoomUserStatusWriter(room.UserRepository.GetAll()).GetAllBytes());
        await userRepository.BroadcastDataAsync(new RoomUserDataWriter(room.UserRepository.GetAll()).GetAllBytes());
    }
}