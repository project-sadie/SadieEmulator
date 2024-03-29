using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Rights;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

public class RoomRemoveUserRightsEvent(
    RoomRemoveUserRightsParser parser,
    IRoomRepository roomRepository, 
    IRoomRightsDao roomRightsDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        if (client.Player == null)
        {
            return;
        }
        
        var playerData = client.Player.Data;
        var (roomFound, room) = roomRepository.TryGetRoomById(playerData.CurrentRoomId);

        if (!roomFound || room == null)
        {
            return;
        }

        foreach (var playerId in parser.Ids)
        {
            await RemoveForPlayerAsync(playerId, room);
        }
    }

    private async Task RemoveForPlayerAsync(long playerId, IRoomData room)
    {
        if (!room.PlayersWithRights.Contains(playerId))
        {
            return;
        }

        if (room.UserRepository.TryGetById(playerId, out var roomUser))
        {
            roomUser!.ControllerLevel = RoomControllerLevel.None;
            roomUser.ApplyFlatCtrlStatus();
            
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter(roomUser.ControllerLevel).GetAllBytes());
        }
        
        room.PlayersWithRights.Remove(playerId);
        await roomRightsDao.DeleteRightsAsync(room.Id, playerId);

        await room.UserRepository.BroadcastDataAsync(
            new RoomRemoveUserRightsWriter(room.Id, playerId).GetAllBytes()
        );
    }
}