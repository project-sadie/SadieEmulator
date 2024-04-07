using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangedMottoEventHandler(
    PlayerChangedMottoEventParser eventParser,
    RoomRepository roomRepository, PlayerConstants constants) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerChangedMotto;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;
        var newMotto = eventParser.Motto;

        if (newMotto.Length >= constants.MaxMottoLength)
        {
            newMotto = newMotto.Truncate(constants.MaxMottoLength);
        }

        player.Data.Motto = newMotto;
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserDataWriter(new List<IRoomUser> { roomUser }).GetAllBytes());
    }
}