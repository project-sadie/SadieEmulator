using Sadie.Game.Players;
using Sadie.Game.Players.Respect;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserRespectEvent(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IPlayerRespectDao respectDao)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Data;
        var targetId = reader.ReadInteger();
        var lastRoom = player.Data.CurrentRoomId;
        
        if (playerData.RespectPoints < 1 || 
            playerData.Id == targetId || 
            !playerRepository.TryGetPlayerById(targetId, out var targetPlayer) || 
            targetPlayer!.Data.CurrentRoomId != 0 && lastRoom != targetPlayer.Data.CurrentRoomId)
        {
            return;
        }

        var targetData = targetPlayer.Data;

        playerData.RespectPoints--;
        targetData.RespectsReceived++;
        
        await respectDao.CreateAsync(playerData.Id, targetId);

        await room!.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter(targetId, targetData.RespectsReceived).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser!.Id, RoomUserAction.ThumbsUp).GetAllBytes());
    }
}