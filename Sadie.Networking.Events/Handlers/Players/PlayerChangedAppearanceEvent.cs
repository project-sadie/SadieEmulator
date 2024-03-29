using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangedAppearanceEvent(
    PlayerChangedAppearanceParser parser, 
    IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        var player = client.Player;
        var playerData = player.Data;
        
        var gender = parser.Gender == "M" ? 
            AvatarGender.Male : 
            AvatarGender.Female;

        var figureCode = parser.FigureCode;
        
        // TODO: Validate inputs above

        playerData.Gender = gender;
        playerData.FigureCode = figureCode;
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter(figureCode, gender).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}