using Sadie.Game.Players;
using Sadie.Game.Players.DaosToDrop;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomLikeEventHandler(PlayerGenericDao playerDao, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomLike;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        var playerData = client.Player!.Data;
        
        if (room.OwnerId == playerData.Id || playerData.LikedRoomIds.Contains(room.Id))
        {
            return;
        }
        
        playerData.LikedRoomIds.Add(room.Id);
        await playerDao.CreatePlayerRoomLikeAsync(playerData.Id, playerData.CurrentRoomId);
    }
}