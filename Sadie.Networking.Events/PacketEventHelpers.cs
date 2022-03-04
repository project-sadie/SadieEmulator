using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;

namespace Sadie.Networking.Events;

internal static class PacketEventHelpers
{
    internal static bool TryResolveRoomObjectsForClient(IRoomRepository roomRepository, INetworkClient client, out IRoom? room, out RoomUser? user)
    {
        if (client.Player == null)
        {
            room = null;
            user = null;
            
            return false;
        }
        
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