using Sadie.Game.Players.Avatar;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Rooms.Users;

public class PlayerChangeAppearanceEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public PlayerChangeAppearanceEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var gender = reader.ReadString() == "M" ? 
            PlayerAvatarGender.Male : 
            PlayerAvatarGender.Female;
        
        var figureCode = reader.ReadString();
        
        // TODO: Validate inputs above
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerChangedAppearanceWriter(figureCode, gender).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<RoomUser> { roomUser! }).GetAllBytes());
    }
}