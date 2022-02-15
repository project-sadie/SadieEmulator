using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;
using Sadie.Networking.Packets.Server.Rooms.Users;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomUserWalkEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserWalkEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var x = reader.ReadInt();
        var y = reader.ReadInt();
        
        var tile = room.Layout.Tiles.FirstOrDefault(z => z.Point.X == x && z.Point.Y == y);
        
        if (tile == null)
        {
            return;
        }

        var userRepository = room.UserRepository;
        
        roomUser.Point = tile.Point;
        
        await userRepository.BroadcastDataToUsersAsync(new RoomUserDataWriter(room.UserRepository.GetAll()).GetAllBytes());
        await userRepository.BroadcastDataToUsersAsync(new RoomUserStatusWriter(room.UserRepository.GetAll()).GetAllBytes());
    }
}