using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;

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
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var gender = Gender == "M" ? 
            AvatarGender.Male : 
            AvatarGender.Female;

        var figureCode = FigureCode;

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