using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangedAppearanceEventHandler(
    PlayerChangedAppearanceEventParser eventParser, 
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerChangedAppearance;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
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
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter(figureCode, gender).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}