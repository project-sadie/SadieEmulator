using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;

namespace Sadie.Networking.Packets;

public class PacketHelpers
{
    public static bool TryResolveRoomObjectsForClient(IRoomRepository roomRepository, INetworkClient client, out Room? room, out RoomUser? user)
    {
        var roomId = client.Player.LastRoomLoaded;
        var (result, roomObject) = roomRepository.TryGetRoomById(roomId);

        if (!result || roomObject == null)
        {
            room = null;
            user = null;
            
            return false;
        }

        if (!roomObject.UserRepository.TryGet(client.Player.Id, out user))
        {
            room = null;
            user = null;
            
            return false;
        }

        room = roomObject;
        return true;
    }
}