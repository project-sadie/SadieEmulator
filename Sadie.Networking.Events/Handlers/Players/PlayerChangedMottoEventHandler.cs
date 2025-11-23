using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Extensions;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerChangedMotto)]
public class PlayerChangedMottoEventHandler(
    IRoomRepository roomRepository, 
    ServerPlayerConstants constants,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public required string Motto { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player?.Player.AvatarData == null)
        {
            return;
        }
        
        var player = client.Player!;
        var newMotto = Motto.Truncate(constants.MaxMottoLength);
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDataWriter{
            Users = [roomUser]
        });

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        await dbContext.PlayerAvatarData
            .Where(x => x.PlayerId == player.Player.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Motto, newMotto));

        player.Player.Player.AvatarData = player.Player.AvatarData with { Motto = newMotto };
    }
}