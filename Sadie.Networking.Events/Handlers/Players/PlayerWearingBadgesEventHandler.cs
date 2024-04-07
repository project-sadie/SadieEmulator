using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerWearingBadgesEventHandler(
    PlayerWearingBadgesEventParser eventParser,
    SadieContext dbContext,
    PlayerRepository playerRepository,
    RoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerWearingBadges;

    public async Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var isPlayerOnline = playerRepository.TryGetPlayerById(playerId, out var player);

        var playerBadges = isPlayerOnline
            ? player!.Badges
            : await dbContext.Set<PlayerBadge>().Where(x => x.PlayerId == playerId).ToListAsync();

        playerBadges = playerBadges.
            Where(x => x.Slot != 0 && x.Slot <= 5).
            DistinctBy(x => x.Slot).
            ToList();
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, networkClient, out var room, out _))
        {
            await networkClient.WriteToStreamAsync(new PlayerWearingBadgesWriter(playerId, playerBadges).GetAllBytes());
            return;
        }
        
        await networkClient.WriteToStreamAsync(new PlayerWearingBadgesWriter(playerId, playerBadges).GetAllBytes());
    }
}