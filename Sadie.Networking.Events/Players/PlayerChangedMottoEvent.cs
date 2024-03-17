using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players;

public class PlayerChangedMottoEvent(IRoomRepository roomRepository, PlayerConstants constants) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var newMotto = reader.ReadString();

        if (newMotto.Length >= constants.MaxMottoLength)
        {
            newMotto = newMotto.Truncate(constants.MaxMottoLength);
        }

        player.Data.Motto = newMotto;
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}