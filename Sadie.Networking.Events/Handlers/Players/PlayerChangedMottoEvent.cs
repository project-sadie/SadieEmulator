using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangedMottoEvent(
    PlayerChangedMottoParser parser,
    IRoomRepository roomRepository, PlayerConstants constants) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var player = client.Player;
        var newMotto = parser.Motto;

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