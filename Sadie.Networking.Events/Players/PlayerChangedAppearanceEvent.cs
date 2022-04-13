using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Networking.Events.Players;

public class PlayerChangedAppearanceEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public PlayerChangedAppearanceEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var playerData = player.Data;
        
        var gender = reader.ReadString() == "M" ? 
            AvatarGender.Male : 
            AvatarGender.Female;
        
        var figureCode = reader.ReadString();
        
        // TODO: Validate inputs above

        playerData.Gender = gender;
        playerData.FigureCode = figureCode;
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter(figureCode, gender).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}