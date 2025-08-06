using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Attributes;
using Sadie.Shared.Extensions;

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
        if (client.Player?.AvatarData == null)
        {
            return;
        }
        
        var player = client.Player!;
        var newMotto = Motto.Truncate(constants.MaxMottoLength);

        player.AvatarData.Motto = newMotto;
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDataWriter{
            Users = [roomUser]
        });

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(player.AvatarData).Property(x => x.Motto).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}