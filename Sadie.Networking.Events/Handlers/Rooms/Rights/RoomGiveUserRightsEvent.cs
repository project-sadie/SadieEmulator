using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Rights;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

public class RoomGiveUserRightsEvent(
    RoomGiveUserRightsParser parser,
    IRoomRepository roomRepository, 
    IRoomRightsDao roomRightsDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var playerId = parser.PlayerId;
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

        if (room.UserRepository.TryGetById(playerId, out var targetRoomUser))
        {
            targetRoomUser!.ControllerLevel = RoomControllerLevel.Rights;
            targetRoomUser.ApplyFlatCtrlStatus();
            
            await targetRoomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter(targetRoomUser.ControllerLevel).GetAllBytes());
        }
        
        room.PlayersWithRights.Add(playerId);
        await roomRightsDao.InsertRightsAsync(room.Id, playerId);
    }
}