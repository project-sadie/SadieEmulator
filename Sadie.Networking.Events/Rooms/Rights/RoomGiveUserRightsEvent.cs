using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Rooms.Rights;

public class RoomGiveUserRightsEvent(IRoomRepository roomRepository, IRoomDao roomDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();
        var playerData = client.Player.Data;
        
        var (roomFound, room) = roomRepository.TryGetRoomById(playerData.CurrentRoomId);

        if (!roomFound || room == null)
        {
            return;
        }

        if (room.PlayersWithRights.Contains(playerId))
        {
            return;
        }

        await room.UserRepository.BroadcastDataAsync(
            new RoomGiveUserRightsWriter(room.Id, playerId, playerData.Username).GetAllBytes()
        );
        
        var roomUser = client.RoomUser;

        if (roomUser != null)
        {
            roomUser.ControllerLevel = RoomControllerLevel.Rights;
            roomUser.ApplyFlatCtrlStatus();
            
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter(roomUser.ControllerLevel).GetAllBytes());
        }

        room.PlayersWithRights.Add(playerId);
        await roomDao.InsertRightsAsync(room.Id, playerId);
    }
}