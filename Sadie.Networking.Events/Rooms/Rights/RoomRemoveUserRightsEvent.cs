using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Rooms.Rights;

public class RoomRemoveUserRightsEvent(IRoomRepository roomRepository, 
    IRoomDao roomDao,
    IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
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
        
        var amount = reader.ReadInteger();

        for (var i = 0; i < amount; i++)
        {
            await RemoveForPlayerAsync(reader.ReadInteger(), room);
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
        await roomDao.DeleteRightsAsync(room.Id, playerId);

        await room.UserRepository.BroadcastDataAsync(
            new RoomRemoveUserRightsWriter(room.Id, playerId).GetAllBytes()
        );
    }
}