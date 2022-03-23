using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;
using Sadie.Shared;
using Sadie.Shared.Extensions;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Networking.Events.Players;

public class PlayerChangedMottoEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public PlayerChangedMottoEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var motto = reader.ReadString();

        if (motto.Length >= SadieConstants.MaxMottoLength)
        {
            motto = motto.Truncate(SadieConstants.MaxMottoLength);
        }

        player.Motto = motto;
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<RoomUser> { roomUser! }).GetAllBytes());
    }
}