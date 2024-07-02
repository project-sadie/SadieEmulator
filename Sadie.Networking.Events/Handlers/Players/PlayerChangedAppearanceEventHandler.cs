using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
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
        
        player.AvatarData.Gender = gender;
        player.AvatarData.FigureCode = figureCode;
        
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
        
        dbContext.PlayerAvatarData.Update(player.AvatarData);
        await dbContext.SaveChangesAsync();
    }
}