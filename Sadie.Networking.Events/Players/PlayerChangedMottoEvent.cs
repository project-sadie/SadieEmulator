using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players;

public class PlayerChangedMottoEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;
    private readonly PlayerConstants _constants;

    public PlayerChangedMottoEvent(IRoomRepository roomRepository, PlayerConstants constants)
    {
        _roomRepository = roomRepository;
        _constants = constants;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var motto = reader.ReadString();

        if (motto.Length >= _constants.MaxMottoLength)
        {
            motto = motto.Truncate(_constants.MaxMottoLength);
        }

        player.Motto = motto;
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}