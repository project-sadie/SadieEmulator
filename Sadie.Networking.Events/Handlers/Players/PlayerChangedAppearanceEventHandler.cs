using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
{
    public required string Gender { get; set; }
    public required string FigureCode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        
        if (player?.AvatarData == null)
        {
            return;
        }

        var gender = Gender == "M" ? 
            AvatarGender.Male : 
            AvatarGender.Female;

        var figureCode = FigureCode;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        if (player.AvatarData.Gender != gender)
        {
            player.AvatarData.Gender = gender;
            dbContext.Entry(player.AvatarData).Property(x => x.Gender).IsModified = true;
        }

        player.AvatarData.FigureCode = figureCode;
        dbContext.Entry(player.AvatarData).Property(x => x.FigureCode).IsModified = true;
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter
        {
            FigureCode = figureCode,
            Gender = gender.ToString()
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDataWriter
        {
            Users = [roomUser]
        });

        await dbContext.SaveChangesAsync();
    }
}