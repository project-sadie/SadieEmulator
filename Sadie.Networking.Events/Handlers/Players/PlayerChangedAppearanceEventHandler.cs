using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerChangedAppearance)]
public class PlayerChangedAppearanceEventHandler(
    PlayerChangedAppearanceEventParser eventParser, 
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }

        eventParser.Parse(reader);
        
        var player = client.Player;
        
        var gender = eventParser.Gender == "M" ? 
            AvatarGender.Male : 
            AvatarGender.Female;

        var figureCode = eventParser.FigureCode;
        
        // TODO: Validate inputs above

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
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter
        {
            Users = [roomUser]
        });
        
        dbContext.PlayerAvatarData.Update(player.AvatarData);
        await dbContext.SaveChangesAsync();
    }
}