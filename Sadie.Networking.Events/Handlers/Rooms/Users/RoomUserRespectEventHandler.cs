using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserRespectEventHandler(
    RoomUserRespectEventParser eventParser,
    PlayerRepository playerRepository,
    RoomRepository roomRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserRespect;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Data;
        var lastRoom = player.CurrentRoomId;
        var targetPlayer = playerRepository.GetPlayerLogicById(eventParser.TargetId);
        
        if (playerData.RespectPoints < 1 || 
            player.Id == eventParser.TargetId || 
            targetPlayer == null || 
            targetPlayer!.CurrentRoomId != 0 && lastRoom != targetPlayer.CurrentRoomId)
        {
            return;
        }

        var respect = new PlayerRespect
        {
            OriginPlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id
        };

        playerData.RespectPoints--;
        targetPlayer.Respects.Add(respect);

        await dbContext.SaveChangesAsync();

        await room!.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter(eventParser.TargetId, targetPlayer.Respects.Count));
        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser!.Id, (int) RoomUserAction.ThumbsUp));
    }
}