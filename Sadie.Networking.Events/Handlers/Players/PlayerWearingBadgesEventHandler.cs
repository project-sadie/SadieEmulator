using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerWearingBadges)]
public class PlayerWearingBadgesEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient networkClient)
    {
        var player = playerRepository.GetPlayerLogicById(PlayerId);
        var playerBadges = player?.Player.Badges;
        
        if (playerBadges == null)
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            
            var dbBadges = await dbContext
                .Set<PlayerBadge>()
                .Where(x =>  x.PlayerId == PlayerId)
                .ToListAsync();
            
            playerBadges = mapper.Map<List<PlayerBadgeDto>>(dbBadges);
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