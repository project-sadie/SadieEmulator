using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerWearingBadges)]
public class PlayerWearingBadgesEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient networkClient)
    {
        var player = playerRepository.GetPlayerLogicById(PlayerId);
        var playerBadges = player?.Badges;
        
        if (playerBadges == null)
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            playerBadges = await dbContext.Set<PlayerBadge>().Where(x =>  x.PlayerId == PlayerId).ToListAsync();;
        }

        playerBadges = playerBadges.
            Where(x => x.Slot != 0 && x.Slot <= 5).
            DistinctBy(x => x.Slot).
            ToList();
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, networkClient, out _, out _))
        {
            return;
        }
        
        await networkClient.WriteToStreamAsync(new PlayerWearingBadgesWriter
        {
            PlayerId = PlayerId,
            Badges = playerBadges
        });
    }
}