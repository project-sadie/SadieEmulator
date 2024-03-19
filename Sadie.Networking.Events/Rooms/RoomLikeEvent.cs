using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms;

public class RoomLikeEvent(IPlayerDao playerDao, IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
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