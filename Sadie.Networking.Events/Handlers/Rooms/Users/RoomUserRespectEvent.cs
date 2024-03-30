using Sadie.Game.Players;
using Sadie.Game.Players.Respect;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserRespectEvent(
    RoomUserRespectParser parser,
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IPlayerRespectDao respectDao)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Data;
        var lastRoom = player.Data.CurrentRoomId;
        
        if (playerData.RespectPoints < 1 || 
            playerData.Id == parser.TargetId || 
            !playerRepository.TryGetPlayerById(parser.TargetId, out var targetPlayer) || 
            targetPlayer!.Data.CurrentRoomId != 0 && lastRoom != targetPlayer.Data.CurrentRoomId)
        {
            return;
        }

        var targetData = targetPlayer.Data;

        playerData.RespectPoints--;
        targetData.RespectsReceived++;
        
        await respectDao.CreateAsync(playerData.Id, parser.TargetId);

        await room!.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter(parser.TargetId, targetData.RespectsReceived).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser!.Id, (int) RoomUserAction.ThumbsUp).GetAllBytes());
    }
}