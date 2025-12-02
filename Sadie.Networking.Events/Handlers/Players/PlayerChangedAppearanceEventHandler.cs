using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Packets.Writers.Players;
using Sadie.Networking.Packets.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public required string Gender { get; set; }
    public required string FigureCode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        
        if (player?.Player.AvatarData == null)
        {
            return;
        }

        var gender = Gender == "M" ? 
            PlayerAvatarGender.Male : 
            PlayerAvatarGender.Female;

        var figureCode = FigureCode;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        if (player.Player.AvatarData.Gender != gender)
        {
            player.Player.AvatarData.Gender = gender;
            dbContext.Entry(player.Player.AvatarData).Property(x => x.Gender).IsModified = true;
        }

        player.Player.AvatarData.FigureCode = figureCode;
        dbContext.Entry(player.Player.AvatarData).Property(x => x.FigureCode).IsModified = true;
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter
        {
            FigureCode = figureCode,
            Gender = gender.ToString()
        });
        
        await room.BroadcastDataAsync(new RoomUserDataWriter
        {
            Users = [roomUser]
        });

        await dbContext.SaveChangesAsync();
    }
}