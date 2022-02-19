using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;

namespace Sadie.Networking.Events;

public class PacketHelpers
{
    public static bool TryResolveRoomObjectsForClient(IRoomRepository roomRepository, INetworkClient client, out Room? room, out RoomUser? user)
    {
        var roomId = client.Player.LastRoomLoaded;
        var (result, roomObject) = roomRepository.TryGetRoomById(roomId);

        if (!result || roomObject == null || client.RoomUser == null)
        {
            room = null;
            user = null;
            
            return false;
        }

        room = roomObject;
        user = client.RoomUser;
        
        return true;
    }
}